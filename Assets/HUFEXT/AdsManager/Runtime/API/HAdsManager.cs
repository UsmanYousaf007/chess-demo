using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdManagers;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.API
{
    public static class HAdsManager
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAdsManager) );

        static AdsManagerConfig config;
        static HUFAdsService adsService;

        static UnityAction<AdsManagerFetchCallbackData> OnAdFetchStatic;

        static AdsManagerConfig Config
        {
            get
            {
                if ( config == null && HConfigs.HasConfig<AdsManagerConfig>() )
                    config = HConfigs.GetConfig<AdsManagerConfig>();
                return config;
            }
        }

        /// <summary>
        /// Occurs when any ad is Fetched.
        /// </summary>
        [PublicAPI]
        public static event UnityAction<AdsManagerFetchCallbackData> OnAdFetch
        {
            add
            {
                if ( adsService == null )
                {
                    OnAdFetchStatic += value;
                    return;
                }

                adsService.OnAdFetch += value;
            }
            remove
            {
                if ( adsService == null )
                {
                    OnAdFetchStatic -= value;
                    return;
                }

                adsService.OnAdFetch -= value;
            }
        }

        /// <summary>
        /// Automatically initializes HUF Ads manager.
        /// </summary>
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( Config != null && Config.AutoInit )
                Init();
        }

        /// <summary>
        /// Initializes HUF Ads manager.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( Config != null )
            {
                adsService = new HUFAdsService();

                if ( OnAdFetchStatic != null )
                {
                    adsService.OnAdFetch = OnAdFetchStatic;
                    OnAdFetchStatic = null;
                }
            }
            else
                HLog.LogError( logPrefix, $"AdsManagerConfig dose not exits" );
        }

        /// <summary>
        /// Use to check if Ads Manager is initialized.
        /// </summary>
        /// <returns>Status of initialization</returns>
        [PublicAPI]
        public static bool IsInitialized()
        {
            return adsService != null;
        }

        /// <summary>
        /// Use this to chcek if an add is loaded and ready to show. (Banners are always ready to show).
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <returns>Ad status if it is ready to show</returns>
        [PublicAPI]
        public static bool CanShowAd( string placementId )
        {
            return adsService != null && adsService.CanShowAd( placementId );
        }

        /// <summary>
        /// Call to show ad, if ad is not ready it will response on callback with false.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Shown ads status</param>
        [PublicAPI]
        public static void ShowAd( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
                return;
            }

            adsService.ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Use to set default banner position. it apply to all new banners and not destroy/move old ones.
        /// </summary>
        /// <param name="position">new position</param>
        /// <returns>if ads manager is not initialized it will return false</returns>
        [PublicAPI]
        public static bool SetNewBannerPosition( BannerPosition position )
        {
            if ( adsService == null )
            {
                return false;
            }

            adsService.SetBannerPosition( position );
            return true;
        }

        /// <summary>
        /// Use to hide banner ad.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        public static void HideBanner( string placementId )
        {
            adsService?.HideBanner( placementId );
        }

        /// <summary>
        /// Use to hide banner ad
        /// </summary>
        [PublicAPI]
        public static void HideBanner()
        {
            adsService?.HideBanner();
        }

        /// <summary>
        /// Use to show first banner ad from config on specific position - will apply to all
        /// new banners like SetNewBannerPosition.
        /// </summary>
        /// <param name="resultCallback">Did banner show correct</param>
        /// <param name="position">New position</param>
        public static void ShowBanner( UnityAction<AdManagerCallback> resultCallback,
            BannerPosition position = BannerPosition.BottomCenter )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", "NONE", AdResult.Failed ) );
                return;
            }

            adsService.ShowBanner( resultCallback, position );
        }

        /// <summary>
        /// Use to show banner ad on specific position  - will apply to all new banners like SetNewBannerPosition.
        /// </summary>
        /// /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Did banner show correct</param>
        /// <param name="position">New position</param>
        [PublicAPI]
        public static void ShowBanner(
            string placementId,
            UnityAction<AdManagerCallback> resultCallback,
            BannerPosition position = BannerPosition.BottomCenter )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
                return;
            }

            adsService.ShowBanner( placementId, resultCallback, position );
        }

        /// <summary>
        /// Use to show banner ad on specific position  - will apply to all new banners like SetNewBannerPosition.
        /// </summary>
        /// /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Did banner show correct</param>
        [PublicAPI]
        public static void ShowBanner( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
                return;
            }

            adsService.ShowBanner( placementId, resultCallback );
        }

        /// <summary>
        /// Use to force ads manager to try show banner constantly (if show will fail then ads manager
        /// will continue to fetch and show banner) until HideBanner function will be not called.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <param name="position">New position</param>
        /// <returns>if ads manager is not initialized it will return false</returns>
        [PublicAPI]
        public static bool ShowBannerPersistent( string placementId,
            BannerPosition position = BannerPosition.BottomCenter )
        {
            if ( adsService == null )
            {
                return false;
            }

            adsService.ShowBannerPersistent( placementId, position );
            return true;
        }

        /// <summary>
        /// Call to show interstitial.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowInterstitial( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
                return;
            }

            adsService.ShowInterstitial( placementId, resultCallback );
        }

        /// <summary>
        /// Call to show default interstitial ad from config.
        /// </summary>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowInterstitial( UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", "NONE", AdResult.Failed ) );
                return;
            }

            adsService.ShowInterstitial( resultCallback );
        }

        /// <summary>
        /// Call to show rewarded.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowRewarded( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
                return;
            }

            adsService.ShowRewarded( placementId, resultCallback );
        }

        /// <summary>
        /// Call to show default rewarded ad from config.
        /// </summary>
        /// <param name="resultCallback">Show status</param>
        public static void ShowRewarded( UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", "NONE", AdResult.Failed ) );
                return;
            }

            adsService.ShowRewarded( resultCallback );
        }
    }
}