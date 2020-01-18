using UnityEngine.Events;

namespace HUF.Ads.API
{
    public interface IBannerAdProvider : IAdProvider
    {
        event UnityAction<IBannerCallbackData> OnBannerShown;
        event UnityAction<IBannerCallbackData> OnBannerFailed;
        event UnityAction<IBannerCallbackData> OnBannerClicked;
        event UnityAction<IBannerCallbackData> OnBannerHidden;
        
        bool Show(BannerPosition position = BannerPosition.BottomCenter);
        bool Show(string placementId, BannerPosition position = BannerPosition.BottomCenter);
        void Hide();
    }
}