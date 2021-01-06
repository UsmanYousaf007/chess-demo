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
        /// Returns whether Firebase Remote Configs is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; }

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
        /// Initializes Firebase Remote Configs.
        /// <returns> Whether or not the initialization will take place</returns>
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
        /// Initializes Firebase Remote Configs.
        /// </summary>
        /// <param name="callback">A callback invoked after the initialization is done (regardless of the outcome).</param>
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
        /// Automatically initializes Firebase Remote Configs. Can be disabled from FirebaseRemoteConfigsConfig.
        /// </summary>
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( Config != null && Config.AutoInit )
                Init();
        }
    }
}