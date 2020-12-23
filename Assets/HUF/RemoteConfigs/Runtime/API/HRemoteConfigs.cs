using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.RemoteConfigs.Runtime.API
{
    public static class HRemoteConfigs
    {
        const string CACHE_KEY = "HRC_Cache{0}";
        const long UTC_OFFSET = 1589440000;

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HRemoteConfigs) );

        static IRemoteConfigsService remoteConfigsService;

        static bool isFetching;

        static HRemoteConfigs()
        {
            OnInitComplete += () => HLog.Log( logPrefix, "RemoteConfig service initialized" );
        }

        /// <summary>
        /// Raised when the service is fully initialized.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnInitComplete;

        /// <summary>
        /// Raised when fetching is completed successfully.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnFetchComplete;

        /// <summary>
        /// Raised when fetching fails.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnFetchFail;

        /// <summary>
        /// Returns last successful UTC cache refresh timestamp. Note that it is not a secure time.
        /// </summary>
        [PublicAPI]
        public static long? CacheTimestamp
        {
            get
            {
                bool? hasCache = HasAnyCache();

                if ( !hasCache.HasValue )
                    return null;

                int timestamp = PlayerPrefs.GetInt( GetServiceCacheKey() );
                return UTC_OFFSET + timestamp;
            }
        }

        /// <summary>
        /// Returns whether the service successfully handled fetching at least once.
        /// </summary>
        [PublicAPI]
        public static bool IsFetchHandled { get; private set; }

        /// <summary>
        /// Returns whether the service is fully initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized =>
            remoteConfigsService != null && remoteConfigsService.IsInitialized;

        /// <summary>
        /// Returns whether the service has cache present (RemoteConfigs was successfully fetched at least once).
        /// </summary>
        [PublicAPI]
        public static bool? HasAnyCache()
        {
            if ( !IsServiceAttached() )
                return null;

            return PlayerPrefs.HasKey( GetServiceCacheKey() );
        }

        /// <summary>
        /// Registers a config service. If an already registered service exists, it will be replaced by the new one.
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
        /// Starts fetching process for remote configs.
        /// </summary>
        [PublicAPI]
        public static void Fetch()
        {
            if ( IsServiceAttached() )
            {
                if ( isFetching )
                    return;

                isFetching = true;
                remoteConfigsService.Fetch();
            }
            else
                OnFetchFail.Dispatch();
        }

        /// <summary>
        /// Returns all values downloaded from the remote as dictionary with configId.
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
        /// Applies values from remote to a specific config.
        /// </summary>
        /// <typeparam name="T">Config type to be applied</typeparam>
        [PublicAPI]
        public static void ApplyConfig<T>( ref T config ) where T : AbstractConfig
        {
            if ( IsServiceAttached() )
                remoteConfigsService?.ApplyConfig( ref config );
        }

        /// <summary>
        /// Applies values from remote to all configs.
        /// </summary>
        [PublicAPI]
        public static void ApplyAllConfigs()
        {
            if ( IsServiceAttached() )
                remoteConfigsService?.ApplyAllConfigs();
        }

        static void DisconnectCallbacks()
        {
            if ( remoteConfigsService == null )
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
            IsFetchHandled = true;
            isFetching = false;
        }

        static void HandleFetchComplete()
        {
            OnFetchComplete.Dispatch();
            IsFetchHandled = true;
            isFetching = false;

            if ( !remoteConfigsService.SupportsCaching )
                return;

            PlayerPrefs.SetInt( GetServiceCacheKey(), (int)( DateTime.UtcNow.ToFileTimeUtc() - UTC_OFFSET ) );
        }

        static bool IsServiceAttached()
        {
            if ( remoteConfigsService != null )
                return true;

            HLog.LogError( logPrefix,
                "No remote config service attached. Please attach remote service to use this feature" );
            return false;
        }

        static string GetServiceCacheKey()
        {
            return string.Format( CACHE_KEY, remoteConfigsService.UID );
        }
    }
}