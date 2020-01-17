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

        //Dispatch Signals
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        public override void Execute()
        {
            if (adsService.IsRewardedVideoAvailable())
            {
                showAdSignal.Dispatch(resultAdsVO);
            }
            else if (adsService.IsInterstitialAvailable())
            {
                var vo = resultAdsVO;
                vo.adsType = AdType.Interstitial;
                vo.rewardType = GSBackendKeys.ClaimReward.TYPE_MATCH_WIN;
                showAdSignal.Dispatch(resultAdsVO);
            }
            else
            {
                var vo = resultAdsVO;
                vo.adsType = AdType.Promotion;
                vo.rewardType = GSBackendKeys.ClaimReward.TYPE_PROMOTION;
                showAdSignal.Dispatch(resultAdsVO);
            }
        }
    }
}
