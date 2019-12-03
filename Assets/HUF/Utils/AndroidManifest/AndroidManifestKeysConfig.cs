using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.Utils.AndroidManifest
{
    public abstract class AndroidManifestKeysConfig : FeatureConfigBase
    {
        [SerializeField] bool autoUpdateAndroidManifest = false;
        
        public abstract string AndroidManifestTemplatePath { get; }

        public bool AutoUpdateAndroidManifest => autoUpdateAndroidManifest;
    }
}