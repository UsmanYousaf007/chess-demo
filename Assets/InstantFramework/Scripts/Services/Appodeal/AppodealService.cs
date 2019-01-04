using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppodealService : IAdsService
    {
        const string APP_KEY = "45b85e74beb4cde289a49529e56579676f5185185e85ac0d";
        RewardedVideoListener rewardedVideoListener = new RewardedVideoListener();

        public void Init()
        {
            // TODO: Get the consent bool from a GDPR popup for European countries
            Appodeal.initialize(APP_KEY, Appodeal.REWARDED_VIDEO | Appodeal.INTERSTITIAL, true);
            Appodeal.setRewardedVideoCallbacks(rewardedVideoListener);

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            Appodeal.setTesting(true);
            #endif
        }

        public bool IsRewardedVideoAvailable()
        {
            return Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            rewardedVideoListener.rewardedVideoFinishedPromise = new Promise<AdsResult>();
            return rewardedVideoListener.rewardedVideoFinishedPromise;
        }

        public bool IsInterstitialAvailable()
        {
            return Appodeal.isLoaded(Appodeal.INTERSTITIAL);
        }

        public void ShowInterstitial()
        {
            Debug.Log("ShowInterstitial");
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
            Debug.Log("onRewardedVideoFinished amount:" + amount + " name:" + name);
        }

        public void onRewardedVideoClosed(bool finished)
        {
            if (!finished)
            {
                rewardedVideoFinishedPromise.Dispatch(AdsResult.FAILED);
            }

            Debug.Log("onRewardedVideoClosed finished:" + finished);
        }

        public void onRewardedVideoFailedToLoad()
        {
            rewardedVideoFinishedPromise.Dispatch(AdsResult.FAILED);
            Debug.Log("onRewardedVideoFailedToLoad");
        }

        public void onRewardedVideoExpired()
        {
            rewardedVideoFinishedPromise.Dispatch(AdsResult.FAILED);
            Debug.Log("onRewardedVideoExpired");
        }

        public void onRewardedVideoLoaded(bool isPrecache) 
        {
            Debug.Log("onRewardedVideoLoaded isPrecache:" + isPrecache);
        }

        public void onRewardedVideoShown() 
        {
            Debug.Log("onRewardedVideoShown");
        }
        #endregion
    }
}
