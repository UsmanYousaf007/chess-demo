using System;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public interface IInterstitialAdProvider : IAdProvider
    {
        event Action<IAdCallbackData> OnInterstitialEnded;
        event Action<IAdCallbackData> OnInterstitialFetched;
        event Action<IAdCallbackData> OnInterstitialClicked;
        
        bool Show();
        bool Show(string placementId);
        bool IsReady();
        bool IsReady(string placementId);
        void Fetch();
        void Fetch(string placementId);
    }
}