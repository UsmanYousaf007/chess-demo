using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.StorageFirebase.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Storage/Firebase/FirebaseStorageConfig", fileName = "FirebaseStorageConfig.asset")]
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
    }
}