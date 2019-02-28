using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public static class MoPubRewardedVideo
    {
        static string adUnit;
        static bool isAvailable;
        static IPromise<AdsResult> showPromise;

        public static void Initialize(MoPubAdUnits pAdUnits)
        {
            MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
            MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoExpiredEvent;
            adUnit = pAdUnits.rewardedVideo;
            string[] adUnits = { adUnit };
            MoPub.LoadRewardedVideoPluginsForAdUnits(adUnits);
            MoPub.RequestRewardedVideo(adUnit);
        }

        public static IPromise<AdsResult> Show()
        {
            // Show the video
            MoPub.ShowRewardedVideo(adUnit);

            // Give the client a hook
            showPromise = new Promise<AdsResult>();
            return showPromise;
        }

        public static bool IsAvailable()
        {
            return isAvailable;
        }

        static void OnRewardedVideoLoadedEvent(string adUnit)
        {
            isAvailable = true;
        }

        static void OnRewardedVideoReceivedRewardEvent(string p1, string p2, float p3)
        {
            showPromise.Dispatch(AdsResult.FINISHED);
        }

        static void OnRewardedVideoFailedToPlayEvent(string p1, string p2)
        {
            showPromise.Dispatch(AdsResult.FAILED);
            isAvailable = false;
            MoPub.RequestRewardedVideo(adUnit);
        }

        static void OnRewardedVideoExpiredEvent(string p1)
        {
            isAvailable = false;
            MoPub.RequestRewardedVideo(adUnit);
        }

        static void OnRewardedVideoClosedEvent(string p1)
        {
            isAvailable = false;
            MoPub.RequestRewardedVideo(adUnit);
        }
    }
}