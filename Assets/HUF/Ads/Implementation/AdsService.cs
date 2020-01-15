using System.Linq;
using HUF.Ads.API;
using HUF.Ads.Implementation.EditorAds;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Ads.Implementation
{
    public class AdsService : IAdsService
    {
        IBannerAdProvider bannerAdProvider;
        IInterstitialAdProvider interstitialAdProvider;
        IRewardedAdProvider rewardedAdProvider;

        readonly string logPrefix;

        bool isServiceInitialized;
        UnityAction OnAdsServiceInitialized;

        static AdsProviderConfig AdsProviderConfig =>
            HConfigs.GetConfigsByBaseClass<AdsProviderConfig>().FirstOrDefault();

        public IBannerAdProvider BannerAdProvider
        {
            get => bannerAdProvider;
            set
            {
#if !DISABLE_HUF_EDITOR_ADS
                if (Application.isEditor && AdsProviderConfig.UseEditorMockProvider)
                {
                    bannerAdProvider = new BannerEditorAdsProvider();
                    bannerAdProvider.Init();
                }
                else
#endif
                {
                    bannerAdProvider = value;
                    if (!bannerAdProvider.IsInitialized)
                    {
                        if (!bannerAdProvider.Init())
                            Debug.LogError($"[{logPrefix}] Failed to initialize banner ad provider! " +
                                           $"Make sure you set application IDs in config file!");
                    }
                }
            }
        }

        public IInterstitialAdProvider InterstitialAdProvider
        {
            get => interstitialAdProvider;
            set
            {
#if !DISABLE_HUF_EDITOR_ADS
                if (Application.isEditor && AdsProviderConfig.UseEditorMockProvider)
                {
                    interstitialAdProvider = new InterstitialEditorAdProvider();
                    interstitialAdProvider.Init();
                }
                else
#endif
                {
                    interstitialAdProvider = value;
                    if (!interstitialAdProvider.IsInitialized)
                    {
                        if (!interstitialAdProvider.Init())
                            Debug.LogError($"[{logPrefix}] Failed to initialize interstitial ad provider! " +
                                           $"Make sure you set application IDs in config file!");
                    }
                }
            }
        }

        public IRewardedAdProvider RewardedAdProvider
        {
            get => rewardedAdProvider;
            set
            {
#if !DISABLE_HUF_EDITOR_ADS
                if (Application.isEditor && AdsProviderConfig.UseEditorMockProvider)
                {
                    rewardedAdProvider = new RewardedEditorAdsProvider();
                    rewardedAdProvider.Init();
                }
                else
#endif
                {
                    rewardedAdProvider = value;
                    if (!rewardedAdProvider.IsInitialized)
                    {
                        if (!rewardedAdProvider.Init())
                            Debug.LogError($"[{logPrefix}] Failed to initialize rewarded ad provider! " +
                                           $"Make sure you set application IDs in config file!");
                    }
                }
            }
        }

        public AdsService()
        {
            logPrefix = GetType().Name;
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            if (IsProviderValid(BannerAdProvider))
                BannerAdProvider.CollectSensitiveData(consentStatus);

            if (InterstitialAdProvider != BannerAdProvider && IsProviderValid(InterstitialAdProvider))
                InterstitialAdProvider.CollectSensitiveData(consentStatus);

            if (RewardedAdProvider != BannerAdProvider && RewardedAdProvider != InterstitialAdProvider &&
                IsProviderValid(RewardedAdProvider))
                RewardedAdProvider.CollectSensitiveData(consentStatus);
        }

        public bool ShowBanner(BannerPosition position = BannerPosition.BottomCenter)
        {
            if (IsProviderValid(BannerAdProvider))
                return BannerAdProvider.Show(position);
            return false;
        }

        public bool ShowBanner(string placementId, BannerPosition position = BannerPosition.BottomCenter)
        {
            if (IsProviderValid(BannerAdProvider))
                return BannerAdProvider.Show(placementId, position);
            return false;
        }

        public void HideBanner()
        {
            if (IsProviderValid(bannerAdProvider))
                BannerAdProvider.Hide();
        }

        public bool TryShowInterstitial()
        {
            if (IsProviderValid(InterstitialAdProvider))
                return InterstitialAdProvider.Show();
            return false;
        }

        public bool TryShowInterstitial(string placementId)
        {
            if (IsProviderValid(InterstitialAdProvider))
                return InterstitialAdProvider.Show(placementId);
            return false;
        }

        public bool IsInterstitialReady()
        {
            if (IsProviderValid(InterstitialAdProvider))
                return InterstitialAdProvider.IsReady();
            return false;
        }

        public bool IsInterstitialReady(string placementId)
        {
            if (IsProviderValid(InterstitialAdProvider))
                return InterstitialAdProvider.IsReady(placementId);
            return false;
        }

        public void FetchInterstitial()
        {
            if (IsProviderValid(InterstitialAdProvider))
                InterstitialAdProvider.Fetch();
        }

        public void FetchInterstitial(string placementId)
        {
            if (IsProviderValid(InterstitialAdProvider))
                InterstitialAdProvider.Fetch(placementId);
        }

        public bool TryShowRewarded()
        {
            if (IsProviderValid(RewardedAdProvider))
                return RewardedAdProvider.Show();
            return false;
        }

        public bool TryShowRewarded(string placementId)
        {
            if (IsProviderValid(RewardedAdProvider))
                return RewardedAdProvider.Show(placementId);
            return false;
        }

        public bool IsRewardedReady()
        {
            if (IsProviderValid(RewardedAdProvider))
                return RewardedAdProvider.IsReady();
            return false;
        }

        public bool IsRewardedReady(string placementId)
        {
            if (IsProviderValid(RewardedAdProvider))
                return RewardedAdProvider.IsReady(placementId);
            return false;
        }

        public void FetchRewarded()
        {
            if (IsProviderValid(RewardedAdProvider))
                RewardedAdProvider.Fetch();
        }

        public void FetchRewarded(string placementId)
        {
            if (IsProviderValid(RewardedAdProvider))
                RewardedAdProvider.Fetch(placementId);
        }

        bool IsProviderValid(object provider)
        {
            if (provider != null)
            {
                return true;
            }

            Debug.LogWarning($"[{logPrefix}] Provider is not initialized yet!");
            return false;
        }
        
        public bool IsServiceInitialized
        {
            get => isServiceInitialized;
        }

        public void ServiceInitialized()
        {
            isServiceInitialized = true;
            OnAdsServiceInitialized.Dispatch();
        }

        public void RegisterToInitializationEvent(UnityAction adsServiceInitializedEvent)
        {
            OnAdsServiceInitialized += adsServiceInitializedEvent;
        }
    }
}