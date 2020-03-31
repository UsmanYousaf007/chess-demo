using UnityEngine.Events;

namespace HUF.Ads.API
{
    public interface IAdsService
    {
        
        
        bool IsServiceInitialized { get;}
        IBannerAdProvider BannerAdProvider { get; set; }
        IInterstitialAdProvider InterstitialAdProvider { get; set; }
        IRewardedAdProvider RewardedAdProvider { get; set; }

        void CollectSensitiveData(bool consentStatus);

        bool ShowBanner(BannerPosition position = BannerPosition.BottomCenter);
        bool ShowBanner(string placementId, BannerPosition position = BannerPosition.BottomCenter);
        void HideBanner();

        bool TryShowInterstitial();
        bool TryShowInterstitial(string placementId);
        bool IsInterstitialReady();
        bool IsInterstitialReady(string placementId);
        void FetchInterstitial();
        void FetchInterstitial(string placementId);

        bool TryShowRewarded();
        bool TryShowRewarded(string placementId);
        bool IsRewardedReady();
        bool IsRewardedReady(string placementId);
        void FetchRewarded();
        void FetchRewarded(string placementId);

        void ServiceInitialized();  
        void RegisterToInitializationEvent(UnityAction adsServiceInitializedEvent);
    }
}