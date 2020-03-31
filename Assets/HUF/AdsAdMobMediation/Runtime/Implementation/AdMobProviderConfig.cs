using System.Collections.Generic;
using HUF.Ads.Implementation;
using HUF.Utils.AndroidManifest;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace HUF.AdsAdMobMediation.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Ads/AdMobProviderConfig", fileName = "AdMobProviderConfig.asset")]
    public class AdMobProviderConfig : AdsProviderConfig
    {
        public override string AndroidManifestTemplatePath =>
            "Assets/Plugins/Android/AdsAdMob/AndroidManifestTemplate.xml";

        public override string PackageName => "AdsAdMobMediation";

        [UsedImplicitly] 
        [AndroidManifest(Tag = "meta-data", Attribute = "android:value", ValueToReplace = "ADMOB_APP_ID")]
        string AppIdForManifest => AppId;

        [AndroidManifest(Tag = "meta-data", Attribute = "android:value", ValueToReplace = "APP_LOVIN_SDK_KEY")]
        [SerializeField] string appLovinSdkKey = "4cGQClILnclNaflJM0vqqIM1cDbpyzU1amRnUk6FKrdwVNEsO7fCQ1D2EqrBnKCL4e7TMDph2QcXprOBz2OO4b";

        public string AppLovinSdkKey => appLovinSdkKey;

        [SerializeField] List<string> testDevices = default;
        public List<string> TestDevices => testDevices;
        
        
        [Header("Optional Adapters")]
        
        [FormerlySerializedAs( "moPubAppId" )] [SerializeField] ApplicationIdentifier moPub = default;
        public string MoPubAppId => moPub.Value;
        
        [FormerlySerializedAs( "oguryAppId" )] [SerializeField] ApplicationIdentifier ogury = default;
        public string OguryAppId => ogury.Value;

    }
}