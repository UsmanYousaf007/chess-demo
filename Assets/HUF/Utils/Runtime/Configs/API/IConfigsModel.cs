using System;
using System.Collections.Generic;

namespace HUF.Utils.Runtime.Configs.API
{
    public interface IConfigsModel
    {
        bool IsAutoInitEnabled { get; }
        T GetConfig<T>() where T : AbstractConfig;
        T GetConfig<T>(string configId) where T : AbstractConfig;
        bool HasConfig<T>() where T : AbstractConfig;
        bool HasConfig<T>(string configId) where T : AbstractConfig;
        IEnumerable<T> GetConfigs<T>() where T : AbstractConfig;
        IEnumerable<T> GetConfigsByBaseClass<T>() where T : AbstractConfig;
        void AddConfig<T>(T config) where T : AbstractConfig;
        void AddConfig<T>(T config, Type type) where T : AbstractConfig;
        void AddConfigs<T>(IEnumerable<T> configs) where T : AbstractConfig;
        void AddConfigsByOwnTypes<T>(IEnumerable<T> configs) where T : AbstractConfig;
        bool TryRemoveConfig<T>(string configId) where T : AbstractConfig;
        bool TryRemoveConfigs<T>() where T : AbstractConfig;
        void ResetConfigs();
    }
}