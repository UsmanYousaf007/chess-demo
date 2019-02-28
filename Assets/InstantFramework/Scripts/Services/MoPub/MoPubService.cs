using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class MoPubService : IAdsService
    {
        MoPubAdUnits adUnits;

        public void Init()
        {
            adUnits = new MoPubAdUnits();
            MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;
            MoPub.InitializeSdk(adUnits.GetGenericAdUnit());
        }

        void OnSdkInitializedEvent(string p1)
        {
            Debug.Log("MoPub: Initialized: " + p1);
            MoPubRewardedVideo.Initialize(adUnits);
            MoPubInterstitial.Initialize(adUnits);
            MoPubBanner.Initialize(adUnits);
        }

        public bool IsRewardedVideoAvailable()
        {
            return MoPubRewardedVideo.IsAvailable();
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            return MoPubRewardedVideo.Show();
        }

        public bool IsInterstitialAvailable()
        {
            return MoPubInterstitial.IsAvailable();
        }

        public void ShowInterstitial()
        {
            MoPubInterstitial.Show();
        }

        public void ShowBanner()
        {
            MoPubBanner.Show(MoPubBase.AdPosition.TopCenter);
        }

        public void HideBanner()
        {
            MoPubBanner.Hide();
        }
    }
}
