using System.Collections.Generic;
using HUF.Ads.Implementation;
using HUF.Utils.AndroidManifest;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AdsAdMobMediation.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Ads/AdMobProviderConfig", fileName = "AdMobProviderConfig.asset")]
    public class AdMobProviderConfig : AdsProviderConfig
    {
        public override string AndroidManifestTemplatePath =>
            "Assets/Plugins/Android/AdsAdMob/AndroidManifestTemplate.xml";

        [UsedImplicitly] 
        [AndroidManifest(Tag = "meta-data", Attribute = "android:value", ValueToReplace = "ADMOB_APP_ID")]
        string AppIdForManifest => AppId;

        [AndroidManifest(Tag = "meta-data", Attribute = "android:value", ValueToReplace = "APP_LOVIN_SDK_KEY")]
        [SerializeField] string appLovinSdkKey = default;

        public string AppLovinSdkKey => appLovinSdkKey;

        [SerializeField] List<string> testDevices = default;
        public List<string> TestDevices => testDevices;
    }
}