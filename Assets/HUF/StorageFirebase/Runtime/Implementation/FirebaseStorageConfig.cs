using HUF.StorageFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.StorageFirebase.Runtime.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Storage/Firebase/Firebase Storage Config", fileName = "FirebaseStorageConfig")]
    public class FirebaseStorageConfig : FeatureConfigBase
    {
        [SerializeField] string databaseAddress = default;
        [SerializeField] bool autoInitDownloadService = true;
        [SerializeField] bool autoInitUploadService = true;
        [SerializeField] bool autoInitRemoveService = true;

        public string DatabaseAddress => databaseAddress;
        public bool AutoInitDownloadService => autoInitDownloadService;
        public bool AutoInitUploadService => autoInitUploadService;
        public bool AutoInitRemoveService => autoInitRemoveService;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Storage - Firebase Download", () => HStorageFirebase.TryInitDownloadService() );
            AddManualInitializer( "Storage - Firebase Remove", () => HStorageFirebase.TryInitRemoveService() );
            AddManualInitializer( "Storage - Firebase Upload", () => HStorageFirebase.TryInitUploadService() );
        }
    }
}