using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsAdMobMediation.Implementation
{
    public class AdMobRewardedProvider : AdMobAdProvider, IRewardedAdProvider
    {
        public event UnityAction<IAdCallbackData> OnRewardedEnded;
        public event UnityAction<IAdCallbackData> OnRewardedFetched;
        public event UnityAction<IAdCallbackData> OnRewardedClicked;

        readonly Dictionary<string, RewardedAd> rewardedAds = new Dictionary<string, RewardedAd>();
        AdPlacementData currentShowingAdData;
        AdPlacementData currentLoadingAdData;
        bool isRewardedCompleted;
        bool isAdShowing;
        bool isAdFetching;
        Coroutine waitForEnd;

        public AdMobRewardedProvider(AdMobProviderBase baseProvider) : base(baseProvider)
        {
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
            if (!baseProvider.IsInitialized || isAdFetching)
            {
                OnRewardedFetched.Dispatch(new AdCallbackData(ProviderId, data.PlacementId, AdResult.Failed));
                return;
            }

            HLog.Log(logPrefix, $"Fetch Rewarded ad with placementId: {data.PlacementId}");
            rewardedAds.TryGetValue(data.PlacementId, out var rewarded);
            if (rewarded != null)
            {
                UnsubscribeCallbacks(rewarded);

            }
            rewarded = new RewardedAd( data.AppId );
            SubscribeCallbacks(rewarded);
            rewardedAds[data.PlacementId] = rewarded;
            isAdFetching = true;
            currentLoadingAdData = data;
            rewarded.LoadAd(baseProvider.CreateRequest());
        }
        
        bool IsReady(AdPlacementData data)
        {
            rewardedAds.TryGetValue(data.PlacementId, out var rewarded);
            return baseProvider.IsInitialized &&
                   rewarded != null &&
                   rewarded.IsLoaded();
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
            if (!baseProvider.IsInitialized || isAdShowing)
            {
                return false;
            }

            HLog.Log(logPrefix, $"Show Rewarded ad with placementId: {data.PlacementId}");

            rewardedAds.TryGetValue(data.PlacementId, out var rewarded);
            if (rewarded == null ||
                !rewarded.IsLoaded())
            {
                HLog.LogWarning(logPrefix, $"Failed to show, Rewarded ad is not ready, placementId: {data.PlacementId} {rewarded == null}");
                return false;
            }

            isAdShowing = true;
            isRewardedCompleted = false;
            currentShowingAdData = data;
            KillWaitForEndCoroutine();
            waitForEnd = CoroutineManager.StartCoroutine(WaitForRewardedEnd());

            rewarded.Show();
            return true;
        }

        void SubscribeCallbacks(RewardedAd rewarded)
        {
            rewarded.OnAdLoaded += OnRewardedLoaded;
            rewarded.OnAdFailedToLoad += OnRewardedFailedToLoad;
            rewarded.OnAdOpening += OnRewardedOpened;
            rewarded.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            rewarded.OnUserEarnedReward += OnRewardedCompleted;
            rewarded.OnAdClosed += OnRewardedClosed;
        }
        
        void UnsubscribeCallbacks(RewardedAd rewarded)
        {
            rewarded.OnAdLoaded -= OnRewardedLoaded;
            rewarded.OnAdFailedToLoad -= OnRewardedFailedToLoad;
            rewarded.OnAdOpening -= OnRewardedOpened;
            rewarded.OnAdFailedToShow -= HandleRewardedAdFailedToShow;
            rewarded.OnUserEarnedReward -= OnRewardedCompleted;
            rewarded.OnAdClosed -= OnRewardedClosed;
        }

        void HandleRewardedAdFailedToShow( object sender, AdErrorEventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    
                    HLog.LogWarning(logPrefix, $"Failed to complete Rewarded ad, placementId: {currentShowingAdData.PlacementId}, error: {args.Message}");
                    isAdShowing = false;
                },
                null);
        }

        void OnRewardedLoaded(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    isAdFetching = false;
                    HLog.Log(logPrefix, $"Rewarded ad loaded, placementId: {currentLoadingAdData.PlacementId}");
                    OnRewardedFetched.Dispatch(
                        new AdCallbackData(ProviderId, currentLoadingAdData.PlacementId, AdResult.Completed));
                },
                null);
        }

        void OnRewardedFailedToLoad(object sender, AdErrorEventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    isAdFetching = false;
                    HLog.LogWarning(logPrefix, $"Failed to load Rewarded ad, placementId: {currentLoadingAdData.PlacementId}, error: {args.Message}");

                    OnRewardedFetched.Dispatch(
                        new AdCallbackData(ProviderId, currentLoadingAdData.PlacementId, AdResult.Failed));
                },
                null);
        }

        void OnRewardedOpened(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    HLog.Log(logPrefix, $"Rewarded ad opened, placementId: {currentShowingAdData.PlacementId}");
                },
                null);
        }
        
        void OnRewardedCompleted(object sender, Reward args)
        {
            syncContext.Post(
                s =>
                {
                    HLog.Log(logPrefix, $"Rewarded ad completed, placementId: {currentShowingAdData.PlacementId}");
                },
                null);
            isRewardedCompleted = true;
        }

        void OnRewardedClosed(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    HLog.Log(logPrefix, $"Rewarded ad closed, placementId: {currentShowingAdData.PlacementId}");
                },
                null);
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
                    new AdCallbackData(ProviderId, currentShowingAdData.PlacementId, AdResult.Completed));
            }
            else
            {
                OnRewardedEnded.Dispatch(
                    new AdCallbackData(ProviderId, currentShowingAdData.PlacementId, AdResult.Skipped));
            }
        }

        void KillWaitForEndCoroutine()
        {
            if (waitForEnd != null)
                CoroutineManager.StopCoroutine(waitForEnd);
        }
    }
}