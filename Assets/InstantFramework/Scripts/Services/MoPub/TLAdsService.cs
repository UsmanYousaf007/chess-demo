using HUF.Ads.Implementation;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using HUF.Ads.API;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class TLAdsService : IAdsService
    {
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private IPromise<AdsResult> rewardedAdPromiseOnSuccess;
        private bool bannerDisplay = false;

        public void Init()
        {
            HAds.Interstitial.Fetch();
            analyticsService.Event(AnalyticsEventId.ad_requested, AnalyticsContext.interstitial_endgame);
            HAds.Rewarded.Fetch();
            analyticsService.Event(AnalyticsEventId.ad_requested, AnalyticsContext.rewarded);

            HAds.Banner.OnShown += OnBannerLoadedEvent;
            HAds.Banner.OnClicked += OnBannerClicked;

            HAds.Interstitial.OnEnded += OnInterstitailEnded;
            HAds.Interstitial.OnClicked += OnInterstitialClicked;
            HAds.Interstitial.OnFetched += OnInterstitialClicked;

            HAds.Rewarded.OnEnded += OnRewardedEnded;
            HAds.Rewarded.OnFetched += OnRewardedVideoLoadedEvent;
            HAds.Rewarded.OnClicked += OnRewardedClicked;

            bannerDisplay = false;
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus && playerModel.adContext != AnalyticsContext.unknown)
            {
                analyticsService.Event(AnalyticsEventId.ad_player_shutdown, playerModel.adContext);
            }
        }

        private void OnApplicationQuit()
        {
            if (playerModel.adContext != AnalyticsContext.unknown)
            {
                analyticsService.Event(AnalyticsEventId.ad_player_shutdown, playerModel.adContext);
            }
        }

        void OnRewardedClicked(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, AnalyticsContext.rewarded);
        }

        void OnInterstitialClicked(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, playerModel.adContext);
        }

        void OnRewardedEnded(IAdCallbackData data)
        {
            HAds.Rewarded.Fetch();
            analyticsService.Event(AnalyticsEventId.ad_requested, AnalyticsContext.rewarded);

            switch (data.Result)
            {
                case AdResult.Completed:
                    preferencesModel.videoFinishedCount++;

                    var videoEventData = new Dictionary<string, string>();
                    videoEventData.Add("network", data.ProviderId);
                    videoEventData.Add("placement", data.PlacementId);

                    appsFlyerService.TrackRichEvent(AnalyticsEventId.video_finished.ToString(), videoEventData);
                    appsFlyerService.TrackLimitedEvent(AnalyticsEventId.video_finished, preferencesModel.videoFinishedCount);

                    hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "rewarded_result_2xcoins", data.ProviderId);
                    analyticsService.Event(AnalyticsEventId.ad_completed, AnalyticsContext.rewarded);

                    rewardedAdPromiseOnSuccess.Dispatch(AdsResult.FINISHED);
                    break;
                case AdResult.Skipped:
                    analyticsService.Event(AnalyticsEventId.ad_skipped, AnalyticsContext.rewarded);
                    rewardedAdPromiseOnSuccess.Dispatch(AdsResult.SKIPPED);
                    break;
                case AdResult.Failed:
                    analyticsService.Event(AnalyticsEventId.ad_failed, AnalyticsContext.rewarded);
                    rewardedAdPromiseOnSuccess.Dispatch(AdsResult.FAILED);
                    break;
            }

            playerModel.adContext = AnalyticsContext.unknown;
        }

        void OnInterstitailEnded(IAdCallbackData data)
        {
            switch (data.Result)
            {
                case AdResult.Completed:
                    analyticsService.Event(AnalyticsEventId.ad_completed, playerModel.adContext);
                    break;
                case AdResult.Skipped:
                    analyticsService.Event(AnalyticsEventId.ad_skipped, playerModel.adContext);
                    break;
                case AdResult.Failed:
                    analyticsService.Event(AnalyticsEventId.ad_failed, playerModel.adContext);
                    break;
            }

            HAds.Interstitial.Fetch();
            analyticsService.Event(AnalyticsEventId.ad_requested, playerModel.adContext);

            hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "interstitial", data.ProviderId);
            rewardedAdPromiseOnSuccess.Dispatch(AdsResult.FINISHED);
            playerModel.adContext = AnalyticsContext.unknown;
        }

        public bool IsRewardedVideoAvailable()
        {
            bool availableFlag = HAds.Rewarded.IsReady();

            if (!availableFlag)
            {
                analyticsService.Event(AnalyticsEventId.ad_not_available, AnalyticsContext.rewarded);
                Debug.Log("[ANALYITCS]: ads_rewared_request:");
                HAds.Rewarded.Fetch();
                analyticsService.Event(AnalyticsEventId.ad_requested, AnalyticsContext.rewarded);
            }

            bool isNotCapped = (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.rewardedVideoCap == 0 || preferencesModel.rewardedAdsCount <= adsSettingsModel.rewardedVideoCap);

            if (!isNotCapped)
            {
                analyticsService.Event(AnalyticsEventId.ad_cap_reached, AnalyticsContext.rewarded);
                playerModel.adContext = AnalyticsContext.interstitial_rewarded_capped_replacement;
            }


            if(!availableFlag)
            {
                playerModel.adContext = AnalyticsContext.interstitial_rewarded_failed_replacement;
            }

            return availableFlag && isNotCapped;
        }

        public void OnBannerLoadedEvent(IBannerCallbackData data)
        {
            Debug.Log("TLAdsService::OnBannerLoadedEvent() called.");
            Debug.Log("[ANALYITCS]: OnBannerLoadedEvent data "+ data.ToString());

            if (bannerDisplay == false)
            {
                Debug.Log("TLAdsService::OnBannerLoadedEvent() will hide ad.");
                HideBanner();
                return;
            }

            Debug.Log("TLAdsService::OnBannerLoadedEvent() will NOT hide ad.");

            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_displayed.ToString());
            hAnalyticsService.LogEvent(AnalyticsEventId.ad_displayed.ToString(), "monetization", "banner", data.ProviderId);
        }

        public void OnRewardedVideoLoadedEvent(IAdCallbackData data)
        {
            Debug.Log("[ANALYITCS]: ads_rewared_success: Result" + data.Result.ToString());
            Debug.Log("[ANALYITCS]: IAdCallbackData ProviderId:" + data.ProviderId);
            analyticsService.Event(AnalyticsEventId.ad_available, AnalyticsContext.rewarded);
        }

        public void OnInterstitialLoadedEvent(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_available, playerModel.adContext);
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            rewardedAdPromiseOnSuccess = new Promise<AdsResult>();
            HAds.Rewarded.TryShow();
            hAnalyticsService.LogEvent(AnalyticsEventId.video_started.ToString(), "monetization", "rewarded_result_2xcoins", HAds.Rewarded.GetAdProviderName());
            return rewardedAdPromiseOnSuccess;
        }

        public bool IsInterstitialAvailable()
        {
            var availableFlag = HAds.Interstitial.IsReady();

            if (!availableFlag)
            {
                analyticsService.Event(AnalyticsEventId.ad_not_available, playerModel.adContext);
                HAds.Interstitial.Fetch();
                analyticsService.Event(AnalyticsEventId.ad_requested, playerModel.adContext);

            }

            bool isNotCapped = (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.interstitialCap == 0 || preferencesModel.interstitialAdsCount <= adsSettingsModel.interstitialCap);

            if (!isNotCapped)
            {
                analyticsService.Event(AnalyticsEventId.ad_cap_reached, playerModel.adContext);
            }

            return availableFlag && isNotCapped;
                //(adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                //(adsSettingsModel.interstitialCap == 0 || preferencesModel.interstitialAdsCount <= adsSettingsModel.interstitialCap);
        }

        public IPromise<AdsResult> ShowInterstitial()
        {
            rewardedAdPromiseOnSuccess = new Promise<AdsResult>();
            HAds.Interstitial.TryShow();
            hAnalyticsService.LogEvent(AnalyticsEventId.video_started.ToString(), "monetization", "interstitial", HAds.Interstitial.GetAdProviderName());
            return rewardedAdPromiseOnSuccess;
        }

        public void ShowBanner()
        {
            Debug.Log("TLAdsService::ShowBanner() called.");
            bannerDisplay = true;
            HAds.Banner.Show(BannerPosition.TopCenter);
        }

        public void HideBanner()
        {
            Debug.Log("TLAdsService::HideBanner() called.");
            bannerDisplay = false;
            HAds.Banner.Hide();
        }

        private void OnBannerClicked(IBannerCallbackData data)
        {
            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_clicked.ToString());
            hAnalyticsService.LogEvent(AnalyticsEventId.ad_clicked.ToString(), "monetization", "banner", data.ProviderId);
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            HAds.CollectSensitiveData(consentStatus);
        }

        public void ShowTestSuite()
        {
            HUF.AdsAdMobMediation.API.HAdsAdMobMediation.ShowTestSuite();
        }
    }
}
