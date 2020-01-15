using System;
using System.Collections.Generic;
using System.Linq;
using GoogleMobileAds.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsAdMobMediation.Implementation
{
    public class AdMobInterstitialProvider : AdMobAdProvider, IInterstitialAdProvider
    {
        public event UnityAction<IAdCallbackData> OnInterstitialEnded;
        public event UnityAction<IAdCallbackData> OnInterstitialFetched;
        public event UnityAction<IAdCallbackData> OnInterstitialClicked;

        readonly Dictionary<string, InterstitialAd> interstitials = new Dictionary<string, InterstitialAd>();

        public AdMobInterstitialProvider(AdMobProviderBase baseProvider) : base(baseProvider)
        {
        }

        public bool Show()
        {
            var data = Config.GetPlacementData(PlacementType.Interstitial);
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
            if (!baseProvider.IsInitialized)
            {
                return false;
            }

            Debug.Log($"[{logPrefix}] Show Interstitial ad with placementId: {data.PlacementId}");

            interstitials.TryGetValue(data.PlacementId, out var interstitial);

            if (interstitial == null || !interstitial.IsLoaded())
            {
                Debug.LogWarning(
                    $"[{logPrefix}] Failed to show, Interstitial ad is not ready, placementId: {data.PlacementId}");

                return false;
            }

            interstitial.Show();
            return true;
        }

        public bool IsReady()
        {
            var data = Config.GetPlacementData(PlacementType.Interstitial);
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
            interstitials.TryGetValue(data.PlacementId, out var interstitial);

            return baseProvider.IsInitialized && interstitial != null && interstitial.IsLoaded();
        }

        public void Fetch()
        {
            var data = Config.GetPlacementData(PlacementType.Interstitial);
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
            if (!baseProvider.IsInitialized)
            {
                OnInterstitialFetched.Dispatch(new AdCallbackData(ProviderId, data.PlacementId, AdResult.Failed));
                return;
            }

            Debug.Log($"[{logPrefix}] Fetch Interstitial ad with placementId: {data.PlacementId}");
            interstitials.TryGetValue(data.PlacementId, out var interstitial);

            if (interstitial != null)
            {
                UnsubscribeCallbacks(interstitial);
                interstitial.Destroy();
            }

            interstitial = new InterstitialAd(data.AppId);
            interstitials[data.PlacementId] = interstitial;
            SubscribeCallbacks(interstitial);
            interstitial.LoadAd(baseProvider.CreateRequest());
        }

        void SubscribeCallbacks(InterstitialAd interstitial)
        {
            interstitial.OnAdLoaded += OnInterstitialLoaded;
            interstitial.OnAdFailedToLoad += OnInterstitialFailedToLoad;
            interstitial.OnAdOpening += OnInterstitialOpened;
            interstitial.OnAdClosed += OnInterstitialClosed;
            interstitial.OnAdLeavingApplication += OnInterstitialAdClicked;
        }

        void UnsubscribeCallbacks(InterstitialAd interstitial)
        {
            interstitial.OnAdLoaded -= OnInterstitialLoaded;
            interstitial.OnAdFailedToLoad -= OnInterstitialFailedToLoad;
            interstitial.OnAdOpening -= OnInterstitialOpened;
            interstitial.OnAdClosed -= OnInterstitialClosed;
            interstitial.OnAdLeavingApplication -= OnInterstitialAdClicked;
        }

        void OnInterstitialLoaded(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    string placementId = GetPlacementIdByInterstitial(sender);
                    Debug.Log($"[{logPrefix}] Interstitial ad loaded, placementId: {placementId}");

                    OnInterstitialFetched.Dispatch(new AdCallbackData(ProviderId, placementId, AdResult.Completed));
                },
                null);
        }

        void OnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    string placementId = GetPlacementIdByInterstitial(sender);
                    Debug.LogWarning(
                        $"[{logPrefix}] Failed to load Interstitial ad, placementId: {placementId}, error: {args.Message}");

                    OnInterstitialFetched.Dispatch(new AdCallbackData(ProviderId, placementId, AdResult.Failed));
                },
                null);
        }

        void OnInterstitialOpened(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    string placementId = GetPlacementIdByInterstitial(sender);
                    Debug.Log($"[{logPrefix}] Interstitial ad opened, placementId: {placementId}");
                },
                null);
        }

        void OnInterstitialClosed(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    string placementId = GetPlacementIdByInterstitial(sender);
                    Debug.Log($"[{logPrefix}] Interstitial ad closed, placementId: {placementId}");

                    OnInterstitialEnded.Dispatch(new AdCallbackData(ProviderId, placementId, AdResult.Completed));
                },
                null);
        }

        void OnInterstitialAdClicked(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    string placementId = GetPlacementIdByInterstitial(sender);
                    Debug.Log($"[{logPrefix}] Leaving app by click on Interstitial ad, placementId: {placementId}");

                    OnInterstitialClicked.Dispatch(new AdCallbackData(ProviderId, placementId, AdResult.Completed));
                },
                null);
        }

        string GetPlacementIdByInterstitial(object sender)
        {
            return interstitials.FirstOrDefault(x => x.Value == sender).Key;
        }
    }
}