using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using System.Collections.Generic;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.AdsAdMobMediation.Runtime.API;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.AdManagers;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class TLAdsService : IAdsService
    {
        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        [Inject] public AppEventSignal appEventSignal { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private IPromise<AdsResult> adEndedPromise;
        private bool bannerDisplay = false;
        private long videoStartTime = 0;

        private const string PLACEMENT_ID_BANNER = "Banner";
        private const string PLACEMENT_ID_REWARDED = "Rewarded";
        private const string PLACEMENT_ID_INTERSTITIAL = "Interstitial";

        public bool adStatus = false;
        public bool showAd = false;

        public void Init()
        {
            appEventSignal.AddListener(OnAppEvent);
            playerModel.adContext = AnalyticsContext.interstitial_endgame;
            HAdsManager.OnAdFetch += OnAdFetched;
            HAds.Banner.OnClicked += OnBannerClicked;
            HAds.Interstitial.OnClicked += OnInterstitialClicked;
            HAds.Rewarded.OnClicked += OnRewardedClicked;
            HAdsManager.SetNewBannerPosition(BannerPosition.TopCenter);
            bannerDisplay = false;
        }

        private void OnAdFetched(AdsManagerFetchCallbackData callbackData)
        {
            switch (callbackData.PlacementId)
            {
                case PLACEMENT_ID_REWARDED:
                    analyticsService.Event(AnalyticsEventId.ad_available, AnalyticsContext.rewarded);
                    break;

                case PLACEMENT_ID_INTERSTITIAL:
                    analyticsService.Event(AnalyticsEventId.ad_available, playerModel.adContext);
                    break;
            }
        }

        public IPromise<AdsResult> ShowInterstitial()
        {
            adStatus = false;
            showAd = true;
            videoStartTime = backendService.serverClock.currentTimestamp;
            adEndedPromise = new Promise<AdsResult>();
            HAdsManager.ShowAd(PLACEMENT_ID_INTERSTITIAL, OnInterstitailEnded);
            hAnalyticsService.LogEvent(AnalyticsEventId.video_started.ToString(), "monetization", "interstitial", HAds.Interstitial.GetAdProviderName(),
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)));
            return adEndedPromise;
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            adStatus = false;
            showAd = true;
            videoStartTime = backendService.serverClock.currentTimestamp;
            adEndedPromise = new Promise<AdsResult>();
            HAdsManager.ShowAd(PLACEMENT_ID_REWARDED, OnRewardedEnded);
            hAnalyticsService.LogEvent(AnalyticsEventId.video_started.ToString(), "monetization", "rewarded_result_2xcoins", HAds.Rewarded.GetAdProviderName(),
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)));
            return adEndedPromise;
        }

        public void ShowBanner()
        {
            bannerDisplay = true;
            HAdsManager.ShowAd(PLACEMENT_ID_BANNER, OnBannerShown);
        }

        public void HideBanner()
        {
            bannerDisplay = false;
            HAdsManager.HideBanner(PLACEMENT_ID_BANNER);
        }

        private void OnRewardedClicked(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, AnalyticsContext.rewarded);
        }

        private void OnInterstitialClicked(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, playerModel.adContext);
        }

        private void OnBannerClicked(IBannerCallbackData data)
        {
            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_clicked.ToString());
            hAnalyticsService.LogEvent(AnalyticsEventId.ad_clicked.ToString(), "monetization", "banner", data.ProviderId);
        }

        public bool IsRewardedVideoAvailable()
        {
            bool availableFlag = HAdsManager.CanShowAd(PLACEMENT_ID_REWARDED);
            bool isNotCapped = (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.rewardedVideoCap == 0 || preferencesModel.rewardedAdsCount <= adsSettingsModel.rewardedVideoCap);

            if (!availableFlag)
            {
                analyticsService.Event(AnalyticsEventId.ad_not_available, AnalyticsContext.rewarded);
                playerModel.adContext = AnalyticsContext.interstitial_rewarded_failed_replacement;
                adStatus = true;
            }
            else if (!isNotCapped)
            {
                analyticsService.Event(AnalyticsEventId.ad_cap_reached, AnalyticsContext.rewarded);
                playerModel.adContext = AnalyticsContext.interstitial_rewarded_capped_replacement;
                adStatus = true;
            }

            return availableFlag && isNotCapped;
        }

        public bool IsInterstitialAvailable()
        {
            var availableFlag = HAdsManager.CanShowAd(PLACEMENT_ID_INTERSTITIAL);
            bool isNotCapped = (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.interstitialCap == 0 || preferencesModel.interstitialAdsCount <= adsSettingsModel.interstitialCap);

            if (!availableFlag)
            {
                analyticsService.Event(AnalyticsEventId.ad_not_available, playerModel.adContext);
                adStatus = true;
            }
            else if (!isNotCapped)
            {
                analyticsService.Event(AnalyticsEventId.ad_cap_reached, playerModel.adContext);
                adStatus = true;
            }

            return availableFlag && isNotCapped;
        }

        void OnRewardedEnded(AdManagerCallback data)
        {
            preferencesModel.videoFinishedCount++;

            var videoEventData = new Dictionary<string, string>();
            videoEventData.Add("network", data.ProviderId);
            videoEventData.Add("placement", data.PlacementId);

            appsFlyerService.TrackRichEvent(AnalyticsEventId.video_finished.ToString(), videoEventData);
            appsFlyerService.TrackLimitedEvent(AnalyticsEventId.video_finished, preferencesModel.videoFinishedCount);

            hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "rewarded_result_2xcoins", data.ProviderId,
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)),
                new KeyValuePair<string, object>("duration", (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - TimeUtil.ToDateTime(videoStartTime)).TotalSeconds),
                new KeyValuePair<string, object>("end_type", data.Result.ToString()));

            switch (data.Result)
            {
                case AdResult.Completed:
                    analyticsService.Event(AnalyticsEventId.ad_completed, AnalyticsContext.rewarded);
                    adEndedPromise.Dispatch(AdsResult.FINISHED);
                    break;

                case AdResult.Skipped:
                    analyticsService.Event(AnalyticsEventId.ad_skipped, AnalyticsContext.rewarded);
                    adEndedPromise.Dispatch(AdsResult.SKIPPED);
                    break;

                case AdResult.Failed:
                    analyticsService.Event(AnalyticsEventId.ad_failed, AnalyticsContext.rewarded);
                    adEndedPromise.Dispatch(AdsResult.FAILED);
                    break;
            }
            adStatus = true;
            showAd = false;
            //playerModel.adContext = AnalyticsContext.unknown;
        }

        void OnInterstitailEnded(AdManagerCallback data)
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

            hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "interstitial", data.ProviderId,
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)),
                new KeyValuePair<string, object>("duration", (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - TimeUtil.ToDateTime(videoStartTime)).TotalSeconds),
                new KeyValuePair<string, object>("end_type", data.Result.ToString()));
            adEndedPromise.Dispatch(AdsResult.FINISHED);

            adStatus = true;
            showAd = false;
            //playerModel.adContext = AnalyticsContext.unknown;
        }

        public void OnBannerShown(AdManagerCallback data)
        {
            if (bannerDisplay == false)
            {
                HideBanner();
                return;
            }

            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_displayed.ToString());
            hAnalyticsService.LogEvent(AnalyticsEventId.ad_displayed.ToString(), "monetization", "banner", data.ProviderId);
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            HAds.CollectSensitiveData(consentStatus);
        }

        public void ShowTestSuite()
        {
            HAdsAdMobMediation.ShowTestSuite();
        }

        public void OnAppEvent(AppEvent evt)
        {
            /*if (evt == AppEvent.ESCAPED && showAd && !adStatus)
            {
                analyticsService.Event(AnalyticsEventId.ad_player_shutdown, playerModel.adContext);
            }
            else if(evt == AppEvent.PAUSED && showAd && !adStatus)
            {
                analyticsService.Event(AnalyticsEventId.ad_player_shutdown, playerModel.adContext);
            }*/
        }
    }
}
