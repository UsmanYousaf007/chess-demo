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
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class ShowAdCommand : Command
    {
        // Parameters
        [Inject] public AdType adType { get; set; }
        [Inject] public string claimRewardType { get; set; }

        // Dispatch signals
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            // All non-rewarded ads skipped if player owns the remove ads feature
            bool removeAds = playerModel.hasRemoveAds(metaDataModel.adsSettings);

            // Case: Ads removed
            if (removeAds)
            {
                if (adType == AdType.RewardedVideo)
                {
                    Retain();
                    ClaimReward(AdsResult.BYPASS);
                }
                return;
            }

            // Case: Show Ads
            if (adType == AdType.Interstitial)
            {
                if (adsService.IsInterstitialAvailable())
                {
                    Debug.Log("[TLADS]: Interstitial is available");
                    adsService.ShowInterstitial();
                }
                else
                {
                    Debug.Log("[TLADS]: Interstitial is NOT available");
                }

                return;
            }
            else
            {
                if (adsService.IsRewardedVideoAvailable())
                {
                    Debug.Log("[TLADS]: Rewarded video is available");
                    Retain();
                    adsService.ShowRewardedVideo().Then(ClaimReward);
                }
                else
                {
                    Debug.Log("[TLADS]: Rewarded video is NOT available");
                    Retain();
                    ClaimReward(AdsResult.BYPASS);
                    return;
                }
            }
        }

        private void ClaimReward(AdsResult result)
        {
            if (result == AdsResult.FINISHED || result == AdsResult.BYPASS)
            {
                Debug.Log("[TLADS]: Rewarded video completed.");
                backendService.ClaimReward(claimRewardType).Then(OnClaimReward);
            }
            else
            {
                Debug.Log("[TLADS]: Rewarded video did not complete.");
                Release();
            }
        }

        private void OnClaimReward(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                Debug.Log("[TLADS]: Rewarding players coins...");
                updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);
            }

            Release();
        }
    }
}
