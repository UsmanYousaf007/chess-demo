using System.Linq;
using GoogleMobileAds.Editor;
using HUF.AdsAdMobMediation.API;
using HUF.AdsAdMobMediation.Implementation;
using HUF.Utils.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HUF.AdsAdMobMediation.Editor
{
    [UsedImplicitly]
    public class AdMobConfigAutoFixer : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1;

        HLogPrefix logPrefix = new HLogPrefix( HAdsAdMobMediation.logPrefix, nameof(AdMobConfigAutoFixer) );

        public virtual void OnPreprocessBuild( BuildReport report )
        {
            AdMobProviderConfig adsConfig = GetAdMobProviderConfig();

            if ( adsConfig == null )
            {
                HLog.LogError( logPrefix, "No Ads Config" );
                return;
            }

            var settingsInstance = GoogleMobileAdsSettings.Instance;

            settingsInstance.AdMobAndroidAppId =
                adsConfig.EditorApplicationIdentifier.EditorAndroidAppId;

            settingsInstance.AdMobIOSAppId =
                adsConfig.EditorApplicationIdentifier.EditoriOSAppId;
            settingsInstance.IsAdManagerEnabled = true;
            settingsInstance.IsAdMobEnabled = true;
            settingsInstance.DelayAppMeasurementInit = true;
            EditorUtility.SetDirty( settingsInstance );
            settingsInstance.WriteSettingsToFile();
        }

        static AdMobProviderConfig GetAdMobProviderConfig()
        {
            var files = AssetDatabase.FindAssets( "t:AdMobProviderConfig" );

            var configPath = files.Select( AssetDatabase.GUIDToAssetPath )
                .First( s => s.Contains( "Resources/HUFConfigs/" ) );
            return AssetDatabase.LoadAssetAtPath<AdMobProviderConfig>( configPath );
        }
    }
}