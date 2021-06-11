using System;
using System.Collections;
using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using UnityEngine;

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
        [Inject] public IRoutineRunner routineRunner { get; set; }

        //Dispatch Signals
        [Inject] public RewardedVideoResultSignal rewardedVideoResultSignal { get; set; }
        [Inject] public ShowGenericProcessingSignal showGenericProcessingSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        private static Coroutine waitForVideoToLoadCoroutine;

        public override void Execute()
        {
            if (!adsService.IsPersonalisedAdDlgShown())
            {
                OnVideoShown(AdsResult.FINISHED);
                return;
            }

            playerModel.adContext = CollectionsUtil.GetAdContextFromAdPlacement(adPlacement);
            analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);

            Retain();

            if (!adsService.IsRewardedVideoAvailable(adPlacement))
            {
                showGenericProcessingSignal.Dispatch(true);
                if (waitForVideoToLoadCoroutine != null) routineRunner.StopCoroutine(waitForVideoToLoadCoroutine);
                waitForVideoToLoadCoroutine = routineRunner.StartCoroutine(WaitForVideoToLoad());
                return;
            }

            switch (adPlacement)
            {
                case AdPlacements.Rewarded_cpu_in_game_power_mode:
                case AdPlacements.Rewarded_cpu_pregame_power_mode:
                case AdPlacements.Rewarded_cpu_resume_power_mode:
                    adPlacement = AdPlacements.Rewarded_powerplay;
                    break;
            }

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

        private IEnumerator WaitForVideoToLoad()
        {
            yield return new WaitForSeconds(2.0f);
            analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
            showGenericProcessingSignal.Dispatch(false);

            if (!adsService.IsRewardedVideoAvailable(adPlacement))
            {
                if (adPlacement == AdPlacements.Rewarded_coins_purchase ||
                    adPlacement == AdPlacements.Rewarded_coins_banner ||
                    adPlacement == AdPlacements.Rewarded_coins_popup)
                {
                    OnVideoShown(AdsResult.FINISHED);
                }
                else
                {
                    rewardedVideoResultSignal.Dispatch(AdsResult.NOT_AVAILABLE, adPlacement);
                }
                Release();
            }
            else
            {
                adsService.ShowRewardedVideo(adPlacement).Then(OnVideoShown);
            }
        }
    }
}
