using System;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public interface IRewardedAdProvider : IAdProvider
    {
        event Action<IAdCallbackData> OnRewardedEnded;
        event Action<IAdCallbackData> OnRewardedFetched;
        event Action<IAdCallbackData> OnRewardedClicked;

        bool Show();
        bool Show(string placementId);
        bool IsReady();
        bool IsReady(string placementId);
        void Fetch();
        void Fetch(string placementId);
    }
}