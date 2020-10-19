using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime.Logging;
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
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAnalyticsAppsFlyer) );
        static AppsFlyerAnalyticsService service;

        /// <summary>
        /// Returns whether the AppsFlyer analytics service is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; }

        /// <summary>
        /// Gets AppsFlyer User Id.
        /// </summary>
        [PublicAPI]
        public static string UserId
        {
            get
            {
                if ( service == null )
                {
                    HLog.LogError( logPrefix, "Service not initialized" );
                    return string.Empty;
                }

                return service.UserId;
            }
        }

        /// <summary>
        /// Initializes the AppsFlyer analytics service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( IsInitialized )
                return;

#if HUF_ANALYTICS_APPSFLYER_DUMMY
            service = new AppsFlyerAnalyticsService( AnalyticsServiceName.APPS_FLYER );
#else
            service = new AppsFlyerAnalyticsService();
#endif
            IsInitialized = HAnalytics.TryRegisterService( service );
        }

        /// <summary>
        /// Gets application installation type. <para />
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
        /// Explicitly anonymizes a user's installs, events and sessions, when used during the SDK Initialization.
        /// Tracking can be restarted by calling it again with "false".
        /// </summary>
        [PublicAPI]
        public static void SetDeviceTrackingDisabled( bool isDisabled )
        {
            service?.CollectSensitiveData( !isDisabled );
        }

        /// <summary>
        /// Sets consent for sending sensitive user data to the analytics services.
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        [PublicAPI]
        public static void CollectSensitiveData( bool consentStatus )
        {
            service?.CollectSensitiveData( consentStatus );
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
#if !HUF_ANALYTICS_APPSFLYER_DUMMY
            if ( HConfigs.HasConfig<AppsFlyerAnalyticsConfig>() &&
                 HConfigs.GetConfig<AppsFlyerAnalyticsConfig>().AutoInit )
#endif
                Init();
        }
    }
}