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
using strange.extensions.promise.api;
using GameSparks.Core;

namespace TurboLabz.InstantGame
{
    public class ShowAdCommand : Command
    {
        // Parameters
        [Inject] public ResultAdsVO resultAdsVO { get; set; }

        //[Inject] public AdType adType { get; set; }
        //[Inject] public string claimRewardType { get; set; }

        // Dispatch signals
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public AdType adType;
        public string claimRewardType;

        public override void Execute()
        {
            // All non-rewarded ads skipped if player owns the remove ads feature
            bool removeAds = playerModel.HasRemoveAds(metaDataModel.adsSettings);

            adType = resultAdsVO.adsType;
            claimRewardType = resultAdsVO.rewardType;

            // Case: Ad removed
            if (removeAds)
            {
                Retain();
                ClaimReward(AdsResult.BYPASS);
                return;
            }

            // Case: Show Ads interstitial
            if (adType == AdType.Interstitial)
            {
                Retain();
                ClaimReward(AdsResult.BYPASS);

                if (adsService.IsInterstitialAvailable())
                {
                    adsService.ShowInterstitial();
                    analyticsService.Event(AnalyticsEventId.ads_interstitial_show);
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.ads_interstitial_failed);
                }
                return;
            }

            // Case: Show Ad rewarded viceo
            if (adsService.IsRewardedVideoAvailable())
            {
                Retain();
                IPromise<AdsResult> p = adsService.ShowRewardedVideo();
                if (p != null)
                {
                    p.Then(ClaimReward);
                }
                return;
            }
        }

        string challengeId = "";
        private void ClaimReward(AdsResult result)
        {
            if ((result == AdsResult.FINISHED || result == AdsResult.BYPASS) && claimRewardType != GSBackendKeys.ClaimReward.NONE)
            {

                GSRequestData jsonData = new GSRequestData().AddString("rewardType", claimRewardType)
                                                            .AddString("challengeId", resultAdsVO.challengeId);
                                                      


                backendService.ClaimReward(jsonData).Then(OnClaimReward);
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
