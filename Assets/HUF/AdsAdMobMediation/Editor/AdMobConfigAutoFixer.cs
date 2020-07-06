﻿using System.IO;
using System.Linq;
using GoogleMobileAds.Editor;
using HUF.AdsAdMobMediation.Runtime.API;
using HUF.AdsAdMobMediation.Runtime.Implementation;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUF.AdsAdMobMediation.Editor
{
    [UsedImplicitly]
    public class AdMobConfigAutoFixer : IPreprocessBuildWithReport
    {
        const string adMobConfigPlace = "Assets/GoogleMobileAds/Resources/";
        
        public int callbackOrder => -1;
        HLogPrefix logPrefix = new HLogPrefix( HAdsAdMobMediation.logPrefix, nameof(AdMobConfigAutoFixer) );
        
        
        public virtual void OnPreprocessBuild( BuildReport report )
        {
            AdMobProviderConfig config = Resources.LoadAll<AdMobProviderConfig>(string.Empty).FirstOrDefault();

            if ( config == null )
            {
                HLog.LogError( logPrefix, $"Missing {nameof(AdMobProviderConfig)}" );
                return;
            }

            if ( !Directory.Exists( adMobConfigPlace ) )
            {
                Directory.CreateDirectory( adMobConfigPlace );
            }
            var settingsInstance = GoogleMobileAdsSettings.Instance;

            settingsInstance.AdMobAndroidAppId =
                config.EditorApplicationIdentifier.EditorAndroidAppId;

            settingsInstance.AdMobIOSAppId =
                config.EditorApplicationIdentifier.EditoriOSAppId;
            settingsInstance.IsAdManagerEnabled = true;
            settingsInstance.IsAdMobEnabled = true;
            settingsInstance.DelayAppMeasurementInit = true;
            EditorUtility.SetDirty( settingsInstance );
            settingsInstance.WriteSettingsToFile();
        }
    }
}