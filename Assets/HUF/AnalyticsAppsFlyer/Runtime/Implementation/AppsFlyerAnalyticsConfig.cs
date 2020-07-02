using System.Linq;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    [CreateAssetMenu(fileName = nameof(AppsFlyerAnalyticsConfig), menuName = "HUF/Analytics/AppsFlyerConfig")]
    public class AppsFlyerAnalyticsConfig : FeatureConfigBase
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AppsFlyerAnalyticsConfig) );

        [SerializeField] string devKey = default;

        [Header("iOS Settings")]
        [SerializeField] string iTunesAppId = default;

        public string DevKey => devKey;

        public string ITunesAppId
        {
            get
            {
                if (iTunesAppId.IsNullOrEmpty())
                {
                    HLog.LogError( logPrefix, "ITunesAppId is empty, fix before releasing!");
                }
                if (!iTunesAppId.All(char.IsDigit))
                {
                    HLog.LogError( logPrefix,"ITunesAppId is not a number or has extra symbols, fix before releasing!");
                }
                return iTunesAppId.Trim();
            }
        }

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Analytics - AppsFlyer", HAnalyticsAppsFlyer.Init );
        }
    }
}