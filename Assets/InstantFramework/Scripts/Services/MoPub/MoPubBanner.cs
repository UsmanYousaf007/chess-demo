using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public static class MoPubBanner
    {
        static string adUnit;
        static bool isLoaded = false;
        static bool isCreated = false;

        public static void Initialize(MoPubAdUnits pAdUnits)
        {
            Debug.Log("[TLADS]: Initializing Banner");

            // Fired when an ad loads in the banner. Includes the ad height.
            MoPubManager.OnAdLoadedEvent += OnBannerLoadedEvent;

            // Fired when an ad fails to load for the banner
            MoPubManager.OnAdFailedEvent += OnBannerFailedEvent;

            adUnit = pAdUnits.banner;
            string[] adUnits = { adUnit };
            MoPub.LoadBannerPluginsForAdUnits(adUnits);

        }

        static void OnBannerLoadedEvent(string adUnit, float height)
        {
            Debug.Log("[TLADS]: Banner created");
            isLoaded = true;
        }

        static void OnBannerFailedEvent(string p1, string p2)
        {
            Debug.Log("[TLADS]: Banner failed to create");
            isLoaded = false;
        }

        public static void Show(MoPubBase.AdPosition pos)
        {
            Debug.Log("[TLADS]: Request to create banner");

            if (!isCreated)
            {
                MoPub.CreateBanner(adUnit, pos);
                isCreated = true;
            }
            else if (isLoaded)
            {
                MoPub.ShowBanner(adUnit, true);
            }
        }

        public static void Hide()
        {
            Debug.Log("[TLADS]: Destroying banner");
            if (isLoaded)
            {
                MoPub.ShowBanner(adUnit, false);
            }
        }
    }
}