using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppodealService : IAdsService
    {
#if UNITY_IOS
        const string APP_KEY = "97dc6148eef6651b724592d50c75923d3d59caaa87ee217d";
#elif UNITY_ANDROID
        const string APP_KEY = "45b85e74beb4cde289a49529e56579676f5185185e85ac0d";
#endif
        RewardedVideoListener rewardedVideoListener = new RewardedVideoListener();

        public void Init()
        {
            //Debug.Log("[TLADS]: Initializing Appodeal with APP KEY:" + APP_KEY);

            // TODO: Get the consent bool from a GDPR popup for European countries
            Appodeal.initialize(APP_KEY, Appodeal.REWARDED_VIDEO | Appodeal.INTERSTITIAL | Appodeal.BANNER_VIEW, true);
            Appodeal.setRewardedVideoCallbacks(rewardedVideoListener);
            Appodeal.setSmartBanners(false);
            Appodeal.setTabletBanners(true);


#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Appodeal.setTesting(true);
#endif
        }

        public void ShowBanner(int x, int y)
        {
            Appodeal.showBannerView(y, Appodeal.BANNER_HORIZONTAL_CENTER, "default");
        }

        public void HideBanner()
        {
            Appodeal.hideBannerView();
        }

        public bool IsRewardedVideoAvailable()
        {
            return Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            rewardedVideoListener.rewardedVideoFinishedPromise = new Promise<AdsResult>();
            //Debug.Log("[TLADS]: Showing rewarded video");
            Appodeal.show(Appodeal.REWARDED_VIDEO);
            return rewardedVideoListener.rewardedVideoFinishedPromise;
        }

        public bool IsInterstitialAvailable()
        {
            return Appodeal.isLoaded(Appodeal.INTERSTITIAL);
        }

        public void ShowInterstitial()
        {
            //Debug.Log("[TLADS]: Showing interstitial");
            Appodeal.show(Appodeal.INTERSTITIAL);
        }
    }

    public class RewardedVideoListener : IRewardedVideoAdListener
    {
        public IPromise<AdsResult> rewardedVideoFinishedPromise;

#region Rewarded Video callback handlers
        public void onRewardedVideoFinished(double amount, string name) 
        {
            rewardedVideoFinishedPromise.Dispatch(AdsResult.FINISHED);
            //Debug.Log("[TLADS]: onRewardedVideoFinished amount:" + amount + " name:" + name);
        }

        public void onRewardedVideoClosed(bool finished)
        {
            if (!finished)
            {
                rewardedVideoFinishedPromise.Dispatch(AdsResult.FAILED);
            }

            //Debug.Log("[TLADS]: onRewardedVideoClosed finished:" + finished);
        }

        public void onRewardedVideoFailedToLoad()
        {
            rewardedVideoFinishedPromise.Dispatch(AdsResult.FAILED);
            //Debug.Log("[TLADS]: onRewardedVideoFailedToLoad");
        }

        public void onRewardedVideoExpired()
        {
            rewardedVideoFinishedPromise.Dispatch(AdsResult.FAILED);
            //Debug.Log("[TLADS]: onRewardedVideoExpired");
        }

        public void onRewardedVideoLoaded(bool isPrecache) 
        {
            //Debug.Log("[TLADS]: onRewardedVideoLoaded isPrecache:" + isPrecache);
        }

        public void onRewardedVideoShown() 
        {
            //Debug.Log("[TLADS]: onRewardedVideoShown");
        }
#endregion
    }
}
