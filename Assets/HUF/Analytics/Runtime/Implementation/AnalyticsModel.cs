using System.Collections.Generic;
using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.Analytics.Runtime.Implementation
{
    public class AnalyticsModel
    {
        const int CACHE_CAPACITY = 50;
        
        readonly HLogPrefix prefix = new HLogPrefix( nameof( AnalyticsModel ) );
        readonly Dictionary<string, IAnalyticsService> services = new Dictionary<string, IAnalyticsService>();
        readonly Dictionary<string, List<AnalyticsEvent>> cache = new Dictionary<string, List<AnalyticsEvent>>( CACHE_CAPACITY );
        
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
            cache.Add( service.Name, new List<AnalyticsEvent>() );
            
            if ( HAnalytics.IsGDPRAccepted )
            {
                service.Init( this );
            }

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
            
            CollectSensitiveData( HAnalytics.IsGDPRAccepted, service );
            LogEventsFromCache( service );
            OnServiceInitializationComplete?.Invoke( serviceName, true );
            HLog.Log( prefix, $"Service {serviceName} initialization completed." );
        }
        
        public void LogEvent( Dictionary<string, object> analyticsParameters, params string[] serviceNames )
        {
            var analyticsEvent = AnalyticsEvent.Create( analyticsParameters );
            if ( analyticsEvent != null )
            {
                LogEvent( analyticsEvent, serviceNames );
            }
        }

        public void LogEvent( AnalyticsEvent analyticsEvent, params string[] serviceNames )
        {
            if ( serviceNames.Length > 0 )
            {
                foreach ( var serviceName in serviceNames )
                {
                    if ( services.TryGetValue( serviceName, out var service ) && !LogEvent( analyticsEvent, service ) )
                    {
                        HLog.Log( prefix, $"Event {analyticsEvent.EventName} cached for {serviceName}." );
                        cache[serviceName].Add( analyticsEvent );
                    }
                }
            }
            else if( services.Count > 0 )
            {
                foreach ( var service in services.Values )
                {
                    if ( !LogEvent( analyticsEvent, service ) )
                    {
                        HLog.Log( prefix, $"Event {analyticsEvent.EventName} cached for {service.Name}." );
                        cache[service.Name].Add( analyticsEvent );
                    }
                }
            }
        }
        
        bool LogEvent( AnalyticsEvent analyticsEvent, IAnalyticsService service )
        {
            if ( service == null || !service.IsInitialized )
            {
                return false;
            }

            HLog.Log( prefix, $"Log event: {analyticsEvent} to service: {service.Name}." );
            service.LogEvent( analyticsEvent );
            return true;
        }
        
        public void LogMonetizationEvent(Dictionary<string, object> analyticsParameters, params string[] serviceNames)
        {
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

        public void LogMonetizationEvent(AnalyticsMonetizationEvent monetizationEvent, params string[] serviceNames)
        {
            if ( serviceNames.Length > 0 )
            {
                foreach ( var serviceName in serviceNames )
                {
                    if ( services.TryGetValue( serviceName, out var service ) &&
                         !LogMonetizationEvent( monetizationEvent, service ) )
                    {
                        HLog.Log( prefix, $"Event {monetizationEvent.EventName} cached for {service.Name}." );
                        cache[service.Name].Add( monetizationEvent );
                    }
                }
            }
            else if( services.Count > 0 )
            {
                foreach ( var service in services.Values )
                {
                    if ( !LogMonetizationEvent( monetizationEvent, service ) )
                    {
                        HLog.Log( prefix, $"Event {monetizationEvent.EventName} cached for {service.Name}." );
                        cache[service.Name].Add( monetizationEvent );
                    }
                }
            }
        }

        bool LogMonetizationEvent(AnalyticsMonetizationEvent monetizationEvent, IAnalyticsService service )
        {
            if ( service == null || !service.IsInitialized )
            {
                return false;
            }

            HLog.Log( prefix, $"Log monetization event: {monetizationEvent}." );
            service.LogMonetizationEvent( monetizationEvent );
            return true;
        }
        
        void LogEventsFromCache( IAnalyticsService service )
        {
            if ( cache[service.Name].Count > 0 )
            {
                foreach ( var ev in cache[service.Name] )
                {
                    if ( ev is AnalyticsMonetizationEvent monetizationEvent )
                    {
                        LogMonetizationEvent( monetizationEvent, service );
                    }
                    else
                    {
                        LogEvent( ev, service );
                    }
                }
            }
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
            if ( service != null )
            {
                if ( service.IsInitialized )
                {
                    service.CollectSensitiveData( consentStatus );
                }
                else
                {
                    HLog.Log( prefix, $"Initializing service {service.Name}..." );
                    service.Init( this );
                }
            }
        }
    }
}