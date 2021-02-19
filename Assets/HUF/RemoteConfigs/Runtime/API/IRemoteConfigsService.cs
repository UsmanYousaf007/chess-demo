using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine.Events;

namespace HUF.RemoteConfigs.Runtime.API
{
    public interface IRemoteConfigsService
    {
        bool HasCachedData { get; }
        bool IsInitialized { get; }
        bool SupportsCaching { get; }
        string UID { get; }

        event Action<RemoteConfigService> OnInitialized;
        event Action<RemoteConfigService> OnFetchComplete;
        event Action<RemoteConfigService> OnFetchFailed;

        void Fetch();
        Dictionary<string, string> GetConfigJSONs();
        string GetConfigJSON(string configId);
        void ApplyConfig<T>(ref T config) where T : AbstractConfig;
        void ApplyAllConfigs();
    }
}