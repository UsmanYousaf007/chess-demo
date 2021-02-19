using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.Implementation;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.Configs.API
{
    public static class HConfigs
    {
        public const string CONFIGS_FOLDER = "HUFConfigs";

        static IConfigsModel configsModel;

        static IConfigsModel ConfigsModel
        {
            get
            {
                if ( configsModel != null )
                {
                    return configsModel;
                }

                configsModel = new ConfigsModel();
                configsModel.InitConfigsMap();
                return configsModel;
            }
        }

        /// <summary>
        /// <para>True if there is no Resources/HUFConfigs/ConfigsInitializationConfig.asset config file</para>
        /// <para>or if it is created with AutoInit checkbox set to true,</para>
        /// <para>False otherwise.</para>
        /// </summary>
        [PublicAPI]
        public static bool IsAutoInitEnabled => ConfigsModel.IsAutoInitEnabled;

        /// <summary>
        /// Gets AbstractConfig of a given type.
        /// </summary>
        [PublicAPI]
        public static T GetConfig<T>() where T : AbstractConfig
        {
            return ConfigsModel?.GetConfig<T>();
        }

        /// <summary>
        /// Gets AbstractConfig of a given type and with specific config ID.
        /// </summary>
        [PublicAPI]
        public static T GetConfig<T>( string configId ) where T : AbstractConfig
        {
            return ConfigsModel?.GetConfig<T>( configId );
        }

        /// <summary>
        /// Checks if there is any AbstractConfig of a given type registered.
        /// </summary>
        [PublicAPI]
        public static bool HasConfig<T>() where T : AbstractConfig
        {
            return ConfigsModel.HasConfig<T>();
        }

        /// <summary>
        /// Checks if there is any AbstractConfig of a given type and specific config ID registered.
        /// </summary>
        [PublicAPI]
        public static bool HasConfig<T>( string configId ) where T : AbstractConfig
        {
            return ConfigsModel.HasConfig<T>( configId );
        }

        /// <summary>
        /// Gets all AbstractConfigs of a given type.
        /// </summary>
        [PublicAPI]
        public static IEnumerable<T> GetConfigs<T>() where T : AbstractConfig
        {
            return ConfigsModel?.GetConfigs<T>();
        }

        /// <summary>
        /// Gets all AbstractConfigs of a given type or derived from given type.
        /// </summary>
        [PublicAPI]
        public static IEnumerable<T> GetConfigsByBaseClass<T>() where T : AbstractConfig
        {
            return ConfigsModel?.GetConfigsByBaseClass<T>();
        }

        /// <summary>
        /// Adds a new config to configs map as specified type to be used in future.
        /// </summary>
        /// <param name="config">A config to be added.</param>
        /// <param name="type">A type of config.</param>
        [PublicAPI]
        public static void AddConfig<T>( T config, Type type ) where T : AbstractConfig
        {
            ConfigsModel.AddConfig( config, type );
        }

        /// <summary>
        /// Adds a new config to configs map to be used in future.
        /// </summary>
        /// <param name="config">A config to be added.</param>
        [PublicAPI]
        public static void AddConfig<T>( T config ) where T : AbstractConfig
        {
            ConfigsModel.AddConfig( config );
        }

        /// <summary>
        /// Adds new configs to configs map to be used in the future.
        /// </summary>
        /// <param name="configsCollection">A collection of configs to be added.</param>
        [PublicAPI]
        public static void AddConfigs<T>( IEnumerable<T> configsCollection ) where T : AbstractConfig
        {
            ConfigsModel.AddConfigs( configsCollection );
        }

        /// <summary>
        /// Adds new configs to configs map to be used in the future.
        /// </summary>
        /// <param name="configsCollection">A collection of configs to be added.</param>
        [PublicAPI]
        public static void AddConfigsByOwnTypes<T>( IEnumerable<T> configsCollection ) where T : AbstractConfig
        {
            ConfigsModel.AddConfigsByOwnTypes( configsCollection );
        }

        /// <summary>
        /// Tries to remove the config with a given ID and type from configs map.
        /// Returns true if the config is removed and false otherwise.
        /// </summary>
        /// <param name="configId">A config ID.</param>
        [PublicAPI]
        public static bool TryRemoveConfig<T>( string configId ) where T : AbstractConfig
        {
            return ConfigsModel.TryRemoveConfig<T>( configId );
        }

        /// <summary>
        /// <para>Tries to remove all configs with a given type. Returns true if configs are removed and false otherwise.</para>
        /// <para>USE WITH CAUTION.</para>
        /// </summary>
        [PublicAPI]
        public static bool TryRemoveConfigs<T>() where T : AbstractConfig
        {
            return ConfigsModel.TryRemoveConfigs<T>();
        }

        /// <summary>
        /// Removes all callbacks connected to every loaded config.
        /// </summary>
        [PublicAPI]
        public static void ResetConfigs()
        {
            ConfigsModel.ResetConfigs();
        }
    }
}