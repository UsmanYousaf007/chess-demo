/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

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
        // Parameters
        [Inject] public bool isRewarded { get; set; }
        [Inject] public string placementId { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public UpdateAdsSignal updateAdSignal { get; set; }
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPreferencesModel prefsModel { get; set; }

        public override void Execute()
        {
            // All non-rewarded ads skipped if player owns the remove ads feature
            if (!isRewarded && playerModel.OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS))
            {
                return;
            }

            adsService.ShowAd(placementId).Then(OnShowAd);

            analyticsService.AdStart(isRewarded);
            Retain();
        }

        private void OnShowAd(AdsResult result)
        {
            if (result == AdsResult.FINISHED)
            {
                analyticsService.AdComplete(isRewarded);

                if (isRewarded)
                {
                    backendService.ClaimReward(GSBackendKeys.ClaimReward.TYPE_AD_BUCKS).Then(OnClaimReward);
                }
                else
                {
                    
                    Release();
                }
            }
            else if (result == AdsResult.SKIPPED)
            {
                analyticsService.AdSkip(isRewarded);
                Release();
            }
            else if (result == AdsResult.FAILED)
            {
                Release();
            }
        }

        private void OnClaimReward(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                prefsModel.adSlotImpressions++;
                updateAdSignal.Dispatch();
                updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);
            }

            Release();
        }
    }
}
