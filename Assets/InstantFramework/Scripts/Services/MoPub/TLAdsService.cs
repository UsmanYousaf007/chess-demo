using HUF.Ads.Implementation;
using HUF.AdsAdMobMediation.API;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using HUF.Ads.API;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class TLAdsService : IAdsService
    {
        //MoPubAdUnits adUnits;
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
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

            //adUnits = new MoPubAdUnits();
            //MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;
            //MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            //MoPubManager.OnAdLoadedEvent += OnBannerLoadedEvent;



            //units.OnSdkInitializedEvent += OnSdkInitializedEvent;
            //units.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            //units.OnAdLoadedEvent += OnBannerLoadedEvent;

            // MoPub.InitializeSdk(adUnits.GetGenericAdUnit());

            // MoPub.InitializeSdk(new MoPub.SdkConfiguration
            // {
            //     AdUnitId = adUnits.GetGenericAdUnit(),

            //     // Set desired log level here to override default level of MPLogLevelNone
            //     LogLevel = MoPub.LogLevel.None,

            //     // Uncomment the following line to allow supported SDK networks to collect user information on the basis
            //     // of legitimate interest.
            //     //AllowLegitimateInterest = true,

            //     // Specify the mediated networks you are using here:
            //     MediatedNetworks = new MoPub.MediatedNetwork[]
            //{
            //    /*
            //        // Example using AdMob.  Follow this template for other supported networks as well.
            //        // Note that keys must be strings, and values must be JSON-serializable (strings only, for MoPubRequestOptions).
            //        new MoPub.SupportedNetwork.AdMob
            //        {
            //            // Network adapter configuration settings (initialization).
            //            NetworkConfiguration = {
            //                { "key1", value },
            //                { "key2", value },
            //            },

            //            // Global mediation settings (per ad request).
            //            MediationSettings = {
            //                { "key1", value },
            //                { "key2", value },
            //            },

            //            // Additional options to pass to the MoPub servers (per ad request).
            //            MoPubRequestOptions = {
            //                { "key1", "value" },
            //                { "key2", "value" },
            //            }
            //        },

            //        // Example using a custom network adapter:
            //        new MoPub.MediatedNetwork
            //        {
            //            // Specify the class name that implements the AdapterConfiguration interface.
            //        #if UNITY_ANDROID
            //            AdapterConfigurationClassName = "classname",  // include the full package name
            //        #else // UNITY_IOS
            //            AdapterConfigurationClassName = "classname",
            //        #endif

            //            // Specify the class name that implements the MediationSettings interface.
            //            // Note: Custom network mediation settings are currently not supported on Android.
            //        #if UNITY_IOS
            //            MediationSettingsClassName = "classname",
            //        #endif

            //            // Fill in settings and configuration options the same way as for supported networks:

            //            NetworkConfiguration = { ... },

            //        #if UNITY_IOS  // See note above.
            //            MediationSettings    = { ... },
            //        #endif

            //            MoPubRequestOptions  = { ... },
            //        }
            //    */
            //},
            // });
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

                    if (preferencesModel.videoFinishedCount <= 20 &&
                        preferencesModel.videoFinishedCount % 5 == 0 ||
                        preferencesModel.videoFinishedCount < 5)
                    {
                        appsFlyerService.TrackRichEvent(string.Format("{0}_{1}", AnalyticsEventId.video_finished, preferencesModel.videoFinishedCount));
                    }

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

            return availableFlag;
        }

        public void OnBannerLoadedEvent(IBannerCallbackData data)
        {
            Debug.Log("[ANALYITCS]: OnBannerLoadedEvent data "+ data.ToString());
            if (appInfoModel.gameMode == GameMode.NONE || appInfoModel.isNotificationActive || appInfoModel.isReconnecting != DisconnectStats.FALSE)
            {
                HideBanner();
            }
            appsFlyerService.TrackRichEvent(AnalyticsEventId.ad_displayed.ToString());
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
            return rewardedAdPromiseOnSuccess;
        }

        public bool IsInterstitialAvailable()
        {
            //return MoPubInterstitial.IsAvailable();
            return HAds.Interstitial.IsReady();
        }

        public void ShowInterstitial()
        {
            //MoPubInterstitial.Show();
            HAds.Interstitial.TryShow();
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
        }
    }
}
