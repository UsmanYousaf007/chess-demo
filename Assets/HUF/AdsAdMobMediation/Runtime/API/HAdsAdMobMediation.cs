using HUF.Ads.Runtime.API;
using HUF.AdsAdMobMediation.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsAdMobMediation.Runtime.API
{
    public static class HAdsAdMobMediation
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( HAds.logPrefix, nameof(HAdsAdMobMediation) );
        static bool isInitialized;

        static AdMobProviderBase baseProvider;
        static AdMobBannerProvider bannerProvider;

        /// <summary>
        /// Raised after an ad is shown. Returns revenue data that should be sent to analytics services.
        /// </summary>
        [PublicAPI]
        public static event UnityAction<PaidEventData> OnPaidEvent;

        /// <summary>
        /// Initialize the service.
        /// Should be called only if the AutoInit option is disabled.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( isInitialized )
            {
                HLog.LogWarning( logPrefix, "Service already initialized" );
                return;
            }

            InstallProvider();
            isInitialized = true;
            HLog.Log( logPrefix, "Service initialized" );
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<AdMobProviderConfig>();

            if ( hasConfig && HConfigs.GetConfig<AdMobProviderConfig>().AutoInit )
                Init();
        }

        static void InstallProvider()
        {
            baseProvider = new AdMobProviderBase();
            var adsService = HAds.Banner.RegisterAdProvider( bannerProvider = new AdMobBannerProvider( baseProvider ) );
            HAds.Interstitial.RegisterAdProvider( new AdMobInterstitialProvider( baseProvider ) );
            HAds.Rewarded.RegisterAdProvider( new AdMobRewardedProvider( baseProvider ) );
            baseProvider.SetAdsService( adsService );
            baseProvider.OnPaidEvent += HandPaidEvent;
        }

        static void HandPaidEvent( PaidEventData eventData )
        {
            OnPaidEvent.Dispatch( eventData );
        }

        /// <summary>
        /// Shows Admob's Mediation test suite.
        /// It's useful to check all the ad networks integration.
        /// </summary>
        [PublicAPI]
        public static void ShowTestSuite()
        {
            if ( isInitialized && baseProvider != null )
            {
                baseProvider.ShowTestSuite();
            }
            else
            {
                HLog.LogError( logPrefix, "The AdMob is not initialized yet, can't show test suite" );
            }
        }

        /// <summary>
        /// Enables a test mode for current device. Called automatically for debug builds.
        /// </summary>
        [PublicAPI]
        public static void EnableTestMode()
        {
            if (isInitialized && baseProvider != null)
            {
                baseProvider.EnableTestMode();
            }
            else
            {
                HLog.LogError(logPrefix, "The AdMob is not initialized yet, can't enable test mode");
            }
        }
        
        /// <summary>
        /// Sets the next shown banner size.
        /// </summary>
        /// <param name="size">GoogleMobileAds.Api.AdSize object (in dp; density-independent pixels):<para />
        /// <para> AdSize.Banner - 320 x 50 <para />
        /// <para> AdSize.Smart - depending on screen height Screen width x 32|50|90 <para />
        /// <para> AdSize.MediumRectangle - 300 x 250 <para />
        /// <para> AdSize.IABBanner - 468 x 60 <para />
        /// <para> AdSize.Leaderboard - 728 x 90 <para />
        /// <para> AdSize.LargeBanner - 320 x 100 <para />
        /// The default setup is AdSize.Smart</param>
        /// <returns>Returns true if operation finished with success.</returns>
        [PublicAPI]
        public static bool SetBannerSize( AdMobBannerAdSize size )
        {
            if ( bannerProvider == null )
            {
                HLog.LogWarning( logPrefix, "Can't set banner size, the banner ads provider is not initialized" );
                return false;
            }

            bannerProvider.SetBannerSize( size );
            return true;
        }
    }
}