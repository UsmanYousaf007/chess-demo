using System;
using GoogleMobileAds.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.AdsAdMobMediation.Implementation
{
    public enum AdMobBannerAdSize
    {
        Banner, //AdSize(320, 50);
        MediumRectangle, //AdSize(300, 250);
        IABBanner, //AdSize(468, 60);
        Leaderboard, //AdSize(728, 90);
        SmartBanner, //AdSize(0, 0, Type.SmartBanner);
        FullWidth //Adaptive6
    }

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
        AdMobBannerAdSize bannerSize = AdMobBannerAdSize.SmartBanner;

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

            HLog.Log(logPrefix, $"Show Banner ad with placementId: {data.PlacementId}");

            if (banner != null)
            {
                UnsubscribeCallbacks();
                banner.Destroy();
            }

            bannerStatus = BannerLoadingStatus.Loading;

            lastShownBannerData = data;

            banner = new BannerView(data.AppId, GetAdSize(), position.ToAdMobBannerPosition());

            SubscribeCallbacks();
            banner.LoadAd(baseProvider.CreateRequest());
            banner.Show();
            return true;
        }

        AdSize GetAdSize()
        {
            switch (bannerSize)
            {
                case AdMobBannerAdSize.Banner:
                    return AdSize.Banner;
                case AdMobBannerAdSize.MediumRectangle:
                    return AdSize.MediumRectangle;
                case AdMobBannerAdSize.IABBanner:
                    return AdSize.IABBanner;
                case AdMobBannerAdSize.Leaderboard:
                    return AdSize.Leaderboard;
                case AdMobBannerAdSize.SmartBanner:
                    return AdSize.SmartBanner;
                case AdMobBannerAdSize.FullWidth:
                    return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                default:
                    return AdSize.SmartBanner;
            }
        }

        public void Hide()
        {
            HLog.Log(logPrefix, "Hide banner ad");

            if (bannerStatus != BannerLoadingStatus.None)
                banner?.Hide();

            bannerStatus = BannerLoadingStatus.None;
        }

        void SubscribeCallbacks()
        {
            if (banner == null)
            {
                HLog.LogError(logPrefix, "Failed to subscribe banner callbacks!");
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
                HLog.LogError(logPrefix, "Failed to unsubscribe banner callbacks!");
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
                    HLog.Log(logPrefix, $"Banner ad loaded, height: ${banner.GetHeightInPixels()}");

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
                    HLog.LogWarning(logPrefix, $"Failed to load banner ad with error: {args.Message}");
                    OnBannerFailed.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnBannerOpened(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    HLog.Log(logPrefix, "Banner ad clicked");
                    OnBannerClicked.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnBannerClosed(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    HLog.Log(logPrefix, "Banner closed");
                    OnBannerHidden.Dispatch(BuildCallbackData());
                },
                null);
        }

        void OnLeavingApplicationFromBanner(object sender, EventArgs args)
        {
            syncContext.Post(
                s =>
                {
                    HLog.Log(logPrefix, "Leaving app by click on Banner ad");
                },
                null);
        }

        BannerCallbackData BuildCallbackData()
        {
            return new BannerCallbackData(ProviderId, lastShownBannerData?.PlacementId, banner?.GetHeightInPixels() ?? 0f);
        }

        public void SetBannerSize(AdMobBannerAdSize size)
        {
            bannerSize = size;
        }
    }
}