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
    public enum RemoteConfigService
    {
        Firebase = 0,
        HUF = 1,
        Other = 99
    }

    public static class HRemoteConfigs
    {
        const string CACHE_KEY = "HRC_Cache{0}";
        const long UTC_OFFSET = 1589440000;

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HRemoteConfigs) );

        static IRemoteConfigsService[] remoteConfigsService =
            new IRemoteConfigsService[Enum.GetNames( typeof(RemoteConfigService) ).Length];

        static bool[] fetchHandled = new bool[Enum.GetNames( typeof(RemoteConfigService) ).Length];
        static bool[] isFetching = new bool[Enum.GetNames( typeof(RemoteConfigService) ).Length];

        static RemoteConfigService? mainService = null;

        /// <summary>
        /// Raised when the service is fully initialized.
        /// </summary>
        [PublicAPI]
        public static event Action<RemoteConfigService> OnInitComplete;

        /// <summary>
        /// Raised when fetching is completed successfully.
        /// </summary>
        [PublicAPI]
        public static event Action<RemoteConfigService> OnFetchComplete;

        /// <summary>
        /// Raised when fetching fails.
        /// </summary>
        [PublicAPI]
        public static event Action<RemoteConfigService?> OnFetchFail;

        /// <summary>
        /// Returns last successful UTC cache refresh timestamp. Note that it is not a secure time.
        /// </summary>
        [PublicAPI]
        public static long? CacheTimestamp( RemoteConfigService? serviceType = null )
        {
            if ( !CheckForService( ref serviceType ) || !HasAnyCache( serviceType ) )
                return null;

            int timestamp = PlayerPrefs.GetInt( GetServiceCacheKey( (RemoteConfigService)serviceType ) );
            return UTC_OFFSET + timestamp;
        }

        /// <summary>
        /// Returns whether the service successfully handled fetching at least once.
        /// </summary>
        [PublicAPI]
        public static bool IsFetchHandled( RemoteConfigService? serviceType = null )
        {
            return CheckForService( ref serviceType, out int storageIndex ) &&
                   remoteConfigsService[storageIndex].HasCachedData;
        }

        /// <summary>
        /// Returns whether the service is fully initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized( RemoteConfigService? serviceType = null )
        {
            return CheckForService( ref serviceType, out int storageIndex ) &&
                   remoteConfigsService[storageIndex].IsInitialized;
        }

        /// <summary>
        /// Returns whether the service has cache present (RemoteConfigs was successfully fetched at least once).
        /// </summary>
        [PublicAPI]
        public static bool HasAnyCache( RemoteConfigService? serviceType = null )
        {
            return CheckForService( ref serviceType ) &&
                   PlayerPrefs.HasKey( GetServiceCacheKey( (RemoteConfigService)serviceType ) );
        }

        /// <summary>
        /// Registers a config service. If an already registered service exists, it will be replaced by the new one.
        /// Could be used to add custom <see cref="IRemoteConfigsService"/> implementation.
        /// </summary>
        /// <param name="configsService">Service to init</param>
        [PublicAPI]
        public static void RegisterService( IRemoteConfigsService configsService,
            RemoteConfigService serviceType,
            bool isMain )
        {
            if ( isMain || mainService == null )
                mainService = serviceType;
            var serviceId = (int)serviceType;

            if ( remoteConfigsService[serviceId] != null )
            {
                HLog.LogError( logPrefix, $"RemoteConfig service {serviceType} already initialized" );
                return;
            }

            remoteConfigsService[serviceId] = configsService;
            remoteConfigsService[serviceId].OnFetchFailed += HandleFetchFailed;
            remoteConfigsService[serviceId].OnFetchComplete += HandleFetchComplete;

            if ( configsService.IsInitialized )
            {
                OnInitComplete.Dispatch( serviceType );
                HLog.Log( logPrefix, $"RemoteConfig service {serviceType} initialized" );
            }
            else
                remoteConfigsService[serviceId].OnInitialized += HandleInitComplete;
        }

        /// <summary>
        /// Starts fetching process for remote configs.
        /// </summary>
        [PublicAPI]
        public static void Fetch( RemoteConfigService? serviceType = null )
        {
            if ( CheckForService( ref serviceType, out int storageIndex ) )
            {
                if ( isFetching[storageIndex] )
                    return;

                isFetching[storageIndex] = true;
                remoteConfigsService[storageIndex].Fetch();
            }
            else
                OnFetchFail.Dispatch( serviceType );
        }

        /// <summary>
        /// Returns all values downloaded from the remote as dictionary with configId.
        /// Returns values from remote if fetch was successful and empty dict in any other case.
        /// </summary>
        [PublicAPI]
        public static Dictionary<string, string> GetDictionary( RemoteConfigService? serviceType )
        {
            return CheckForService( ref serviceType, out int storageIndex )
                ? remoteConfigsService[storageIndex].GetConfigJSONs()
                : new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns raw config data for given key - config name.
        /// </summary>
        /// <param name="configId">Config Id <see cref="AbstractConfig.ConfigId"/></param>
        [PublicAPI]
        public static string GetValue( string configId, RemoteConfigService? serviceType = null )
        {
            return CheckForService( ref serviceType, out int storageIndex )
                ? remoteConfigsService[storageIndex].GetConfigJSON( configId )
                : string.Empty;
        }

        /// <summary>
        /// Applies values from remote to a specific config.
        /// </summary>
        /// <typeparam name="T">Config type to be applied</typeparam>
        [PublicAPI]
        public static void ApplyConfig<T>( ref T config, RemoteConfigService? serviceType = null )
            where T : AbstractConfig
        {
            if ( CheckForService( ref serviceType, out int storageIndex ) )
                remoteConfigsService[storageIndex].ApplyConfig( ref config );
            else
                HLog.LogWarning( logPrefix, $"ApplyConfig failed, because service {serviceType} is not initialized" );
        }

        /// <summary>
        /// Applies values from remote to all configs.
        /// </summary>
        [PublicAPI]
        public static void ApplyAllConfigs( RemoteConfigService? serviceType = null )
        {
            if ( CheckForService( ref serviceType, out int storageIndex ) )
                remoteConfigsService[storageIndex].ApplyAllConfigs();
            else
                HLog.LogWarning( logPrefix, $"ApplyConfig failed, because service {serviceType} is not initialized" );
        }

        static void HandleInitComplete( RemoteConfigService service )
        {
            OnInitComplete.Dispatch( service );
        }

        static void HandleFetchFailed( RemoteConfigService service )
        {
            OnFetchFail.Dispatch( service );
            isFetching[(int)service] = false;
        }

        static void HandleFetchComplete( RemoteConfigService service )
        {
            OnFetchComplete.Dispatch( service );
            fetchHandled[(int)service] = true;
            isFetching[(int)service] = false;

            if ( !remoteConfigsService[(int)service].SupportsCaching )
                return;

            PlayerPrefs.SetInt( GetServiceCacheKey( service ), (int)( DateTime.UtcNow.ToFileTimeUtc() - UTC_OFFSET ) );
        }

        static string GetServiceCacheKey( RemoteConfigService service )
        {
            return string.Format( CACHE_KEY, remoteConfigsService[(int)service].UID );
        }

        static bool CheckForService( ref RemoteConfigService? serviceType, out int storageIndex )
        {
            if ( serviceType == null )
                serviceType = mainService;
            storageIndex = serviceType != null ? (int)serviceType : -1;
            return storageIndex >= 0 && remoteConfigsService[storageIndex] != null;
        }

        static bool CheckForService( ref RemoteConfigService? serviceType )
        {
            if ( serviceType == null )
                serviceType = mainService;
            var storageIndex = serviceType != null ? (int)serviceType : -1;
            return storageIndex >= 0 && remoteConfigsService[storageIndex] != null;
        }
    }
}