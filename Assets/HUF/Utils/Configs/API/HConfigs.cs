using System;
using System.Collections.Generic;
using HUF.Utils.Configs.Implementation;
using JetBrains.Annotations;

namespace HUF.Utils.Configs.API
{
    public static class HConfigs
    {
        public const string CONFIGS_FOLDER = "HUFConfigs";
        
        static IConfigsModel configsModel;
        static IConfigsModel ConfigsModel => configsModel ?? (configsModel = new ConfigsModel());

        /// <summary>
        /// True if there is no Resources/HUFConfigs/ConfigsInitializationConfig.asset config file <para />
        /// or if it's created with AutoInit checkbox set to true, <para />
        /// False otherwise 
        /// </summary>
        [PublicAPI]
        public static bool IsAutoInitEnabled => ConfigsModel.IsAutoInitEnabled;
        
        /// <summary>
        /// Get AbstractConfig of given type
        /// </summary>
        [PublicAPI]
        public static T GetConfig<T>() where T : AbstractConfig
        {
            return ConfigsModel?.GetConfig<T>();
        }

        /// <summary>
        /// Get AbstractConfig of given type and specific ConfigId
        /// </summary>
        [PublicAPI]
        public static T GetConfig<T>(string configId) where T : AbstractConfig
        {
            return ConfigsModel?.GetConfig<T>(configId);
        }
        
        /// <summary>
        /// Check if there is any AbstractConfig of given type registered
        /// </summary>
        [PublicAPI]
        public static bool HasConfig<T>() where T : AbstractConfig
        {
            return ConfigsModel.HasConfig<T>();
        }

        /// <summary>
        /// Check if there is any AbstractConfig of given type and specific ConfigId registered
        /// </summary>
        [PublicAPI]
        public static bool HasConfig<T>(string configId) where T : AbstractConfig
        {
            return ConfigsModel.HasConfig<T>(configId);
        }
        
        /// <summary>
        /// Get all AbstractConfigs of given type
        /// </summary>
        [PublicAPI]
        public static IEnumerable<T> GetConfigs<T>() where T : AbstractConfig
        {
            return ConfigsModel?.GetConfigs<T>();
        }
        
        /// <summary>
        /// Get all AbstractConfigs of given type or derived from given type
        /// </summary>
        [PublicAPI]
        public static IEnumerable<T> GetConfigsByBaseClass<T>() where T : AbstractConfig
        {
            return ConfigsModel?.GetConfigsByBaseClass<T>();
        }

        /// <summary>
        /// Adds new config to configs map as specified type to be used in future.
        /// </summary>
        /// <param name="config">Config to be added</param>
        /// /// <param name="type">Type of config</param>
        [PublicAPI]
        public static void AddConfig<T>(T config, Type type) where T : AbstractConfig
        {
            ConfigsModel.AddConfig(config, type);
        }
        
        /// <summary>
        /// Adds new config to configs map to be used in future.
        /// </summary>
        /// <param name="config">Config to be added</param>
        [PublicAPI]
        public static void AddConfig<T>(T config) where T : AbstractConfig
        {
            ConfigsModel.AddConfig(config);
        }

        /// <summary>
        /// Adds new configs to map to be used in future.
        /// </summary>
        /// <param name="configsCollection">Collection of configs to be added</param>
        [PublicAPI]
        public static void AddConfigs<T>(IEnumerable<T> configsCollection) where T : AbstractConfig
        {
            ConfigsModel.AddConfigs(configsCollection);
        }
        
        /// <summary>
        /// Adds new configs to map to be used in future.
        /// </summary>
        /// <param name="configsCollection">Collection of configs to be added</param>
        [PublicAPI]
        public static void AddConfigsByOwnTypes<T>(IEnumerable<T> configsCollection) where T : AbstractConfig
        {
            ConfigsModel.AddConfigsByOwnTypes(configsCollection);
        }

        /// <summary>
        /// Tries to remove config with given id and type from map.
        /// Returns true if config is removed and false otherwise.
        /// </summary>
        /// <param name="configId">Config identificator</param>
        [PublicAPI]
        public static bool TryRemoveConfig<T>(string configId) where T : AbstractConfig
        {
            return ConfigsModel.TryRemoveConfig<T>(configId);
        }

        /// <summary>
        /// Tries to remove all configs with given type. Returns true if configs are removed and false otherwise.
        /// USE WITH CAUTION.
        /// </summary>
        [PublicAPI]
        public static bool TryRemoveConfigs<T>() where T : AbstractConfig
        {
            return ConfigsModel.TryRemoveConfigs<T>();
        }

        /// <summary>
        /// Remove all callbacks connected to every loaded config
        /// </summary>
        [PublicAPI]
        public static void ResetConfigs()
        {
            ConfigsModel.ResetConfigs();
        }
    }
}