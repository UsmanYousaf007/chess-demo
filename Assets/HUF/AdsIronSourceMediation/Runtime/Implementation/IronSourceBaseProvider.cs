using HUF.Ads.Runtime.API;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    public class IronSourceBaseProvider : IAdProvider
    {
        static HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(IronSourceBaseProvider) );

        readonly IronSourceAdsProviderConfig config;

        IAdsService adsService;

        public IronSourceBaseProvider()
        {
            config = HConfigs.GetConfig<IronSourceAdsProviderConfig>();
        }

        public string ProviderId => "IronSource";
        public bool IsInitialized { get; private set; }

        public bool Init()
        {
            if ( IsInitialized )
            {
                return false;
            }

            var ironSourceEvents = new GameObject( "IronSourceEvents" );
            ironSourceEvents.AddComponent<IronSourceEvents>();
            HLog.Log( logPrefix, $"Initializing IronSource version {IronSource.pluginVersion()}" );

#if HUF_ANALYTICS_HBI
            if ( HUF.AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId == string.Empty )
            {
                HLog.LogError( logPrefix, $"HDS user id is empty {HUF.AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId}" );
                HUF.Analytics.Runtime.API.HAnalytics.OnServiceInitializationComplete +=
                    HandleAnalyticsServiceInitializationComplete;
            }
            else
            {
                HLog.Log( logPrefix, $"HDS user id {HUF.AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId} set in Ironsource" );
                IronSource.Agent.setUserId( HUF.AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId );
            }
#else
            HLog.Log( logPrefix, "HDS not found, setting device id in Ironsource" );
            IronSource.Agent.setUserId( SystemInfo.deviceUniqueIdentifier );
#endif

            //Facebook.Unity.FB.Mobile.SetAdvertiserTrackingEnabled( HAds.HasPersonalizedAdConsent() == true );

            IronSource.Agent.setConsent( HAds.HasPersonalizedAdConsent() == true );
            IronSource.Agent.init(
                config.AppId,
                IronSourceAdUnits.BANNER,
                IronSourceAdUnits.INTERSTITIAL,
                IronSourceAdUnits.REWARDED_VIDEO );
            IronSource.Agent.setAdaptersDebug( Debug.isDebugBuild );

            if ( Debug.isDebugBuild )
                IronSource.Agent.validateIntegration();

            PauseManager.Instance.OnAppPause += HandleAppPause;
            IsInitialized = true;

            if ( adsService != null )
                adsService.ServiceInitialized();
            return true;
        }

#if HUF_ANALYTICS_HBI
        void HandleAnalyticsServiceInitializationComplete( string serviceName, bool didInitialize )
        {
            if ( !didInitialize )
                return;
            if ( serviceName == HUF.Analytics.Runtime.API.AnalyticsServiceName.HBI )
            {
                HUF.Analytics.Runtime.API.HAnalytics.OnServiceInitializationComplete -=
                    HandleAnalyticsServiceInitializationComplete;
                HLog.Log( logPrefix, $"HDS user id {HUF.AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId} set in Ironsource" );
                IronSource.Agent.setUserId( HUF.AnalyticsHBI.Runtime.API.HAnalyticsHBI.UserId );
            }
        }
#endif

        public void CollectSensitiveData( bool consentStatus )
        {
            IronSource.Agent.setConsent( consentStatus );
        }

        void HandleAppPause( bool pauseStatus )
        {
            IronSource.Agent.onApplicationPause( pauseStatus );
        }

        ~IronSourceBaseProvider()
        {
            PauseManager.Instance.OnAppPause -= HandleAppPause;
        }

        public void SetAdsService( IAdsService inAdsService )
        {
            adsService = inAdsService;

            if ( IsInitialized )
                adsService.ServiceInitialized();
        }
    }
}