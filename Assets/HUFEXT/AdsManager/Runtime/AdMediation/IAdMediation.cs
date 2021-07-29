using System;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUFEXT.AdsManager.Runtime.AdManagers;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdMediation
{
    public interface IAdMediation
    {
        event Action OnInitialize;
        
        event Action<AdCallback> OnBannerFetched;
        event Action<AdCallback> OnBannerShown;
        event Action<AdCallback> OnBannerClicked;
        event Action<AdCallback> OnBannerHidden;
        
        event Action<AdCallback> OnInterstitialEnded;
        event Action<AdCallback> OnInterstitialFetched;
        event Action<AdCallback> OnInterstitialClicked;
        
        event Action<AdCallback> OnRewardedEnded;
        event Action<AdCallback> OnRewardedFetched;
        event Action<AdCallback> OnRewardedClicked;
        
        string MediationId { get; }
        bool IsInitialized { get; }
        AdsProviderConfig AdsProviderConfig { get; }
        AdsMediator AdsMediation { get; }
        void Init();
        void CollectSensitiveData(bool consentStatus);
        
        void SetBannerPosition(BannerPosition position = BannerPosition.BottomCenter);
        void FetchBanner(string placementId);
        bool IsBannerReady(string placementId);
        void ShowBanner(string placementId);
        void HideBanner(string placementId);
        
        void FetchInterstitial(string placementId);
        bool IsInterstitialReady(string placementId);
        void ShowInterstitial(string placementId);

        void FetchRewarded(string placementId);
        void ShowRewarded(string placementId);
        bool IsRewardedReady(string placementId);
    }
}