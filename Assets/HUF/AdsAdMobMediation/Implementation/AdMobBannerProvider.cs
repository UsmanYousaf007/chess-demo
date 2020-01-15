using System;
using System.Threading;
using GoogleMobileAds.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsAdMobMediation.Implementation
{
    public class AdMobBannerProvider : AdMobAdProvider, IBannerAdProvider
    {
        enum BannerLoadingStatus
        {
            None,
            Loading,
            Loaded
        }

        public event UnityAction<IBannerCallbackData> OnBannerShown;
        public event UnityAction<IBannerCallbackData> OnBannerFailed;
        public event UnityAction<IBannerCallbackData> OnBannerClicked;
        public event UnityAction<IBannerCallbackData> OnBannerHidden;

        BannerView banner;
        AdPlacementData lastShownBannerData;
        AdSize bannerSize = AdSize.SmartBanner;

        BannerLoadingStatus bannerStatus;

        public AdMobBannerProvider(AdMobProviderBase baseProvider) : base(baseProvider)
        {
        }

        public bool Show(BannerPosition position = BannerPosition.BottomCenter)
        {
            var data = Config.GetPlacementData(PlacementType.Banner);
            if (data == null)
                return false;

            return Show(data, position);
        }

        public bool Show(string placementId, BannerPosition position = BannerPosition.BottomCenter)
        {
            var data = Config.GetPlacementData(placementId);
            if (data == null)
                return false;

            return Show(data, position);
        }

        bool Show(AdPlacementData data, BannerPosition position)
        {
            if (bannerStatus == BannerLoadingStatus.Loading || !baseProvider.IsInitialized)
            {
                return false;
            }

            if (Debug.isDebugBuild)
                Debug.Log($"[{logPrefix}] Show Banner ad with placementId: {data.PlacementId}");

            if (banner != null)
            {
                UnsubscribeCallbacks();
                banner.Destroy();
            }

            bannerStatus = BannerLoadingStatus.Loading;

            lastShownBannerData = data;
            banner = new BannerView(data.AppId, bannerSize, position.ToAdMobBannerPosition());
            SubscribeCallbacks();
            banner.LoadAd(baseProvider.CreateRequest());
            banner.Show();
            return true;
        }

        public void Hide()
        {
            if (Debug.isDebugBuild)
                Debug.Log($"[{logPrefix}] Hide banner ad");

            if (bannerStatus != BannerLoadingStatus.None)
                banner?.Hide();

            bannerStatus = BannerLoadingStatus.None;
        }

        void SubscribeCallbacks()
        {
            if (banner == null)
            {
                Debug.LogError($"[{logPrefix}] Failed to subscribe banner callbacks!");
                return;
            }

            banner.OnAdLoaded += OnBannerLoaded;
            banner.OnAdFailedToLoad += OnBannerFailedToLoad;
            banner.OnAdOpening += OnBannerOpened;
            banner.OnAdClosed += OnBannerClosed;
            banner.OnAdLeavingApplication += OnLeavingApplicationFromBanner;
        }

        void UnsubscribeCallbacks()
        {
            if (banner == null)
            {
                Debug.LogError($"[{logPrefix}] Failed to unsubscribe banner callbacks!");
                return;
            }

            banner.OnAdLoaded -= OnBannerLoaded;
            banner.OnAdFailedToLoad -= OnBannerFailedToLoad;
            banner.OnAdOpening -= OnBannerOpened;
            banner.OnAdClosed -= OnBannerClosed;
            banner.OnAdLeavingApplication -= OnLeavingApplicationFromBanner;
        }

        void OnBannerLoaded(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    if (Debug.isDebugBuild)
                        Debug.Log($"[{logPrefix}] Banner ad loaded, height: ${banner.GetHeightInPixels()}");

                    if (bannerStatus == BannerLoadingStatus.None)
                    {
                        return;
                    }

                    bannerStatus = BannerLoadingStatus.Loaded;

                    OnBannerShown.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    bannerStatus = BannerLoadingStatus.None;
                    Debug.LogWarning($"[{logPrefix}] Failed to load banner ad with error: {args.Message}");
                    OnBannerFailed.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnBannerOpened(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    if (Debug.isDebugBuild)
                        Debug.Log($"[{logPrefix}] Banner ad clicked");

                    OnBannerClicked.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnBannerClosed(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    if (Debug.isDebugBuild)
                        Debug.Log($"[{logPrefix}] Banner closed");

                    OnBannerHidden.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnLeavingApplicationFromBanner(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    if (Debug.isDebugBuild)
                        Debug.Log($"[{logPrefix}] Leaving app by click on Banner ad");
                },
                null);
        }

        BannerCallbackData BuildCallbackData()
        {
            return new BannerCallbackData(ProviderId, lastShownBannerData?.PlacementId, banner?.GetHeightInPixels() ?? 0f);
        }

        public void SetBannerSize(AdSize size)
        {
            bannerSize = size;
        }
    }
}