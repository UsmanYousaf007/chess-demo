using UnityEngine.Events;

namespace HUF.Ads.API
{
    public interface IRewardedAdProvider : IAdProvider
    {
        event UnityAction<IAdCallbackData> OnRewardedEnded;
        event UnityAction<IAdCallbackData> OnRewardedFetched;
        event UnityAction<IAdCallbackData> OnRewardedClicked;

        bool Show();
        bool Show(string placementId);
        bool IsReady();
        bool IsReady(string placementId);
        void Fetch();
        void Fetch(string placementId);
    }
}