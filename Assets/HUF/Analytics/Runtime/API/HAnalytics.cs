using System;
using System.Collections.Generic;
using HUF.Analytics.Runtime.Implementation;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Analytics.Runtime.API
{
    public static class HAnalytics
    {
        public static readonly HLogPrefix prefix = new HLogPrefix( nameof( HAnalytics ) );
        const string ANALYTICS_CONSENT_SENSITIVE_DATA = "HUFAnalyticsConsentSensitiveData";

        static AnalyticsModel analyticsModel;
        static AnalyticsModel AnalyticsModel => analyticsModel ?? ( analyticsModel = new AnalyticsModel() );

        /// <summary>
        /// Return true if consent key is set to any value.
        /// </summary>
        public static bool ConsentFlagExist => HPlayerPrefs.HasKey( ANALYTICS_CONSENT_SENSITIVE_DATA );

        /// <summary>
        /// Return true if flag exist and is set to true.
        /// </summary>
        public static bool IsGDPRAccepted => ConsentFlagExist && HPlayerPrefs.GetBool( ANALYTICS_CONSENT_SENSITIVE_DATA );

        /// <summary>
        /// Occurs when called CollectSensitiveData or SetConsent
        /// </summary>
        [PublicAPI]
        public static event UnityAction<bool> OnCollectSensitiveDataSet
        {
            add => AnalyticsModel.OnCollectSensitiveDataCallback += value;
            remove => AnalyticsModel.OnCollectSensitiveDataCallback -= value;
        }

        /// <summary>
        /// Occurs when any service complete initialization.
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string, bool> OnServiceInitializationComplete
        {
            add => AnalyticsModel.OnServiceInitializationComplete += value;
            remove => AnalyticsModel.OnServiceInitializationComplete -= value;
        }

        /// <summary>
        /// Tries to register analytics service for future use. The service is automatically initialized
        /// after registration.
        /// </summary>
        /// <param name="service">Service to be registered</param>
        /// <returns>If service is registered correctly returns TRUE. Returns FALSE otherwise.</returns>
        [PublicAPI]
        public static bool TryRegisterService( IAnalyticsService service )
        {
            try
            {
                AnalyticsModel.TryRegisterService( service );
            }
            catch ( Exception exception )
            {
                HLog.LogError( prefix, exception.ToString() );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to register analytics service for future use. The service is automatically initialized
        /// after registration.
        /// </summary>
        /// <param name="service">Service to be registered</param>
        /// <param name="callback">Callback invoked after initialization is finished regardless of the outcome</param>
        [PublicAPI]
        public static void TryRegisterService( IAnalyticsService service, Action callback )
        {
            if ( callback == null )
            {
                TryRegisterService( service );
                return;
            }

            void CheckInitialization( string serviceName, bool status )
            {
                if ( string.Equals( serviceName, service.Name ) )
                    HandleInitializationEnd();
            }

            void HandleInitializationEnd()
            {
                OnServiceInitializationComplete -= CheckInitialization;
                callback();
            }

            OnServiceInitializationComplete += CheckInitialization;

            if ( TryRegisterService( service ) )
                return;

            HandleInitializationEnd();
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
        public static void LogEvent( AnalyticsEvent analyticsEvent, params string[] serviceNames )
        {
            AnalyticsModel.LogEvent( analyticsEvent, serviceNames );
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
        public static void LogEvent( Dictionary<string, object> analyticsParameters, params string[] serviceNames )
        {
            AnalyticsModel.LogEvent( analyticsParameters, serviceNames );
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
        public static void LogMonetizationEvent( AnalyticsMonetizationEvent monetizationEvent,
                                                 params string[] serviceNames )
        {
            AnalyticsModel.LogMonetizationEvent( monetizationEvent, serviceNames );
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
        public static void LogMonetizationEvent( Dictionary<string, object> analyticsParameters,
                                                 params string[] serviceNames )
        {
            AnalyticsModel.LogMonetizationEvent( analyticsParameters, serviceNames );
        }

        /// <summary>
        /// Sets consent for sending sensible user data to analytics services<para/>
        /// set consent to all registered services <para/>
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        [PublicAPI]
        public static void CollectSensitiveData( bool consentStatus )
        {
            if ( ConsentFlagExist && consentStatus == IsGDPRAccepted )
            {
                HLog.Log( prefix, $"CollectSensitiveData({consentStatus}), value isn't changed." );
                return;
            }

            HPlayerPrefs.SetBool( ANALYTICS_CONSENT_SENSITIVE_DATA, consentStatus );
            AnalyticsModel.CollectSensitiveData( consentStatus );

            HLog.Log( prefix, $"CollectSensitiveData({consentStatus}), value changed." );
        }

        [PublicAPI]
        [Obsolete("Use `CollectSensitiveData` instead.")]
        public static void SetConsent( bool consentStatus )
        {
            CollectSensitiveData( consentStatus );
        }

        /// <summary>
        /// Returns consent for sending sensible user data to analytics services.
        /// <returns>If consent is not set returns null. Returns consent value otherwise.</returns>
        /// </summary>
        [PublicAPI]
        public static bool? GetGDPRConsent()
        {
            if ( !HPlayerPrefs.HasKey( ANALYTICS_CONSENT_SENSITIVE_DATA ) )
            {
                return null;
            }

            return HPlayerPrefs.GetBool( ANALYTICS_CONSENT_SENSITIVE_DATA );
        }
    }
}