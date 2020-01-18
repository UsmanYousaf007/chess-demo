using HUF.Ads.Implementation;
using HUF.AdsAdMobMediation.API;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using HUF.Ads.API;
using System.Collections.Generic;
using HUF.Analytics.API;

namespace TurboLabz.InstantFramework
{
    public class TLAdsService : IAdsService
    {
        //MoPubAdUnits adUnits;
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        IPromise<AdsResult> rewardedAdPromiseOnSuccess;

        public void Init()
        {
            HAdsAdMobMediation.Init();
            HAds.Interstitial.Fetch();
            HAds.Rewarded.Fetch();

            HAds.Banner.OnShown += OnBannerLoadedEvent;
            HAds.Banner.OnClicked += OnBannerClicked;
            HAds.Interstitial.OnEnded += OnInterstitailEnded;
            HAds.Rewarded.OnEnded += OnRewardedEnded;
            HAds.Rewarded.OnFetched += OnRewardedVideoLoadedEvent;
        }

        void OnRewardedEnded(IAdCallbackData data)
        {
            HAds.Rewarded.Fetch();

            switch(data.Result)
            {
                case AdResult.Completed:
                    var videoEventData = new Dictionary<string, string>();
                    videoEventData.Add("network", data.ProviderId);
                    videoEventData.Add("placement", data.PlacementId);

                    appsFlyerService.TrackRichEvent(AnalyticsEventId.video_finished.ToString(), videoEventData);

                    preferencesModel.videoFinishedCount++;
                    appsFlyerService.TrackLimitedEvent(AnalyticsEventId.video_finished, preferencesModel.videoFinishedCount);

                    var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.video_finished.ToString())
                        .ST1("monetization")
                        .ST2("rewarded_result_2xcoins")
                        .ST3(data.ProviderId);
                    HAnalytics.LogEvent(analyticsEvent);

                    rewardedAdPromiseOnSuccess.Dispatch(AdsResult.FINISHED);
                    break;
                case AdResult.Skipped:
                    break;
                case AdResult.Failed:
                    break;
            }
        }

        void OnInterstitailEnded(IAdCallbackData data)
        {
            HAds.Interstitial.Fetch();
            var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.video_finished.ToString())
                .ST1("monetization")
                .ST2("interstitial")
                .ST3(data.ProviderId);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public bool IsRewardedVideoAvailable()
        {
            //bool availableFlag = MoPubRewardedVideo.IsAvailable();
            bool availableFlag = HAds.Rewarded.IsReady();

            if (!availableFlag)
            {
                Debug.Log("[ANALYITCS]: ads_rewared_request:");
                analyticsService.Event(AnalyticsEventId.ads_rewared_request);
            }

            return availableFlag &&
                (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.rewardedVideoCap == 0 || preferencesModel.rewardedAdsCount <= adsSettingsModel.rewardedVideoCap);
        }

        public void OnBannerLoadedEvent(IBannerCallbackData data)
        {
            Debug.Log("[ANALYITCS]: OnBannerLoadedEvent data "+ data.ToString());
            if (appInfoModel.gameMode == GameMode.NONE || appInfoModel.isNotificationActive || appInfoModel.isReconnecting != DisconnectStates.FALSE)
            {
                HideBanner();
            }

            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_displayed.ToString());
            var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.ad_displayed.ToString())
                .ST1("monetization")
                .ST2("banner")
                .ST3(data.ProviderId);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void OnRewardedVideoLoadedEvent(IAdCallbackData data)
        {
            Debug.Log("[ANALYITCS]: ads_rewared_success: Result" + data.Result.ToString());
            Debug.Log("[ANALYITCS]: IAdCallbackData ProviderId:" + data.ProviderId);

            //switch (data.Result)
            //{
            //    case AdResult.Completed:
            //        analyticsService.Event(AnalyticsEventId.ads_rewared_success);
            //        break;
            //    case AdResult.Skipped:
            //        break;
            //    case AdResult.Failed:
            //        break;
            //}
            //analyticsService.Event(AnalyticsEventId.ads_rewared_success);
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            //return MoPubRewardedVideo.Show();
            rewardedAdPromiseOnSuccess = new Promise<AdsResult>();
            HAds.Rewarded.TryShow();
            var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.video_started.ToString())
                .ST1("monetization")
                .ST2("rewarded_result_2xcoins")
                .ST3(HAds.Rewarded.GetAdProviderName());
            HAnalytics.LogEvent(analyticsEvent);
            return rewardedAdPromiseOnSuccess;
        }

        public bool IsInterstitialAvailable()
        {
            return HAds.Interstitial.IsReady() &&
                (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.interstitialCap == 0 || preferencesModel.interstitialAdsCount <= adsSettingsModel.interstitialCap);
        }

        public void ShowInterstitial()
        {
            //MoPubInterstitial.Show();
            HAds.Interstitial.TryShow();
            var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.video_started.ToString())
                .ST1("monetization")
                .ST2("interstitial")
                .ST3(HAds.Interstitial.GetAdProviderName());
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void ShowBanner()
        {
            // MoPubBanner.Show(MoPub.AdPosition.TopCenter);
            HAds.Banner.Show(BannerPosition.TopCenter);
        }

        public void HideBanner()
        {
            HAds.Banner.Hide();
           // MoPubBanner.Hide();
        }

        private void OnBannerClicked(IBannerCallbackData data)
        {
            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_clicked.ToString());
            var analyticsEvent = AnalyticsEvent.Create(AnalyticsEventId.ad_clicked.ToString())
                .ST1("monetization")
                .ST2("banner")
                .ST3(data.ProviderId);
            HAnalytics.LogEvent(analyticsEvent);
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            HAds.CollectSensitiveData(consentStatus);
        }
    }
}
