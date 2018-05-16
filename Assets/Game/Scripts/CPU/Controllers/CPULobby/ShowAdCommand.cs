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

namespace TurboLabz.InstantChess
{
    public class ShowAdCommand : Command
    {
        // Dispatch signals
        [Inject] public LoadCPUGameSignal loadCPUGameSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateFreeBucksRewardSignal updatedFreeBucksRewardSignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            if (adsService.isRewardedAdAvailable)
            {
                adsService.ShowRewardedAd().Then(OnShowAd);
                analyticsService.AdStart(false, UnityAdsPlacementId.REWARDED_VIDEO);
                Retain();
            }
        }

        private void OnShowAd(AdsResult result)
        {
            if (result == AdsResult.FINISHED)
            {
                int rewardBucks = playerModel.adLifetimeImpressions * metaDataModel.adSettings.adsRewardIncrement;
                playerModel.bucks += rewardBucks;

                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FREE_BUCKS_REWARD_DLG);
                updatedFreeBucksRewardSignal.Dispatch(rewardBucks);

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
