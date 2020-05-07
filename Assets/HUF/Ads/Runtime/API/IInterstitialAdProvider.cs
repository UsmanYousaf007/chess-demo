using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public interface IInterstitialAdProvider : IAdProvider
    {
        event UnityAction<IAdCallbackData> OnInterstitialEnded;
        event UnityAction<IAdCallbackData> OnInterstitialFetched;
        event UnityAction<IAdCallbackData> OnInterstitialClicked;
        
        bool Show();
        bool Show(string placementId);
        bool IsReady();
        bool IsReady(string placementId);
        void Fetch();
        void Fetch(string placementId);
    }
}