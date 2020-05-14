using System;
using HUF.RemoteConfigs.Runtime.API;
using HUF.RemoteConfigsFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.Runtime.API
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
        /// <returns> Whether or not initialization will take place</returns>
        /// </summary>
        [PublicAPI]
        public static bool Init()
        {
            if ( Config != null && ( !Application.isEditor || Config.EnableInEditor ) )
            {
                HRemoteConfigs.RegisterService(new FirebaseRemoteConfigsService());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes Firebase Remote Config Service for use.
        /// </summary>
        /// <param name="callback">Callback invoked after initialization is done (regardless of the outcome)</param>
        [PublicAPI]
        public static void Init( Action callback )
        {
            if ( callback == null )
            {
                Init();
                return;
            }

            void HandleInitComplete()
            {
                HRemoteConfigs.OnInitComplete -= HandleInitComplete;
                callback();
            }

            HRemoteConfigs.OnInitComplete += HandleInitComplete;

            if ( Init() )
                return;

            HandleInitComplete();
        }
    }
}