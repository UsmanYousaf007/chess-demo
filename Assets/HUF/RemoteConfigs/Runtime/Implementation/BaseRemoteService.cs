using System;
using System.Collections.Generic;
using System.Linq;
using HUF.RemoteConfigs.Runtime.API;
using HUF.Utils.Runtime.Configs.API;

namespace HUF.RemoteConfigs.Runtime.Implementation
{
    public abstract class BaseRemoteService : IRemoteConfigsService
    {
        public abstract bool HasCachedData { get; }
        public abstract bool IsInitialized { get; }
        public abstract bool SupportsCaching { get; }
        public abstract string UID { get; }

        public abstract event Action<RemoteConfigService> OnInitialized;
        public abstract event Action<RemoteConfigService> OnFetchComplete;
        public abstract event Action<RemoteConfigService> OnFetchFailed;

        public abstract void Fetch();

        public abstract Dictionary<string, string> GetConfigJSONs();

        public abstract string GetConfigJSON(string configId);

        public abstract void ApplyConfig<T>(ref T config) where T : AbstractConfig;

        public virtual void ApplyAllConfigs()
        {
            var configs = HConfigs.GetConfigsByBaseClass<AbstractConfig>().ToList();

            for (int i = 0; i < configs.Count; i++)
            {
                AbstractConfig config = configs[i];
                ApplyConfig(ref config);
            }
        }
    }

}