using HUF.Ads.Runtime.Implementation;
using HUF.Utils;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdMediation;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerRewardedQueue : AdManagerRewarded
    {
        public AdManagerRewardedQueue(
            AdPlacementData inAdPlacementData,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsService )
        {
            logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerRewardedQueue) );
        }

        protected override void HandleFetched( AdCallback callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            adStatus = AdStatus.WaitingForFetch;
            adsService.RemoveFromRewardedAdQueue( adPlacementData.PlacementId );
            base.HandleFetched( callbackData );
        }

        protected override void StartFetching()
        {
            adStatus = AdStatus.WaitingForFetch;

            if ( adsService.CanFetchRewardedAd( adPlacementData.PlacementId ) )
            {
                base.StartFetching();
            }
        }

        public void TryFetch()
        {
            if ( adStatus == AdStatus.Fetching )
                return;

            StartFetching();
        }
    }
}