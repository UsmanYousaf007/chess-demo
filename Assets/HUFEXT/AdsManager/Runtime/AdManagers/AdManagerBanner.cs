using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerBanner : AdManagerBase
    {
        bool didFetchBanner;
        bool isFetchingBanner;

        public AdManagerBanner( AdPlacementData inAdPlacementData,
            AdsManagerConfig inAdsManagerConfig,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsManagerConfig,
            inAdsService )
        {
            adStatus = AdStatus.ReadyToShow;
            logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerBanner) );
        }

        public void HideBanner()
        {
            tryShowBannerPersistent = false;
            StartFetching();
        }

        public override void ShowAd( UnityAction<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            isFetchingBanner = true;
            HLog.Log( logPrefix, $"ShowAd {adPlacementData.PlacementId}" );
            base.ShowAd( resultCallback, alternativeAdPlacement );

            if ( HAds.Banner.Show( adPlacementData.PlacementId ) == false )
            {
                OnFailToShow();
            }
        }

        protected override void StartFetching()
        {
            if ( didFetchBanner || isFetchingBanner )
            {
                HAds.Banner.Hide();
                didFetchBanner = false;
            }

            isFetchingBanner = false;
            base.StartFetching();
        }

        protected override void RegisterEvents()
        {
            HAds.Banner.OnShown += OnShownBanner;
            HAds.Banner.OnFailed += OnFailed;
        }

        void OnShownBanner( IBannerCallbackData callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            HLog.Log( logPrefix, $"OnShownBanner {adPlacementData.PlacementId}" );
            didFetchBanner = true;
            currentFetchTimes = 0;

            showResponseCallbacks.Dispatch( new AdManagerCallback( HAds.Banner.GetAdProviderName(),
                adPlacementData.PlacementId,
                callbackData.HeightInPx ) );
            showResponseCallbacks = null;
        }

        void OnFailed( IBannerCallbackData callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            HLog.Log( logPrefix, $"OnFailed {adPlacementData.PlacementId}" );
            OnFailToShow();
        }

        protected override void Fetch()
        {
            if ( tryShowBannerPersistent )
            {
                base.Fetch();
                ShowAd( showResponseCallbacks, shownPlacementId );
                return;
            }

            adStatus = AdStatus.ReadyToShow;

            adsService.OnAdFetch.Dispatch(
                new AdsManagerFetchCallbackData( GetAdMediationName(), adPlacementData.PlacementId ) );

            for ( int i = 0; i < alternativePlacements.Count; i++ )
            {
                adsService.OnAdFetch.Dispatch(
                    new AdsManagerFetchCallbackData( GetAdMediationName(), alternativePlacements[i] ) );
            }

            currentFetchTimes = 0;
        }

        protected override void SendAdEvent( AdResult result, string placementId )
        {
            showResponseCallbacks.Dispatch(
                new AdManagerCallback(
                    GetAdMediationName(),
                    placementId,
                    result ) );
        }

        protected override string GetAdMediationName()
        {
            return HAds.Banner.GetAdProviderName();
        }

        bool tryShowBannerPersistent = false;

        public void ShowBannerPersistent( string placementId )
        {
            tryShowBannerPersistent = true;
            ShowAd( null, placementId );
        }

        protected override void DisconnectFromAds()
        {
            HAds.Banner.OnShown -= OnShownBanner;
            HAds.Banner.OnFailed -= OnFailed;
        }

        protected override bool NativeIsReady()
        {
            HLog.Log( logPrefix, $"NativeIsReady {adPlacementData.PlacementId}" );
            return true;
        }
    }
}