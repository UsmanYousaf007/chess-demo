using HUF.Analytics.Runtime.API;
using JetBrains.Annotations;
using UnityEngine;
#if HUF_ANALYTICS_APPSFLYER_DUMMY
using AppsFlyerAnalyticsService = HUF.Analytics.Runtime.Implementation.DummyAnalyticsService;
#else
using HUF.Utils.Runtime.Configs.API;
using HUF.AnalyticsAppsFlyer.Runtime.Implementation;
using AppsFlyerAnalyticsService = HUF.AnalyticsAppsFlyer.Runtime.Implementation.AppsFlyerAnalyticsService;
#endif

namespace HUF.AnalyticsAppsFlyer.Runtime.API
{
    public static class HAnalyticsAppsFlyer
    {
        static AppsFlyerAnalyticsService service;

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
#if !HUF_ANALYTICS_APPSFLYER_DUMMY
            if ( HConfigs.HasConfig<AppsFlyerAnalyticsConfig>() &&
                 HConfigs.GetConfig<AppsFlyerAnalyticsConfig>().AutoInit )
            {
                Init();
            }
#endif
        }

        /// <summary>
        /// Use this method to initialize AppsFlyer analytics service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
#if HUF_ANALYTICS_APPSFLYER_DUMMY
            service = new AppsFlyerAnalyticsService( AnalyticsServiceName.APPS_FLYER );
#else
            service = new AppsFlyerAnalyticsService();
#endif
            HAnalytics.TryRegisterService( service );
        }

        /// <summary>
        /// Get application installation type <para />
        /// 1. NotSpecified - don't have information about installation <para />
        /// 2. Organic <para />
        /// 3. NonOrganic <para />
        /// </summary>
        [PublicAPI]
        public static InstallType InstallType
        {
#if HUF_ANALYTICS_APPSFLYER_DUMMY
            get => InstallType.NotSpecified;
            internal set {}
#else
            get => service?.InstallType ?? InstallType.NotSpecified;
            internal set
            {
                if ( service != null )
                    service.InstallType = value;
            }
#endif
        }

        /// <summary>
        /// Get AppsFlyer User Id
        /// </summary>
        [PublicAPI]
        public static string UserId => service == null ? string.Empty : service.UserId;

        /// <summary>
        /// Use this API during the SDK Initialization to explicitly anonymize a user's installs, events and sessions <para />
        /// Tracking can be restarted by calling it again with "false"
        /// </summary>
        [PublicAPI]
        public static void SetDeviceTrackingDisabled( bool isDisabled )
        {
#if !HUF_ANALYTICS_APPSFLYER_DUMMY
            AppsFlyer.setDeviceTrackingDisabled( isDisabled );
#endif
        }

    }
}