using System;
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
        [Inject] public IAnalyticsService analyticsService { get; set; }
      
        //Dispatch Signals
        [Inject] public RewardedVideoResultSignal rewardedVideoResultSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            if (!adsService.IsPersonalisedAdDlgShown())
            {
                OnVideoShown(AdsResult.FINISHED);
                return;
            }

            playerModel.adContext = CollectionsUtil.GetAdContextFromAdPlacement(adPlacement);
            analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);

            if (!adsService.IsRewardedVideoAvailable(adPlacement))
            {
                rewardedVideoResultSignal.Dispatch(AdsResult.NOT_AVAILABLE, adPlacement);

                //Special case for lobby chest
                if (adPlacement == AdPlacements.Rewarded_lobby_chest)
                {
                    OnVideoShown(AdsResult.FINISHED);
                }

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

            preferencesModel.intervalBetweenPregameAds = DateTime.Now;
            var jsonData = new GSRequestData().AddString("rewardType", CollectionsUtil.GetRewardTypeFromAdPlacement(adPlacement));
            if (adPlacement == AdPlacements.RV_rating_booster)
            {
                jsonData.AddString("challengeId", matchInfoModel.lastCompletedMatch.challengeId);
            }
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
