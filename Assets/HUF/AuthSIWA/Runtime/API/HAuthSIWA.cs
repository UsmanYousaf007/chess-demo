using HUF.Auth.API;
using HUF.AuthSIWA.Runtime.Config;
using HUF.AuthSIWA.Runtime.Implementation;
using HUF.Utils.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AuthSIWA.Runtime.API
{
    public static class HAuthSIWA
    {
        static AuthSIWAService service;
        static readonly HLogPrefix prefix = new HLogPrefix(nameof(HAuthSIWA));

        public static AuthSIWAService Service => service;


        /// <summary>
        /// Returns if AuthSIWAService is supported on device
        /// </summary>
        [PublicAPI]
        public static bool IsSupported => AuthSIWAService.IsSupported;

        /// <summary>
        /// Returns UserName of currently signed-in user or empty string if user is not signed-in
        /// </summary>
        [PublicAPI]
        public static string DisplayName
        {
            get
            {
                if (service != null)
                {
                    return service.DisplayName;
                }

                HLog.LogWarning(prefix, "Can't get UserName, service is not initialized");
                return string.Empty;
            }
        }

        [PublicAPI]
        public static string Email
        {
            get
            {
                if (service != null)
                {
                    return service.Email;
                }

                HLog.Log(prefix, "Can't get Email, service is not initialized");
                return string.Empty;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            bool hasConfig = HConfigs.HasConfig<AuthSIWAConfig>();
            if (hasConfig && HConfigs.GetConfig<AuthSIWAConfig>().AutoInit)
            {
                Init();
            }
        }

        /// <summary>
        /// Use this method to initialize SignInWithApple auth service
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if (!HAuth.IsServiceRegistered(AuthServiceName.SIWA))
            {
                service = new AuthSIWAService();
                HAuth.TryRegisterService(service);
            }
        }
    }
}