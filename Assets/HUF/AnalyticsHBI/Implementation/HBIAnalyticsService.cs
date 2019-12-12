using System;
using System.Collections.Generic;
using HUF.Analytics.API;
using HUF.Utils.Configs.API;
using UnityEngine;
using HBIAnalytics = huuuge.Analytics;

namespace HUF.AnalyticsHBI.Implementation
{
    public class HBIAnalyticsService : IAnalyticsService
    {
        readonly string className = typeof(HBIAnalyticsService).Name;
        
        HBIAnalytics hbi;

        public string Name => AnalyticsServiceName.HBI;

        public void Init()
        {
            var config = HConfigs.GetConfig<HBIAnalyticsConfig>();

            if (!HasCorrectSettings(config))
                return;
            
            hbi = new HBIAnalytics(config.ProjectName, config.Sku, Debug.isDebugBuild);

            Debug.Log($"[{className}] UserID: {UserId}");
        }
        
        public string UserId => IsHBIInstantiated() ? hbi.UserId() : String.Empty;

        bool HasCorrectSettings(HBIAnalyticsConfig config)
        {
            var configType = typeof(HBIAnalyticsConfig).Name;
            
            if (config == null)
            {
                Debug.LogError($"[{className}] Can't find {configType}");
                return false;
            }
            
            if (config.ProjectName.Length > HBIAnalyticsConfig.MaxProjectLength)
            {
                Debug.LogError($"[{className}] Project name is too long - check {configType}");
                return false;
            }
            
            if(config.Sku.Length > HBIAnalyticsConfig.MaxSKULength)
            {
                Debug.LogError($"[{className}] SKU is too long - check {configType}");
                return false;
            }
            
            return true;
        }
        
        public void LogEvent(AnalyticsEvent analyticsEvent)
        {
            if (!IsHBIInstantiated())
                return;
            
            var parameters = GetHBIConvertedParameters(analyticsEvent.EventData);
            hbi.LogEvent(parameters);
        }

        public void LogMonetizationEvent(AnalyticsMonetizationEvent analyticsEvent)
        {
            if (!IsHBIInstantiated())
                return;
            
            var parameters = GetHBIConvertedParameters(analyticsEvent.EventData);
            hbi.LogMonetizeEvent(parameters, analyticsEvent.Cents);
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            if (!IsHBIInstantiated())
                return;
            
            hbi.AllowSendSensitiveData(consentStatus);    
        }

        Dictionary<string, object> GetHBIConvertedParameters(Dictionary<string, object> input)
        {
            var valueKey = AnalyticsEvent.EventConsts.VALUE_KEY;
            
            if (input.ContainsKey(valueKey) && !IsIntegerLikeType(input[valueKey]))
            {
                Debug.LogError($"[{className}] Value parameter could be only int/long/uint type");
                var output = new Dictionary<string, object>(input);
                output.Remove(valueKey);
                return output;
            }
            
            return input;
        }

        bool IsIntegerLikeType(object value)
        {
            return value is int ||
                   value is long ||
                   value is uint;
        }

        bool IsHBIInstantiated()
        {
            if (hbi == null)
                Debug.LogError($"[{className}] HBIAnalytics instance is not created");

            return hbi != null;
        }
    }
}