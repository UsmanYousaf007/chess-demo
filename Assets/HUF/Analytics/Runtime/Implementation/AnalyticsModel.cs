using System.Collections.Generic;
using HUF.Analytics.API;
using HUF.Utils;
using HUF.Utils.PlayerPrefs;
using UnityEngine;

namespace HUF.Analytics.Implementation
{
    public class AnalyticsModel
    {
        readonly Dictionary<string, IAnalyticsService> services = new Dictionary<string, IAnalyticsService>();

        public bool TryRegisterService(IAnalyticsService service)
        {
            if (IsAlreadyRegistered(service))
            {
                Debug.LogWarningFormat($"[AnalyticsModel] Service {service.Name} already added to model.");
                return false;
            }

            services.Add(service.Name, service);
            service.Init();

            bool? consentValue = HAnalytics.GetGDPRConsent();

            if (consentValue.HasValue)
            {
                CollectSensitiveData(consentValue.Value,service.Name);
            }
            return true;
        }

        bool IsAlreadyRegistered(IAnalyticsService service)
        {
            return services.ContainsKey(service.Name);
        }

        public void LogEvent(Dictionary<string, object> analyticsParameters, params string[] serviceNames)
        {
            var analyticsEvent = AnalyticsEvent.Create(analyticsParameters);
            if (analyticsEvent != null)
                LogEvent(analyticsEvent, serviceNames);
        }

        public void LogEvent(AnalyticsEvent analyticsEvent, params string[] serviceNames)
        {
            if (serviceNames.Length > 0)
            {
                HLogs.LogFormatDebug("[AnalyticsModel] Log event: {0} to services: [{1}]",
                                     analyticsEvent, string.Join(",", serviceNames));
                foreach (var serviceName in serviceNames)
                {
                    LogEvent(analyticsEvent, serviceName);
                }
            }
            else
            {
                HLogs.LogFormatDebug("[AnalyticsModel] Log event: {0}", analyticsEvent);
                foreach (var service in services.Values)
                {
                    LogEvent(analyticsEvent, service.Name);
                }
            }
        }

        void LogEvent(AnalyticsEvent analyticsEvent, string serviceName)
        {
            services.TryGetValue(serviceName, out var service);

            if (service != null)
                service.LogEvent(analyticsEvent);
            else
                Debug.LogWarningFormat($"[AnalyticsModel] Can't find service {serviceName}");
        }

        public void LogMonetizationEvent(Dictionary<string, object> analyticsParameters, params string[] serviceNames)
        {
            if (!analyticsParameters.ContainsKey(AnalyticsMonetizationEvent.CENTS_KEY))
            {
                Debug.LogWarningFormat($"[AnalyticsModel] Missing {AnalyticsMonetizationEvent.CENTS_KEY} parameter");
                return;
            }

            if (!(analyticsParameters[AnalyticsMonetizationEvent.CENTS_KEY] is int cents))
            {
                Debug.LogWarningFormat($"[AnalyticsModel] {AnalyticsMonetizationEvent.CENTS_KEY} parameter is in wrong type");
                return;
            }

            var analyticsEvent = AnalyticsMonetizationEvent.Create(analyticsParameters, cents);

            if (analyticsEvent != null)
                LogMonetizationEvent(analyticsEvent, serviceNames);
        }

        public void LogMonetizationEvent(AnalyticsMonetizationEvent monetizationEvent, params string[] serviceNames)
        {
            if (serviceNames.Length > 0)
            {
                HLogs.LogFormatDebug("[AnalyticsModel] Log monetization event: {0} to services: [{1}]",
                                     monetizationEvent, string.Join(",", serviceNames));
                foreach (var serviceName in serviceNames)
                {
                    LogMonetizationEvent(monetizationEvent, serviceName);
                }
            }
            else
            {
                HLogs.LogFormatDebug("[AnalyticsModel] Log monetization event: {0}", monetizationEvent);
                foreach (var service in services.Values)
                {
                    LogMonetizationEvent(monetizationEvent, service.Name);
                }
            }
        }

        void LogMonetizationEvent(AnalyticsMonetizationEvent monetizationEvent, string serviceName)
        {
            services.TryGetValue(serviceName, out var service);

            if (service != null)
                service.LogMonetizationEvent(monetizationEvent);
            else
                Debug.LogWarningFormat($"[AnalyticsModel] Can't find service {serviceName}");
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            HLogs.LogFormatDebug("[AnalyticsModel] Set consent: {0}", consentStatus);
            foreach (var service in services.Values)
            {
                CollectSensitiveData(consentStatus, service.Name);
            }
        }

        void CollectSensitiveData(bool consentStatus, string serviceName)
        {
            services.TryGetValue(serviceName, out var service);

            if (service != null)
                service.CollectSensitiveData(consentStatus);
            else
                Debug.LogWarningFormat($"[AnalyticsModel] Can't find service {serviceName}");
        }
    }
}