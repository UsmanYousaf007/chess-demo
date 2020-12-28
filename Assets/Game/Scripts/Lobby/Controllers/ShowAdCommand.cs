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
        [Inject] public bool forcedShow { get; set; }

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
        
        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }

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
        private WaitForSeconds waitForOneSecond;

        public override void Execute()
        {
            bool skipAd = false;
            if (forcedShow == false && CanShowAd(resultAdsVO.actionCode) == false)
            {
                skipAd = true;
            }

            // All non-rewarded ads skipped if player owns the remove ads feature
            bool removeAds = playerModel.HasRemoveAds(metaDataModel.adsSettings);

            adType = resultAdsVO.adsType;
            claimRewardType = resultAdsVO.rewardType;

            // Case: Ad removed
            if (removeAds || skipAd)
            {
                if (adType == AdType.RewardedVideo)
                {
                    if (adsService.IsRewardedVideoAvailable(resultAdsVO.placementId))
                    {
                        preferencesModel.globalAdsCount++;
                        preferencesModel.rewardedAdsCount++;
                        Retain();
                        IPromise<AdsResult> p = adsService.ShowRewardedVideo(resultAdsVO.placementId);

                        if (p != null)
                        {
                            p.Then(LoadLobby);
                            p.Then(RewardedAdCompleteHandler);
                            p.Then(ClaimReward);
                            p.Then(ShowPromotionOnVictory);
                        }
                        else
                        {
                            Release();
                        }
                    }
                }
                else if (adType == AdType.Interstitial)
                {
                    if (IsPregameAd())
                    {
                        LoadGameStartSignal();
                    }
                    else if (playerModel.adContext == AnalyticsContext.interstitial_endgame)
                    {
                        Retain();
                        ClaimReward(AdsResult.BYPASS);
                        LoadLobby();
                    }
                    else if (playerModel.adContext == AnalyticsContext.interstitial_tournament_endcard_continue)
                    {
                        // Go to tournaments leaderboard view here.
                        InterstitialAdCompleteHandler(AdsResult.FINISHED);
                    }
                    else
                    {
                        resultAdsVO.OnAdCompleteCallback?.Invoke(false);
                        resultAdsVO.RemoveCallback();
                    }
                }

                return;
            }

            // Initializing wait for one second to be used in WaitForPregameAdCoroutine
            waitForOneSecond = new WaitForSeconds(1f);

            switch (adType)
            {
                case AdType.Interstitial:

                    analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);

                    Retain();
                    if (IsPregameAd())
                    {
                        if (adsService.IsInterstitialAvailable(resultAdsVO.placementId))
                        {
                            var promise = adsService.ShowInterstitial(resultAdsVO.placementId);
                            if (promise != null)
                            {
                                promise.Then(PregameAdCompleteHandler);
                            }
                            else
                            {
                                LoadGameStartSignal();
                            }
                        }
                        else
                        {
                            // If the the ad was unavailable becuause it wasn't loaded then we wait for it to load here, otherwise we assume that
                            // it was not available because of some other cap settings and we dispatch the load game signal.
                            if (adsService.IsInterstitialReady(resultAdsVO.placementId) == false && adsService.IsInterstitialNotCapped() == true)
                            {
                                if (adsSettingsModel.waitForPregameAdLoadSeconds > 0)
                                {
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
                        if (adsService.IsInterstitialAvailable(resultAdsVO.placementId))
                        {
                            var promise = adsService.ShowInterstitial(resultAdsVO.placementId);
                            if (promise != null)
                            {
                                if (playerModel.adContext == AnalyticsContext.interstitial_endgame)
                                {
                                    promise.Then(InterstitialAdCompleteHandler);
                                    promise.Then(LoadLobby);
                                    promise.Then(ClaimReward);
                                    promise.Then(ShowPromotionOnVictory);
                                }
                                else if (playerModel.adContext == AnalyticsContext.interstitial_tournament_endcard_continue)
                                {
                                    promise.Then(InterstitialAdCompleteHandler);
                                    promise.Then(ClaimReward);
                                    promise.Then(ShowPromotionOnVictory);
                                }
                                else
                                {
                                    promise.Then(InterstitialAdCompleteHandler);
                                }
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

                    analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);

                    if (adsService.IsRewardedVideoAvailable(resultAdsVO.placementId))
                    {
                        preferencesModel.globalAdsCount++;
                        preferencesModel.rewardedAdsCount++;
                        Retain();
                        IPromise<AdsResult> p = adsService.ShowRewardedVideo(resultAdsVO.placementId);

                        if (p != null)
                        {
                            p.Then(LoadLobby);
                            p.Then(RewardedAdCompleteHandler);
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
                yield return waitForOneSecond;
                waitSeconds--;

                if (adsService.IsInterstitialReady(resultAdsVO.placementId))
                {
                    //-- Show pregame Ad
                    var promise = adsService.ShowInterstitial(resultAdsVO.placementId);
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
            refreshFriendsSignal.Dispatch();
            refreshCommunitySignal.Dispatch(false);
            cancelHintSingal.Dispatch();
        }

        private void PregameAdCompleteHandler(AdsResult result = AdsResult.FINISHED)
        {
            if (result == AdsResult.FINISHED || result == AdsResult.SKIPPED)
            {
                preferencesModel.globalAdsCount++;
                preferencesModel.interstitialAdsCount++;

                if (IsPregameAd())
                {
                    preferencesModel.pregameAdsPerDayCount++;
                }

                preferencesModel.intervalBetweenPregameAds = DateTime.Now;
            }

            LoadGameStartSignal();

            resultAdsVO.OnAdCompleteCallback?.Invoke(true);
            resultAdsVO.RemoveCallback();
        }

        private void RewardedAdCompleteHandler(AdsResult result = AdsResult.FINISHED)
        {
            if (result == AdsResult.FINISHED || result == AdsResult.SKIPPED)
            {
                preferencesModel.intervalBetweenPregameAds = DateTime.Now;
            }

            resultAdsVO.OnAdCompleteCallback?.Invoke(true);
            resultAdsVO.RemoveCallback();
        }

        private void InterstitialAdCompleteHandler(AdsResult result = AdsResult.FINISHED)
        {
            if (result == AdsResult.FINISHED || result == AdsResult.SKIPPED)
            {
                preferencesModel.intervalBetweenPregameAds = DateTime.Now;
            }

            resultAdsVO.OnAdCompleteCallback?.Invoke(true);
            resultAdsVO.RemoveCallback();
        }

        private void LoadGameStartSignal()
        {
            //playerModel.adContext = AnalyticsContext.interstitial_endgame;
            Debug.Log("ACTION CODE: " + resultAdsVO.actionCode);

            if (resultAdsVO.actionCode == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                FindMatchAction.RandomLong(findMatchSignal);
            }
            else if (resultAdsVO.actionCode == FindMatchAction.ActionCode.Random1.ToString() || resultAdsVO.actionCode == FindMatchAction.ActionCode.Random3.ToString() || resultAdsVO.actionCode == FindMatchAction.ActionCode.Random.ToString()
              || resultAdsVO.actionCode == FindMatchAction.ActionCode.Random10.ToString() || resultAdsVO.actionCode == FindMatchAction.ActionCode.Random30.ToString())
            {
                //FindMatchAction.Random(findMatchSignal, resultAdsVO.actionCode.ToString(), resultAdsVO.tournamentId);
            }
            else if (resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge1.ToString() || resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge3.ToString() || resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge.ToString() ||
                resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge10.ToString() || resultAdsVO.actionCode == FindMatchAction.ActionCode.Challenge30.ToString())
            {
                FindMatchAction.Challenge(findMatchSignal, resultAdsVO.isRanked, resultAdsVO.friendId, resultAdsVO.actionCode.ToString());
            }
            else if (resultAdsVO.actionCode == "ChallengeClassic")
            {
                tapLongMatchSignal.Dispatch(resultAdsVO.friendId, resultAdsVO.isRanked);
            }
            
            else
            {
                startCPUGameSignal.Dispatch();
            }

            Release();
        }

        private bool IsPregameAd()
        {
            return (playerModel.adContext == AnalyticsContext.interstitial_pregame ||
                        playerModel.adContext == AnalyticsContext.interstitial_tournament_pregame);
        }

        private bool CanShowAd(string actionCode = null)
        {
            bool retVal = false;

            double minutesBetweenLastAdShown = (DateTime.Now - preferencesModel.intervalBetweenPregameAds).TotalMinutes;

            bool isOneMinuteGame = actionCode != null &&
                                    (actionCode == FindMatchAction.ActionCode.Challenge1.ToString() ||
                                    actionCode == FindMatchAction.ActionCode.Random1.ToString());

            if (!preferencesModel.isRateAppDialogueFirstTimeShown && resultAdsVO.adsType == AdType.Interstitial)
            {
                retVal = false;
            }
            else if (resultAdsVO.adsType == AdType.Interstitial && !IsPregameAd() && rateAppService.CanShowRateDialogue())
            {
                retVal = false;
            }
            else if (isOneMinuteGame && adsSettingsModel.showPregameInOneMinute == false)
            {
                retVal = false;
            }
            else if (preferencesModel.sessionsBeforePregameAdCount > adsSettingsModel.sessionsBeforePregameAd &&
                    preferencesModel.pregameAdsPerDayCount < adsSettingsModel.maxPregameAdsPerDay &&
                    (preferencesModel.intervalBetweenPregameAds == DateTime.MaxValue || (preferencesModel.intervalBetweenPregameAds != DateTime.MaxValue &&
                    minutesBetweenLastAdShown >= adsSettingsModel.intervalsBetweenPregameAds)))
            {
                retVal = true;
            }

            return retVal;
        }
    }
}
