using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.Implementation
{
    [CreateAssetMenu(menuName = "HUF/RemoteConfigs/FirebaseRemoteConfigsConfig",
        fileName = "FirebaseRemoteConfigsConfig.asset")]
    public class FirebaseRemoteConfigsConfig : FeatureConfigBase
    {
        [SerializeField] bool enableInEditor = true;

        [SerializeField] int cacheExpirationInSeconds = 3600;

        public bool EnableInEditor => enableInEditor;

        public int CacheExpirationInSeconds => cacheExpirationInSeconds;
    }
}