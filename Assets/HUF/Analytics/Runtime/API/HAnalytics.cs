#define HUF_ANALYTICS

using System;
using System.Collections.Generic;
using HUF.Analytics.Implementation;
using HUF.Utils.PlayerPrefs;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Analytics.API
{
    public static class HAnalytics
    {
        const string ANALYTICS_CONSENT_SENSITIVE_DATA = "HUFAnalyticsConsentSensitiveData";
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
        /// set consent to all registered services <para/>
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        [PublicAPI]
        public static void CollectSensitiveData(bool consentStatus)
        {
            bool? previousValue = GetGDPRConsent();

            if (previousValue.HasValue && previousValue.Value == consentStatus)
            {
                return;
            }

            Debug.Log($"{ANALYTICS_CONSENT_SENSITIVE_DATA}: {consentStatus.ToString()}");
            HPlayerPrefs.SetBool(ANALYTICS_CONSENT_SENSITIVE_DATA, consentStatus);
            AnalyticsModel.CollectSensitiveData(consentStatus);
            OnCollectSensitiveDataSetEvent?.Invoke(consentStatus);
        }

        [PublicAPI]
        [Obsolete("Use `CollectSensitiveData` instead.")]
        public static void SetConsent(bool consentStatus)
        {
            CollectSensitiveData(consentStatus);
        }

        /// <summary>
        /// Returns consent for sending sensible user data to analytics services<para/>
        /// <returns>If no consent is set returns null. Returns consent value otherwise </returns>
        /// </summary>
        [PublicAPI]
        public static bool? GetGDPRConsent()
        {
            if (!HPlayerPrefs.HasKey(ANALYTICS_CONSENT_SENSITIVE_DATA))
            {
                return null;
            }
            return HPlayerPrefs.GetBool(ANALYTICS_CONSENT_SENSITIVE_DATA);
        }

        static event UnityAction<bool> OnCollectSensitiveDataSetEvent;

        /// <summary>
        /// Occurs when called CollectSensitiveData or SetConsent
        /// </summary>
        [PublicAPI]
        public static event UnityAction<bool> OnCollectSensitiveDataSet
        {
            add => OnCollectSensitiveDataSetEvent += value;
            remove => OnCollectSensitiveDataSetEvent -= value;
        }
    }
}