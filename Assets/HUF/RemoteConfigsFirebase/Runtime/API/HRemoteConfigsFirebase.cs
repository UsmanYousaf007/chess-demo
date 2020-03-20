#define HUF_REMOTECONFIGS_FIREBASE

using HUF.RemoteConfigs.API;
using HUF.RemoteConfigsFirebase.Implementation;
using HUF.Utils.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.API
{
    public static class HRemoteConfigsFirebase
    {
        static FirebaseRemoteConfigsConfig config;

        static FirebaseRemoteConfigsConfig Config
        {
            get
            {
                if (config == null && HConfigs.HasConfig<FirebaseRemoteConfigsConfig>())
                    config = HConfigs.GetConfig<FirebaseRemoteConfigsConfig>();
                return config;
            }
        }

        /// <summary>
        /// Automatically initializes Firebase Remote Config. Can be disabled from FirebaseRemoteConfigConfig.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if (Config != null && Config.AutoInit)
                Init();
        }

        /// <summary>
        /// Initializes Firebase Remote Config Service for use.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if (Config != null && (!Application.isEditor || Config.EnableInEditor))
                HRemoteConfigs.RegisterService(new FirebaseRemoteConfigsService());
        }
    }
}