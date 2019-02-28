using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public static class MoPubBanner
    {
        static string adUnit;

        public static void Initialize(MoPubAdUnits pAdUnits)
        {
            adUnit = pAdUnits.banner;
            string[] adUnits = { adUnit };
            MoPub.LoadBannerPluginsForAdUnits(adUnits);
        }

        public static void Show(MoPubBase.AdPosition pos)
        {
            MoPub.CreateBanner(adUnit, pos);
        }

        public static void Hide()
        {
            MoPub.DestroyBanner(adUnit);
        }
    }
}