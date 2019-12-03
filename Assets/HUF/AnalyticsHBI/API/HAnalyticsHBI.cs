using HUF.Analytics.API;
using HUF.AnalyticsHBI.Implementation;
using HUF.Utils.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AnalyticsHBI.API
{
    public static class HAnalyticsHBI
    {
        static HBIAnalyticsService service;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<HBIAnalyticsConfig>();
            if (hasConfig && HConfigs.GetConfig<HBIAnalyticsConfig>().AutoInit)
                Init();
        }

        /// <summary>
        /// Use this method to initialize HBI analytics service
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            service = new HBIAnalyticsService();
            HAnalytics.TryRegisterService(service);
        }

        /// <summary>
        /// Get UserId from HBI service
        /// </summary>
        [PublicAPI]
        public static string UserId => service.UserId;
    }
}