using GoogleMobileAds.Api;
using HUF.Ads.API;
using HUF.AdsAdMobMediation.Implementation;
using HUF.Utils.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AdsAdMobMediation.API
{
    public static class HAdsAdMobMediation
    {
        static readonly string className = nameof(HAdsAdMobMediation);
        
        static bool isInitialized;

        static AdMobProviderBase baseProvider;
        static AdMobBannerProvider bannerProvider;

        /// <summary>
        /// Use this method to initialize AdMob manually
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if (isInitialized)
                return;
            
            InstallProvider();
            isInitialized = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<AdMobProviderConfig>();
            if (hasConfig && HConfigs.GetConfig<AdMobProviderConfig>().AutoInit)
                Init();
        }

        static void InstallProvider()
        {
            baseProvider = new AdMobProviderBase();
            var adsService = HAds.Banner.RegisterAdProvider(bannerProvider = new AdMobBannerProvider(baseProvider));
            HAds.Interstitial.RegisterAdProvider(new AdMobInterstitialProvider(baseProvider));
            HAds.Rewarded.RegisterAdProvider(new AdMobRewardedProvider(baseProvider));
            baseProvider.SetAdsService(adsService);
        }

        /// <summary>
        /// Use this method to show Admob's Mediation test suite </para>
        /// It's useful to check if all Ad networks in mediation were setup correctly
        /// </summary>
        [PublicAPI]
        public static void ShowTestSuite()
        {
            if (isInitialized && baseProvider != null)
            {
                baseProvider.ShowTestSuite();
            }
            else
            {
                Debug.LogError($"[{typeof(HAdsAdMobMediation).Name}] AdMob is not initialized yet, can't show test suite");
            }
        }
        
        /// <summary>
        /// Use this to set banner size to be later shown by HAds.Banner.Show()
        /// </summary>
        /// <param name="size">GoogleMobileAds.Api.AdSize object. Use one of the predefined ones: <para />
        /// AdSize.Banner - 320 x 50 <para />
        /// AdSize.Smart - depending on screen height Screen width x 32|50|90 <para />
        /// AdSize.MediumRectangle - 300 x 250 <para />
        /// AdSize.IABBanner - 468 x 60 <para />
        /// AdSize.Leaderboard - 728 x 90 <para />
        /// Not supported yet: AdSize.LargeBanner - 320 x 100 <para />
        /// By default it's set to AdSize.Smart</param>
        /// <returns>Return TRUE if operation is finished with success. FALSE otherwise. </returns>
        [PublicAPI]
        public static bool SetBannerSize(AdSize size)
        {
            if (bannerProvider == null)
            {
                Debug.LogWarning($"[{className}] Can't SetBannerSize(), banner ads provider is not initialized");
                return false;
            }

            bannerProvider.SetBannerSize(size);
            return true;
        }
    }
}