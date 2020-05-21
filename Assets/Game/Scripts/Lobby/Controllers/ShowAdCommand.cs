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
using System.Collections;

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
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSingal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public ShowProcessingSignal showProcessingSignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        [Inject] public IRoutineRunner routineRunner { get; set; }

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
                if (adType == AdType.RewardedVideo)
                {
                    Retain();
                    ClaimReward(AdsResult.BYPASS);
                    LoadLobby();
                }
                else if (adType == AdType.Interstitial)
                {
                    if (playerModel.adContext == AnalyticsContext.interstitial_pregame)
                    {
                        LoadGameStartSignal();
                    }
                }

                return;
            }

            switch (adType)
            {
                case AdType.Interstitial:

                    Retain();
                    if (playerModel.adContext == AnalyticsContext.interstitial_pregame)
                    {
                        if (adsService.IsInterstitialAvailable())
                        {
                            //-- Show UI blocker and spinner here
                            showProcessingSignal.Dispatch(true, true);

                            var promise = adsService.ShowInterstitial();
                            if (promise != null)
                            {
                                promise.Then(PregameAdCompleteHandler);
                            }
                            else
                            {
                                Release();
                            }
                        }
                        else
                        {
                            // If the the ad was unavailable becuause it wasn't loaded then we wait for it to load here, otherwise we assume that
                            // it was not available because of some other cap settings and we dispatch the load game signal.
                            if (adsService.IsInterstitialReady() == false && adsService.IsInterstitialNotCapped() == true)
                            {
                                if (adsSettingsModel.waitForPregameAdLoadSeconds > 0)
                                {
                                    //-- Show UI blocker and spinner here
                                    showProcessingSignal.Dispatch(true, true);

                                    //-- Start ad waiting coroutine here
                                    routineRunner.StartCoroutine(WaitForPregameAdCoroutine(adsSettingsModel.waitForPregameAdLoadSeconds));
                                }
                                else
                                {
                                    LoadGameStartSignal();
                                }
                            }
                            else
                            {
                                LoadGameStartSignal();
                            }
                        }
                    }
                    else
                    {
                        if (adsService.IsInterstitialAvailable())
                        {
                            var promise = adsService.ShowInterstitial();
                            if (promise != null)
                            {
                                promise.Then(LoadLobby);
                                promise.Then(ClaimReward);
                                promise.Then(ShowPromotionOnVictory);
                            }
                            else
                            {
                                Release();
                            }
                        }
                        else
                        {
                            ShowPromotion();
                        }
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
                            p.Then(LoadLobby);
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

        private IEnumerator WaitForPregameAdCoroutine(int waitSeconds)
        {
            while (waitSeconds > 0)
            {
                yield return new WaitForSeconds(1);
                waitSeconds--;

                if (adsService.IsInterstitialReady())
                {
                    //-- Show pregame Ad
                    var promise = adsService.ShowInterstitial();
                    if (promise != null)
                    {
                        promise.Then(PregameAdCompleteHandler);
                    }
                    else
                    {
                        Release();
                    }

                    yield break;
                }
            }

            //-- If ad is not loaded when we reach here then we consider it as failed
            PregameAdCompleteHandler(AdsResult.FAILED);
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
            //analyticsService.Event(AnalyticsEventId.ad_shown, AnalyticsContext.interstitial_replacement);
            LoadLobby();
            //promotionAdPromise = new Promise<AdsResult>();
            //promotionAdPromise.Then(ClaimReward);
            //showPromotionDlgSignal.Dispatch(promotionAdPromise, InternalAdType.INTERAL_AD);
        }

        private void ShowPromotionOnVictory(AdsResult result)
        {
            if (preferencesModel.hasRated
                && !playerModel.HasSubscription()
                && metaDataModel.adsSettings.minutesForVictoryInternalAd > 0
                && (DateTime.Now - preferencesModel.timeAtSubscrptionDlgShown).TotalMinutes > metaDataModel.adsSettings.minutesForVictoryInternalAd
                && resultAdsVO.playerWins)
            {
                showPromotionDlgSignal.Dispatch(null, InternalAdType.FORCED_ON_WIN);
            }
        }

        private void LoadLobby(AdsResult result = AdsResult.FINISHED)
        {
            loadLobbySignal.Dispatch();
            refreshCommunitySignal.Dispatch();
            refreshFriendsSignal.Dispatch();
            cancelHintSingal.Dispatch();
        }

        private void PregameAdCompleteHandler(AdsResult result = AdsResult.FINISHED)
        {
            Debug.LogError("Adresult: " + result);

            if (result == AdsResult.FINISHED || result == AdsResult.SKIPPED)
            {
                analyticsService.Event(AnalyticsEventId.ad_shown, playerModel.adContext);

                preferencesModel.globalAdsCount++;
                preferencesModel.interstitialAdsCount++;

                if (playerModel.adContext == AnalyticsContext.interstitial_pregame)
                {
                    preferencesModel.pregameAdsPerDayCount++;
                }

                preferencesModel.intervalBetweenPregameAds = DateTime.Now;
            }

            //-- Hide UI blocker and spinner here
            showProcessingSignal.Dispatch(false, false);

            LoadGameStartSignal();
        }

        private void LoadGameStartSignal()
        {
            //playerModel.adContext = AnalyticsContext.interstitial_endgame;
            Debug.Log("ACTION CODE: " + resultAdsVO.actionCode);

            if (resultAdsVO.actionCode == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                analyticsService.Event("classic_" + AnalyticsEventId.match_find_random, AnalyticsContext.start_attempt);
                FindMatchAction.RandomLong(findMatchSignal);
            }
            else if (resultAdsVO.actionCode == FindMatchAction.ActionCode.Random.ToString()
              || resultAdsVO.actionCode == FindMatchAction.ActionCode.Random10.ToString())
            {

                if (resultAdsVO.actionCode == FindMatchAction.ActionCode.Random.ToString())
                    analyticsService.Event("5m_" + AnalyticsEventId.match_find_random.ToString(), AnalyticsContext.start_attempt);
                else
                    analyticsService.Event("10m_" + AnalyticsEventId.match_find_random.ToString(), AnalyticsContext.start_attempt);

                FindMatchAction.Random(findMatchSignal, resultAdsVO.actionCode.ToString());
            }
            else if (resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge.ToString() ||
                resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge10.ToString())
            {
                if (resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge.ToString())
                    analyticsService.Event("5m_" + AnalyticsEventId.match_find_friends.ToString(), AnalyticsContext.start_attempt);
                else
                    analyticsService.Event("10m_" + AnalyticsEventId.match_find_friends.ToString(), AnalyticsContext.start_attempt);

                FindMatchAction.Challenge(findMatchSignal, resultAdsVO.isRanked, resultAdsVO.friendId, resultAdsVO.actionCode.ToString());
            }
            else if (resultAdsVO.actionCode == "ChallengeClassic")
            {
                analyticsService.Event("classic_" + AnalyticsEventId.match_find_friends, AnalyticsContext.start_attempt);
                tapLongMatchSignal.Dispatch(resultAdsVO.friendId, resultAdsVO.isRanked);
            }
            
            else
            {
                startCPUGameSignal.Dispatch();
            }

            Release();
        }
    }
}
