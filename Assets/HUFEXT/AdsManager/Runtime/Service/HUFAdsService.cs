using System.Collections.Generic;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdManagers;
using HUFEXT.AdsManager.Runtime.AdMediation;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.Service
{
    public class HUFAdsService
    {
        public static HLogPrefix logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(HUFAdsService) );

        public string DefaultBannerPlacement => bannerPlacement;
        public string DefaultInterstitialPlacement => interstitialPlacement;
        public string DefaultRewardedPlacement => rewardedPlacement;

        readonly AdsManagerConfig adsManagerConfig;
        Dictionary<string, AdManagerBase> ads = new Dictionary<string, AdManagerBase>();
        string bannerPlacement = null;
        string interstitialPlacement = null;
        string rewardedPlacement = null;
        List<string> rewardedAdsQueue = new List<string>();

        IAdMediation mediation;
        bool isAlternativeMediation;

        internal AdsManagerConfig ManagerConfig => adsManagerConfig;
        internal IAdMediation Mediation => mediation;

        public event UnityAction<AdsManagerFetchCallbackData> OnAdFetch;

        public HUFAdsService( IAdMediation inMediation, bool inIsAlternativeMediation = true )
        {
            isAlternativeMediation = inIsAlternativeMediation;
            mediation = inMediation;

            if ( isAlternativeMediation )
            {
                logPrefix = new HLogPrefix( logPrefix, mediation.MediationId );
            }

            adsManagerConfig = HConfigs.GetConfig<AdsManagerConfig>();

            if ( mediation.IsInitialized == false )
                mediation.OnInitialize += HandleAdsServiceInitialized;
            else
                HandleAdsServiceInitialized();
        }

        bool IsMediationUsingOnlyAdPlacements()
        {
            var mediator = isAlternativeMediation ? mediation.AdsMediation : adsManagerConfig.AdsProvider;

            return mediator == AdsMediator.IronSource ||
                   mediator == AdsMediator.UnityAds ||
                   mediator == AdsMediator.Chartboost;
        }

        void ClearAdsData()
        {
            foreach ( var ad in ads )
            {
                ad.Value.OnAdFetch -= HandleAdFetch;
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
            HandleAdsServiceInitialized();
        }

        void HandleAdsServiceInitialized()
        {
            HLog.Log( logPrefix, $" Ads Service Initialized" );
            ClearAdsData();

            if ( mediation.AdsProviderConfig == null )
            {
                HLog.LogError( logPrefix, $" No ads mediation config" );
                return;
            }

            mediation.AdsProviderConfig.OnChanged -= AdsConfigOnOnChanged;
            mediation.AdsProviderConfig.OnChanged += AdsConfigOnOnChanged;
            HLog.Log( logPrefix, $" config {mediation.AdsProviderConfig.name}" );
            var adPlacements = mediation.AdsProviderConfig.AdPlacementData;

            var queueFetchingRewarded =
                ( isAlternativeMediation ? mediation.AdsMediation : adsManagerConfig.AdsProvider ) == AdsMediator.AdMob;
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
                        ad = new AdManagerBanner( adPlacements[i], this );
                    }
                        break;
                    case PlacementType.Interstitial:
                    {
                        if ( interstitialPlacement == null )
                            interstitialPlacement = adPlacements[i].PlacementId;
                        ad = new AdManagerInterstitial( adPlacements[i], this );
                    }
                        break;
                    case PlacementType.Rewarded:
                    {
                        if ( rewardedPlacement == null )
                            rewardedPlacement = adPlacements[i].PlacementId;

                        if ( queueFetchingRewarded )
                            ad = new AdManagerRewardedQueue( adPlacements[i], this );
                        else
                            ad = new AdManagerRewarded( adPlacements[i], this );
                    }
                        break;
                    default:
                        continue;
                }

                ad.OnAdFetch += HandleAdFetch;
                ads.Add( adPlacements[i].PlacementId, ad );

                if ( !IsMediationUsingOnlyAdPlacements() )
                    createdAds.Add( adPlacements[i].AppId, ad );
            }
        }

        void HandleAdFetch( AdsManagerFetchCallbackData adData )
        {
            OnAdFetch.Dispatch( adData );
        }

        public string GetMainPlacementId( string placementId )
        {
            if ( placementId == null )
            {
                HLog.LogError( logPrefix, "Tried to get mainPlacementId using null placementId" );
                return string.Empty;
            }

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

        public bool CanShowAd( string placementId, bool checkAlternative = false )
        {
            if ( placementId == null )
                return false;

            string mainPlacement = GetMainPlacementId( placementId );
            return ads.ContainsKey( mainPlacement ) && ads[mainPlacement].IsReady();
        }

        public void ShowAd( string placementId, UnityAction<AdManagerCallback> resultCallback )
        {
            HLog.Log( logPrefix, $"Try show ad {placementId} {CanShowAd( placementId )}" );

            if ( !CanShowAd( placementId ) )
            {
                resultCallback.Dispatch( new AdManagerCallback( mediation.MediationId,
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
            Mediation.SetBannerPosition( position );
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

        public void ShowBannerPersistent( string placementId, BannerPosition position )
        {
            SetBannerPosition( position );
            HLog.Log( logPrefix, $"Try show ad {placementId} {CanShowAd( placementId )}" );

            if ( !CanShowAd( placementId ) )
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