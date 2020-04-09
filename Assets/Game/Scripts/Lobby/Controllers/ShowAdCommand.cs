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
using strange.extensions.promise.impl;

namespace TurboLabz.InstantGame
{
    public class ShowAdCommand : Command
    {
        // Parameters
        [Inject] public ResultAdsVO resultAdsVO { get; set; }

        //[Inject] public AdType adType { get; set; }
        //[Inject] public string claimRewardType { get; set; }

        // Dispatch signals
        [Inject] public UpdatePlayerRewardsPointsSignal updatePlayerRewardsPointsSignal { get; set; }
        [Inject] public RewardUnlockedSignal rewardUnlockedSignal { get; set; }
        [Inject] public ShowPromotionDlgSignal showPromotionDlgSignal { get; set; }
        [Inject] public ShowAdSkippedDlgSignal showAdSkippedDlgSignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public AdType adType;
        public string claimRewardType;
        private AdsRewardVO adsRewardData;
        private IPromise<AdsResult> promotionAdPromise;

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

            switch (adType)
            {
                case AdType.Interstitial:

                    Retain();
                    if (adsService.IsInterstitialAvailable())
                    {
                        preferencesModel.globalAdsCount++;
                        preferencesModel.interstitialAdsCount++;

                        var promise = adsService.ShowInterstitial();
                        if (promise != null)
                        {
                            promise.Then(ClaimReward);
                            promise.Then(ShowPromotionOnVictory);
                        }
                        else
                        {
                            Release();
                        }

                        analyticsService.Event(AnalyticsEventId.ads_interstitial_show);
                    }
                    else
                    {
                        ShowPromotion();
                        analyticsService.Event(AnalyticsEventId.ads_interstitial_failed);
                    }

                    break;

                case AdType.RewardedVideo:

                    if (adsService.IsRewardedVideoAvailable())
                    {
                        preferencesModel.globalAdsCount++;
                        preferencesModel.rewardedAdsCount++;
                        Retain();
                        IPromise<AdsResult> p = adsService.ShowRewardedVideo();

                        if (p != null)
                        {
                            p.Then(ClaimReward);
                            p.Then(ShowPromotionOnVictory);
                        }
                        else
                        {
                            Release();
                        }
                    }

                    break;

                case AdType.Promotion:

                    Retain();
                    ShowPromotion();
                    break;
            }
        }

        string challengeId = "";
        private void ClaimReward(AdsResult result)
        {
            if ((result == AdsResult.FINISHED || result == AdsResult.BYPASS) && claimRewardType != GSBackendKeys.ClaimReward.NONE)
            {
                adsRewardData = playerModel.GetAdsRewardsData();

                GSRequestData jsonData = new GSRequestData().AddString("rewardType", claimRewardType)
                                                            .AddString("challengeId", resultAdsVO.challengeId);

                backendService.ClaimReward(jsonData).Then(OnClaimReward);
            }
            else if (result == AdsResult.SKIPPED)
            {
                if (!preferencesModel.isSkipVideoDlgShown)
                {
                    preferencesModel.isSkipVideoDlgShown = true;
                    showAdSkippedDlgSignal.Dispatch();
                }

                Release();
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
                if (playerModel.rewardQuantity > 0)
                {
                    playerModel.UpdateGoodsInventory(adsRewardData.shortCode, playerModel.rewardQuantity);
                    updatePlayerRewardsPointsSignal.Dispatch(0, playerModel.rewardCurrentPoints);
                    rewardUnlockedSignal.Dispatch(adsRewardData.shortCode, playerModel.rewardQuantity);
                }
                else
                {
                    updatePlayerRewardsPointsSignal.Dispatch(adsRewardData.currentPoints, playerModel.rewardCurrentPoints);
                }
            }

            Release();
        }

        private void ShowPromotion()
        {
            promotionAdPromise = new Promise<AdsResult>();
            promotionAdPromise.Then(ClaimReward);
            showPromotionDlgSignal.Dispatch(promotionAdPromise, InternalAdType.INTERAL_AD);
        }

        private void ShowPromotionOnVictory(AdsResult result)
        {
            if ((DateTime.Now - preferencesModel.timeAtSubscrptionDlgShown).TotalMinutes > metaDataModel.adsSettings.minutesForVictoryInternalAd && resultAdsVO.playerWins)
            {
                showPromotionDlgSignal.Dispatch(null, InternalAdType.FORCED_ON_WIN);
            }
        }
    }
}
