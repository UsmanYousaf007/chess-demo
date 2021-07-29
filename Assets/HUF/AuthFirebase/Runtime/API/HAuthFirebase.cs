using System;
using HUF.Auth.Runtime.API;
using HUF.AuthFirebase.Runtime.Config;
using HUF.AuthFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AuthFirebase.Runtime.API
{
    public static class HAuthFirebase
    {
        static FirebaseAuthService service;
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAuth.logPrefix, nameof(HAuthFirebase) );

        /// <summary>
        /// Returns the UserName of currently signed-in user or empty string if no user is signed-in.
        /// </summary>
        [PublicAPI]
        public static string UserName
        {
            get
            {
                if ( service != null )
                {
                    return service.UserName;
                }

                LogServiceNotInitializedWarning( "Can't get UserName" );
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the PhotoUrl as a System.Uri object of currently signed-in user or null if no user is signed-in.
        /// </summary>
        [PublicAPI]
        public static Uri PhotoUrl
        {
            get
            {
                if ( service != null )
                {
                    return service.PhotoUrl;
                }

                LogServiceNotInitializedWarning( "Can't get PhotoUrl" );
                return null;
            }
        }

        /// <summary>
        /// Returns the Email of the currently signed-in user or empty string if no user is signed-in.
        /// </summary>
        [PublicAPI]
        public static string Email
        {
            get
            {
                if ( service != null )
                {
                    return service.Email;
                }

                LogServiceNotInitializedWarning( "Can't get Email" );
                return string.Empty;
            }
        }

        /// <summary>
        /// Raised when signing in to Firebase with Facebook credentials succeed. <para />
        /// Bool parameter = true if it was the first login to Firebase through this Facebook account, false otherwise.<para/>
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnSignInWithFacebookSuccess
        {
            add
            {
                if ( service != null )
                    service.OnSignInWithFacebookSuccess += value;
                else
                    LogServiceNotInitializedWarning( "Adding listener to OnSignInWithFacebookSuccess failed" );
            }
            remove
            {
                if ( service != null )
                    service.OnSignInWithFacebookSuccess -= value;
                else
                    LogServiceNotInitializedWarning( "Removing listener from OnSignInWithFacebookSuccess failed" );
            }
        }

        /// <summary>
        /// Raised when signing in to Firebase with Facebook credentials failed. <para />
        /// </summary>
        [PublicAPI]
        public static event Action OnSignInWithFacebookFailure
        {
            add
            {
                if ( service != null )
                    service.OnSignInWithFacebookFailure += value;
                else
                    LogServiceNotInitializedWarning( "Adding listener to OnSignInWithFacebookFailure failed" );
            }
            remove
            {
                if ( service != null )
                    service.OnSignInWithFacebookFailure -= value;
                else
                    LogServiceNotInitializedWarning( "Removing listener from OnSignInWithFacebookFailure failed" );
            }
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<FirebaseAuthConfig>();

            if ( hasConfig && HConfigs.GetConfig<FirebaseAuthConfig>().AutoInit )
            {
                Init();
            }
        }

        /// <summary>
        /// Initializes the Firebase auth service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( !HAuth.IsServiceRegistered( AuthServiceName.FIREBASE ) )
            {
                service = new FirebaseAuthService();
                HAuth.TryRegisterService( service );
            }
        }

        /// <summary>
        /// Initializes the Firebase auth service.
        /// </summary>
        /// <param name="callback">A callback invoked after initialization is finished regardless of the outcome.</param>
        [PublicAPI]
        public static void Init( Action callback )
        {
            if ( callback == null )
            {
                Init();
                return;
            }

            if ( !HAuth.IsServiceRegistered( AuthServiceName.FIREBASE ) )
            {
                service = new FirebaseAuthService();
                HAuth.TryRegisterService( service, callback );
                return;
            }

            callback();
        }

        /// <summary>
        /// Sign in to the Firebase using a Facebook access token.
        /// </summary>
        /// <param name="accessToken">An facebook user access token.</param>
        /// <returns>Returns TRUE if signing-in call succeeds. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool SignInWithFacebook( string accessToken )
        {
            if ( service != null )
            {
                return service.SignInWithFacebook( accessToken );
            }

            LogServiceNotInitializedWarning( "SignInWithFacebookAccessToken failed" );
            return false;
        }

        static void LogServiceNotInitializedWarning( string message )
        {
            HLog.LogWarning( logPrefix, $"{message}, service is not initialized" );
        }
    }
}