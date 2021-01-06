using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    public class IronSourceBannerProvider : IronSourceAdProvider, IBannerAdProvider
    {
        static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(IronSourceBannerProvider) );

        string lastPlacementId;
        IronSourceBannerSize bannerSize = IronSourceBannerSize.BANNER;
        float shownBannerHeight;

        bool isLoaded = false;
        bool HideOnLoad = false;

        public IronSourceBannerProvider( IronSourceBaseProvider baseProvider )
            : base( baseProvider, PlacementType.Banner ) { }

        public event UnityAction<IBannerCallbackData, bool> OnBannerShown;
        public event UnityAction<IBannerCallbackData> OnBannerFailed;
        public event UnityAction<IBannerCallbackData> OnBannerClicked;
        public event UnityAction<IBannerCallbackData> OnBannerHidden;

        public bool Show( BannerPosition position = BannerPosition.BottomCenter )
        {
            var data = config.GetPlacementData( placementType );
            return data != null && ShowBanner( data, position );
        }

        public bool Show( string placementId, BannerPosition position = BannerPosition.BottomCenter )
        {
            var data = config.GetPlacementData( placementId );
            return data != null && ShowBanner( data, position );
        }

        bool ShowBanner( AdPlacementData data, BannerPosition position )
        {
            HideOnLoad = false;
            lastPlacementId = data.PlacementId;
            IronSource.Agent.loadBanner( bannerSize, position.ToIronSourceBannerPosition(), data.PlacementId );
            IronSource.Agent.displayBanner();
            return true;
        }

        public void Hide()
        {
            if ( lastPlacementId == null )
                return;

            if ( isLoaded == false )
            {
                HideOnLoad = true;
                return;
            }
            isLoaded = false;
            IronSource.Agent.hideBanner();
            IronSource.Agent.destroyBanner();
            HLog.Log( logPrefix, $"Ad Hidden" );
            OnBannerHidden.Dispatch( new BannerCallbackData( ProviderId, lastPlacementId, shownBannerHeight ) );
            lastPlacementId = null;
            shownBannerHeight = 0f;
        }

        protected override void SubscribeEvents()
        {
            IronSourceEvents.onBannerAdLoadedEvent += HandleBannerAdLoaded;
            IronSourceEvents.onBannerAdLoadFailedEvent += HandleBannerAdLoadFailed;
            IronSourceEvents.onBannerAdClickedEvent += HandleBannerAdClicked;
        }

        protected override void UnsubscribeEvents()
        {
            IronSourceEvents.onBannerAdLoadedEvent -= HandleBannerAdLoaded;
            IronSourceEvents.onBannerAdLoadFailedEvent -= HandleBannerAdLoadFailed;
            IronSourceEvents.onBannerAdClickedEvent -= HandleBannerAdClicked;
        }

        void HandleBannerAdLoaded()
        {
            bool isRefresh = isLoaded;
            isLoaded = true;

            if ( HideOnLoad )
            {
                HideOnLoad = false;
                Hide();
                return;
            }

            HLog.Log( logPrefix, $"Banner ad loaded, is refresh {isRefresh}" );
            shownBannerHeight = HAdsUtils.ConvertDpToPixels( CalculateBannerHeightInDp( bannerSize ) );
            OnBannerShown.Dispatch( new BannerCallbackData( ProviderId, lastPlacementId, shownBannerHeight ), isRefresh);
        }

        void HandleBannerAdLoadFailed( IronSourceError error )
        {
            HLog.Log( logPrefix, $"Load failed with error: {error.getDescription()}" );
            OnBannerFailed.Dispatch( new BannerCallbackData( ProviderId, lastPlacementId, 0f ) );
        }

        void HandleBannerAdClicked()
        {
            HLog.Log( logPrefix, $"Ad clicked" );
            OnBannerClicked.Dispatch( new BannerCallbackData( ProviderId, lastPlacementId, shownBannerHeight ) );
        }

        public void SetBannerSize( IronSourceBannerSize size )
        {
            bannerSize = size;
        }

        static float CalculateBannerHeightInDp( IronSourceBannerSize size )
        {
            if ( size == IronSourceBannerSize.BANNER )
                return 50f;

            if ( size == IronSourceBannerSize.LARGE )
                return 90f;

            if ( size == IronSourceBannerSize.RECTANGLE )
                return 250f;

            if ( size == IronSourceBannerSize.SMART )
            {
                if ( Application.platform == RuntimePlatform.IPhonePlayer )
                {
                    var isIpad = SystemInfo.deviceModel.Contains( "iPad" );
                    return isIpad ? 90f : 50f;
                }

                var benchmarkSize = 720 * HAdsUtils.DpToPxScaleFactor;

                if ( Screen.height <= benchmarkSize || Screen.width <= benchmarkSize )
                {
                    return 50f;
                }

                return 90f;
            }

            return size.Height;
        }
    }
}