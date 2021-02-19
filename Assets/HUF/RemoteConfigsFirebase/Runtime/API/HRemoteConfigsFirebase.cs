using System;
using HUF.RemoteConfigs.Runtime.API;
using HUF.RemoteConfigsFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.Runtime.API
{
    public static class HRemoteConfigsFirebase
    {
        const RemoteConfigService serviceType = RemoteConfigService.Firebase;
        static FirebaseRemoteConfigsConfig config;

        /// <summary>
        /// Raised when the service is fully initialized.
        /// </summary>
        [PublicAPI]
        public static event Action OnInitialized;

        static Action initializationCallback;

        /// <summary>
        /// Returns whether Firebase Remote Configs is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; }

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

            HRemoteConfigs.OnInitComplete += HandleInitComplete;

            try
            {
                HRemoteConfigs.RegisterService( new FirebaseRemoteConfigsService(), serviceType, Config.IsMain );
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
            if ( callback != null )
            {
                if ( initializationCallback != null )
                    initializationCallback += callback;
                else
                    initializationCallback = callback;
            }

            Init();
        }

        static void HandleInitComplete( RemoteConfigService service )
        {
            HRemoteConfigs.OnInitComplete -= HandleInitComplete;
            initializationCallback.Dispatch();
            initializationCallback = null;
            OnInitialized.Dispatch();
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