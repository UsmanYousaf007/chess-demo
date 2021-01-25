using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using System.Collections.Generic;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
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
        [Inject] public INavigatorModel navigatorModel { get; set; }

        //Dispatch Signals
        [Inject] public RewardedVideoAvailableSignal rewardedVideoAvailableSignal { get; set; }

        private IPromise<AdsResult> adEndedPromise;
        private long videoStartTime = 0;

        private const string PLACEMENT_ID_BANNER = "Banner";
        //private const string PLACEMENT_ID_REWARDED = "Rewarded";
        //private const string PLACEMENT_ID_INTERSTITIAL = "Interstitial";

        public void Init()
        {
            appEventSignal.AddListener(OnAppEvent);
            playerModel.adContext = AnalyticsContext.interstitial_endgame;
            HAdsManager.OnAdFetch += OnAdFetched;
            HAds.Banner.OnClicked += OnBannerClicked;
            HAds.Banner.OnFailed += OnBannerFailed;
            HAds.Interstitial.OnClicked += OnInterstitialClicked;
            HAds.Rewarded.OnClicked += OnRewardedClicked;
            HAdsManager.SetNewBannerPosition(BannerPosition.TopCenter);
        }

        private void OnAdFetched(AdsManagerFetchCallbackData callbackData)
        {
            var placementId = CollectionsUtil.GetAdPlacementsIdFromString(callbackData.PlacementId);

            switch (placementId)
            {
                case AdPlacements.Rewarded_daily_reward:
                case AdPlacements.Rewarded_lobby_chest:
                case AdPlacements.Rewarded_coins_purchase:
                case AdPlacements.Rewarded_coins_banner:
                case AdPlacements.Rewarded_coins_popup:
                    analyticsService.Event(AnalyticsEventId.ad_available, AnalyticsContext.rewarded);
                    rewardedVideoAvailableSignal.Dispatch(placementId);
                    break;

                case AdPlacements.Interstitial_endgame:
                case AdPlacements.Interstitial_pregame:
                case AdPlacements.interstitial_in_game_30_min:
                case AdPlacements.interstitial_in_game_cpu:
                    analyticsService.Event(AnalyticsEventId.ad_available, AnalyticsContext.interstitial);
                    break;

                case AdPlacements.Banner:
                    analyticsService.Event(AnalyticsEventId.ad_available, AnalyticsContext.banner);
                    break;
            }
        }

        public IPromise<AdsResult> ShowInterstitial(AdPlacements placementId)
        {
            videoStartTime = backendService.serverClock.currentTimestamp;
            adEndedPromise = new Promise<AdsResult>();
            HAdsManager.ShowAd(placementId.ToString(), OnInterstitailEnded);
            analyticsService.Event(AnalyticsEventId.ad_shown, playerModel.adContext);
            hAnalyticsService.LogEvent(AnalyticsEventId.video_started.ToString(), "monetization", "interstitial", HAds.Interstitial.GetAdProviderName(),
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)));
            return adEndedPromise;
        }

        public IPromise<AdsResult> ShowRewardedVideo(AdPlacements placementId)
        {
            videoStartTime = backendService.serverClock.currentTimestamp;
            adEndedPromise = new Promise<AdsResult>();
            HAdsManager.ShowAd(placementId.ToString(), OnRewardedEnded);
            analyticsService.Event(AnalyticsEventId.ad_shown, playerModel.adContext);
            hAnalyticsService.LogEvent(AnalyticsEventId.video_started.ToString(), "monetization", "rewarded_result_2xcoins", HAds.Rewarded.GetAdProviderName(),
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)));
            return adEndedPromise;
        }

        public void ShowBanner()
        {
            HAdsManager.ShowAd(PLACEMENT_ID_BANNER, OnBannerShown);
            analyticsService.Event(AnalyticsEventId.ad_user_requested, AnalyticsContext.banner);
        }

        public void HideBanner()
        {
            HAdsManager.HideBanner(PLACEMENT_ID_BANNER);
        }

        private void OnRewardedClicked(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, playerModel.adContext);
        }

        private void OnInterstitialClicked(IAdCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, playerModel.adContext);
        }

        private void OnBannerClicked(IBannerCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_clicked, AnalyticsContext.banner);
            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_clicked.ToString());
            hAnalyticsService.LogEvent(AnalyticsEventId.ad_clicked.ToString(), "monetization", "banner", data.ProviderId);
        }

        public bool IsRewardedVideoAvailable(AdPlacements placementId)
        {
            bool availableFlag = HAdsManager.CanShowAd(placementId.ToString());
            bool isNotCapped = (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.rewardedVideoCap == 0 || preferencesModel.rewardedAdsCount <= adsSettingsModel.rewardedVideoCap);

            if (!availableFlag)
            {
                analyticsService.Event(AnalyticsEventId.ad_not_available, playerModel.adContext);
                //playerModel.adContext = AnalyticsContext.interstitial_rewarded_failed_replacement;
            }
            else if (!isNotCapped)
            {
                analyticsService.Event(AnalyticsEventId.ad_cap_reached, playerModel.adContext);
                playerModel.adContext = AnalyticsContext.interstitial_rewarded_capped_replacement;
            }

            return availableFlag && isNotCapped;
        }

        /// <summary>
        /// Use this to chcek if an interstitial ad is loaded and ready to show.
        /// </summary>
        /// <returns></returns>
        public bool IsInterstitialReady(AdPlacements placementId)
        {
            return HAdsManager.CanShowAd(placementId.ToString());
        }

        public bool IsInterstitialNotCapped()
        {
            return (adsSettingsModel.globalCap == 0 || preferencesModel.globalAdsCount <= adsSettingsModel.globalCap) &&
                (adsSettingsModel.interstitialCap == 0 || preferencesModel.interstitialAdsCount <= adsSettingsModel.interstitialCap);
        }

        public bool IsInterstitialAvailable(AdPlacements placementId)
        {
            bool availableFlag = IsInterstitialReady(placementId);
            bool isNotCapped = IsInterstitialNotCapped();

            if (!availableFlag)
            {
                analyticsService.Event(AnalyticsEventId.ad_not_available, playerModel.adContext);
            }
            else if (!isNotCapped)
            {
                analyticsService.Event(AnalyticsEventId.ad_cap_reached, playerModel.adContext);
            }

            return availableFlag && isNotCapped;
        }

        void OnRewardedEnded(AdManagerCallback data)
        {
            preferencesModel.videoFinishedCount++;

            var videoEventData = new Dictionary<string, object>();
            videoEventData.Add("network", data.ProviderId);
            videoEventData.Add("placement", data.PlacementId);

            appsFlyerService.TrackRichEvent(AnalyticsEventId.video_finished.ToString(), videoEventData);
            appsFlyerService.TrackLimitedEvent(AnalyticsEventId.video_finished, preferencesModel.videoFinishedCount);

            hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "rewarded_result_2xcoins", data.ProviderId,
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)),
                new KeyValuePair<string, object>("duration", (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - TimeUtil.ToDateTime(videoStartTime)).TotalSeconds),
                new KeyValuePair<string, object>("end_type", data.Result.ToString()));

            if(!preferencesModel.isInstallDayOver && preferencesModel.videoFinishedCount <= 10)
            {
                appsFlyerService.TrackRichEvent("install_day_video_finished_" + preferencesModel.videoFinishedCount, videoEventData);

                hAnalyticsService.LogEvent("install_day_video_finished_" + preferencesModel.videoFinishedCount, "monetization", "rewarded_result_2xcoins", data.ProviderId,
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)),
                new KeyValuePair<string, object>("duration", (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - TimeUtil.ToDateTime(videoStartTime)).TotalSeconds),
                new KeyValuePair<string, object>("end_type", data.Result.ToString()));
            }

            switch (data.Result)
            {
                case AdResult.Completed:
                    analyticsService.Event(AnalyticsEventId.ad_completed, playerModel.adContext);
                    adEndedPromise.Dispatch(AdsResult.FINISHED);
                    break;

                case AdResult.Skipped:
                    analyticsService.Event(AnalyticsEventId.ad_skipped, playerModel.adContext);
                    adEndedPromise.Dispatch(AdsResult.SKIPPED);
                    break;

                case AdResult.Failed:
                    analyticsService.Event(AnalyticsEventId.ad_failed, playerModel.adContext);
                    adEndedPromise.Dispatch(AdsResult.FAILED);
                    break;
            }
        }

        void OnInterstitailEnded(AdManagerCallback data)
        {
            AdsResult adResultInstantFramework = AdsResult.FINISHED;
            switch (data.Result)
            {
                case AdResult.Completed:
                    analyticsService.Event(AnalyticsEventId.ad_completed, playerModel.adContext);
                    adResultInstantFramework = AdsResult.FINISHED;
                    break;

                case AdResult.Skipped:
                    analyticsService.Event(AnalyticsEventId.ad_skipped, playerModel.adContext);
                    adResultInstantFramework = AdsResult.SKIPPED;
                    break;

                case AdResult.Failed:
                    adResultInstantFramework = AdsResult.FAILED;
                    analyticsService.Event(AnalyticsEventId.ad_failed, playerModel.adContext);
                    break;
            }

            hAnalyticsService.LogEvent(AnalyticsEventId.video_finished.ToString(), "monetization", "interstitial", data.ProviderId,
                new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, videoStartTime)),
                new KeyValuePair<string, object>("duration", (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - TimeUtil.ToDateTime(videoStartTime)).TotalSeconds),
                new KeyValuePair<string, object>("end_type", data.Result.ToString()));
            adEndedPromise.Dispatch(adResultInstantFramework);
        }

        public void OnBannerShown(AdManagerCallback data)
        {
            if (data.Result == AdResult.Completed)
            {
                var currentState = navigatorModel.currentState.GetType();
                var previousState = navigatorModel.previousState.GetType();
                var canShowBanner = currentState == typeof(NSMultiplayer) ||
                                    currentState == typeof(NSCPU) ||
                                   (currentState == typeof(NSChat) && previousState == typeof(NSMultiplayer));

                if (!canShowBanner)
                {
                    //GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Debug, $"OnBannerShown.currentState.{currentState}");
                    HideBanner();
                }

                analyticsService.Event(AnalyticsEventId.ad_shown, AnalyticsContext.banner);
                appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_displayed.ToString());
                hAnalyticsService.LogEvent(AnalyticsEventId.ad_displayed.ToString(), "monetization", "banner", data.ProviderId);
            }
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            HAds.CollectSensitiveData(consentStatus);
        }

        public void ShowTestSuite()
        {
            //HAdsManager.ShowAd(PLACEMENT_ID_INTERSTITIAL, null);
        }

        public void OnAppEvent(AppEvent evt)
        {
            //if (evt == AppEvent.ESCAPED && showAd && !adStatus)
            //{
            //    analyticsService.Event(AnalyticsEventId.ad_player_shutdown, playerModel.adContext);
            //}
            //else if(evt == AppEvent.PAUSED && showAd && !adStatus)
            //{
            //    analyticsService.Event(AnalyticsEventId.ad_player_shutdown, playerModel.adContext);
            //}
        }

        private void OnBannerFailed(IBannerCallbackData data)
        {
            analyticsService.Event(AnalyticsEventId.ad_failed, AnalyticsContext.banner);
        }
    }
}
