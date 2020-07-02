using System;
using System.Collections.Generic;
using System.Linq;
using HUF.Analytics.Runtime.API;
using HUF.Analytics.Runtime.Implementation;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    public class AppsFlyerAnalyticsService : IAnalyticsService
    {
        const string AF_PURCHASE_NAME = "af_purchase";
        const string AF_REVENUE_KEY = "af_revenue";
        const string AF_CURRENCY_KEY = "af_currency";
        const string AF_CURRENCY_VALUE = "USD";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof( AppsFlyerAnalyticsService ) );

        public string Name => AnalyticsServiceName.APPS_FLYER;
        public bool IsInitialized { private set; get; } = false;

        readonly CustomPP<InstallType> installType = new CustomPP<InstallType>( "AppsFlyerAnalyticsService.InstallType", InstallType.NotSpecified );
        
        public InstallType InstallType
        {
            get => installType.Value;
            internal set => installType.Value = value;
        }

        public string UserId => AppsFlyer.getAppsFlyerId();

        public void Init( AnalyticsModel model )
        {
            if ( Application.isEditor )
            {
                HLog.LogWarning( logPrefix, "Initialization is not available under Editor platform." );
                model?.CompleteServiceInitialization( Name, false );
                return;
            }
            
            var config = HConfigs.GetConfig<AppsFlyerAnalyticsConfig>();
            if ( config == null )
            {
                HLog.LogWarning( logPrefix, "Missing AppsFlyer Analytics config." );
                model?.CompleteServiceInitialization( Name, false );
                return;
            }

            HLog.Log( logPrefix, "Initializing..." );
            AppsFlyer.setCustomerUserID( SystemInfo.deviceUniqueIdentifier );
            AppsFlyer.setAppsFlyerKey( config.DevKey );
            AppsFlyer.setIsDebug( Debug.isDebugBuild );

            Initialize( config );

            IsInitialized = true;
            model?.CompleteServiceInitialization( Name, IsInitialized );
        }
#if UNITY_IOS
        static void Initialize( AppsFlyerAnalyticsConfig config )
        {
            AppsFlyer.setAppID( config.ITunesAppId );
            AppsFlyer.getConversionData();
            AppsFlyer.trackAppLaunch();
        }
        
#elif UNITY_ANDROID
        static void Initialize( AppsFlyerAnalyticsConfig config)
        {
            var callbacksClassName = AppsFlyerTrackerCallbacks.Instance.GetType().Name;
            if ( AppsFlyerTrackerCallbacks.Instance != null )
            {
                AppsFlyerTrackerCallbacks.Instance.gameObject.name = callbacksClassName;
            }
            AppsFlyer.setAppID( Application.identifier );
            AppsFlyer.init(config.DevKey, callbacksClassName);
        }
        
#else
        static void Initialize( AppsFlyerAnalyticsConfig config )
        {
            HLog.LogWarning( logPrefix, $"Platform {Application.platform} is not supported." );
        }
#endif
        public void LogEvent( AnalyticsEvent analyticsEvent )
        {
            AppsFlyer.trackRichEvent( analyticsEvent.EventName, GetParameters( analyticsEvent ) );
        }

        public void LogMonetizationEvent( AnalyticsMonetizationEvent analyticsEvent )
        {
            AppsFlyer.trackRichEvent( AF_PURCHASE_NAME, GetMonetizationParameters( analyticsEvent, analyticsEvent.Cents ) );
        }
        
        public void CollectSensitiveData( bool consentStatus )
        {
            AppsFlyer.stopTracking( !consentStatus );
        }

        static Dictionary<string, string> GetParameters( AnalyticsEvent analyticsEvent )
        {
            return analyticsEvent.EventData.ToDictionary( q => q.Key, q => q.Value.ToString() );
        }

        static Dictionary<string, string> GetMonetizationParameters( AnalyticsEvent analyticsEvent, int cents )
        {
            var parameters = GetParameters( analyticsEvent );
            parameters[AF_REVENUE_KEY] = GetDollarsValue( cents ).ToString( "0.00" );
            parameters[AF_CURRENCY_KEY] = AF_CURRENCY_VALUE;
            return parameters;
        }

        static double GetDollarsValue( int cents )
        {
            return Math.Round( ( float ) cents / 100, 2, MidpointRounding.AwayFromZero );
        }
    }
}