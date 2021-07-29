using HUF.Ads.Runtime.API;
using HUF.AdsIronSourceMediation.Runtime.Implementation;
using HUF.Analytics.Runtime.API;
using HUF.AnalyticsHBI.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using Facebook.Unity;

namespace HUF.AdsIronSourceMediation.Runtime.API
{
    public static class HAdsIronSourceMediation
    {
        const float DELAY_BETWEEN_CHECKING_FACEBOOK = 0.5f;

        public static readonly HLogPrefix logPrefix = new HLogPrefix( HAds.logPrefix, nameof(HAdsIronSourceMediation) );

        static bool isInitialized;
        static bool isInitializing;
        static IronSourceBannerProvider bannerProvider;

        /// <summary>
        /// Initializes the service.
        /// Should be called only if AutoInit option is disabled.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            var hasConfig = HConfigs.HasConfig<IronSourceAdsProviderConfig>();
            if ( !hasConfig)
            {
                HLog.LogError( logPrefix, "IronSource Config is Missing" );
                return;
            }

            if ( isInitializing || isInitialized )
            {
                return;
            }

            isInitializing = true;
            HAnalytics.OnServiceInitializationComplete += HandleAnalyticsInitializationComplete;
            CheckFacebookInit();
            CheckInitializationStatus();
        }

        /// <summary>
        /// Sets the next shown banner size.
        /// </summary>
        /// <param name="size">IronSourceBannerSize object (in dp; density-independent pixels): <para />
        /// <para> IronSourceBannerSize.BANNER - 320 x 50 <para />
        /// <para> IronSourceBannerSize.LARGE - 320 x 90 <para />
        /// <para> IronSourceBannerSize.RECTANGLE - 300 x 250 <para />
        /// <para> IronSourceBannerSize.SMART - depending on screen height and device <para />
        /// <para> iOS: If (iPhone) 320 x 50, if (iPad) 728 x 90 <para />
        /// <para> Android: If (screen height ≤ 720) 320 x 50, If (screen height > 720) 728 x 90 <para />
        /// By default it is set to IronSourceBannerSize.BANNER.</param>
        /// <returns>Returns true if operation finishes with success.</returns>
        [PublicAPI]
        public static bool SetBannerSize( IronSourceBannerSize size )
        {
            if ( bannerProvider == null )
            {
                HLog.LogWarning( logPrefix, $"Cannot set banner size, the banner ads provider is not initialized" );
                return false;
            }

            bannerProvider.SetBannerSize( size );
            return true;
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<IronSourceAdsProviderConfig>();

            if ( hasConfig && HConfigs.GetConfig<IronSourceAdsProviderConfig>().AutoInit )
            {
                Init();
            }
        }

        static void CheckInitializationStatus()
        {
            HLog.Log( logPrefix, $"CheckInitializationStatus {HAnalyticsHBI.IsInitialized} && {FB.IsInitialized}" );
            if ( HAnalyticsHBI.IsInitialized && FB.IsInitialized)
            {
                Initialize();
            }
        }

        static void HandleAnalyticsInitializationComplete( string serviceName, bool didInitialize )
        {
            if ( !didInitialize || serviceName != AnalyticsServiceName.HBI )
                return;

            HAnalytics.OnServiceInitializationComplete -= HandleAnalyticsInitializationComplete;
            CheckInitializationStatus();
        }

        static void CheckFacebookInit()
        {
            if ( !FB.IsInitialized && IntervalManager.Instance != null )
            {
                IntervalManager.Instance.RunWithDelay( CheckFacebookInit, DELAY_BETWEEN_CHECKING_FACEBOOK );
                return;
            }

            CheckInitializationStatus();
        }

        static void Initialize()
        {
            if ( isInitialized )
                return;

            var provider = new IronSourceBaseProvider();
            var adsService = HAds.Interstitial.RegisterAdProvider( new IronSourceInterstitialProvider( provider ) );
            HAds.Rewarded.RegisterAdProvider( new IronSourceRewardedProvider( provider ) );
            HAds.Banner.RegisterAdProvider( bannerProvider = new IronSourceBannerProvider( provider ) );
            isInitialized = true;
            provider.SetAdsService( adsService );
        }
    }
}