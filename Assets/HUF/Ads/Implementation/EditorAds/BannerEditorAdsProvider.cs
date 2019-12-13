using HUF.Ads.API;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Ads.Implementation.EditorAds
{
    public class BannerEditorAdsProvider : IBannerAdProvider
    {
        public string ProviderId => "EditorAds";
        public bool IsInitialized => true;
        string logPrefix;

        public event UnityAction<IBannerCallbackData> OnBannerShown;
        public event UnityAction<IBannerCallbackData> OnBannerFailed;
        public event UnityAction<IBannerCallbackData> OnBannerClicked;
        public event UnityAction<IBannerCallbackData> OnBannerHidden;

        public bool Init()
        {
            logPrefix = $"[{GetType().Name}]";
            Debug.Log($"{logPrefix} Initialized Editor Interstitial ads provider");
            return true;
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            Debug.Log($"{logPrefix} Collect sensitive data: {consentStatus}");
        }

        public bool Show(BannerPosition position = BannerPosition.BottomCenter)
        {
            return false;
        }

        public bool Show(string placementId, BannerPosition position = BannerPosition.BottomCenter)
        {
            return false;
        }

        public void Hide() { }
    }
}