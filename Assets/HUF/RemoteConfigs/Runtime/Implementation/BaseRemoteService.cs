using System.Collections.Generic;
using System.Linq;
using HUF.RemoteConfigs.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine.Events;

namespace HUF.RemoteConfigs.Runtime.Implementation
{
    public abstract class BaseRemoteService : IRemoteConfigsService
    {
        public abstract bool IsInitialized { get; }

        public abstract event UnityAction OnInitComplete;
        public abstract event UnityAction OnFetchComplete;
        public abstract event UnityAction OnFetchFailed;

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