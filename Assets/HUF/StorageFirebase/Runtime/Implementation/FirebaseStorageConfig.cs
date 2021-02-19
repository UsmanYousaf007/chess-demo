using HUF.StorageFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.StorageFirebase.Runtime.Implementation
{
    [CreateAssetMenu( menuName = "HUF/Storage/Firebase/Firebase Storage Config", fileName = "FirebaseStorageConfig" )]
    public class FirebaseStorageConfig : FeatureConfigBase
    {
        [SerializeField] string databaseAddress = default;
        [SerializeField] bool isMainStorage = false;

        public string DatabaseAddress => databaseAddress;
        public bool IsMain => isMainStorage;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Storage - Firebase", () => HStorageFirebase.Initialize() );
        }
    }
}