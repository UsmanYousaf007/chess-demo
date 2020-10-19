using System;
using HUF.RemoteConfigs.Runtime.API;
using HUF.RemoteConfigsFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.Runtime.API
{
    public static class HRemoteConfigsFirebase
    {
        /// <summary>
        /// Returns whether Firebase Remote Config is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; } = false;

        static FirebaseRemoteConfigsConfig config;

        static FirebaseRemoteConfigsConfig Config
        {
            get
            {
                if ( config == null && HConfigs.HasConfig<FirebaseRemoteConfigsConfig>() )
                    config = HConfigs.GetConfig<FirebaseRemoteConfigsConfig>();
                return config;
            }
        }

        /// <summary>
        /// Initializes Firebase Remote Config.
        /// <returns> Whether or not initialization will take place</returns>
        /// </summary>
        [PublicAPI]
        public static bool Init()
        {
            if ( IsInitialized || Config == null || ( Application.isEditor && !Config.EnableInEditor ) )
                return false;

            try
            {
                HRemoteConfigs.RegisterService( new FirebaseRemoteConfigsService() );
                IsInitialized = true;
            }
            catch ( Exception exception )
            {
                HLog.LogError( new HLogPrefix( nameof(HRemoteConfigsFirebase) ), exception.ToString() );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes Firebase Remote Config.
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

        /// <summary>
        /// Automatically initializes Firebase Remote Config. Can be disabled from FirebaseRemoteConfigConfig.
        /// </summary>
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( Config != null && Config.AutoInit )
                Init();
        }
    }
}