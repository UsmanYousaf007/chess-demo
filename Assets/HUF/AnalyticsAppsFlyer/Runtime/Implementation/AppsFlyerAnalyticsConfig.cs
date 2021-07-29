using System.Linq;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.Runtime.AndroidManifest;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    [CreateAssetMenu( fileName = nameof(AppsFlyerAnalyticsConfig), menuName = "HUF/Analytics/AppsFlyerConfig" )]
    public class AppsFlyerAnalyticsConfig : AndroidManifestKeysConfig
    {
        new static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AppsFlyerAnalyticsConfig) );

        [SerializeField] string devKey = default;

        [Header( "Android Settings" )]
        [SerializeField]
        bool buildForAmazon = false;

        [Header( "iOS Settings" )]
        [SerializeField]
        string iTunesAppId = default;

        public string DevKey => devKey;

        public override string PackageName => "AnalyticsAppsFlyer";

        public bool BuildForAmazon
        {
            get
            {
#if HUF_AMAZON
                return true;
#else
                return buildForAmazon;
#endif
            }
        }

        public string ITunesAppId
        {
            get
            {
                if ( iTunesAppId.IsNullOrEmpty() )
                {
                    HLog.LogError( logPrefix, "ITunesAppId is empty, fix before releasing!" );
                }

                if ( !iTunesAppId.All( char.IsDigit ) )
                {
                    HLog.LogError( logPrefix,
                        "ITunesAppId is not a number or has extra symbols, fix before releasing!" );
                }

                return iTunesAppId.Trim();
            }
        }

        public override string AndroidManifestTemplatePath =>
            "Assets/Plugins/Android/AnalyticsAppsFlyer/AndroidManifestTemplate.xml";

        [AndroidManifest( Tag = "meta-data", Attribute = "android:value", ValueToReplace = "amazon" ), UsedImplicitly]
        string BuildForAmazonString =>
            BuildForAmazon ? "amazon" : null;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Analytics - AppsFlyer", HAnalyticsAppsFlyer.Init );
        }
    }
}