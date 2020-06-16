using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HUF.Utils.Runtime.Configs.Implementation
{
    [UsedImplicitly]
    public class ConfigsModel :
#if UNITY_EDITOR
        UnityEditor.AssetPostprocessor,
#endif
        IConfigsModel
    {
        public static readonly string configsPathInfo = Path.Combine( "Resources", HConfigs.CONFIGS_FOLDER );
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(ConfigsModel) );

        Dictionary<Type, List<AbstractConfig>> configMap;

        public bool IsAutoInitEnabled
        {
            get
            {
                var initializationConfig = Resources.Load<ConfigsInitializationConfig>(
                    Path.Combine(
                        HConfigs.CONFIGS_FOLDER,
                        ConfigsInitializationConfig.NAME ) );
                return initializationConfig == null || initializationConfig.AutoInit;
            }
        }

#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
        static bool shouldReload = true;

        static void OnPostprocessAllAssets( string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths )
        {
            if ( importedAssets.Any( path => path.Contains( HConfigs.CONFIGS_FOLDER ) )
                 || deletedAssets.Any( path => path.Contains( HConfigs.CONFIGS_FOLDER ) ) )
                shouldReload = true;
        }
#endif

        public void InitConfigsMap()
        {
            if ( IsAutoInitEnabled )
            {
                BuildConfigMap();
            }
            else
            {
                configMap = new Dictionary<Type, List<AbstractConfig>>();
            }
        }

        void BuildConfigMap()
        {
            var configsArray = Resources.LoadAll<AbstractConfig>( HConfigs.CONFIGS_FOLDER );

            if ( Application.isEditor )
            {
                for ( var i = 0; i < configsArray.Length; i++ )
                {
                    configsArray[i] = Object.Instantiate( configsArray[i] );
                }
            }

            configMap = new Dictionary<Type, List<AbstractConfig>>();

            foreach ( var config in configsArray )
            {
                var type = config.GetType();

                if ( !configMap.ContainsKey( type ) )
                    configMap[type] = new List<AbstractConfig>();
                configMap[type].Add( config );
                config.ValidateConfig();
            }

#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            shouldReload = false;
#endif
            HLog.Log( logPrefix, $"Applied {configMap.Count} configs from '{configsPathInfo}' folder out of {configsArray.Length}." );
        }

        public T GetConfig<T>() where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var type = typeof(T);

            if ( !configMap.ContainsKey( type ) )
            {
                HLog.LogError( logPrefix, $"Config of type: {type.Name} not found." );
                return null;
            }

            if ( configMap[type].Count > 1 )
                HLog.LogWarning( logPrefix, $"More than one occurrence of config with type: {type.Name}" );
            return configMap[type][0] as T;
        }

        public T GetConfig<T>( string configId ) where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var type = typeof(T);

            if ( !configMap.ContainsKey( type ) )
            {
                HLog.LogError( logPrefix, $"Config of type: {type.Name} not found." );
                return null;
            }

            var configsList = configMap[type].FindAll( x => x.ConfigId.Equals( configId ) );

            if ( configsList.Count == 0 )
            {
                HLog.LogError( logPrefix, $"Config with type: {type.Name} and configId: {configId} not found." );
                return null;
            }

            if ( configsList.Count > 1 )
                HLog.LogWarning( logPrefix, $"Multiple configs with same configID: {configId}" );
            return configsList.Last() as T;
        }

        public bool HasConfig<T>() where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var type = typeof(T);
            return configMap.ContainsKey( type );
        }

        public bool HasConfig<T>( string configId ) where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var type = typeof(T);
            return configMap.ContainsKey( type ) && configMap[type].Any( x => x.ConfigId.Equals( configId ) );
        }

        public IEnumerable<T> GetConfigs<T>() where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var type = typeof(T);

            if ( !configMap.ContainsKey( type ) || configMap[type].Count == 0 )
            {
                HLog.LogError( logPrefix, $"Configs of type: {type.Name} not found." );
                return new List<T>();
            }

            return configMap[type].Select( q => q as T );
        }

        public IEnumerable<T> GetConfigsByBaseClass<T>() where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var type = typeof(T);
            var configs = new List<AbstractConfig>();

            foreach ( var mapKeyType in configMap.Keys )
            {
                if ( type.IsAssignableFrom( mapKeyType ) )
                {
                    configs.AddRange( configMap[mapKeyType] );
                }
            }

            return configs.Select( q => q as T );
        }

        public void AddConfigs<T>( IEnumerable<T> configs ) where T : AbstractConfig
        {
            foreach ( var config in configs )
            {
                AddConfig( config );
            }
        }

        public void AddConfigsByOwnTypes<T>( IEnumerable<T> configs ) where T : AbstractConfig
        {
            foreach ( var config in configs )
            {
                AddConfig( config, config.GetType() );
            }
        }

        public void AddConfig<T>( T config ) where T : AbstractConfig
        {
            var type = typeof(T);
            AddConfigToMap( config, type );
        }

        public void AddConfig<T>( T config, Type type ) where T : AbstractConfig
        {
            AddConfigToMap( config, type );
        }

        void AddConfigToMap<T>( T config, Type type ) where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var configIndex = -1;

            if ( configMap.ContainsKey( type ) )
            {
                configIndex = configMap[type].FindIndex( q => q.ConfigId.Equals( config.ConfigId ) );
            }
            else
            {
                configMap.Add( type, new List<AbstractConfig>() );
            }

            if ( configIndex >= 0 )
            {
                Debug.LogWarning(
                    $"[{logPrefix.Prefix}] Config with type: {type.Name} and configId: {config.ConfigId} " +
                    "already found in config map. Replacing." );
                configMap[type][configIndex] = config;
            }
            else
            {
                configMap[type].Add( config );
            }
        }

        public bool TryRemoveConfigs<T>() where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var configType = typeof(T);

            if ( !configMap.ContainsKey( configType ) )
                return false;

            configMap.Remove( configType );
            return true;
        }

        public bool TryRemoveConfig<T>( string configId ) where T : AbstractConfig
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            var configType = typeof(T);

            if ( !configMap.ContainsKey( configType ) )
                return false;

            return configMap[configType].RemoveAll( q => q.ConfigId.Equals( configId ) ) > 0;
        }

        public void ResetConfigs()
        {
#if UNITY_EDITOR && !UNITY_CLOUD_BUILD
            if ( shouldReload )
                BuildConfigMap();
#endif
            foreach ( var pair in configMap )
            {
                foreach ( var config in pair.Value )
                {
                    config.ResetOnChanged();
                }
            }
        }
    }
}