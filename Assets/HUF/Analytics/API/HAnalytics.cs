using System;
using System.Collections.Generic;
using HUF.Analytics.Implementation;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Analytics.API
{
    public static class HAnalytics
    {
        static AnalyticsModel analyticsModel;

        static AnalyticsModel AnalyticsModel => analyticsModel ?? (analyticsModel = new AnalyticsModel());

        /// <summary>
        /// Tries to register analytics service for future use. The service is automatically initialized
        /// after registration.
        /// </summary>
        /// <param name="service">Service to be registered</param>
        /// <returns>If service is registered correctly returns TRUE. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool TryRegisterService(IAnalyticsService service)
        {
            return AnalyticsModel.TryRegisterService(service);
        }

        /// <summary>
        /// Sends event to analytics services <para/>
        /// If no serviceNames provided - send event to all registered services [DEFAULT] <para/>
        /// If any serviceNames provided - send event only to these services <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/> <para/>
        /// </summary>
        /// <param name="analyticsEvent">Event to be sent </param>
        /// <param name="serviceNames">Set of target service names</param>
        [PublicAPI]
        public static void LogEvent(AnalyticsEvent analyticsEvent, params string[] serviceNames)
        {
            AnalyticsModel.LogEvent(analyticsEvent, serviceNames);
        }

        /// <summary>
        /// Sends event to analytics services <para/>
        /// Parameter with key "name" is required in dictionary, should store event name and can't be empty or null. <para />
        /// Supported types: bool, int, long, double or string <para />
        /// If no serviceNames provided - send event to all registered services [DEFAULT] <para/>
        /// If any serviceNames provided - send event only to these services <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/> <para/>
        /// </summary>
        /// <param name="analyticsParameters">Parameters send to analytics service </param>
        /// <param name="serviceNames">Set of target service names</param>
        [PublicAPI]
        public static void LogEvent(Dictionary<string, object> analyticsParameters, params string[] serviceNames)
        {
            AnalyticsModel.LogEvent(analyticsParameters, serviceNames);
        }

        /// <summary>
        /// Sends monetization event analytics services <para/>
        /// If no serviceNames provided - send event to all registered services [DEFAULT] <para/>
        /// If any serviceNames provided - send event only to these services <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/> <para/>
        /// </summary>
        /// <param name="monetizationEvent">Event to be sent </param>
        /// <param name="serviceNames">Set of target service names</param>
        [PublicAPI]
        public static void LogMonetizationEvent(AnalyticsMonetizationEvent monetizationEvent,
            params string[] serviceNames)
        {
            AnalyticsModel.LogMonetizationEvent(monetizationEvent, serviceNames);
        }

        /// <summary>
        /// Sends monetization event analytics services <para/>
        /// Parameter with key "name" is required in dictionary, should store event name and can't be empty or null. <para />
        /// Parameter with key "cents" is required in dictionary and should be int type
        /// Supported types: bool, int, long, double or string <para />
        /// If no serviceNames provided - send event to all registered services [DEFAULT] <para/>
        /// If any serviceNames provided - send event only to these services <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/> <para/>
        /// </summary>
        /// <param name="analyticsParameters">Parameters send to analytics service </param>
        /// <param name="serviceNames">Set of target service names</param>
        [PublicAPI]
        public static void LogMonetizationEvent(Dictionary<string, object> analyticsParameters,
            params string[] serviceNames)
        {
            AnalyticsModel.LogMonetizationEvent(analyticsParameters, serviceNames);
        }

        /// <summary>
        /// Sets consent for sending sensible user data to analytics services<para/>
        /// If no serviceNames provided - set consent to all registered services [DEFAULT] <para/>
        /// If any serviceNames provided - set consent only to these services <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/> <para/> 
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        /// <param name="serviceNames">Set of target service names</param>
        [PublicAPI]
        public static void CollectSensitiveData(bool consentStatus, params string[] serviceNames)
        {
            AnalyticsModel.CollectSensitiveData(consentStatus, serviceNames);
            OnCollectSensitiveDataSetEvent?.Invoke(consentStatus, serviceNames);
        }

        [PublicAPI]
        [Obsolete("Use `CollectSensitiveData` instead.")]
        public static void SetConsent(bool consentStatus, params string[] serviceNames)
        {
            CollectSensitiveData(consentStatus, serviceNames);
        }
        
        static event UnityAction<bool, string[]> OnCollectSensitiveDataSetEvent;
        
        /// <summary>
        /// Occurs when called CollectSensitiveData or SetConsent
        /// </summary>
        [PublicAPI]
        public static event UnityAction<bool, string[]> OnCollectSensitiveDataSet
        {
            add => OnCollectSensitiveDataSetEvent += value;
            remove => OnCollectSensitiveDataSetEvent -= value;
        }
    }
}