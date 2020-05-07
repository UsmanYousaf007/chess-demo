using HUF.Analytics.Runtime.API;
using HUF.AnalyticsHBI.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AnalyticsHBI.Runtime.API
{
    public static class HAnalyticsHBI
    {
        static HBIAnalyticsService service;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if ( HConfigs.HasConfig<HBIAnalyticsConfig>() && HConfigs.GetConfig<HBIAnalyticsConfig>().AutoInit )
            {
                Init();
            }
        }

        /// <summary>
        /// Use this method to initialize HBI analytics service
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            service = new HBIAnalyticsService();
            HAnalytics.TryRegisterService( service );
        }

        /// <summary>
        /// Get UserId from HBI service
        /// </summary>
        [PublicAPI]
        public static string UserId => service != null ? service.UserId : string.Empty;
    }
}