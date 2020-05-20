using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.RemoteConfigs.Runtime.API
{
    public static class HRemoteConfigs
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HRemoteConfigs) );

        static IRemoteConfigsService remoteConfigsService;

        static HRemoteConfigs()
        {
            OnInitComplete += () => HLog.Log( logPrefix, "RemoteConfig service initialized" );
        }

        /// <summary>
        /// Is set to TRUE if service is fully initialized and FALSE otherwise
        /// </summary>
        [PublicAPI] public static bool IsInitialized =>
            remoteConfigsService != null && remoteConfigsService.IsInitialized;

        /// <summary>
        /// Occurs when service service is fully initialized
        /// </summary>
        [PublicAPI] public static event UnityAction OnInitComplete;

        /// <summary>
        /// Dispatched when fetch is completed successfully.
        /// </summary>
        [PublicAPI] public static event UnityAction OnFetchComplete;

        /// <summary>
        /// Dispatched when fetch failed. Provides reason as string
        /// </summary>
        [PublicAPI] public static event UnityAction OnFetchFail;

        /// <summary>
        /// Registers config service. If there is already registered service the services are swapped.
        /// Could be used to add custom <see cref="IRemoteConfigsService"/> implementation.
        /// </summary>
        /// <param name="configsService">Service to init</param>
        [PublicAPI]
        public static void RegisterService( IRemoteConfigsService configsService )
        {
            DisconnectCallbacks();

            remoteConfigsService = configsService;

            remoteConfigsService.OnFetchFailed += HandleFetchFailed;
            remoteConfigsService.OnFetchComplete += HandleFetchComplete;

            if ( configsService.IsInitialized )
                OnInitComplete.Dispatch();
            else
                remoteConfigsService.OnInitComplete += HandleInitComplete;
        }

        /// <summary>
        /// Starts fetch process for remote configs.
        /// </summary>
        [PublicAPI]
        public static void Fetch()
        {
            if ( IsServiceAttached() )
                remoteConfigsService.Fetch();
            else
                OnFetchFail.Dispatch();
        }

        /// <summary>
        /// Returns all values downloaded from remote as dictionary with configId.
        /// Returns values from remote if fetch was successful and empty dict in any other case.
        /// </summary>
        [PublicAPI]
        public static Dictionary<string, string> GetDictionary()
        {
            return IsServiceAttached() ? remoteConfigsService.GetConfigJSONs() : new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns raw config data for given key - config name.
        /// </summary>
        /// <param name="configId">Config Id <see cref="AbstractConfig.ConfigId"/></param>
        [PublicAPI]
        public static string GetValue( string configId )
        {
            return IsServiceAttached() ? remoteConfigsService?.GetConfigJSON( configId ) : string.Empty;
        }

        /// <summary>
        /// Applies value from remote to specific config.
        /// </summary>
        /// <typeparam name="T">Config type to be applied</typeparam>
        [PublicAPI]
        public static void ApplyConfig<T>( ref T config ) where T : AbstractConfig
        {
            if ( IsServiceAttached() )
                remoteConfigsService?.ApplyConfig( ref config );
        }

        /// <summary>
        /// Applies value from remote for all configs.
        /// </summary>
        [PublicAPI]
        public static void ApplyAllConfigs()
        {
            if ( IsServiceAttached() )
                remoteConfigsService?.ApplyAllConfigs();
        }

        static void DisconnectCallbacks()
        {
            if (remoteConfigsService == null)
                return;

            remoteConfigsService.OnInitComplete -= HandleInitComplete;
            remoteConfigsService.OnFetchFailed -= HandleFetchFailed;
            remoteConfigsService.OnFetchComplete -= HandleFetchComplete;
        }

        static void HandleInitComplete()
        {
            OnInitComplete.Dispatch();
        }

        static void HandleFetchFailed()
        {
            OnFetchFail.Dispatch();
        }

        static void HandleFetchComplete()
        {
            OnFetchComplete.Dispatch();
        }

        static bool IsServiceAttached()
        {
            if ( remoteConfigsService != null )
                return true;

            HLog.LogError(logPrefix, "No remote config service attached. " +
                                 "Please attach remote service to use this feature");
            return false;
        }
    }
}