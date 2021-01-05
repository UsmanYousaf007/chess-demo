using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class ShowRewardedAdCommand : Command
    {
        // Parameters
        [Inject] public AdPlacements adPlacement { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        //Dispatch Signals
        [Inject] public RewardedVideoResultSignal rewardedVideoResultSignal { get; set; }

        public override void Execute()
        {
            if (!adsService.IsRewardedVideoAvailable(adPlacement))
            {
                rewardedVideoResultSignal.Dispatch(AdsResult.NOT_AVAILABLE, adPlacement);
                return;
            }

            Retain();
            adsService.ShowRewardedVideo(adPlacement).Then(OnVideoShown);
        }

        private void OnVideoShown(AdsResult result)
        {
            if (result != AdsResult.FINISHED)
            {
                rewardedVideoResultSignal.Dispatch(AdsResult.FAILED, adPlacement);
                Release();
                return;
            }

            var jsonData = new GSRequestData().AddString("rewardType", CollectionsUtil.GetRewardTypeFromAdPlacement(adPlacement));
            backendService.ClaimReward(jsonData).Then(OnRewardClaimed);
        }

        private void OnRewardClaimed(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                rewardedVideoResultSignal.Dispatch(AdsResult.FINISHED, adPlacement);
            }
            else
            {
                rewardedVideoResultSignal.Dispatch(AdsResult.FAILED, adPlacement);
            }

            Release();
        }
    }
}
