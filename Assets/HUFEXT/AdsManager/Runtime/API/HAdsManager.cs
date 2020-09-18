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
                adsService = new HUFAdsService( new BaseAdMediation(), false );
                mediationsList.Insert( 0, adsService );
                adsService.OnAdFetch += HandleAdFetch;
                HLog.Log( logPrefix, $"Service initialized" );
            }
            else
                HLog.LogError( logPrefix, $"AdsManagerConfig does not exist" );
        }

        static void HandleAdFetch( AdsManagerFetchCallbackData adData )
        {
            if ( adsService != null )
                OnAdFetch.Dispatch( adData );
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
        /// Use this to check if an ad is loaded and ready to show. (Banners are always ready to show).
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <returns>Ad status if it is ready to show</returns>
        [PublicAPI]
        public static bool CanShowAd( string placementId )
        {
            if ( adsService != null && !placementId.IsNullOrEmpty())
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
        /// Call to show ad, if ad is not ready it will response on callback with false.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Shown ads status</param>
        [PublicAPI]
        public static void ShowAd( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            if ( adsService == null  || placementId.IsNullOrEmpty())
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
        /// Use to set default banner position. It applies to all new banners and doesn't destroy/move old ones.
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

            lastBannerPosition = position;

            foreach ( var mediation in mediationsList )
            {
                mediation.SetBannerPosition( position );
            }

            return true;
        }

        /// <summary>
        /// Use to hide banner ad.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
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
        /// Use to hide banner ad
        /// </summary>
        [PublicAPI]
        public static void HideBanner()
        {
            HideBanner( adsService.DefaultBannerPlacement );
        }

        /// <summary>
        /// Use to show first banner ad from config on specific position - will apply to all
        /// new banners like SetNewBannerPosition.
        /// </summary>
        /// <param name="resultCallback">Did banner show correctly</param>
        /// <param name="position">New position</param>
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
        /// Use to show banner ad on specific position  - will apply to all new banners like SetNewBannerPosition.
        /// </summary>
        /// /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Did banner show correctly</param>
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
        /// Use to show banner ad on specific position  - will apply to all new banners like SetNewBannerPosition.
        /// </summary>
        /// /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Did banner show correct</param>
        [PublicAPI]
        public static void ShowBanner( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Use to force ads manager to try showing banner constantly (if show fails then ads manager
        /// will continue to fetch and banner ads) until HideBanner function is called.
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
            ShowAd( placementId, resultCallback );
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
                resultCallback.Dispatch( new AdManagerCallback( "HUF", string.Empty, AdResult.Failed ) );
                return;
            }

            ShowAd( adsService.DefaultInterstitialPlacement, resultCallback );
        }

        /// <summary>
        /// Call to show rewarded.
        /// </summary>
        /// <param name="placementId">Ad placement id</param>
        /// <param name="resultCallback">Show status</param>
        [PublicAPI]
        public static void ShowRewarded( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        /// <summary>
        /// Call to show default rewarded ad from config.
        /// </summary>
        /// <param name="resultCallback">Show status</param>
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
        /// Used by HUF to add additional alternative mediation
        /// </summary>
        /// <param name="mediation">alternative mediation to check for ads</param>
        public static void RegisterAlternativeMediation( HUFAdsService mediation )
        {
            if ( alternativeMediations.Contains( mediation ) )
            {
                HLog.LogWarning( logPrefix, $"Don't add same mediation multiple times! {nameof(mediation)}" );
                return;
            }
            alternativeMediations.Add( mediation );
            mediationsList.Add( mediation );
            mediation.SetBannerPosition( lastBannerPosition );
            mediation.OnAdFetch += HandleAdFetch;
        }
    }
}