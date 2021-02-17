using System;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public interface IBannerAdProvider : IAdProvider
    {
        event Action<IBannerCallbackData, bool> OnBannerShown;
        event Action<IBannerCallbackData> OnBannerFailed;
        event Action<IBannerCallbackData> OnBannerClicked;
        event Action<IBannerCallbackData> OnBannerHidden;
        
        bool Show(BannerPosition position = BannerPosition.BottomCenter);
        bool Show(string placementId, BannerPosition position = BannerPosition.BottomCenter);
        void Hide();
    }
}