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

        private IPromise<AdsResult> rewardedAdPromiseOnSuccess;
        private bool bannerDisplay = false;

        public void Init()
        {
            HAds.Interstitial.Fetch();
            HAds.Rewarded.Fetch();

            HAds.Banner.OnShown += OnBannerLoadedEvent;
            HAds.Banner.OnClicked += OnBannerClicked;
            HAds.Interstitial.OnEnded += OnInterstitailEnded;
            HAds.Rewarded.OnEnded += OnRewardedEnded;
            HAds.Rewarded.OnFetched += OnRewardedVideoLoadedEvent;

            bannerDisplay = false;
        }

        void OnRewardedEnded(IAdCallbackData data)
        {
            HAds.Rewarded.Fetch();

            switch(data.Result)
            {
                case AdResult.Completed:
                    preferencesModel.videoFinishedCount++;

                    var videoEventData = new Dictionary<string, string>();
                    videoEventData.Add("network", data.ProviderId);
                    videoEventData.Add("placement", data.PlacementId);

                    appsFlyerService.TrackRichEvent(AnalyticsEventId.video_finished.ToString(), videoEventData);
                    appsFlyerService.TrackLimitedEvent(AnalyticsEventId.video_finished, preferencesModel.videoFinishedCount);
                    hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "rewarded_result_2xcoins", data.ProviderId);

                    rewardedAdPromiseOnSuccess.Dispatch(AdsResult.FINISHED);
                    break;
                case AdResult.Skipped:
                    rewardedAdPromiseOnSuccess.Dispatch(AdsResult.SKIPPED);
                    break;
                case AdResult.Failed:
                    break;
            }
        }

        void OnInterstitailEnded(IAdCallbackData data)
        {
            HAds.Interstitial.Fetch();
            hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "interstitial", data.ProviderId);
            rewardedAdPromiseOnSuccess.Dispatch(AdsResult.FINISHED);
        }

        public bool IsRewardedVideoAvailable()
        {
            bool availableFlag = HAds.Rewarded.IsReady();

            if (!availableFlag)
            {
                Debug.Log("[ANALYITCS]: ads_rewared_request:");
                HAds.Rewarded.Fetch();
                analyticsService.Event(AnalyticsEventId.ads_rewared_request);
            }

            return availableFlag &&
                (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.rewardedVideoCap == 0 || preferencesModel.rewardedAdsCount <= adsSettingsModel.rewardedVideoCap);
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
                HAds.Interstitial.Fetch();
            }

            return availableFlag &&
                (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.interstitialCap == 0 || preferencesModel.interstitialAdsCount <= adsSettingsModel.interstitialCap);
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
    }
}
