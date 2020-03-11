using System.Linq;
using HUF.Utils.Assets.Editor;
using HUF.Utils.Editor.AssetsBuilder;
using HUF.Utils.Extensions;
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

            ReplaceAndroidManifestKeys();

        }
        
        public static void ReplaceAndroidManifestKeys()
        {
            var configs = AssetsUtils.GetByFilter<AndroidManifestKeysConfig>($"t: {nameof(AndroidManifestKeysConfig)}");
            if (configs.Count == 0)
                return;
            
            foreach (var config in configs)
            {
                if (config == null)
                    continue;
                
                if (config.PackageName.IsNullOrEmpty() && config.AutoUpdateAndroidManifest)
                    AndroidManifestKeyReplacer.CreateFinalManifest(config, config.AndroidManifestTemplatePath, config.AndroidManifestTemplatePath);

                if ( GetAndroidManifestTemplateDataToCopy( config, out string templatePath, out string savePath ) )
                {
                    AndroidManifestKeyReplacer.CreateFinalManifest( config, templatePath, $"{savePath}/{HUFBuildAssetsResolver.MANIFEST_FULL_NAME}");
                    HUFBuildAssetsResolver.CreateProjectPropertyFile( savePath );
                }
            }
        }

        static bool GetAndroidManifestTemplateDataToCopy(AndroidManifestKeysConfig config, out string templatePath, out string savePath)
        {
            templatePath = string.Empty;
            savePath = string.Empty;
            
            if ( config.PackageName.IsNullOrEmpty() )
                return false;

            var manifests = HUFBuildAssetsResolver.GetHUFManifests(HUFBuildAssetsResolver.MANIFEST_TEMPLATE_SEARCH, config.PackageName ).ToList();

            if ( !manifests.Any())
                return false;

            templatePath = manifests.First();
            savePath = HUFBuildAssetsResolver.GetAndroidManifestEndPath( templatePath );
            return true;
        }
        
    }
}