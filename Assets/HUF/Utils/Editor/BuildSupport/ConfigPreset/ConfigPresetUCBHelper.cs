#if UNITY_CLOUD_BUILD
using HUF.Utils.Runtime.BuildSupport.ConfigPreset;

namespace HUF.Utils.Editor.BuildSupport.ConfigPreset
{
    public static class ConfigPresetUCBHelper
    {
        public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
#if HUF_PRESET_CONFIG_DEBUG
            bool isDebugBuild = true;
#else
            bool isDebugBuild = false;
#endif
            ConfigPresets.ApplyConfigPresets(isDebugBuild);
        }
    }
}
#endif