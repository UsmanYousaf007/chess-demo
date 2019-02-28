using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class MoPubAdUnits
    {
        public string banner;
        public string interstitial;
        public string rewardedVideo;

#if UNITY_IOS
        const string testBanner = "0ac59b0996d947309c33f59d6676399f";
        const string testInterstitial = "4f117153f5c24fa6a3a92b818a5eb630";
        const string testRewardedVideo = "8f000bd5e00246de9c789eed39ff6096";
        const string phoneRewardedVideo = "6edd1019bc1e49b78679c12199f137e1";
        const string tabletRewardedVideo = "7abb8b82956e4b79942263469fb4d501";
        const string phoneInterstitial = "f063e4f79d4a4ef59db7e317a0827a6c";
        const string tabletInterstitial = "fa727b241efd480891e717c9ee9ba487";
        const string phoneBanner = "04fc711689454bdd8662eaaff54e1a14";
        const string tabletBanner = "dbb905128ad44e3b95d7a3361bb1dde1";
#elif UNITY_ANDROID || UNITY_EDITOR
        const string testBanner = "b195f8dd8ded45fe847ad89ed1d016da";
        const string testInterstitial = "24534e1901884e398f1253216226017e";
        const string testRewardedVideo = "920b6145fb1546cf8b5cf2ac34638bb7";
        const string phoneRewardedVideo = "d99563525b2e481cadab8cf4c65cdf63";
        const string tabletRewardedVideo = "ab069e2568b1488797908f15d0454a81";
        const string phoneInterstitial = "c87f2f8282f24d61b9f1eb78fda513f8";
        const string tabletInterstitial = "95f2478981334a4f906bfa9521ed2d62";
        const string phoneBanner = "70a72d7e3019492d97da1d455e3fb309";
        const string tabletBanner = "c6afb0b982ca449894666af7cc8c12ec";
#endif

        public MoPubAdUnits()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            banner = testBanner;
            interstitial = testInterstitial;
            rewardedVideo = testRewardedVideo;
#else
            if (IsTablet())
            {
                banner = tabletBanner;
                interstitial = tabletInterstitial;
                rewardedVideo = tabletRewardedVideo;
            }
            else
            {
                banner = phoneBanner;
                interstitial = phoneInterstitial;
                rewardedVideo = phoneRewardedVideo;
            }
#endif

        }

        public string GetGenericAdUnit()
        {
            return banner;
        }

        // https://forum.unity.com/threads/detecting-between-a-tablet-and-mobile.367274/
        bool IsTablet()
        {
            float ssw;
            if (Screen.width > Screen.height) { ssw = Screen.width; } else { ssw = Screen.height; }

            if (ssw < 800) return false;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                float screenWidth = Screen.width / Screen.dpi;
                float screenHeight = Screen.height / Screen.dpi;
                float size = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
                if (size >= 6.5f) return true;
            }

            return false;
        }
    }
}