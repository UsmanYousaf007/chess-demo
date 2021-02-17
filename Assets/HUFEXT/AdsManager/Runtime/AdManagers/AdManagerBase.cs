using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.AdMediation;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public abstract class AdManagerBase
    {
        const float MINIMUM_LONG_FETCH_TIME = 5f;
        const float MINIMUM_SHORT_FETCH_TIME = 1f;

        protected readonly HLogPrefix logPrefix;
        protected readonly AdPlacementData adPlacementData;
        protected readonly HUFAdsService adsService;
        
        protected AdStatus adStatus = AdStatus.WaitingForFetch;
        protected int currentFetchTimes;
        protected string shownPlacementId;
        
        protected Action<AdManagerCallback> showResponseCallbacks;

        List<string> alternativePlacements = new List<string>();
        Coroutine delayFetchCoroutine;

        public AdManagerBase( AdPlacementData inAdPlacementData,
            HUFAdsService inAdsService )
        {
            adPlacementData = inAdPlacementData;
            adsService = inAdsService;

            logPrefix = new HLogPrefix( HAdsManager.logPrefix,
                $"{( ServiceName != null ? ServiceName : "" )}.{adsService.Mediation.MediationId}" );
            SubscribeToEvents();
            StartFetching();
        }

        public event Action<AdsManagerFetchCallbackData> OnAdFetch;

        protected enum AdStatus
        {
            WaitingForFetch,
            Fetching,
            ReadyToShow,
            Showing
        }

        protected abstract void UnsubscribeFromEvents();
        protected abstract string ServiceName { get; }
        protected abstract bool NativeIsReady();

        protected virtual void SendAdEvent( AdResult result, string placementId ) { }
        protected virtual void SubscribeToEvents() { }

        public void Destroy()
        {
            if ( delayFetchCoroutine != null )
                CoroutineManager.StopCoroutine( delayFetchCoroutine );
            UnsubscribeFromEvents();
        }

        public void AddAlternativePlacement( string inPlacement )
        {
            alternativePlacements.Add( inPlacement );

            if ( adStatus == AdStatus.ReadyToShow )
            {
                OnAdFetch.Dispatch( new AdsManagerFetchCallbackData( GetAdMediationName(), inPlacement ) );
            }
        }

        public bool ContainsPlacement( string inPlacement )
        {
            return alternativePlacements.Contains( inPlacement );
        }

        public bool IsReady()
        {
            if ( CanAdExpire() && adStatus == AdStatus.ReadyToShow && NativeIsReady() == false )
            {
                HLog.LogWarning( logPrefix, $"Ad did expired - fetch new one {adPlacementData.PlacementId}" );
                StartFetching();
            }

            return adStatus == AdStatus.ReadyToShow;
        }

        public virtual void ShowAd( Action<AdManagerCallback> resultCallback, string alternativeAdPlacement )
        {
            adStatus = AdStatus.Showing;
            showResponseCallbacks = resultCallback;

            if ( alternativePlacements.Contains( alternativeAdPlacement ) )
                shownPlacementId = alternativeAdPlacement;
            else
                shownPlacementId = adPlacementData.PlacementId;
        }

        protected virtual void StartFetching()
        {
            if ( delayFetchCoroutine != null )
            {
                HLog.Log( logPrefix, $"Another Coroutine is running" );
                return;
            }

            delayFetchCoroutine = CoroutineManager.StartCoroutine( FetchWithDelay() );
        }

        protected virtual void Fetch()
        {
            HLog.Log( logPrefix, $"Fetch {adPlacementData.PlacementId} {adPlacementData.AppId}" );
            adStatus = AdStatus.Fetching;
            currentFetchTimes++;
        }

        protected virtual void HandleFetched( AdCallback callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            HLog.Log( logPrefix, $"OnFetched {adPlacementData.PlacementId} Result: {callbackData.Result.ToString()}" );

            if ( callbackData.Result == AdResult.Completed )
            {
                currentFetchTimes = 0;
                adStatus = AdStatus.ReadyToShow;

                OnAdFetch.Dispatch(
                    new AdsManagerFetchCallbackData( GetAdMediationName(), adPlacementData.PlacementId ) );

                for ( int i = 0; i < alternativePlacements.Count; i++ )
                {
                    OnAdFetch.Dispatch(
                        new AdsManagerFetchCallbackData( GetAdMediationName(), alternativePlacements[i] ) );
                }
            }
            else
            {
                StartFetching();
            }
        }

        protected void HandleEnded( AdCallback callbackData )
        {
            if ( callbackData.PlacementId != shownPlacementId && callbackData.PlacementId != adPlacementData.PlacementId)
                return;

            HLog.Log( logPrefix,
                $"HandleEnded {shownPlacementId} Result: {callbackData.Result.ToString()}" );
            AdEnded( callbackData.Result );
        }

        protected void HandleFailToShow()
        {
            AdEnded( AdResult.Failed );
        }

        protected void AdEnded( AdResult adResult )
        {
            adStatus = AdStatus.WaitingForFetch;
            SendAdEvent( adResult, shownPlacementId );
            showResponseCallbacks = null;
            StartFetching();
        }

        protected string GetAdMediationName()
        {
            return adsService.Mediation.MediationId;
        }

        IEnumerator FetchWithDelay()
        {
            adStatus = AdStatus.WaitingForFetch;

            if ( currentFetchTimes <= adsService.ManagerConfig.FetchShortTimes )
            {
                if ( adsService.ManagerConfig.DelayBetweenFetchShort == 0 )
                    yield return null;
                else
                    yield return new WaitForSeconds( Mathf.Max( adsService.ManagerConfig.DelayBetweenFetchShort,
                        MINIMUM_SHORT_FETCH_TIME ) );
            }
            else
            {
                yield return new WaitForSeconds( Mathf.Max( adsService.ManagerConfig.DelayBetweenFetchLong,
                    MINIMUM_LONG_FETCH_TIME ) );
            }

            yield return null;

            delayFetchCoroutine = null;
            Fetch();
        }

        bool CanAdExpire()
        {
            return adsService.Mediation.AdsMediation != AdsMediator.HuuugeAds;
        }
    }
}