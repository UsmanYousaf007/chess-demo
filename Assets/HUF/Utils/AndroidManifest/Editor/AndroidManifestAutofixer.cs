using HUF.Utils.Assets.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HUF.Utils.AndroidManifest.Editor
{
    [UsedImplicitly]
    public class AndroidManifestAutoFixer : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        static bool IsCorrectPlatform(BuildReport report)
        {
            return report.summary.platform == BuildTarget.Android;
        }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            if (!IsCorrectPlatform(report)) 
                return;

            var configs = AssetsUtils.GetByFilter<AndroidManifestKeysConfig>($"t: {nameof(AndroidManifestKeysConfig)}");
            if (configs.Count == 0)
                return;

            var manifestKeyReplacer = new AndroidManifestKeyReplacer();
            foreach (var config in configs)
            {
                if (config != null && config.AutoUpdateAndroidManifest)
                    manifestKeyReplacer.CreateFinalManifest(config);
            }
        }
        
    }
}