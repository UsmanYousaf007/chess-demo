using System.Collections.Generic;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    public class IronSourceInterstitialProvider : IronSourceAdProvider, IInterstitialAdProvider
    {
        readonly List<AdPlacementData> fetchingPlacements;
        string lastPlacementId;

        static HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(IronSourceInterstitialProvider) );

        public IronSourceInterstitialProvider( IronSourceBaseProvider baseProvider )
            : base( baseProvider, PlacementType.Interstitial )
        {
            fetchingPlacements = new List<AdPlacementData>();
        }

        public event UnityAction<IAdCallbackData> OnInterstitialEnded;
        public event UnityAction<IAdCallbackData> OnInterstitialFetched;
        public event UnityAction<IAdCallbackData> OnInterstitialClicked;

        public bool Show()
        {
            var data = config.GetPlacementData( placementType );
            return data != null && TryShow( data );
        }

        public bool Show( string placementId )
        {
            var data = config.GetPlacementData( placementId );

            return data != null
                   && IsCorrectPlacementType( data.PlacementType )
                   && TryShow( data );
        }

        bool TryShow( AdPlacementData data )
        {
            if ( !IsInterstitialReady() )
            {
                HLog.Log( logPrefix, $"Interstitial is not ready to show! Make sure you fetch it before showing!" );
                return false;
            }

            lastPlacementId = data.PlacementId;
            IronSource.Agent.showInterstitial( data.PlacementId );
            return true;
        }

        public bool IsReady()
        {
            var data = config.GetPlacementData( placementType );
            return data != null && IsInterstitialReady();
        }

        public bool IsReady( string placementId )
        {
            var data = config.GetPlacementData( placementId );
            return data != null && IsCorrectPlacementType( data.PlacementType ) && IsInterstitialReady();
        }

        bool IsInterstitialReady()
        {
            return IronSource.Agent.isInterstitialReady();
        }

        public void Fetch()
        {
            var data = config.GetPlacementData( placementType );

            if ( data != null )
            {
                FetchInterstitial( data );
            }
        }

        public void Fetch( string placementId )
        {
            var data = config.GetPlacementData( placementId );

            if ( data != null && IsCorrectPlacementType( data.PlacementType ) )
            {
                FetchInterstitial( data );
            }
        }

        void FetchInterstitial( AdPlacementData data )
        {
            if ( !fetchingPlacements.Contains( data ) )
            {
                fetchingPlacements.Add( data );
            }

            IronSource.Agent.loadInterstitial();
        }

        protected override void SubscribeEvents()
        {
            IronSourceEvents.onInterstitialAdLoadFailedEvent += HandleInterstitialAdLoadFailed;
            IronSourceEvents.onInterstitialAdShowFailedEvent += HandleInterstitialAdShowFailed;
            IronSourceEvents.onInterstitialAdClosedEvent += HandleInterstitialAdClosed;
            IronSourceEvents.onInterstitialAdClickedEvent += HandleInterstitialAdClicked;
            IronSourceEvents.onInterstitialAdReadyEvent += HandleInterstitialAdReady;
            IronSourceEvents.onInterstitialAdOpenedEvent += HandleInterstitialAdOpened;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += HandleInterstitialAdShowSucceeded;
        }

        protected override void UnsubscribeEvents()
        {
            IronSourceEvents.onInterstitialAdLoadFailedEvent -= HandleInterstitialAdLoadFailed;
            IronSourceEvents.onInterstitialAdShowFailedEvent -= HandleInterstitialAdShowFailed;
            IronSourceEvents.onInterstitialAdClosedEvent -= HandleInterstitialAdClosed;
            IronSourceEvents.onInterstitialAdClickedEvent -= HandleInterstitialAdClicked;
            IronSourceEvents.onInterstitialAdReadyEvent -= HandleInterstitialAdReady;
            IronSourceEvents.onInterstitialAdOpenedEvent -= HandleInterstitialAdOpened;
            IronSourceEvents.onInterstitialAdShowSucceededEvent -= HandleInterstitialAdShowSucceeded;
        }

        void HandleInterstitialAdLoadFailed( IronSourceError error )
        {
            foreach ( var placement in fetchingPlacements )
            {
                OnInterstitialFetched.Dispatch(
                    new AdCallbackData( ProviderId, placement.PlacementId, AdResult.Failed ) );
            }

            HLog.Log( logPrefix, $"Failed to load interstitial with error: {error.getDescription()}" );
        }

        void HandleInterstitialAdShowFailed( IronSourceError error )
        {
            OnInterstitialEnded.Dispatch( new AdCallbackData( ProviderId, lastPlacementId, AdResult.Failed ) );
            HLog.Log( logPrefix, $"Failed to show interstitial with error: {error.getDescription()}" );
        }

        void HandleInterstitialAdClosed()
        {
            OnInterstitialEnded.Dispatch( new AdCallbackData( ProviderId, lastPlacementId, AdResult.Completed ) );
            HLog.Log( logPrefix, $"Closed" );
        }

        void HandleInterstitialAdClicked()
        {
            OnInterstitialClicked.Dispatch( new AdCallbackData( ProviderId, lastPlacementId, AdResult.Completed ) );
            HLog.Log( logPrefix, $"Clicked" );
        }

        void HandleInterstitialAdReady()
        {
            foreach ( var placement in fetchingPlacements )
            {
                OnInterstitialFetched.Dispatch(
                    new AdCallbackData( ProviderId, placement.PlacementId, AdResult.Completed ) );
            }

            fetchingPlacements.Clear();
            HLog.Log( logPrefix, $"Ready" );
        }

        void HandleInterstitialAdOpened()
        {
            HLog.Log( logPrefix, $"Opened" );
        }

        void HandleInterstitialAdShowSucceeded()
        {
            HLog.Log( logPrefix, $"Show succeeded" );
        }
    }
}