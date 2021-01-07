using System.Linq;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdMediation;
using HUFEXT.AdsManager.Runtime.Config;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class BaseAdMediation : IAdMediation
    {
        public string MediationId
        {
            get
            {
                if ( HAds.Banner == null )
                {
                    return AdsProviderConfig.PackageName;
                }
                return HAds.Banner.GetAdProviderName();
            }
        }

        public bool IsInitialized => HAds.IsAdsServiceInitialized();

        static readonly HLogPrefix logPrefix = new HLogPrefix( HAds.logPrefix, nameof(BaseAdMediation) );

        const int BANNER_HEIGHT = 50;
        AdsProviderConfig adsConfig;
        BannerPosition bannerPosition = BannerPosition.BottomCenter;

        public event UnityAction OnInitialize;
        public event UnityAction<AdCallback> OnBannerFetched;
        public event UnityAction<AdCallback> OnBannerShown;
        public event UnityAction<AdCallback> OnBannerClicked;
        public event UnityAction<AdCallback> OnBannerHidden;
        public event UnityAction<AdCallback> OnInterstitialEnded;
        public event UnityAction<AdCallback> OnInterstitialFetched;
        public event UnityAction<AdCallback> OnInterstitialClicked;
        public event UnityAction<AdCallback> OnRewardedEnded;
        public event UnityAction<AdCallback> OnRewardedFetched;
        public event UnityAction<AdCallback> OnRewardedClicked;

        
        public AdsMediator AdsMediation
        {
            get
            {
                AdsManagerConfig adsManagerConfig = HConfigs.GetConfig<AdsManagerConfig>();

                if ( adsManagerConfig != null )
                    return adsManagerConfig.AdsProvider;
                else
                    HLog.LogError( logPrefix, $"No ads manager config" );

                return AdsMediator.Other;
            }
        }

        public BaseAdMediation()
        {
            SubscribeToEvents();
        }

        public void Init() { }

        void SubscribeToEvents()
        {
            HAds.OnAdsServiceInitialized += HandleAdsServiceInitialized;
            
            HAds.Banner.OnShown += HandleBannerShow;
            HAds.Banner.OnFailed += HandleBannerFailed;
            HAds.Banner.OnClicked += HandleBannerClicked;
            HAds.Banner.OnHidden += HandleBannerHidden;
            
            HAds.Interstitial.OnClicked += HandleInterstitialClicked;
            HAds.Interstitial.OnFetched += HandleInterstitialFetched;
            HAds.Interstitial.OnEnded += HandleInterstitialEnded;
            
            HAds.Rewarded.OnClicked += HandleRewardedClicked;
            HAds.Rewarded.OnFetched += HandleRewardedFetched;
            HAds.Rewarded.OnEnded += HandleRewardedEnded;
        }
        
        void HandleAdsServiceInitialized()
        {
            OnInitialize.Dispatch();
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HAds.CollectSensitiveData( consentStatus );
        }

        public AdsProviderConfig AdsProviderConfig
        {
            get
            {
                if ( adsConfig == null )
                {
                    var adsProviderConfigs = HConfigs.GetConfigsByBaseClass<AdsProviderConfig>().ToList();

                    if ( adsProviderConfigs.Count == 0 )
                    {
                        HLog.LogError( logPrefix, $"No ads mediation config" );
                        return null;
                    }

                    adsConfig = adsProviderConfigs[0];

                    if ( adsProviderConfigs.Count <= 1 )
                        return adsConfig;

                    for ( var index = 0; index < adsProviderConfigs.Count; index++ )
                    {
                        if ( adsProviderConfigs[index].ConfigId.Contains( "HADS" ) ||
                             adsProviderConfigs[index].ConfigId.Contains( "Huuuge" ) )
                            continue;

                        adsConfig = adsProviderConfigs[index];
                        break;
                    }
                }

                return adsConfig;
            }
        }

        public void SetBannerPosition( BannerPosition position = BannerPosition.BottomCenter )
        {
            bannerPosition = position;
        }

        public void FetchBanner( string placementId )
        {
            OnBannerFetched.Dispatch( new AdCallback( placementId, MediationId, AdResult.Completed, BANNER_HEIGHT) );
        }

        public bool IsBannerReady( string placementId )
        {
            return IsInitialized;
        }

        public void ShowBanner( string placementId )
        {
            if ( !HAds.Banner.Show( placementId, bannerPosition ) )
            {
                OnBannerShown.Dispatch( new AdCallback( placementId, MediationId, AdResult.Failed ) );
            }
        }

        public void HideBanner( string placementId )
        {
            HAds.Banner.Hide();
        }

        public void FetchInterstitial( string placementId )
        {
            HAds.Interstitial.Fetch( placementId );
        }

        public bool IsInterstitialReady( string placementId )
        {
            return HAds.Interstitial.IsReady( placementId );
        }

        public void ShowInterstitial( string placementId )
        {
            if ( !HAds.Interstitial.TryShow( placementId ) )
            {
                OnInterstitialEnded.Dispatch( new AdCallback( placementId, MediationId, AdResult.Failed ) );
            }
        }

        public void FetchRewarded( string placementId )
        {
            HLog.Log( logPrefix, $"FetchRewarded" );
            HAds.Rewarded.Fetch( placementId );
        }

        public void ShowRewarded( string placementId )
        {
            if ( !HAds.Rewarded.TryShow( placementId ) )
            {
                OnInterstitialEnded.Dispatch( new AdCallback( placementId, MediationId, AdResult.Failed ) );
            }
        }

        public bool IsRewardedReady( string placementId )
        {
            return HAds.Rewarded.IsReady( placementId );
        }

        void HandleBannerFailed( IBannerCallbackData callbackData )
        {
            HLog.Log( logPrefix, $"OnFailed {callbackData.PlacementId}" );

            OnBannerShown.Dispatch( new AdCallback( callbackData.PlacementId,
                MediationId,
                AdResult.Failed,
                callbackData.HeightInPx ) );
        }

        void HandleBannerShow( IBannerCallbackData callbackData, bool isRefresh )
        {
            if (isRefresh)
                return;

            OnBannerShown.Dispatch( new AdCallback( callbackData.PlacementId,
                MediationId,
                AdResult.Completed,
                callbackData.HeightInPx ) );
        }

        void HandleBannerHidden( IBannerCallbackData callbackData )
        {
            OnBannerHidden.Dispatch( new AdCallback( callbackData.PlacementId,
                MediationId,
                AdResult.Completed,
                callbackData.HeightInPx ) );
        }

        void HandleBannerClicked( IBannerCallbackData callbackData )
        {
            OnBannerClicked.Dispatch( new AdCallback( callbackData.PlacementId,
                MediationId,
                AdResult.Completed,
                callbackData.HeightInPx ) );
        }
        
        void HandleInterstitialEnded( IAdCallbackData callbackData )
        {
            OnInterstitialEnded.Dispatch( new AdCallback( callbackData.PlacementId, callbackData.ProviderId, callbackData.Result));
        }

        void HandleInterstitialFetched( IAdCallbackData callbackData )
        {
            OnInterstitialFetched.Dispatch( new AdCallback( callbackData.PlacementId, callbackData.ProviderId, callbackData.Result));
        }

        void HandleInterstitialClicked( IAdCallbackData callbackData )
        {
            OnInterstitialClicked.Dispatch( new AdCallback( callbackData.PlacementId, callbackData.ProviderId, callbackData.Result));
        }
        
        void HandleRewardedEnded( IAdCallbackData callbackData )
        {
            OnRewardedEnded.Dispatch( new AdCallback( callbackData.PlacementId, callbackData.ProviderId, callbackData.Result));
        }

        void HandleRewardedFetched( IAdCallbackData callbackData )
        {
            OnRewardedFetched.Dispatch( new AdCallback( callbackData.PlacementId, callbackData.ProviderId, callbackData.Result));
        }

        void HandleRewardedClicked( IAdCallbackData callbackData )
        {
            OnRewardedClicked.Dispatch( new AdCallback( callbackData.PlacementId, callbackData.ProviderId, callbackData.Result));
        }
    }
}