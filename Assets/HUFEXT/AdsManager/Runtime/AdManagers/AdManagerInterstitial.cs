using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerInterstitial : AdManagerBase
    {
        public AdManagerInterstitial(
            AdPlacementData inAdPlacementData,
            AdsManagerConfig inAdsManagerConfig,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsManagerConfig,
            inAdsService )
        {
            logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerInterstitial) );
        }

        protected override void RegisterEvents()
        {
            HAds.Interstitial.OnFetched += OnFetched;
            HAds.Interstitial.OnEnded += OnEnded;
        }

        protected override void Fetch()
        {
            base.Fetch();
            HAds.Interstitial.Fetch( adPlacementData.PlacementId );
        }

        public override void ShowAd( UnityAction<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            base.ShowAd( resultCallback, alternativeAdPlacement );

            if ( HAds.Interstitial.TryShow( adPlacementData.PlacementId ) == false )
            {
                OnFailToShow();
            }
        }

        protected override void SendAdEvent( AdResult result, string placementId )
        {
            showResponseCallbacks.Dispatch(
                new AdManagerCallback(
                    GetAdMediationName(),
                    placementId,
                    result ) );
        }

        protected override string GetAdMediationName()
        {
            return HAds.Interstitial.GetAdProviderName();
        }

        protected override void DisconnectFromAds()
        {
            HAds.Interstitial.OnFetched -= OnFetched;
            HAds.Interstitial.OnEnded -= OnEnded;
        }

        protected override bool NativeIsReady()
        {
            HLog.Log( logPrefix, $"NativeIsReady {adPlacementData.PlacementId}" );
            return HAds.Interstitial.IsReady( adPlacementData.PlacementId );
        }
    }
}