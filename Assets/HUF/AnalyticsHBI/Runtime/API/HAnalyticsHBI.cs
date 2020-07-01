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

        /// <summary>
        /// Use this method to initialize HDS analytics service
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
#if !HUF_ANALYTICS_HBI_DUMMY
            service = new HBIAnalyticsService();
#else
            service = new HBIAnalyticsService( AnalyticsServiceName.HBI );
#endif
            HAnalytics.TryRegisterService( service );
        }

        /// <summary>
        /// Get User ID from HDS service
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

        /// <summary>
        /// Get Session ID from HDS service
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

        static bool CheckInitialization()
        {
            if ( service != null && service.IsInitialized )
                return true;

            HLog.LogError( logPrefix, "Service not initialized." );
            return false;
        }
    }
}