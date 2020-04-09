using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerRewardedQueue : AdManagerRewarded
    {
        public AdManagerRewardedQueue(
            AdPlacementData inAdPlacementData,
            AdsManagerConfig inAdsManagerConfig,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsManagerConfig,
            inAdsService )
        {
            logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerRewardedQueue) );
        }

        protected override void OnFetched( IAdCallbackData callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            adStatus = AdStatus.WaitingForFetch;
            adsService.RemoveFromRewardedAdQueue( adPlacementData.PlacementId );
            base.OnFetched( callbackData );
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