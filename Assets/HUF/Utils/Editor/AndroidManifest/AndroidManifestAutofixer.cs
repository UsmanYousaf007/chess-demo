using System.Collections.Generic;
using System.Linq;
using HUF.Utils.Editor.BuildSupport.AssetsBuilder;
using HUF.Utils.Runtime.AndroidManifest;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUF.Utils.Editor.AndroidManifest
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
        
        [MenuItem( "HUF/Utils/Builds/Build Assets From Templates" )]
        public static void ReplaceAndroidManifestKeys()
        {
            var configs = Resources.LoadAll<AbstractConfig>("")
                .Select(x => x as AndroidManifestKeysConfig)
                .Where(x => x != null).ToList();
            
            if (configs.Count == 0)
                return;

            var configsString = new List<string>();
            for ( int i = 0; i < configs.Count; i++ )
            {
                configsString.Add( AssetDatabase.GetAssetPath( configs[i] ) );
            }
            
            foreach (var configString in configsString)
            {
                var config = (AndroidManifestKeysConfig) AssetDatabase.LoadAssetAtPath( configString, typeof(AndroidManifestKeysConfig));
                
                if (config == null)
                {
                    HLog.LogWarning( new HLogPrefix(nameof(AndroidManifestAutoFixer) ) , $"Could not load config {configString}" );
                    continue;
                }

                if (config.AndroidManifestTemplatePath.IsNullOrEmpty())
                    continue;
                
                if ( config.PackageName.IsNullOrEmpty())
                {
                    AndroidManifestKeyReplacer.CreateFinalManifest( config,
                        config.AndroidManifestTemplatePath,
                        config.AndroidManifestTemplatePath );
                }
                else if ( GetAndroidManifestTemplateDataToCopy( config, out string templatePath, out string savePath ) )
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