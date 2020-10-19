using HUF.Ads.Runtime.Implementation;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime.AndroidManifest;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    [CreateAssetMenu( menuName = "HUF/Ads/IronSourceAdsProviderConfig", fileName = "IronSourceAdsProviderConfig" )]
    public class IronSourceAdsProviderConfig : AdsProviderConfig
    {
        public override string PackageName => "IronSourceMediation";

        public override string AndroidManifestTemplatePath =>
            "Assets/Plugins/Android/AdsIronSourceMediation/AndroidManifestTemplate.xml";

        [Header( "Keys to be replaced in Android Manifest Templates" )]
        [AndroidManifest( Tag = "meta-data", Attribute = "android:value", ValueToReplace = "ADMOB_APP_ID" )]
        [SerializeField]
        string androidAdMobKey = default;

        [SerializeField] string iOSAdMobKey = default;

        [PublicAPI] public string AndroidAdMobKey => androidAdMobKey;
        [PublicAPI] public string IOSAdMobKey => iOSAdMobKey;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Ads - IronSource", HAdsIronSourceMediation.Init );
        }
    }
}