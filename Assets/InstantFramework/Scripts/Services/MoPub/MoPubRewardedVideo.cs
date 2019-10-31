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
            Debug.Log("[TLADS]: Initializing Rewarded Video");

            showPromise = null;
            MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
            MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoExpiredEvent;
            MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;

            adUnit = pAdUnits.rewardedVideo;
            string[] adUnits = { adUnit };
            MoPub.LoadRewardedVideoPluginsForAdUnits(adUnits);

            RequestRewardedVideo();
        }

        public static IPromise<AdsResult> Show()
        {
            if (!isAvailable)
            {
                Debug.Log("[TLADS]: Attempting to show Rewarded Video that is not available");
                return null;
            }

            // Show the video
            MoPub.ShowRewardedVideo(adUnit);
            Debug.Log("[TLADS]: Showing Rewarded Video");

            // Give the client a hook
            showPromise = new Promise<AdsResult>();
            return showPromise;
        }

        public static bool IsAvailable()
        {
            Debug.Log("[TLADS]: Rewarded Video available: " + isAvailable);

            if (!isAvailable)
            {
                RequestRewardedVideo();
            }
            return isAvailable;
        }

        static void RequestRewardedVideo()
        {
            Debug.Log("[TLADS]: Request Rewarded Video");
            MoPub.RequestRewardedVideo(adUnit);
        }

        static void OnRewardedVideoLoadedEvent(string adUnit)
        {
            Debug.Log("[TLADS]: Rewarded Video loaded");
            isAvailable = true;
        }

        static void OnRewardedVideoFailedEvent(string p1, string p2)
        {
            Debug.Log("[TLADS]: Rewarded Video failed to load");
            isAvailable = false;
        }

        static void OnRewardedVideoReceivedRewardEvent(string p1, string p2, float p3)
        {
            Debug.Log("[TLADS]: Rewarded Video recieve reward event");
            showPromise.Dispatch(AdsResult.FINISHED);
        }

        static void OnRewardedVideoFailedToPlayEvent(string p1, string p2)
        {
            Debug.Log("[TLADS]: Rewarded Video failed to play");

            if(showPromise != null)
            {
                showPromise.Dispatch(AdsResult.FAILED);
            }

            isAvailable = false;
            RequestRewardedVideo();
        }

        static void OnRewardedVideoExpiredEvent(string p1)
        {
            Debug.Log("[TLADS]: Rewarded Video expired");
            isAvailable = false;
            RequestRewardedVideo();
        }

        static void OnRewardedVideoClosedEvent(string p1)
        {
            Debug.Log("[TLADS]: Rewarded Video closed");
            isAvailable = false;
            RequestRewardedVideo();
        }
    }
}