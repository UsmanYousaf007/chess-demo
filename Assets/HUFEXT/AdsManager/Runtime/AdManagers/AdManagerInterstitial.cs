using System;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerInterstitial : AdManagerBase
    {
        protected override string ServiceName => nameof(AdManagerInterstitial);

        public AdManagerInterstitial(
            AdPlacementData inAdPlacementData,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsService )
        {
        }

        protected override void Fetch()
        {
            base.Fetch();
            adsService.Mediation.FetchInterstitial( adPlacementData.PlacementId );
        }

        public override void ShowAd( Action<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            base.ShowAd( resultCallback, alternativeAdPlacement );
            adsService.Mediation.ShowInterstitial( shownPlacementId );
        }

        protected override void SendAdEvent( AdResult result, string placementId )
        {
            showResponseCallbacks.Dispatch(
                new AdManagerCallback(
                    GetAdMediationName(),
                    placementId,
                    result ) );
        }

        protected override void SubscribeToEvents()
        {
            adsService.Mediation.OnInterstitialFetched += HandleFetched;
            adsService.Mediation.OnInterstitialEnded += HandleEnded;
        }

        protected override void UnsubscribeFromEvents()
        {
            adsService.Mediation.OnInterstitialFetched -= HandleFetched;
            adsService.Mediation.OnInterstitialEnded -= HandleEnded;
        }

        protected override bool NativeIsReady()
        {
            HLog.Log( logPrefix, $"NativeIsReady {adPlacementData.PlacementId}" );
            return adsService.Mediation.IsInterstitialReady( adPlacementData.PlacementId );
        }
    }
}