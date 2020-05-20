using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.Utils.Runtime.AndroidManifest
{
    public abstract class AndroidManifestKeysConfig : FeatureConfigBase
    {
        [SerializeField] bool autoUpdateAndroidManifest = false;
        
        public abstract string AndroidManifestTemplatePath { get; }
        
        public virtual string PackageName{ get; }

        public bool AutoUpdateAndroidManifest => autoUpdateAndroidManifest;
    }
}