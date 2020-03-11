using HUF.Utils.Wrappers.BuildSupport.ConfigPreset;
using UnityEditor;
#if UNITY_CLOUD_BUILD
namespace HUF.Utils.Wrappers.BuildSupport.ConfigPreset.Editor
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