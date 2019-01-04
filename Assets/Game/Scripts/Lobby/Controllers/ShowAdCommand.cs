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

        // Dispatch signals
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
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

                Release();
                return;
            }
            else
            {
                if (adsService.IsRewardedVideoAvailable())
                {
                    adsService.ShowRewardedVideo().Then(OnShowRewardedVideo);
                    Retain();
                }
                else
                {
                    Release();
                    return;
                }
            }
        }

        private void OnShowRewardedVideo(AdsResult result)
        {
            if (result == AdsResult.FINISHED)
            {
                backendService.ClaimReward(GSBackendKeys.ClaimReward.TYPE_AD_BUCKS).Then(OnClaimReward);
            }
            else
            {
                Release();
            }
        }

        private void OnClaimReward(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);
            }

            Release();
        }
    }
}
