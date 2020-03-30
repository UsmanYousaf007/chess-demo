using HUF.Auth.API;
using HUF.Utils;
using HUF.Utils.Extensions;
using HUF.Utils.PlayerPrefs;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SignInWithApple;

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
                var version = UnityEngine.iOS.Device.systemVersion.Split('.')[0];

                return (int.TryParse(version, out int versionNumber) && versionNumber >= MINUMUM_IOS_VERSION);
#endif
            }
        }

        static readonly HLogPrefix prefix = new HLogPrefix(nameof(AuthSIWAService));

        public string Name => AuthServiceName.SIWA;
        public bool IsSignedIn => userInfo.userDetectionStatus == UserDetectionStatus.LikelyReal;
        public string UserId => IsSignedIn ? userInfo.userId : string.Empty;
        public bool IsInitialized => signInWithApple != null;
        public string DisplayName => IsSignedIn
            ? userInfo.displayName
            : string.Empty;
        public string Email => IsSignedIn ? userInfo.email : string.Empty;

        public SignInWithApple ServiceComponent => signInWithApple;
        public event UnityAction<string> OnInitialized;
        public event UnityAction<string> OnSignInSuccess;
        public event UnityAction<string> OnSignInFailure;
        public event UnityAction<string> OnSignOutComplete;

        SignInWithApple signInWithApple;
        UserInfo userInfo = new UserInfo {userDetectionStatus = UserDetectionStatus.Unknown};

        public void Init()
        {
            if (IsInitialized)
                return;

            var go = UnityEngine.Object.Instantiate(new GameObject());
            signInWithApple = go.AddComponent<SignInWithApple>();
            go.AddComponent<DontDestroyOnLoad>();

            if(signInWithApple.onCredentialState == null)
            {
                signInWithApple.onCredentialState = new SignInWithAppleEvent();
                signInWithApple.onError = new SignInWithAppleEvent();
            }

            signInWithApple.onCredentialState.AddListener(OnCredentialState);
            signInWithApple.onError.AddListener(OnError);

            if (HPlayerPrefs.HasKey(AUTH_SIWA_USER_INFO_KEY))
            {
                var content = HPlayerPrefs.GetString(AUTH_SIWA_USER_INFO_KEY);
                userInfo = JsonUtility.FromJson<UserInfo>(content);
            }

            OnInitialized.Dispatch(Name);

            if(IsSupported && IsSignedIn)
            {
                signInWithApple.GetCredentialState(userInfo.userId);
            }
        }

        void OnError(SignInWithApple.CallbackArgs args)
        {
            HLog.LogWarning(prefix,$"SignIn failed, UserCredentialState: {args.credentialState.ToString()}, error: {args.error}");
            OnSignInFailure.Dispatch(Name);
        }

        void OnCredentialState(SignInWithApple.CallbackArgs args)
        {
            HLog.Log(prefix,$"OnCredentialState credientialState: {args.credentialState.ToString()}");
            switch (args.credentialState)
            {
                case UserCredentialState.Revoked:
                    userInfo = new UserInfo();
                    HPlayerPrefs.DeleteKey(AUTH_SIWA_USER_INFO_KEY);
                    OnSignOutComplete.Dispatch(Name);
                    break;
                case UserCredentialState.Authorized:
                    OnSignInSuccess.Dispatch(Name);
                    break;
            }
        }

        public bool SignIn()
        {
            if (!IsInitialized)
            {

                HLog.LogWarning(prefix,"SignIn failed, auth is not initialized yet.");
                return false;
            }

            if (!IsSupported)
            {
                HLog.Log(prefix,"SignIn failed, device version is to low, minimum is {MINUMUM_IOS_VERSION}");
                return false;
            }

            if (IsSignedIn)
            {
                HLog.Log(prefix,"SignIn failed, user is already authenticated.");
                return false;
            }

            signInWithApple.Login(OnSignIn);
            return true;
        }

        void OnSignIn(SignInWithApple.CallbackArgs args)
        {
            userInfo = args.userInfo;
            HPlayerPrefs.SetString(AUTH_SIWA_USER_INFO_KEY,JsonUtility.ToJson(userInfo));
            HLog.Log(prefix,"Authentication succeed.");
            OnSignInSuccess.Dispatch(Name);
        }

        public bool SignOut()
        {
            HLog.LogWarning(prefix,"SignOut failed, just don't do it, user can handle it in iOS settings");
            return false;
        }
    }
}