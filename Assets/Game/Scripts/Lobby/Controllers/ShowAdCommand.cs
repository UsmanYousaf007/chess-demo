/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class ShowAdCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateFreeBucksRewardSignal updatedFreeBucksRewardSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public ToggleAdBlockerSignal toggleAdBlockerSignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            if (adsService.isRewardedAdAvailable)
            {
                adsService.ShowRewardedAd().Then(OnShowAd);
                analyticsService.AdStart(false, UnityAdsPlacementId.REWARDED_VIDEO);
                toggleAdBlockerSignal.Dispatch(true);
                Retain();
            }
        }

        private void OnShowAd(AdsResult result)
        {
            toggleAdBlockerSignal.Dispatch(false);

            if (result == AdsResult.FINISHED)
            {
                playerModel.adSlotImpressions++;
                backendService.ClaimReward("rewardAdBucks");

                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FREE_BUCKS_REWARD_DLG);
               // updatedFreeBucksRewardSignal.Dispatch(rewardBucks);

                loadLobbySignal.Dispatch();

                analyticsService.AdComplete(false, UnityAdsPlacementId.REWARDED_VIDEO);
            }
            else if (result == AdsResult.SKIPPED)
            {
                analyticsService.AdSkip(false, UnityAdsPlacementId.REWARDED_VIDEO);
            }

            Release();
        }
    }
}
