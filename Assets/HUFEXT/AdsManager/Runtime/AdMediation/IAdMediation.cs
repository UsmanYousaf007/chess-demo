using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUFEXT.AdsManager.Runtime.AdManagers;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdMediation
{
    public interface IAdMediation
    {
        event UnityAction OnInitialize;
        
        event UnityAction<AdCallback> OnBannerFetched;
        event UnityAction<AdCallback> OnBannerShown;
        event UnityAction<AdCallback> OnBannerClicked;
        event UnityAction<AdCallback> OnBannerHidden;
        
        event UnityAction<AdCallback> OnInterstitialEnded;
        event UnityAction<AdCallback> OnInterstitialFetched;
        event UnityAction<AdCallback> OnInterstitialClicked;
        
        event UnityAction<AdCallback> OnRewardedEnded;
        event UnityAction<AdCallback> OnRewardedFetched;
        event UnityAction<AdCallback> OnRewardedClicked;
        
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