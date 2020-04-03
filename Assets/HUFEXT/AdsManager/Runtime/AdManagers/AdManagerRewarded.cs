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
    public class AdManagerRewarded : AdManagerBase
    {
        public AdManagerRewarded(
            AdPlacementData inAdPlacementData,
            AdsManagerConfig inAdsManagerConfig,
            HUFAdsService inAdsService ) : base(
            inAdPlacementData,
            inAdsManagerConfig,
            inAdsService )
        {
            logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerRewarded) );
        }

        protected override void RegisterEvents()
        {
            HAds.Rewarded.OnFetched += OnFetched;
            HAds.Rewarded.OnEnded += OnEnded;
        }

        protected override void Fetch()
        {
            base.Fetch();
            HAds.Rewarded.Fetch( adPlacementData.PlacementId );
        }

        public override void ShowAd( UnityAction<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            base.ShowAd( resultCallback, alternativeAdPlacement );

            if ( HAds.Rewarded.TryShow( adPlacementData.PlacementId ) == false )
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
            return HAds.Rewarded.GetAdProviderName();
        }

        protected override void DisconnectFromAds()
        {
            HAds.Rewarded.OnFetched -= OnFetched;
            HAds.Rewarded.OnEnded -= OnEnded;
        }

        protected override bool NativeIsReady()
        {
            HLog.Log( logPrefix, $"NativeIsReady {adPlacementData.PlacementId}" );
            return HAds.Rewarded.IsReady( adPlacementData.PlacementId );
        }
    }
}