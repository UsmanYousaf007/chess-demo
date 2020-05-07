using System.Collections.Generic;
using HUF.Ads.Runtime.Implementation;
using HUF.AdsAdMobMediation.Runtime.API;
using HUF.Utils.Runtime.AndroidManifest;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace HUF.AdsAdMobMediation.Runtime.Implementation
{
    [CreateAssetMenu( menuName = "HUF/Ads/AdMobProviderConfig", fileName = "AdMobProviderConfig" )]
    public class AdMobProviderConfig : AdsProviderConfig
    {
        [SerializeField] bool iOSPauseAppDuringAdPlay = true;
        
        [Header( "Optional Adapters" )]
        [SerializeField] string appLovinSdkKey =
            "4cGQClILnclNaflJM0vqqIM1cDbpyzU1amRnUk6FKrdwVNEsO7fCQ1D2EqrBnKCL4e7TMDph2QcXprOBz2OO4b";
        [FormerlySerializedAs( "moPubAppId" )] [SerializeField]
        ApplicationIdentifier moPub = default;


        [FormerlySerializedAs( "oguryAppId" )] [SerializeField]
        ApplicationIdentifier ogury = default;
        
        
        [Header( "Tests" )]
        [SerializeField] List<string> testDevices = default;


        public bool PauseAppDuringAdPlay => iOSPauseAppDuringAdPlay;
        public string AppLovinSdkKey => appLovinSdkKey;
        public string MoPubAppId => moPub.Value;
        public string OguryAppId => ogury.Value;
        public List<string> TestDevices => testDevices;

        
        public override string AndroidManifestTemplatePath =>
            "Assets/Plugins/Android/AdsAdMob/AndroidManifestTemplate.xml";

        public override string PackageName => "AdsAdMobMediation";
        
        [UsedImplicitly]
        [AndroidManifest( Tag = "meta-data", Attribute = "android:value", ValueToReplace = "ADMOB_APP_ID" )]
        string AppIdForManifest => AppId;

        [AndroidManifest( Tag = "meta-data", Attribute = "android:value", ValueToReplace = "APP_LOVIN_SDK_KEY" )]
        
        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "HAds AdMob", HAdsAdMobMediation.Init );
        }
    }
}