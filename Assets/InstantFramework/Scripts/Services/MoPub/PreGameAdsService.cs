using System;
using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class PreGameAdsService : IPreGameAdsService
    {
        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        //Services
        [Inject] public IRateAppService rateAppService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IRoutineRunner routineRuner { get; set; }

        private IPromise servicePromise;
        private AdPlacements placementId;

        public IPromise ShowPreGameAd(string actionCode = null)
        {
            servicePromise = new Promise();
            routineRuner.StartCoroutine(ShowAd(actionCode));
            return servicePromise;
        }

        IEnumerator ShowAd(string actionCode = null)
        {
            yield return new WaitForEndOfFrame();
            playerModel.adContext = AnalyticsContext.interstitial_pregame;
            placementId = AdPlacements.Interstitial_pregame;

            if (CanShowAd(actionCode))
            {
                analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
                adsService.ShowInterstitial(placementId).Then(OnAdShown);
            }
            else
            {
                servicePromise.Dispatch();
            }
        }

        private void OnAdShown(AdsResult result)
        {
            if (result == AdsResult.FINISHED || result == AdsResult.SKIPPED)
            {
                preferencesModel.intervalBetweenPregameAds = DateTime.Now;
            }

            servicePromise.Dispatch();
        }

        private bool CanShowAd(string actionCode = null)
        {
            bool retVal = false;

            double minutesBetweenLastAdShown = (DateTime.Now - preferencesModel.intervalBetweenPregameAds).TotalMinutes;

            bool isOneMinuteGame = actionCode != null &&
                                    (actionCode == FindMatchAction.ActionCode.Challenge1.ToString() ||
                                    actionCode == FindMatchAction.ActionCode.Random1.ToString());

            if (playerModel.HasRemoveAds())
            {
                retVal = false;
            }
            else if (!adsService.IsInterstitialAvailable(placementId))
            {
                analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
                retVal = false;
            }
            else if (!preferencesModel.isRateAppDialogueFirstTimeShown)
            {
                retVal = false;
            }
            else if (rateAppService.CanShowRateDialogue())
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
