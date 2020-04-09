using System.Collections.Generic;
using System.Linq;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdManagers;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.Service
{
    public class HUFAdsService
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(HUFAdsService) );
        readonly AdsManagerConfig adsManagerConfig;
        AdsProviderConfig adsConfig;
        Dictionary<string, AdManagerBase> ads = new Dictionary<string, AdManagerBase>();
        string bannerPlacement = null;
        string interstitialPlacement = null;
        string rewardedPlacement = null;
        BannerPosition bannerPosition = BannerPosition.BottomCenter;
        List<string> rewardedAdsQueue = new List<string>();

        public UnityAction<AdsManagerFetchCallbackData> OnAdFetch;

        public HUFAdsService()
        {
            adsManagerConfig = HConfigs.GetConfig<AdsManagerConfig>();

            if ( HAds.IsAdsServiceInitialized() == false )
                HAds.OnAdsServiceInitialized += OnAdsServiceInitialized;
            else
                OnAdsServiceInitialized();
        }

        void GetProperAdsConfig()
        {
            if ( adsConfig != null )
            {
                adsConfig.OnChanged -= AdsConfigOnOnChanged;
            }

            var adsProviderConfigs = HConfigs.GetConfigsByBaseClass<AdsProviderConfig>().ToList();

            if ( adsProviderConfigs.Count == 0 )
            {
                HLog.LogError( logPrefix, $"No ads mediation config" );
                return;
            }

            adsConfig = adsProviderConfigs[0];

            if ( adsProviderConfigs.Count > 1 )
            {
                var adMediationName = HAds.Interstitial.GetAdProviderName().ToLower();

                foreach ( var adConfig in adsProviderConfigs )
                {
                    if ( adConfig.name.ToLower().Contains( adMediationName ) )
                    {
                        adsConfig = adConfig;
                        return;
                    }
                }
            }
        }

        bool IsMediationUsingOnlyAdPlacements()
        {
            return adsManagerConfig.AdsProvider == AdsMediator.Ironsource ||
                   adsManagerConfig.AdsProvider == AdsMediator.UnityAds ||
                   adsManagerConfig.AdsProvider == AdsMediator.Chartboost;
        }

        void ClearAdsData()
        {
            foreach ( var ad in ads )
            {
                ad.Value.Destroy();
            }

            ads.Clear();
            bannerPlacement = null;
            interstitialPlacement = null;
            rewardedPlacement = null;
            rewardedAdsQueue = new List<string>();
        }

        void AdsConfigOnOnChanged( AbstractConfig arg0 )
        {
            OnAdsServiceInitialized();
        }

        void OnAdsServiceInitialized()
        {
            HLog.Log( logPrefix, $" Ads Service Initialized" );
            GetProperAdsConfig();
            ClearAdsData();

            if ( adsConfig == null )
            {
                HLog.LogError( logPrefix, $" No ads mediation config" );
                return;
            }

            HLog.Log( logPrefix, $" config {adsConfig.name}" );
            adsConfig.OnChanged += AdsConfigOnOnChanged;
            var adPlacements = adsConfig.AdPlacementData;
            var queueFetchingRewarded = adsManagerConfig.AdsProvider == AdsMediator.AdMob;
            var createdAds = new Dictionary<string, AdManagerBase>();

            for ( int i = 0; i < adPlacements.Count; i++ )
            {
                if ( !IsMediationUsingOnlyAdPlacements() &&
                     createdAds.ContainsKey( adPlacements[i].AppId ) )
                {
                    createdAds[adPlacements[i].AppId].AddAlternativePlacement( adPlacements[i].PlacementId );
                    ads.Add( adPlacements[i].PlacementId, createdAds[adPlacements[i].AppId] );
                    continue;
                }

                AdManagerBase ad = null;

                switch ( adPlacements[i].PlacementType )
                {
                    case PlacementType.Banner:
                    {
                        if ( bannerPlacement == null )
                            bannerPlacement = adPlacements[i].PlacementId;
                        ad = new AdManagerBanner( adPlacements[i], adsManagerConfig, this );
                    }
                        break;
                    case PlacementType.Interstitial:
                    {
                        if ( interstitialPlacement == null )
                            interstitialPlacement = adPlacements[i].PlacementId;
                        ad = new AdManagerInterstitial( adPlacements[i], adsManagerConfig, this );
                    }
                        break;
                    case PlacementType.Rewarded:
                    {
                        if ( rewardedPlacement == null )
                            rewardedPlacement = adPlacements[i].PlacementId;

                        if ( queueFetchingRewarded )
                            ad = new AdManagerRewardedQueue( adPlacements[i], adsManagerConfig, this );
                        else
                            ad = new AdManagerRewarded( adPlacements[i], adsManagerConfig, this );
                    }
                        break;
                    default:
                        continue;
                }

                ads.Add( adPlacements[i].PlacementId, ad );

                if ( !IsMediationUsingOnlyAdPlacements() )
                    createdAds.Add( adPlacements[i].AppId, ad );
            }
        }

        public string GetMainPlacementId( string placementId )
        {
            if ( ads.ContainsKey( placementId ) )
                return placementId;

            foreach ( var ad in ads )
            {
                if ( ad.Value.ContainsPlacement( placementId ) )
                {
                    return ad.Key;
                }
            }

            return placementId;
        }

        public bool CanShowAd( string placementId )
        {
            string mainPlacement = GetMainPlacementId( placementId );
            return ads.ContainsKey( mainPlacement ) && ads[mainPlacement].IsReady();
        }

        public void ShowAd( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            HLog.Log( logPrefix, $"Try show ad {placementId} {CanShowAd( placementId )}" );

            if ( placementId == null || !CanShowAd( placementId ) )
            {
                resultCallback.Dispatch( new AdManagerCallback( HAds.Interstitial.GetAdProviderName(),
                    placementId,
                    AdResult.Failed ) );
                return;
            }

            HLog.Log( logPrefix, $"Show ad {placementId}" );
            string mainPlacement = GetMainPlacementId( placementId );
            ads[mainPlacement].ShowAd( resultCallback, placementId );
        }

        public void SetBannerPosition( BannerPosition position )
        {
            bannerPosition = position;
        }

        public void HideBanner( string placementId )
        {
            string mainPlacement = GetMainPlacementId( placementId );

            if ( ads.ContainsKey( mainPlacement ) == false )
                return;

            var bannerAd = (AdManagerBanner)ads[mainPlacement];

            if ( bannerAd == null )
            {
                HLog.LogError( logPrefix, $" Try show banner ad {placementId} with is not a banner ad" );
                return;
            }

            bannerAd.HideBanner();
        }

        public void HideBanner()
        {
            HideBanner( bannerPlacement );
        }

        public void ShowBanner( UnityAction<AdManagerCallback> resultCallback, BannerPosition position )
        {
            ShowBanner( bannerPlacement, resultCallback, position );
        }

        public void ShowBanner( string placementId,
            UnityAction<AdManagerCallback> resultCallback,
            BannerPosition position )
        {
            SetBannerPosition( position );
            ShowAd( placementId, resultCallback );
        }

        public void ShowBanner( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowBanner( placementId, resultCallback, bannerPosition );
        }

        public void ShowBannerPersistent( string placementId, BannerPosition position )
        {
            SetBannerPosition( position );
            HLog.Log( logPrefix, $"Try show ad {placementId} {CanShowAd( placementId )}" );

            if ( placementId == null || !CanShowAd( placementId ) )
            {
                return;
            }

            HLog.Log( logPrefix, $"Show Banner Persistent {placementId}" );
            string mainPlacement = GetMainPlacementId( placementId );
            var bannerAd = (AdManagerBanner)ads[mainPlacement];

            if ( bannerAd == null )
            {
                HLog.LogError( logPrefix, $"Try show banner ad {placementId} with is not a banner ad" );
                return;
            }

            bannerAd.ShowBannerPersistent( placementId );
        }

        public void ShowInterstitial( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        public void ShowInterstitial( UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( interstitialPlacement, resultCallback );
        }

        public void ShowRewarded( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( placementId, resultCallback );
        }

        public void ShowRewarded( UnityAction<AdManagerCallback> resultCallback )
        {
            ShowAd( rewardedPlacement, resultCallback );
        }

        public void RemoveFromRewardedAdQueue( string placementId )
        {
            rewardedAdsQueue.Remove( placementId );

            if ( rewardedAdsQueue.Count > 0 )
            {
                if ( !ads[rewardedAdsQueue[0]].IsReady() )
                {
                    ( (AdManagerRewardedQueue)ads[rewardedAdsQueue[0]] ).TryFetch();
                }
                else
                {
                    RemoveFromRewardedAdQueue( rewardedAdsQueue[0] );
                }
            }
        }

        public bool CanFetchRewardedAd( string placementId )
        {
            if ( rewardedAdsQueue.Count == 0 || !rewardedAdsQueue.Contains( placementId ) )
            {
                rewardedAdsQueue.Add( placementId );
            }

            return rewardedAdsQueue[0] == placementId;
        }
    }
}