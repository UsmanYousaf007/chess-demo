using System;
using System.Collections;
using GoogleMobileAds.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsAdMobMediation.Implementation
{
    public class AdMobRewardedProvider : AdMobAdProvider, IRewardedAdProvider
    {
        public event UnityAction<IAdCallbackData> OnRewardedEnded;
        public event UnityAction<IAdCallbackData> OnRewardedFetched;
        public event UnityAction<IAdCallbackData> OnRewardedClicked;

        RewardBasedVideoAd rewarded;
        AdPlacementData lastRewardedAdPlacementData;
        bool isRewardedCompleted;
        bool isAdShowing;
        Coroutine waitForEnd;

        public AdMobRewardedProvider(AdMobProviderBase baseProvider) : base(baseProvider) { }       

        public bool Show()
        {
            var data = Config.GetPlacementData(PlacementType.Rewarded);
            if (data == null)
                return false;

            return Show(data);
        }

        public bool Show(string placementId)
        {
            var data = Config.GetPlacementData(placementId);
            if (data == null)
                return false;

            return Show(data);
        }

        bool Show(AdPlacementData data)
        {
            Debug.Log($"[{logPrefix}] Show Rewarded ad with placementId: {data.PlacementId}");

            if (rewarded == null || !rewarded.IsLoaded() || 
                lastRewardedAdPlacementData == null || data.AppId != lastRewardedAdPlacementData.AppId )
            {
                Debug.LogWarning(
                    $"[{logPrefix}] Failed to show, Rewarded ad is not ready, placementId: {data.PlacementId}");
                return false;
            }

            isAdShowing = true;
            isRewardedCompleted = false;

            KillWaitForEndCoroutine();
            waitForEnd = CoroutineManager.StartCoroutine(WaitForRewardedEnd());
            
            rewarded.Show();
            return true;
        }

        public bool IsReady()
        {
            var data = Config.GetPlacementData(PlacementType.Rewarded);
            if (data == null)
                return false;

            return IsReady(data);
        }

        public bool IsReady(string placementId)
        {
            var data = Config.GetPlacementData(placementId);
            if (data == null)
                return false;

            return IsReady(data);
        }

        bool IsReady(AdPlacementData data)
        {
            return rewarded != null && rewarded.IsLoaded() &&
                   lastRewardedAdPlacementData != null && data.AppId == lastRewardedAdPlacementData.AppId;
        }

        public void Fetch()
        {
            var data = Config.GetPlacementData(PlacementType.Rewarded);
            if (data == null)
                return;

            Fetch(data);
        }

        public void Fetch(string placementId)
        {
            var data = Config.GetPlacementData(placementId);
            if (data == null)
                return;

            Fetch(data);
        }

        void Fetch(AdPlacementData data)
        {
            Debug.Log($"[{logPrefix}] Fetch Rewarded ad with placementId: {data.PlacementId}");
            lastRewardedAdPlacementData = data;
            if (rewarded == null)
            {
                rewarded = RewardBasedVideoAd.Instance;
                SubscribeCallbacks();
            }

            rewarded.LoadAd(baseProvider.CreateRequest(), data.AppId);
        }

        void SubscribeCallbacks()
        {
            rewarded.OnAdLoaded += OnRewardedLoaded;
            rewarded.OnAdFailedToLoad += OnRewardedFailedToLoad;
            rewarded.OnAdOpening += OnRewardedOpened;
            rewarded.OnAdStarted += OnRewardedStarted;
            rewarded.OnAdRewarded += OnRewardedCompleted;
            rewarded.OnAdClosed += OnRewardedClosed;
            rewarded.OnAdLeavingApplication += OnRewardedAdClicked;
        }

        void OnRewardedLoaded(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Rewarded ad loaded, placementId: {lastRewardedAdPlacementData.PlacementId}");
            OnRewardedFetched.Dispatch(
                new AdCallbackData(ProviderId, lastRewardedAdPlacementData.PlacementId, AdResult.Completed));
        }

        void OnRewardedFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogWarning(
                $"[{logPrefix}] Failed to load Rewarded ad, placementId: {lastRewardedAdPlacementData.PlacementId}, error: {args.Message}");
            OnRewardedFetched.Dispatch(
                new AdCallbackData(ProviderId, lastRewardedAdPlacementData.PlacementId, AdResult.Failed));
        }

        void OnRewardedOpened(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Rewarded ad opened, placementId: {lastRewardedAdPlacementData.PlacementId}");
        }

        void OnRewardedStarted(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Rewarded ad started, placementId: {lastRewardedAdPlacementData.PlacementId}");
        }

        void OnRewardedCompleted(object sender, Reward args)
        {
            Debug.Log($"[{logPrefix}] Rewarded ad completed, placementId: {lastRewardedAdPlacementData.PlacementId}");
            isRewardedCompleted = true;
        }

        void OnRewardedClosed(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Rewarded ad closed, placementId: {lastRewardedAdPlacementData.PlacementId}");
            
            isAdShowing = false;
        }

        IEnumerator WaitForRewardedEnd()
        {
            do
            {
                yield return null;
            } while (isAdShowing);

            if (isRewardedCompleted)
            {
                OnRewardedEnded.Dispatch(
                    new AdCallbackData(ProviderId, lastRewardedAdPlacementData.PlacementId, AdResult.Completed));
            }
            else
            {
                OnRewardedEnded.Dispatch(
                    new AdCallbackData(ProviderId, lastRewardedAdPlacementData.PlacementId, AdResult.Skipped));
            }
        }
        
        void KillWaitForEndCoroutine()
        {
            if (waitForEnd != null)
                CoroutineManager.StopCoroutine(waitForEnd);
        }

        void OnRewardedAdClicked(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Leaving app by click on Rewarded ad, placementId: {lastRewardedAdPlacementData.PlacementId}");
            OnRewardedClicked.Dispatch(
                new AdCallbackData(ProviderId, lastRewardedAdPlacementData.PlacementId, AdResult.Completed));
        }
    }
}