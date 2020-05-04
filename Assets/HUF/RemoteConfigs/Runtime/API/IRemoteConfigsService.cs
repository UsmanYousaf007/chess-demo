using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine.Events;

namespace HUF.RemoteConfigs.Runtime.API
{
    public interface IRemoteConfigsService
    {
        bool IsInitialized { get; }

        event UnityAction OnInitComplete;
        event UnityAction OnFetchComplete;
        event UnityAction OnFetchFailed;
        
        void Fetch();
        Dictionary<string, string> GetConfigJSONs();
        string GetConfigJSON(string configId);
        void ApplyConfig<T>(ref T config) where T : AbstractConfig;
        void ApplyAllConfigs();
    }
}