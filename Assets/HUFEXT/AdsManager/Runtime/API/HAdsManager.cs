using System.Collections.Generic;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdManagers;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.LowLevel;

namespace HUFEXT.AdsManager.Runtime.API
{
    public static class HAdsManager
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAdsManager) );

        static AdsManagerConfig config;
        static HUFAdsService adsService;
        static List<HUFAdsService> alternativeMediations = new List<HUFAdsService>();
        static List<HUFAdsService> mediationsList = new List<HUFAdsService>();
        static BannerPosition lastBannerPosition = BannerPosition.BottomCenter;

        /// <summary>
        /// Occurs when any ad is Fetched.
        /// </summary>
        [PublicAPI] public static UnityAction<AdsManagerFetchCallbackData> OnAdFetch;

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
        /// Initializes HUF Ads manager.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( IsInitialized() )
            {
                HLog.Log( logPrefix, $"HUF Ads manager is already initialized" );
                return;
            }

            if ( Config != null )
            {
                adsService = new HUFAdsService( new BaseAdMediation(), false );
                mediationsList.Insert( 0, adsService );
                adsService.OnAdFetch += HandleAdFetch;
                HLog.Log( logPrefix, $"Service initialized" );
            }
            else
                HLog.LogError( logPrefix, $"AdsManagerConfig does not exist" );
        }

        /// <summary>
        /// Checks if the ads manager is initialized.
        /// </summary>
        /// <returns>Status of initialization</returns>
        [PublicAPI]
        public static bool IsInitialized()
        {
            return adsService != null;
        }

        /// <summary>
        /// Checks if an ad is loaded and ready to show. (Banners are always ready to show).
        /// </summary>
        /// <param name="placementId">Ad placement ID</param>
        /// <returns>Ad status if it is ready to show</returns>
        [PublicAPI]
        public static bool CanShowAd( string placementId )
        {
            if ( adsService != null && !placementId.IsNullOrEmpty() )
            {
                foreach ( var mediation in mediationsList )
                {
                    if ( mediation.CanShowAd( placementId ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Shows an ad. If the ad is not ready, resultCallback will be dispatched.
        /// </summary>
        /// <param name="placementId">Ad placement ID</param>
        /// <param name="resultCallback">Shown ads status</param>
        [PublicAPI]
        public static void ShowAd( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null || placementId.IsNullOrEmpty() )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
                return;
            }

            foreach ( var mediation in mediationsList )
            {
                if ( mediation.CanShowAd( placementId ) )
                {
                    mediation.ShowAd( placementId, resultCallback );
                    return;
                }
            }

            resultCallback.Dispatch( new AdManagerCallback( "HUF", placementId, AdResult.Failed ) );
        }

        /// <summary>
        /// Sets default banner position. It applies to all new banners and does not destroy/move old ones.
        /// </summary>
        /// <param name="position">new position</param>
        /// <returns>if the ads manager is not initialized, it will return false</returns>
        [PublicAPI]
        public static bool SetNewBannerPosition( BannerPosition position )
        {
            if ( adsService == null )
            {
                return false;
            }

            lastBannerPosition = position;

            foreach ( var mediation in mediationsList )
            {
                mediation.SetBannerPosition( position );
            }

            return true;
        }

        /// <summary>
        /// Hides the banner ad.
        /// </summary>
        /// <param name="placementId">Ad placement ID</param>
        [PublicAPI]
        public static void HideBanner( string placementId )
        {
            if ( adsService == null )
            {
                HLog.LogWarning( logPrefix, "AdsManagerIsNotInitialized" );
                return;
            }

            foreach ( var mediation in mediationsList )
            {
                if ( !mediation.CanShowAd( placementId ) )
                {
                    mediation.HideBanner( placementId );
                }
            }
        }

        /// <summary>
        /// Hides the banner ad.
        /// </summary>
        [PublicAPI]
        public static void HideBanner()
        {
            HideBanner( adsService.DefaultBannerPlacement );
        }

        /// <summary>
        /// Shows first banner ad from config on specified position - will apply to all
        /// new banners like SetNewBannerPosition.
        /// </summary>
        /// <param name="resultCallback">Did the banner show correctly</param>
        /// <param name="position">New position</param>
        [PublicAPI]
        public static void ShowBanner( UnityAction<AdManagerCallback> resultCallback,
            BannerPosition position = BannerPosition.BottomCenter )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", string.Empty, AdResult.Failed ) );
                return;
            }

            SetNewBannerPosition( position );
            ShowAd( adsService.DefaultBannerPlacement, resultCallback );
        }

        /// <summary>
        /// Shows a banner ad on specified position  - will apply to all new banners like SetNewBannerPosition.
        /// </summary>
        /// /// <param name="placementId">Ad placement ID</param>
        /// <param name="resultCallback">Did the banner show correctly</param>
        /// <param name="position">New position</param>
        [PublicAPI]
        public static void ShowBanner(
            string placementId,
            UnityAction<AdManagerCallback> resultCallback,
            BannerPosition position = BannerPosition.BottomCenter )
        {
            SetNewBannerPosition( position );
            ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Shows a banner ad on specified position  - will apply to all new banners like SetNewBannerPosition.
        /// </summary>
        /// /// <param name="placementId">Ad placement ID</param>
        /// <param name="resultCallback">Did the banner show correctly</param>
        [PublicAPI]
        public static void ShowBanner( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Forces the ads manager to try to keep showing a banner constantly (if show fails, then the ads manager
        /// will continue trying to fetch and show banner ads) until HideBanner function is called.
        /// </summary>
        /// <param name="placementId">Ad placement ID</param>
        /// <param name="position">New position</param>
        /// <returns>if the ads manager is not initialized, it will return false</returns>
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
        /// Shows an interstitial ad.
        /// </summary>
        /// <param name="placementId">Ad placement ID</param>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowInterstitial( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Shows default interstitial ad from config.
        /// </summary>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowInterstitial( UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", string.Empty, AdResult.Failed ) );
                return;
            }

            ShowAd( adsService.DefaultInterstitialPlacement, resultCallback );
        }

        /// <summary>
        /// Shows a rewarded ad.
        /// </summary>
        /// <param name="placementId">Ad placement ID</param>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowRewarded( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Shows default rewarded ad from config.
        /// </summary>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowRewarded( UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null )
            {
                resultCallback.Dispatch( new AdManagerCallback( "HUF", string.Empty, AdResult.Failed ) );
                return;
            }

            ShowAd( adsService.DefaultRewardedPlacement, resultCallback );
        }

        /// <summary>
        /// Registers additional alternative mediation.
        /// </summary>
        /// <param name="mediation">alternative mediation to check for ads</param>
        [PublicAPI]
        public static void RegisterAlternativeMediation( HUFAdsService mediation )
        {
            if ( alternativeMediations.Contains( mediation ) )
            {
                HLog.LogWarning( logPrefix, $"Do not add same mediation multiple times! {nameof(mediation)}" );
                return;
            }

            alternativeMediations.Add( mediation );
            mediationsList.Add( mediation );
            mediation.SetBannerPosition( lastBannerPosition );
            mediation.OnAdFetch += HandleAdFetch;
        }

        /// <summary>
        /// Automatically initializes HUF Ads Manager.
        /// </summary>
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( Config != null && Config.AutoInit )
                Init();
        }

        static void HandleAdFetch( AdsManagerFetchCallbackData adData )
        {
            if ( adsService != null )
                OnAdFetch.Dispatch( adData );
        }
    }
}