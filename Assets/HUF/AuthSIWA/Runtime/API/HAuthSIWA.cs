using HUF.Auth.Runtime.API;
using HUF.AuthSIWA.Runtime.Config;
#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
using HUF.AuthSIWA.Runtime.Implementation;
#else
using AuthSIWAService = HUF.Auth.Runtime.Implementation.DummyAuthService;
#endif
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AuthSIWA.Runtime.API
{
    public static class HAuthSIWA
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( HAuth.logPrefix, nameof(HAuthSIWA) );
        static AuthSIWAService service;

        public static AuthSIWAService Service => service;

        /// <summary>
        /// Returns if AuthSIWAService is supported on device
        /// </summary>
        [PublicAPI]
        public static bool IsSupported =>
#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
            AuthSIWAService.IsSupported;
#else
            false;
#endif

        /// <summary>
        /// Returns UserName of currently signed-in user or empty string if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static string DisplayName
        {
            get
            {
#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
                if (service != null)
                {
                    return service.DisplayName;
                }

                HLog.LogWarning(logPrefix, "Can't get UserName, service is not initialized");
#endif
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns Email of currently signed-in user or empty string if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static string Email
        {
            get
            {
#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
                if (service != null)
                {
                    return service.Email;
                }
                HLog.LogWarning(logPrefix, "Can't get Email, service is not initialized");
#endif
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns identity token of currently signed-in user or empty string if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static string IdToken
        {
            get
            {
#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
                if ( service != null )
                {
                    return service.IdToken;
                }

                HLog.LogWarning(logPrefix, "Can't get IdToken, service is not initialized");
#endif
                return string.Empty;
            }
        }

        [PublicAPI]
        public static string AuthorizationCode
        {
            get
            {
#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
                if (service != null)
                {
                    return service.AuthorizationCode;
                }

                HLog.LogWarning(logPrefix, "Can't get AuthorizationCode, service is not initialized");
#endif
                return string.Empty;
            }
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            bool hasConfig = HConfigs.HasConfig<AuthSIWAConfig>();

            if ( hasConfig && HConfigs.GetConfig<AuthSIWAConfig>().AutoInit )
            {
                Init();
            }
        }

        /// <summary>
        /// Initializes SignInWithApple auth service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( HAuth.IsServiceRegistered( AuthServiceName.SIWA ) )
                return;

#if UNITY_IOS && !HUF_AUTH_SIWA_DUMMY
            service = new AuthSIWAService();
#else
            service = new AuthSIWAService( AuthServiceName.SIWA );
#endif
            HAuth.TryRegisterService( service );
        }
    }
}