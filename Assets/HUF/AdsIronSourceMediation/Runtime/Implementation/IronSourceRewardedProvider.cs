using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    public class IronSourceRewardedProvider : IronSourceAdProvider, IRewardedAdProvider
    {
        const float FETCHING_INTERVAL = 0.5f;
        const float FETCHING_LIMIT = 5f;

        new static HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(IronSourceRewardedProvider) );

        readonly List<AdPlacementData> fetchingPlacementsIds;

        string lastPlacementId;
        bool adRewarded;
        Coroutine fetchingCoroutine;

        public IronSourceRewardedProvider( IronSourceBaseProvider baseProvider )
            : base( baseProvider, PlacementType.Rewarded )
        {
            fetchingPlacementsIds = new List<AdPlacementData>();
        }

        public event Action<IAdCallbackData> OnRewardedEnded;
        public event Action<IAdCallbackData> OnRewardedFetched;
        public event Action<IAdCallbackData> OnRewardedClicked;

        public bool Show()
        {
            var data = config.GetPlacementData( placementType );
            return data != null && TryShow( data );
        }

        public bool Show( string placementId )
        {
            var data = config.GetPlacementData( placementId );
            return data != null && IsCorrectPlacementType( data.PlacementType ) && TryShow( data );
        }

        bool TryShow( AdPlacementData data )
        {
            if ( !IsRewardedReady() )
            {
                HLog.Log( logPrefix, $"Rewarded is not ready to show! Make sure you fetch it before showing!" );
                return false;
            }

            adRewarded = false;
            lastPlacementId = data.PlacementId;
            IronSource.Agent.showRewardedVideo( data.PlacementId );
            return true;
        }

        public bool IsReady()
        {
            var data = config.GetPlacementData( placementType );
            return data != null && IsRewardedReady();
        }

        public bool IsReady( string placementId )
        {
            var data = config.GetPlacementData( placementId );

            return data != null && IsCorrectPlacementType( data.PlacementType ) && IsRewardedReady() &&
                   !IronSource.Agent.isRewardedVideoPlacementCapped( placementId );
        }

        bool IsRewardedReady()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public void Fetch()
        {
            var data = config.GetPlacementData( placementType );

            if ( data != null )
            {
                TryFetch( data );
            }
        }

        public void Fetch( string placementId )
        {
            var data = config.GetPlacementData( placementId );

            if ( data != null && IsCorrectPlacementType( data.PlacementType ) )
            {
                TryFetch( data );
            }
        }

        void TryFetch( AdPlacementData data )
        {
            if ( !fetchingPlacementsIds.Contains( data ) )
            {
                fetchingPlacementsIds.Add( data );
            }

            if ( fetchingCoroutine == null )
            {
                fetchingCoroutine = CoroutineManager.StartCoroutine( FetchRewarded() );
            }
        }

        IEnumerator FetchRewarded()
        {
            var idleTime = new WaitForSecondsRealtime( FETCHING_INTERVAL );
            bool isAdReady;
            var fetchingTime = 0f;

            while ( !( isAdReady = IsRewardedReady() ) && fetchingTime < FETCHING_LIMIT )
            {
                yield return idleTime;

                fetchingTime += FETCHING_INTERVAL;
            }

            var result = isAdReady ? AdResult.Completed : AdResult.Failed;

            foreach ( var placement in fetchingPlacementsIds )
            {
                OnRewardedFetched.Dispatch( new AdCallbackData( ProviderId, placement.PlacementId, result ) );
            }

            fetchingPlacementsIds.Clear();
            fetchingCoroutine = null;
        }

        protected override void SubscribeEvents()
        {
            IronSourceEvents.onRewardedVideoAdOpenedEvent += HandleRewardedVideoAdOpened;
            IronSourceEvents.onRewardedVideoAdEndedEvent += HandleRewardedVideoAdEnded;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += HandleRewardedVideoAdRewarded;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += HandleRewardedVideoAdShowFailed;
            IronSourceEvents.onRewardedVideoAdClosedEvent += HandleRewardedVideoAdClosed;
            IronSourceEvents.onRewardedVideoAdClickedEvent += HandleRewardedVideoAdClicked;
            IronSourceEvents.onRewardedVideoAdStartedEvent += HandleRewardedVideoAdStarted;
        }

        protected override void UnsubscribeEvents()
        {
            IronSourceEvents.onRewardedVideoAdOpenedEvent -= HandleRewardedVideoAdOpened;
            IronSourceEvents.onRewardedVideoAdEndedEvent -= HandleRewardedVideoAdEnded;
            IronSourceEvents.onRewardedVideoAdRewardedEvent -= HandleRewardedVideoAdRewarded;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent -= HandleRewardedVideoAdShowFailed;
            IronSourceEvents.onRewardedVideoAdClosedEvent -= HandleRewardedVideoAdClosed;
            IronSourceEvents.onRewardedVideoAdClickedEvent -= HandleRewardedVideoAdClicked;
            IronSourceEvents.onRewardedVideoAdStartedEvent -= HandleRewardedVideoAdStarted;
        }

        void HandleRewardedVideoAdOpened()
        {
            HLog.Log( logPrefix, $"Opened" );
        }

        void HandleRewardedVideoAdEnded()
        {
            HLog.Log( logPrefix, $"Ended" );
        }

        void HandleRewardedVideoAdRewarded( IronSourcePlacement placement )
        {
            adRewarded = true;
            HLog.Log( logPrefix, $"Rewarded" );
        }

        void HandleRewardedVideoAdShowFailed( IronSourceError error )
        {
            OnRewardedEnded.Dispatch( new AdCallbackData( ProviderId, lastPlacementId, AdResult.Failed ) );
            HLog.Log( logPrefix, $"Failed to show with error: {error.getDescription()}" );
        }

        void HandleRewardedVideoAdClosed()
        {
            CoroutineManager.StartCoroutine( WaitAndSendCloseEvent() );
            HLog.Log( logPrefix, $"Closed" );
        }

        void HandleRewardedVideoAdClicked( IronSourcePlacement placement )
        {
            OnRewardedClicked.Dispatch(
                new AdCallbackData( ProviderId, lastPlacementId, AdResult.Completed ) );
            HLog.Log( logPrefix, $"Clicked" );
        }

        void HandleRewardedVideoAdStarted()
        {
            HLog.Log( logPrefix, $"Started" );
        }

        IEnumerator WaitAndSendCloseEvent()
        {
            yield return null;

            OnRewardedEnded.Dispatch( new AdCallbackData( ProviderId,
                lastPlacementId,
                adRewarded ? AdResult.Completed : AdResult.Skipped ) );
        }
    }
}