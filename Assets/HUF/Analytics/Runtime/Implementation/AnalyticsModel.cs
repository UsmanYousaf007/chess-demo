using System.Collections.Generic;
using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.Analytics.Runtime.Implementation
{
    public class AnalyticsModel
    {
        readonly HLogPrefix prefix = new HLogPrefix( nameof(AnalyticsModel) );
        readonly Dictionary<string, IAnalyticsService> services = new Dictionary<string, IAnalyticsService>();
        readonly Dictionary<string, CacheService> cache = new Dictionary<string, CacheService>();
        int cacheSend = 0;

        public event UnityAction<bool> OnCollectSensitiveDataCallback;
        public event UnityAction<string, bool> OnServiceInitializationComplete;

        bool IsAlreadyRegistered( IAnalyticsService service )
        {
            return services.ContainsKey( service.Name );
        }

        public bool TryRegisterService( IAnalyticsService service )
        {
            if ( IsAlreadyRegistered( service ) )
            {
                HLog.LogWarning( prefix, $"Service {service.Name} is already registered." );

                if ( !service.IsInitialized )
                {
                    HLog.LogWarning( prefix, $"Service {service.Name} is not initialized." );
                }

                return false;
            }

            services.Add( service.Name, service );
            cache.Add( service.Name, new CacheService( service.Name ) );

            service.Init( this );

            return true;
        }

        public void CompleteServiceInitialization( string serviceName, bool status )
        {
            if ( status == false )
            {
                HLog.LogWarning( prefix, $"Unable to initialize {serviceName} service." );
                OnServiceInitializationComplete?.Invoke( serviceName, false );
                return;
            }

            services.TryGetValue( serviceName, out var service );

            if ( service == null )
            {
                HLog.LogWarning( prefix, $"Unable to find {serviceName} service." );
                OnServiceInitializationComplete?.Invoke( serviceName, false );
                return;
            }

            if ( HAnalytics.IsGDPRAccepted )
            {
                CollectSensitiveData( HAnalytics.IsGDPRAccepted, service );
            }

            OnServiceInitializationComplete?.Invoke( serviceName, true );
            HLog.Log( prefix, $"Service {serviceName} initialization completed." );
        }

        public void LogEvent( Dictionary<string, object> analyticsParameters, params string[] serviceNames )
        {
            if ( HAnalytics.GetGDPRConsent() == false)
                return;

            var analyticsEvent = AnalyticsEvent.Create( analyticsParameters );

            if ( analyticsEvent != null )
            {
                LogEvent( analyticsEvent, serviceNames );
            }
        }

        public void LogEvent( AnalyticsEvent analyticsEvent, params string[] serviceNames )
        {
            if ( HAnalytics.GetGDPRConsent() == false)
                return;

            if ( serviceNames.Length > 0 )
            {
                foreach ( var serviceName in serviceNames )
                {
                    if ( services.TryGetValue( serviceName, out var service ) )
                    {
                        LogEvent( analyticsEvent, service );
                    }
                }
            }
            else if ( services.Count > 0 )
            {
                foreach ( var service in services.Values )
                {
                    LogEvent( analyticsEvent, service );
                }
            }
        }

        void LogEvent( AnalyticsEvent analyticsEvent, IAnalyticsService service )
        {
            if ( service == null)
                return;

            if (!service.IsInitialized || HAnalytics.GetGDPRConsent() == null)
            {
                HLog.Log( prefix, $"Event {analyticsEvent.EventName} cached for {service.Name}." );
                cache[service.Name].AddEvent( analyticsEvent );
                return;
            }

            HLog.Log( prefix, $"Log event: {analyticsEvent} to service: {service.Name}." );
            service.LogEvent( analyticsEvent );
        }

        public void LogMonetizationEvent( Dictionary<string, object> analyticsParameters, params string[] serviceNames )
        {
            if ( HAnalytics.GetGDPRConsent() == false)
                return;

            if ( !analyticsParameters.ContainsKey( AnalyticsMonetizationEvent.CENTS_KEY ) )
            {
                HLog.LogWarning( prefix, $"Missing {AnalyticsMonetizationEvent.CENTS_KEY} parameter." );
                return;
            }

            if ( !( analyticsParameters[AnalyticsMonetizationEvent.CENTS_KEY] is int cents ) )
            {
                HLog.LogWarning( prefix, $"Wrong type of parameter: {AnalyticsMonetizationEvent.CENTS_KEY}." );
                return;
            }

            var analyticsEvent = AnalyticsMonetizationEvent.Create( analyticsParameters, cents );

            if ( analyticsEvent != null )
            {
                LogMonetizationEvent( analyticsEvent, serviceNames );
            }
        }

        public void LogMonetizationEvent( AnalyticsMonetizationEvent monetizationEvent, params string[] serviceNames )
        {
            if ( HAnalytics.GetGDPRConsent() == false)
                return;

            if ( serviceNames.Length > 0 )
            {
                foreach ( var serviceName in serviceNames )
                {
                    if ( services.TryGetValue( serviceName, out var service ) )
                    {
                        LogMonetizationEvent( monetizationEvent, service );
                    }
                }
            }
            else if ( services.Count > 0 )
            {
                foreach ( var service in services.Values )
                {
                    LogMonetizationEvent( monetizationEvent, service );
                }
            }
        }

        void LogMonetizationEvent( AnalyticsMonetizationEvent monetizationEvent, IAnalyticsService service )
        {
            if ( service == null)
                return;

            if (!service.IsInitialized || HAnalytics.GetGDPRConsent() == null)
            {
                HLog.Log( prefix, $"Event {monetizationEvent.EventName} cached for {service.Name}." );
                cache[service.Name].AddEvent( monetizationEvent );
                return;
            }

            HLog.Log( prefix, $"Log monetization event: {monetizationEvent}." );
            service.LogMonetizationEvent( monetizationEvent );
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HLog.Log( prefix, $"Set consent for all services to {consentStatus}." );

            foreach ( var service in services.Values )
            {
                CollectSensitiveData( consentStatus, service );
            }

            OnCollectSensitiveDataCallback?.Invoke( consentStatus );
        }

        void CollectSensitiveData( bool consentStatus, IAnalyticsService service )
        {
            if ( service == null || !service.IsInitialized )
                return;

            service.CollectSensitiveData( consentStatus );

            if ( consentStatus )
            {
                SendCachedEvents( service );
            }
        }

        void SendCachedEvents( IAnalyticsService service)
        {
            cache[service.Name].SendEvents( cacheSend++ * ( 1f / (cache.Count + 1)));
        }
    }
}