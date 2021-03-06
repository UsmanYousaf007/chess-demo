using System;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdMediation;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerBanner : AdManagerBase
    {
        bool didFetchBanner;
        bool isFetchingBanner;
        bool tryShowBannerPersistent;

        protected override string ServiceName => nameof(AdManagerBanner);

        public AdManagerBanner( AdPlacementData inAdPlacementData,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsService ) { }

        public void HideBanner()
        {
            HLog.Log( logPrefix, $"HideBanner {isFetchingBanner} {didFetchBanner} {tryShowBannerPersistent}" );
            tryShowBannerPersistent = false;
            StartFetching();
        }

        public override void ShowAd( Action<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            isFetchingBanner = true;
            HLog.Log( logPrefix, $"ShowAd {adPlacementData.PlacementId}" );
            base.ShowAd( resultCallback, alternativeAdPlacement );
            adsService.Mediation.ShowBanner( shownPlacementId );
        }

        protected override void StartFetching()
        {
            HLog.Log( logPrefix, $"StartFetching  {isFetchingBanner} {didFetchBanner}" );

            if ( didFetchBanner || isFetchingBanner )
            {
                adsService.Mediation.HideBanner( shownPlacementId );
                didFetchBanner = false;
            }

            isFetchingBanner = false;
            base.StartFetching();
        }

        void HandleShown( AdCallback callbackData )
        {
            if ( callbackData.PlacementId != shownPlacementId )
                return;

            if ( callbackData.Result != AdResult.Completed )
            {
                HLog.Log( logPrefix, $"HandleShown Failed {adPlacementData.PlacementId}" );
                isFetchingBanner = false;
                HandleFailToShow();
                return;
            }

            HLog.Log( logPrefix, $"HandleShown {adPlacementData.PlacementId}" );
            didFetchBanner = true;
            currentFetchTimes = 0;

            showResponseCallbacks.Dispatch( new AdManagerCallback( callbackData.MediationId,
                adPlacementData.PlacementId,
                callbackData.HeightInPx ) );
            showResponseCallbacks = null;
        }

        protected override void HandleFetched( AdCallback callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            isFetchingBanner = false;
            base.HandleFetched( callbackData );

            if ( callbackData.Result != AdResult.Completed )
                return;

            if ( tryShowBannerPersistent )
                ShowAd( showResponseCallbacks, shownPlacementId );
        }

        void HandleHidden( AdCallback callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            isFetchingBanner = false;
            didFetchBanner = false;
            HLog.Log( logPrefix, $"OnFailed {adPlacementData.PlacementId}" );
            AdEnded( AdResult.Completed );
        }

        protected override void Fetch()
        {
            base.Fetch();
            adsService.Mediation.FetchBanner( adPlacementData.PlacementId );
        }

        protected override void SendAdEvent( AdResult result, string placementId )
        {
            showResponseCallbacks.Dispatch(
                new AdManagerCallback(
                    GetAdMediationName(),
                    placementId,
                    result ) );
        }

        public void ShowBannerPersistent( string placementId )
        {
            HLog.Log( logPrefix, $"ShowBannerPersistent {tryShowBannerPersistent}" );
            tryShowBannerPersistent = true;

            if ( IsReady() )
                ShowAd( null, placementId );
        }

        protected override void SubscribeToEvents()
        {
            adsService.Mediation.OnBannerShown += HandleShown;
            adsService.Mediation.OnBannerFetched += HandleFetched;
            adsService.Mediation.OnBannerHidden += HandleHidden;
        }

        protected override void UnsubscribeFromEvents()
        {
            adsService.Mediation.OnBannerShown -= HandleShown;
            adsService.Mediation.OnBannerFetched -= HandleFetched;
            adsService.Mediation.OnBannerHidden -= HandleHidden;
        }

        protected override bool NativeIsReady()
        {
            return adsService.Mediation.IsBannerReady( adPlacementData.PlacementId );
        }
    }
}