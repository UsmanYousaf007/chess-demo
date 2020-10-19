using HUF.Analytics.Runtime.API;
#if !HUF_ANALYTICS_HBI_DUMMY
using HUF.AnalyticsHBI.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
#else
using HBIAnalyticsService = HUF.Analytics.Runtime.Implementation.DummyAnalyticsService;
using HUF.Analytics.Runtime.Implementation;
#endif
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AnalyticsHBI.Runtime.API
{
    public static class HAnalyticsHBI
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAnalyticsHBI) );
        static HBIAnalyticsService service;

        /// <summary>
        /// Returns whether HAnalyticsHBI is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; } = false;

        /// <summary>
        /// Gets Session ID from HDS service.
        /// </summary>
        [PublicAPI]
        public static string SessionId
        {
            get
            {
                if ( CheckInitialization() )
                    return service.SessionId;

                return string.Empty;
            }
        }

        /// <summary>
        /// Initializes the HDS analytics service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( IsInitialized )
                return;

#if !HUF_ANALYTICS_HBI_DUMMY
            service = new HBIAnalyticsService();
#else
            service = new HBIAnalyticsService( AnalyticsServiceName.HBI );
#endif
            IsInitialized = HAnalytics.TryRegisterService( service );
        }

        /// <summary>
        /// Gets User ID from HDS service.
        /// </summary>
        [PublicAPI]
        public static string UserId
        {
            get
            {
                if ( CheckInitialization() )
                    return service.UserId;

                return string.Empty;
            }
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
#if !HUF_ANALYTICS_HBI_DUMMY
            if ( HConfigs.HasConfig<HBIAnalyticsConfig>() && HConfigs.GetConfig<HBIAnalyticsConfig>().AutoInit )
#endif
            {
                Init();
            }
        }

        static bool CheckInitialization()
        {
            if ( service != null && service.IsInitialized )
                return true;

            HLog.LogError( logPrefix, "Service not initialized." );
            return false;
        }
    }
}