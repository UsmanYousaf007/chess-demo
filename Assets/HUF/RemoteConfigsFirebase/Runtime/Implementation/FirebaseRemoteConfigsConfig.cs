using HUF.RemoteConfigsFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.Runtime.Implementation
{
    [CreateAssetMenu( menuName = "HUF/RemoteConfigs/Firebase Remote Configs Config",
        fileName = "FirebaseRemoteConfigsConfig" )]
    public class FirebaseRemoteConfigsConfig : FeatureConfigBase
    {
        [SerializeField] bool enableInEditor = true;

        [SerializeField] int cacheExpirationInSeconds = 3600;

        public bool EnableInEditor => enableInEditor;

        public int CacheExpirationInSeconds => cacheExpirationInSeconds;

        public override void RegisterManualInitializers()
        {
            AddManualSynchronousInitializer( "Remote Configs - Firebase", HRemoteConfigsFirebase.Init );
        }
    }
}