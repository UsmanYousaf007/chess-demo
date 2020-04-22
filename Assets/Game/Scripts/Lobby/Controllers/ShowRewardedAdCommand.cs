using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class ShowRewardedAdCommand : Command
    {
        // Parameters
        [Inject] public ResultAdsVO resultAdsVO { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Dispatch Signals
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        public override void Execute()
        {
            analyticsService.Event(AnalyticsEventId.ads_rewarded_show_new);

            if (adsService.IsRewardedVideoAvailable())
            {
                showAdSignal.Dispatch(resultAdsVO);
            }
            else if (adsService.IsInterstitialAvailable())
            {
                var vo = resultAdsVO;
                vo.adsType = AdType.Interstitial;
                vo.rewardType = GSBackendKeys.ClaimReward.TYPE_MATCH_WIN;
                showAdSignal.Dispatch(vo);
                analyticsService.Event(AnalyticsEventId.ads_rewarded_interstitial_show);
            }
            else
            {
                var vo = resultAdsVO;
                vo.adsType = AdType.Promotion;
                vo.rewardType = GSBackendKeys.ClaimReward.TYPE_PROMOTION;
                showAdSignal.Dispatch(vo);
                analyticsService.Event(AnalyticsEventId.ads_rewarded_interstitial_not_available);
            }
        }
    }
}
