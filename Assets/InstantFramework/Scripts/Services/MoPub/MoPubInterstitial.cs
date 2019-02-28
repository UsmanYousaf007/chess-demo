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
            MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
            MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
            MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;
            adUnit = pAdUnits.interstitial;
            string[] adUnits = { adUnit };
            MoPub.LoadInterstitialPluginsForAdUnits(adUnits);
            MoPub.RequestInterstitialAd(adUnit);
        }

        public static void Show()
        {
            // Show the interstitial
            MoPub.ShowInterstitialAd(adUnit);
        }

        public static bool IsAvailable()
        {
            return isAvailable;
        }

        static void OnInterstitialLoadedEvent(string adUnit)
        {
            isAvailable = true;
        }
       
        static void OnInterstitialFailedEvent(string p1, string p2)
        { 
            isAvailable = false;
            MoPub.RequestInterstitialAd(adUnit);
        }

        static void OnInterstitialExpiredEvent(string p1)
        {
            isAvailable = false;
            MoPub.RequestInterstitialAd(adUnit);
        }

        static void OnInterstitialDismissedEvent(string p1)
        {
            isAvailable = false;
            MoPub.RequestInterstitialAd(adUnit);
        }
    }
}