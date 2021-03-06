#if UNITY_IOS
using System;
using System.IO;
using System.Threading;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using HUF.Utils.Runtime.UI.CanvasBlocker;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.iOS;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

#endif
namespace HUF.PolicyGuard.Runtime.Implementations
{
    public static class AppTrackingTransparencyBridge
    {
        const string REQUEST_KEY = "ATTRequest";
#if UNITY_EDITOR
        const string LAST_ATT_STATUS_KEY = "HUFLastATTStatus";
#endif
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AppTrackingTransparencyBridge) );
        static AuthorizationStatus currentStatus = AuthorizationStatus.NotDetermined;
        static bool isTrackingFocus;
        static SynchronizationContext syncContext;
        static Action<AuthorizationStatus> cachedCallbacks;
        static Action<AuthorizationStatus> currentStatusCallback;

        delegate void AuthorizationStatusCallback( int status );

        public static event Action<AuthorizationStatus> OnAuthorizationStatusChanged;

        /// <summary>
        /// Available AppTrackingTransparency statuses
        /// </summary>
        /// <remarks>
        /// Source: https://developer.apple.com/documentation/apptrackingtransparency/attrackingmanager/authorizationstatus#topics
        /// Frameworks/AppTrackingTransparency/ATTrackingManager.h/ATTrackingManagerAuthorizationStatus
        /// </remarks>
        [PublicAPI]
        public enum AuthorizationStatus : int
        {
            /// <summary>
            /// The value returned if a user has not yet received an authorization request
            /// to authorize access to app-related data that can be used for tracking the user or the device.
            /// </summary>
            NotDetermined = 0,

            /// <summary>
            /// The value returned if authorization to access app-related data that can be used for tracking the user or the device is restricted.
            /// </summary>
            Restricted,

            /// <summary>
            /// The value returned if the user denies authorization to access app-related data that
            /// can be used for tracking the user or the device.
            /// </summary>
            Denied,

            /// <summary>
            /// The value returned if the user authorizes access to app-related data that
            /// can be used for tracking the user or the device.
            /// </summary>
            Authorized
        }

        public static bool HasDoneInitialRequest => PlayerPrefs.HasKey( REQUEST_KEY );

        public static bool IsATTAuthorized => IsIOSLowerThan14() ||
                                              GetCurrentAuthorizationStatus() ==
                                              AuthorizationStatus.Authorized;

        public static AuthorizationStatus GetCurrentAuthorizationStatus()
        {
            if ( IsIOSLowerThan14() )
                return AuthorizationStatus.Authorized;
#if UNITY_EDITOR
            return (AuthorizationStatus)HPlayerPrefs.GetInt( LAST_ATT_STATUS_KEY );
#else
            return (AuthorizationStatus)HUF_CurrentPermissionStatus();
#endif
        }

        public static bool IsIOSLowerThan14()
        {
#if UNITY_EDITOR
            return false;
#else
            int current = int.Parse( Device.systemVersion.Split( '.' )[0] );
            return current <= 13;
#endif
        }

        public static void OpenATTSettings()
        {
#if UNITY_EDITOR
            return;
#else
            HUF_OpenATTSettings();
#endif
        }


        internal static void CheckAuthorizationStatus( Action<AuthorizationStatus> callback = null )
        {
            syncContext = SynchronizationContext.Current;
            PlayerPrefs.SetInt( REQUEST_KEY, 1 );
            cachedCallbacks += callback;
#if UNITY_EDITOR
            Mock( ReceiveAuthorizationStatus );
#else
            HUF_RequestTrackingPermission( ReceiveAuthorizationStatus );
#endif
        }

#if EDITOR_ATT_CHECK_FOCUS || !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
        internal static void StartFocusTracking()
        {
            if ( isTrackingFocus )
                return;

            PauseManager.Instance.OnApplicationFocusChange += HandleApplicationFocusChange;
            isTrackingFocus = true;
        }
#endif

        [AOT.MonoPInvokeCallback( typeof(AuthorizationStatusCallback) )]
        static void ReceiveAuthorizationStatus( int result )
        {
            syncContext.Post( data =>
                {
                    AuthorizationStatus status = (AuthorizationStatus)result;
                    HLog.Log( logPrefix, $"Received status: {status}" );

                    if ( currentStatus != status )
                    {
                        currentStatus = status;
#if UNITY_EDITOR
                        HPlayerPrefs.SetInt( LAST_ATT_STATUS_KEY, (int)currentStatus );
#endif
                        OnAuthorizationStatusChanged.Dispatch( status );
                    }

                    cachedCallbacks.Dispatch( status );
                    cachedCallbacks = null;
                },
                null );
        }

        [AOT.MonoPInvokeCallback( typeof(AuthorizationStatusCallback) )]
        static void CurrentAuthorizationStatus( int result )
        {
            AuthorizationStatus status = (AuthorizationStatus)result;
            HLog.Log( logPrefix, $"Current status: {status}" );
            currentStatusCallback.Dispatch( status );
            currentStatusCallback = null;

            if ( currentStatus != status )
            {
                currentStatus = status;
                OnAuthorizationStatusChanged.Dispatch( status );
            }
        }

        static void TryCheck()
        {
            if ( PlayerPrefs.HasKey( REQUEST_KEY ) || IsIOSLowerThan14() )
            {
                CheckAuthorizationStatus();
            }
        }

        static void HandleApplicationFocusChange( bool hasFocus )
        {
            if ( !hasFocus )
                return;

            TryCheck();
        }

        // See: /Classes/Unity/DeviceSettings.mm/QueryASIdentifierManager() for details
        [Obsolete( "For some reason Unity sometimes is unable to load ASIdentifierManager.", true )]
        static void SetStatusFallback()
        {
            int status = Device.advertisingTrackingEnabled
                ? (int)AuthorizationStatus.Authorized
                : (int)AuthorizationStatus.Denied;
            ReceiveAuthorizationStatus( status );
        }

#if UNITY_EDITOR
        [PostProcessBuild( 1 )]
        static void OnPostProcessBuild( BuildTarget target, string path )
        {
            if ( target != BuildTarget.iOS )
                return;

            string projectPath = PBXProject.GetPBXProjectPath( path );
            PBXProject project = new PBXProject();
            project.ReadFromString( File.ReadAllText( projectPath ) );
#if UNITY_2019_3_OR_NEWER
            var targetGuid = project.GetUnityFrameworkTargetGuid();
#else
            string targetName = PBXProject.GetUnityTargetName();
            var targetGuid = project.TargetGuidByName( targetName );
#endif
            project.AddFrameworkToProject( targetGuid, "AdSupport.framework", false );
#if !XCODE_11
            project.AddFrameworkToProject( targetGuid, "AppTrackingTransparency.framework", false );
#endif
            File.WriteAllText( projectPath, project.WriteToString() );
        }

        static void Mock( AuthorizationStatusCallback callback )
        {
            DebugButtonsScreen.Instance.Hide();

            foreach ( AuthorizationStatus status in Enum.GetValues( typeof(AuthorizationStatus) ) )
            {
                DebugButtonsScreen.Instance
                    .AddGUIButton( status.ToString(), () => callback( (int)status ) );
            }

            DebugButtonsScreen.Instance.Show( "App Tracking Permission" );
        }
#endif
        [UsedImplicitly]
        [System.Runtime.InteropServices.DllImport( "__Internal" )]
        static extern void HUF_RequestTrackingPermission( AuthorizationStatusCallback callback );

        [UsedImplicitly]
        [System.Runtime.InteropServices.DllImport( "__Internal" )]
        static extern int HUF_CurrentPermissionStatus();

        [UsedImplicitly]
        [System.Runtime.InteropServices.DllImport( "__Internal" )]
        static extern void HUF_OpenATTSettings();

    }
}
#endif