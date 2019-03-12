using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public static class MoPubInterstitial
    {
        static string adUnit;
        static bool isAvailable;

        public static void Initialize(MoPubAdUnits pAdUnits)
        {
            Debug.Log("[TLADS]: Initializing Interstitial");

            MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
            MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
            MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;

            adUnit = pAdUnits.interstitial;
            string[] adUnits = { adUnit };
            MoPub.LoadInterstitialPluginsForAdUnits(adUnits);

            RequestInterstitialAd();
        }

        public static void Show()
        {
            if (!isAvailable)
            {
                Debug.Log("[TLADS]: Attempting to show interstitial that is not available");
                return;
            }

            // Show the interstitial
            MoPub.ShowInterstitialAd(adUnit);
            Debug.Log("[TLADS]: Showing Interstitial");
        }

        public static bool IsAvailable()
        {
            Debug.Log("[TLADS]: Interstitial available: " + isAvailable);

            if (!isAvailable)
            {
                RequestInterstitialAd();
            }
            return isAvailable;
        }

        static void RequestInterstitialAd()
        {
            Debug.Log("[TLADS]: Request intersitial");
            MoPub.RequestInterstitialAd(adUnit);
        }

        static void OnInterstitialLoadedEvent(string adUnit)
        {
            Debug.Log("[TLADS]: Interstitial loaded");
            isAvailable = true;
        }
       
        static void OnInterstitialFailedEvent(string p1, string p2)
        {
            Debug.Log("[TLADS]: Interstitial failed to load " + "p1=" + p1 + " p2=" + p2);
            isAvailable = false;
        }

        static void OnInterstitialExpiredEvent(string p1)
        {
            Debug.Log("[TLADS]: Interstitial expired");
            isAvailable = false;
            RequestInterstitialAd();
        }

        static void OnInterstitialDismissedEvent(string p1)
        {
            Debug.Log("[TLADS]: Interstitial dismissed");
            isAvailable = false;
            RequestInterstitialAd();
        }
    }
}