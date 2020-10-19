using System;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using HUF.Analytics.Runtime.API;
using HUF.Analytics.Runtime.Implementation;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Runtime.Implementation
{
    public class AppsFlyerAnalyticsService : IAnalyticsService
    {
        const string AF_CURRENCY_VALUE = "USD";

        static readonly HLogPrefix logPrefix = new HLogPrefix( HAnalyticsAppsFlyer.logPrefix, nameof(AppsFlyerAnalyticsService) );

        public string Name => AnalyticsServiceName.APPS_FLYER;
        public bool IsInitialized { private set; get; }

        readonly CustomPP<InstallType> installType =
            new CustomPP<InstallType>( "AppsFlyerAnalyticsService.InstallType", InstallType.NotSpecified );

        public InstallType InstallType
        {
            get => installType.Value;
            internal set => installType.Value = value;
        }

        public string UserId => AppsFlyer.getAppsFlyerId();

        public void Init( AnalyticsModel model )
        {
            var config = HConfigs.GetConfig<AppsFlyerAnalyticsConfig>();

            if ( config == null )
            {
                HLog.LogWarning( logPrefix, "Missing AppsFlyer Analytics config." );
                model?.CompleteServiceInitialization( Name, false );
                return;
            }

            HLog.Log( logPrefix, "Initializing..." );
            var didSetCustomerUserID = false;
#if HUF_ANALYTICS_HBI
            if ( !AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId.IsNullOrEmpty() )
            {
                HLog.Log( logPrefix, "Set HDS user Id" );
                AppsFlyer.setCustomerUserId( AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId );
                didSetCustomerUserID = true;
            }
#endif
            if ( !didSetCustomerUserID && !SystemInfo.deviceUniqueIdentifier.IsNullOrEmpty() )
                AppsFlyer.setCustomerUserId( SystemInfo.deviceUniqueIdentifier );
            AppsFlyer.setIsDebug( Debug.isDebugBuild );
            var callbacksClassName = AppsFlyerTrackerCallbacks.Instance.GetType().Name;
            MonoBehaviour callbacksObject = null;

            if ( AppsFlyerTrackerCallbacks.Instance != null )
            {
                AppsFlyerTrackerCallbacks.Instance.gameObject.name = callbacksClassName;

                callbacksObject =
                    AppsFlyerTrackerCallbacks.Instance.gameObject.GetComponent<AppsFlyerTrackerCallbacks>();
            }

            Initialize( config, callbacksObject );
            AppsFlyer.startSDK();
            IsInitialized = true;
            model?.CompleteServiceInitialization( Name, IsInitialized );
        }

#if UNITY_IOS
        static void Initialize( AppsFlyerAnalyticsConfig config, MonoBehaviour callbacks)
        {
            AppsFlyer.initSDK( config.DevKey, config.ITunesAppId, callbacks);
        }

#elif UNITY_ANDROID
        static void Initialize( AppsFlyerAnalyticsConfig config, MonoBehaviour callbacks )
        {
            AppsFlyer.initSDK( config.DevKey, null, callbacks );
        }

#else
        static void Initialize( AppsFlyerAnalyticsConfig config, MonoBehaviour callbacks )
        {
            HLog.LogWarning( logPrefix, $"Platform {Application.platform} is not supported." );
        }
#endif
        public void LogEvent( AnalyticsEvent analyticsEvent )
        {
            HLog.Log( logPrefix, $"LogEvent {analyticsEvent.EventName}" );
            AppsFlyer.sendEvent( analyticsEvent.EventName, GetParameters( analyticsEvent ) );
        }

        public void LogMonetizationEvent( AnalyticsMonetizationEvent analyticsEvent )
        {
            HLog.Log( logPrefix, $"LogMonetizationEvent {analyticsEvent.EventName}" );
            AppsFlyer.sendEvent( AFInAppEvents.PURCHASE,
                GetMonetizationParameters( analyticsEvent, analyticsEvent.Cents ) );
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HLog.Log( logPrefix, $"CollectSensitiveData {consentStatus}" );
            AppsFlyer.anonymizeUser( !consentStatus );
        }

        static Dictionary<string, string> GetParameters( AnalyticsEvent analyticsEvent )
        {
            return analyticsEvent.EventData.ToDictionary( q => q.Key, q => q.Value.ToString() );
        }

        static Dictionary<string, string> GetMonetizationParameters( AnalyticsEvent analyticsEvent, int cents )
        {
            var parameters = GetParameters( analyticsEvent );
            parameters[AFInAppEvents.REVENUE] = GetDollarsValue( cents ).ToString( "0.00" );
            parameters[AFInAppEvents.CURRENCY] = AF_CURRENCY_VALUE;
            return parameters;
        }

        static double GetDollarsValue( int cents )
        {
            return Math.Round( cents / 100f, 2, MidpointRounding.AwayFromZero );
        }
    }
}