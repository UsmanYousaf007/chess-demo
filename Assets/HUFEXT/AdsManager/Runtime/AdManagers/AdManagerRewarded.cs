using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerRewarded : AdManagerBase
    {
        public AdManagerRewarded(
            AdPlacementData inAdPlacementData,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsService )
        {
            logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerRewarded) );
        }

        protected override void SubscribeToEvents()
        {
            adsService.Mediation.OnRewardedFetched += HandleFetched;
            adsService.Mediation.OnRewardedEnded += HandleEnded;
        }

        protected override void Fetch()
        {
            base.Fetch();
            adsService.Mediation.FetchRewarded( adPlacementData.PlacementId );
        }

        public override void ShowAd( UnityAction<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            base.ShowAd( resultCallback, alternativeAdPlacement );
            adsService.Mediation.ShowRewarded( adPlacementData.PlacementId );
        }

        protected override void SendAdEvent( AdResult result, string placementId )
        {
            showResponseCallbacks.Dispatch(
                new AdManagerCallback(
                    GetAdMediationName(),
                    placementId,
                    result ) );
        }

        protected override void UnsubscribeFromEvents()
        {
            adsService.Mediation.OnRewardedFetched -= HandleFetched;
            adsService.Mediation.OnRewardedEnded -= HandleEnded;
        }

        protected override bool NativeIsReady()
        {
            HLog.Log( logPrefix, $"NativeIsReady {adPlacementData.PlacementId}" );
            return adsService.Mediation.IsRewardedReady( adPlacementData.PlacementId );
        }
    }
}