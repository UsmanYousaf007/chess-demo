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
        public static readonly HLogPrefix prefix = new HLogPrefix( nameof(HAnalytics) );
        const string ANALYTICS_CONSENT_SENSITIVE_DATA = "HUFAnalyticsConsentSensitiveData";

        static AnalyticsModel analyticsModel;
        static AnalyticsModel AnalyticsModel => analyticsModel ?? ( analyticsModel = new AnalyticsModel() );

        /// <summary>
        /// Returns true if the consent key is set to any value.
        /// </summary>
        [PublicAPI]
        public static bool ConsentFlagExist => HPlayerPrefs.HasKey( ANALYTICS_CONSENT_SENSITIVE_DATA );

        /// <summary>
        /// Returns true if GDPR flag exists and is set to true.
        /// </summary>
        [PublicAPI]
        public static bool IsGDPRAccepted =>
            ConsentFlagExist && HPlayerPrefs.GetBool( ANALYTICS_CONSENT_SENSITIVE_DATA );

        /// <summary>
        /// Raised when CollectSensitiveData or SetConsent is called.
        /// </summary>
        [PublicAPI]
        public static event UnityAction<bool> OnCollectSensitiveDataSet
        {
            add => AnalyticsModel.OnCollectSensitiveDataCallback += value;
            remove => AnalyticsModel.OnCollectSensitiveDataCallback -= value;
        }

        /// <summary>
        /// Raised when any service completes initialization.
        /// </summary>
        [PublicAPI]
        public static event UnityAction<string, bool> OnServiceInitializationComplete
        {
            add => AnalyticsModel.OnServiceInitializationComplete += value;
            remove => AnalyticsModel.OnServiceInitializationComplete -= value;
        }

#if UNITY_IOS
        /// <summary>
        /// Raised when an AppTrackingTransparency authorization status is changed.
        /// <para>See <see cref="AppTrackingTransparencyBridge.AuthorizationStatus"/> for list of statuses.</para>
        /// </summary>
        [PublicAPI]
        public static event Action<AppTrackingTransparencyBridge.AuthorizationStatus> OnATTStatusChanged
        {
            add => AppTrackingTransparencyBridge.OnAuthorizationStatusChanged += value;
            remove => AppTrackingTransparencyBridge.OnAuthorizationStatusChanged -= value;
        }

        /// <summary>
        /// Checks AppTrackingTransparency status. When checked for the first time causes a system popup to appear.
        /// </summary>
        /// <param name="callback">A nullable callback with an authorization status.</param>
        [PublicAPI]
        public static void CheckATTStatus( Action<AppTrackingTransparencyBridge.AuthorizationStatus> callback )
        {
            AppTrackingTransparencyBridge.CheckAuthorizationStatus( callback );
        }

        /// <summary>
        /// Checks if the AppTrackingTransparency system popup was ever displayed.
        /// </summary>
        [PublicAPI]
        public static bool WasATTPopupDisplayed()
        {
            return AppTrackingTransparencyBridge.HasDoneInitialRequest;
        }
#endif

        /// <summary>
        /// Tries to register analytics service for future use. The service is automatically initialized
        /// after registration.
        /// </summary>
        /// <param name="service">Service to be registered.</param>
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
        /// <param name="service">Service to be registered.</param>
        /// <param name="callback">Callback invoked after initialization is sent regardless of the outcome.</param>
        [PublicAPI]
        public static bool TryRegisterService( IAnalyticsService service, Action callback )
        {
            if ( callback == null )
            {
                return TryRegisterService( service );
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
                return true;

            HandleInitializationEnd();
            return false;
        }

        /// <summary>
        /// Sends the event to the analytics services <para/>
        /// If no <paramref name="serviceNames"/> are provided - sends event to all registered services [DEFAULT]. <para/>
        /// If any <paramref name="serviceNames"/> are provided - sends event only to these services. <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/>. <para/>
        /// </summary>
        /// <param name="analyticsEvent">Event to be sent.</param>
        /// <param name="serviceNames">Set of target service names.</param>
        [PublicAPI]
        public static void LogEvent( AnalyticsEvent analyticsEvent, params string[] serviceNames )
        {
            AnalyticsModel.LogEvent( analyticsEvent, serviceNames );
        }

        /// <summary>
        /// Sends the event to the analytics services.<para/>
        /// Parameter with key "name" is required in dictionary, should store event name and can't be empty or null. <para />
        /// Supported types: bool, int, long, double or string. <para />
        /// If no <paramref name="serviceNames"/> are provided - sends event to all registered services [DEFAULT]. <para/>
        /// If any <paramref name="serviceNames"/> are provided - sends event only to these services. <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/>. <para/>
        /// </summary>
        /// <param name="analyticsEvent">Event to be sent.</param>
        /// <param name="serviceNames">Set of target service names.</param>
        [PublicAPI]
        public static void LogEvent( Dictionary<string, object> analyticsParameters, params string[] serviceNames )
        {
            AnalyticsModel.LogEvent( analyticsParameters, serviceNames );
        }

        /// <summary>
        /// Sends the monetization event to the analytics services. <para/>
        /// If no <paramref name="serviceNames"/> are provided - sends event to all registered services [DEFAULT]. <para/>
        /// If any <paramref name="serviceNames"/> are provided - sends event only to these services. <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/>. <para/>
        /// </summary>
        /// <param name="analyticsEvent">Event to be sent.</param>
        /// <param name="serviceNames">Set of target service names.</param>
        [PublicAPI]
        public static void LogMonetizationEvent( AnalyticsMonetizationEvent monetizationEvent,
            params string[] serviceNames )
        {
            AnalyticsModel.LogMonetizationEvent( monetizationEvent, serviceNames );
        }

        /// <summary>
        /// Sends the monetization event to the analytics services. <para/>
        /// Parameter with key "name" is required in dictionary, should store event name and can't be empty or null. <para />
        /// Supported types: bool, int, long, double or string <para />
        /// Parameter with key "cents" is required in dictionary and should be of int type <para />
        /// If no <paramref name="serviceNames"/> are provided - sends event to all registered services [DEFAULT]. <para/>
        /// If any <paramref name="serviceNames"/> are provided - sends event only to these services. <para/>
        /// Supported service names can be found as constants in <see cref="AnalyticsServiceName"/>. <para/>
        /// </summary>
        /// <param name="analyticsEvent">Event to be sent.</param>
        /// <param name="serviceNames">Set of target service names.</param>
        [PublicAPI]
        public static void LogMonetizationEvent( Dictionary<string, object> analyticsParameters,
            params string[] serviceNames )
        {
            AnalyticsModel.LogMonetizationEvent( analyticsParameters, serviceNames );
        }

        /// <summary>
        /// Sets consent for sending sensitive user data to the analytics services<para/>
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
        [Obsolete( "Use `CollectSensitiveData` instead." )]
        public static void SetConsent( bool consentStatus )
        {
            CollectSensitiveData( consentStatus );
        }

        /// <summary>
        /// Returns consent for sending sensitive user data to the analytics services.
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