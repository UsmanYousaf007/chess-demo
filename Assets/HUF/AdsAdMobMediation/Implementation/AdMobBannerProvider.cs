using System;
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
        public event UnityAction<IBannerCallbackData> OnBannerShown;
        public event UnityAction<IBannerCallbackData> OnBannerFailed;
        public event UnityAction<IBannerCallbackData> OnBannerClicked;
        public event UnityAction<IBannerCallbackData> OnBannerHidden;

        BannerView banner;
        AdPlacementData lastShownBannerData;
        AdSize bannerSize = AdSize.SmartBanner;
        
        public AdMobBannerProvider(AdMobProviderBase baseProvider) : base(baseProvider) { }
        
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
            Debug.Log($"[{logPrefix}] Show Banner ad with placementId: {data.PlacementId}");

            if (banner != null)
            {
                UnsubscribeCallbacks();
                banner.Destroy();
            }

            lastShownBannerData = data;
            banner = new BannerView(data.AppId, bannerSize, position.ToAdMobBannerPosition());
            SubscribeCallbacks();
            banner.LoadAd(baseProvider.CreateRequest());
            banner.Show();
            return true;
        }

        public void Hide()
        {
            Debug.Log($"[{logPrefix}] Hide banner ad");
            banner?.Hide();
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
            var bannerHeight = banner.GetHeightInPixels();
            Debug.Log($"[{logPrefix}] Banner ad loaded, height: ${bannerHeight}");
            OnBannerShown.Dispatch(BuildCallbackData());
        }

        void OnBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogWarning($"[{logPrefix}] Failed to load banner ad with error: {args.Message}");
            OnBannerFailed.Dispatch(BuildCallbackData());
        }

        void OnBannerOpened(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Banner ad clicked");
            OnBannerClicked.Dispatch(BuildCallbackData());
        }

        void OnBannerClosed(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Banner closed");
            OnBannerHidden.Dispatch(BuildCallbackData());
        }

        void OnLeavingApplicationFromBanner(object sender, EventArgs args)
        {
            Debug.Log($"[{logPrefix}] Leaving app by click on Banner ad");
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