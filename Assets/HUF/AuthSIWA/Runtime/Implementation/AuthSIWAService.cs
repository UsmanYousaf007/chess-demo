#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using HUF.Auth.Runtime.API;
using HUF.AuthSIWA.Runtime.API;
using HUF.Utils;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AuthSIWA.Runtime.Implementation
{
    public class AuthSIWAService : IAuthService
    {
        const int MINUMUM_IOS_VERSION = 13;
        const string AUTH_SIWA_USER_INFO_KEY = "AuthSIWA_userInfo";

        public static bool IsSupported
        {
            get
            {
#if !UNITY_IOS || UNITY_EDITOR
                return false;
#else
                var version = UnityEngine.iOS.Device.systemVersion.Split ('.') [0];

                return (int.TryParse (version, out int versionNumber) && versionNumber >= MINUMUM_IOS_VERSION);
#endif
            }
        }

        static readonly HLogPrefix logPrefix = new HLogPrefix( HAuthSIWA.logPrefix, nameof(AuthSIWAService) );

        public string Name => AuthServiceName.SIWA;
        public bool IsSignedIn => userInfo.userDetectionStatus != UserDetectionStatus.Unsupported;
        public string UserId => IsSignedIn ? userInfo.userId : string.Empty;
        public string IdToken => IsSignedIn ? userInfo.idToken : string.Empty;
        public bool IsInitialized => wrapperSiwa != null;
        public string DisplayName => IsSignedIn ? userInfo.displayName : string.Empty;
        public string Email => IsSignedIn ? userInfo.email : string.Empty;
        public string AuthorizationCode => IsSignedIn ? userInfo.authorizationCode : string.Empty;

        public WrapperSIWA ServiceComponent => wrapperSiwa;
#pragma warning disable CS0067
        public event Action<string> OnInitialized;
        public event Action OnInitializationFailure;
        public event Action<string, AuthSignInResult> OnSignInResult;
        public event Action<string> OnSignOutComplete;
#pragma warning restore CS0067

        WrapperSIWA wrapperSiwa;
        UserInfo userInfo = new UserInfo {userDetectionStatus = UserDetectionStatus.Unsupported};

        public void Init()
        {
            if ( IsInitialized )
                return;

            var go = UnityEngine.Object.Instantiate( new GameObject() );
            wrapperSiwa = go.AddComponent<WrapperSIWA>();

            if ( HPlayerPrefs.HasKey( AUTH_SIWA_USER_INFO_KEY ) )
            {
                var content = HPlayerPrefs.GetString( AUTH_SIWA_USER_INFO_KEY );
                userInfo = JsonUtility.FromJson<UserInfo>( content );
            }

            OnInitialized.Dispatch( Name );

            if ( IsSupported && IsSignedIn )
            {
                wrapperSiwa.AuthManager.GetCredentialState(userInfo.userId,
                    CredentialStateSuccessCallback, CredentialStateErrorCallback);
            }
        }

        private void CredentialStateErrorCallback(IAppleError error)
        {
            HLog.LogWarning( logPrefix,
                $"SignIn failed, reason: {error.LocalizedFailureReason}, code: {error.Code}" );
            OnSignInResult.Dispatch( Name, AuthSignInResult.ConnectionFailure );
        }

        private void CredentialStateSuccessCallback(CredentialState credentialState)
        {
            HLog.Log( logPrefix, $"OnCredentialState credientialState: {credentialState.ToString()}" );

            switch ( credentialState )
            {
                case CredentialState.Revoked:
                case CredentialState.NotFound:
                    userInfo = new UserInfo {userDetectionStatus = UserDetectionStatus.Unsupported};
                    HPlayerPrefs.DeleteKey( AUTH_SIWA_USER_INFO_KEY );
                    OnSignOutComplete.Dispatch( Name );
                    break;
                case CredentialState.Authorized:
                    OnSignInResult.Dispatch( Name, AuthSignInResult.Success );
                    break;
                case CredentialState.Transferred:
                    wrapperSiwa.AuthManager.QuickLogin(new AppleAuthQuickLoginArgs(), TransferedSuccessCallback, LoginErrorCallback);
                    break;
            }
        }

        public bool SignIn()
        {
            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix, "SignIn failed, auth is not initialized yet." );
                return false;
            }

            if ( !IsSupported )
            {
                HLog.Log( logPrefix, $"SignIn failed, device version is to low, minimum is iOS {MINUMUM_IOS_VERSION}" );
                return false;
            }

            if ( IsSignedIn )
            {
                HLog.Log( logPrefix, "SignIn failed, user is already authenticated." );
                return false;
            }

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            wrapperSiwa.AuthManager.LoginWithAppleId(loginArgs, LoginSuccessCallback, LoginErrorCallback);
            return true;
        }

        private void LoginErrorCallback(IAppleError error)
        {
            HLog.Log( logPrefix, error.LocalizedFailureReason);
            OnSignInResult.Dispatch( Name, AuthSignInResult.ConnectionFailure );
        }

        private void TransferedSuccessCallback(ICredential credential)
        {
            var appleIdCredential = credential as IAppleIDCredential;
            if (appleIdCredential != null)
            {
                var newInfo = UserInfo.BuildFromIAppleIDCredential(appleIdCredential);
                userInfo.authorizationCode = newInfo.authorizationCode;
                userInfo.idToken = newInfo.idToken;
                userInfo.userId = newInfo.userId;
                HPlayerPrefs.SetString( AUTH_SIWA_USER_INFO_KEY, JsonUtility.ToJson( userInfo ) );
                HLog.Log( logPrefix, "Authentication succeed." );
                OnSignInResult.Dispatch( Name, AuthSignInResult.Success );
            }
            else
            {
                HLog.LogError( logPrefix, "appleIdCredential is null." );
                OnSignInResult.Dispatch( Name, AuthSignInResult.UnspecifiedFailure );
            }
        }

        private void LoginSuccessCallback(ICredential credential)
        {
            HLog.Log(logPrefix, credential.User);
            var appleIdCredential = credential as IAppleIDCredential;
            if (appleIdCredential != null)
            {
                var json = JsonUtility.ToJson(appleIdCredential);
                HLog.Log(logPrefix, json);
                userInfo = UserInfo.BuildFromIAppleIDCredential(appleIdCredential);
                HPlayerPrefs.SetString( AUTH_SIWA_USER_INFO_KEY, JsonUtility.ToJson( userInfo ) );
                HLog.Log( logPrefix, "Authentication succeed." );
                OnSignInResult.Dispatch( Name, AuthSignInResult.Success );
            }
            else
            {
                HLog.LogError( logPrefix, "appleIdCredential is null." );
                OnSignInResult.Dispatch( Name, AuthSignInResult.UnspecifiedFailure );
            }
        }

        public bool SignOut()
        {
            HLog.LogWarning( logPrefix, "SignOut failed, just don't do it, user can handle it in iOS settings" );
            return false;
        }
    }
}
#endif