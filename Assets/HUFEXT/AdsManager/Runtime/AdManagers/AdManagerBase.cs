using System.Collections;
using System.Collections.Generic;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.AdsManager.Runtime.API;
using HUFEXT.AdsManager.Runtime.Config;
using HUFEXT.AdsManager.Runtime.Service;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public abstract class AdManagerBase
    {
        const float MINIMUM_LONG_FETCH_TIME = 5f;
        const float MINIMUM_SHORT_FETCH_TIME = 1f;

        protected enum AdStatus
        {
            WaitingForFetch,
            Fetching,
            ReadyToShow,
            Showing
        }

        protected AdsManagerConfig adsManagerConfig;
        protected AdPlacementData adPlacementData;
        protected HUFAdsService adsService;
        protected AdStatus adStatus = AdStatus.WaitingForFetch;
        protected int currentFetchTimes = 0;
        protected List<string> alternativePlacements = new List<string>();

        protected UnityAction<AdManagerCallback> showResponseCallbacks;

        protected string shownPlacementId;

        protected HLogPrefix logPrefix = new HLogPrefix( HAdsManager.logPrefix, nameof(AdManagerBase) );

        Coroutine delayFetchCoroutine = null;

        public AdManagerBase( AdPlacementData inAdPlacementData,
            AdsManagerConfig inAdsManagerConfig,
            HUFAdsService inAdsService )
        {
            adPlacementData = inAdPlacementData;
            adsManagerConfig = inAdsManagerConfig;
            adsService = inAdsService;
            RegisterEvents();
            StartFetching();
        }

        public void AddAlternativePlacement( string inPlacement )
        {
            alternativePlacements.Add( inPlacement );

            if ( adStatus == AdStatus.ReadyToShow )
            {
                adsService.OnAdFetch.Dispatch( new AdsManagerFetchCallbackData( GetAdMediationName(), inPlacement ) );
            }
        }

        public bool ContainsPlacement( string inPlacement )
        {
            return alternativePlacements.Contains( inPlacement );
        }

        public bool IsReady()
        {
            if ( adStatus == AdStatus.ReadyToShow && NativeIsReady() == false )
            {
                HLog.Log( logPrefix, $"Ad did expired - fetch new one {adPlacementData.PlacementId}" );
                StartFetching();
            }

            return adStatus == AdStatus.ReadyToShow;
        }

        protected abstract bool NativeIsReady();

        public virtual void ShowAd( UnityAction<AdManagerCallback> resultCallback, string alternativeAdPlacement )
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

        protected virtual void RegisterEvents() { }

        IEnumerator FetchWithDelay()
        {
            adStatus = AdStatus.WaitingForFetch;

            if ( currentFetchTimes <= adsManagerConfig.FetchShortTimes )
            {
                if ( adsManagerConfig.DelayBetweenFetchShort == 0 )
                    yield return null;
                else
                    yield return new WaitForSeconds( Mathf.Max( adsManagerConfig.DelayBetweenFetchShort,
                        MINIMUM_SHORT_FETCH_TIME ) );
            }
            else
            {
                yield return new WaitForSeconds( Mathf.Max( adsManagerConfig.DelayBetweenFetchLong,
                    MINIMUM_LONG_FETCH_TIME ) );
            }

            yield return null;

            delayFetchCoroutine = null;
            Fetch();
        }

        protected virtual void Fetch()
        {
            HLog.Log( logPrefix, $"Fetch {adPlacementData.PlacementId} {adPlacementData.AppId}" );
            adStatus = AdStatus.Fetching;
            currentFetchTimes++;
        }

        protected virtual void OnFetched( IAdCallbackData callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            HLog.Log( logPrefix, $"OnFetched {adPlacementData.PlacementId} Result: {callbackData.Result.ToString()}" );

            if ( callbackData.Result == AdResult.Completed )
            {
                currentFetchTimes = 0;
                adStatus = AdStatus.ReadyToShow;

                adsService.OnAdFetch.Dispatch(
                    new AdsManagerFetchCallbackData( GetAdMediationName(), adPlacementData.PlacementId ) );

                for ( int i = 0; i < alternativePlacements.Count; i++ )
                {
                    adsService.OnAdFetch.Dispatch(
                        new AdsManagerFetchCallbackData( GetAdMediationName(), alternativePlacements[i] ) );
                }
            }
            else
            {
                StartFetching();
            }
        }

        protected void OnEnded( IAdCallbackData callbackData )
        {
            if ( callbackData.PlacementId != adPlacementData.PlacementId )
                return;

            HLog.Log( logPrefix, $"ad show {adPlacementData.PlacementId} Result: {callbackData.Result.ToString()}" );
            AdEnded( callbackData.Result );
        }

        protected void OnFailToShow()
        {
            AdEnded( AdResult.Failed );
        }

        void AdEnded( AdResult adResult )
        {
            adStatus = AdStatus.WaitingForFetch;
            SendAdEvent( adResult, shownPlacementId );
            showResponseCallbacks = null;
            StartFetching();
        }

        protected virtual void SendAdEvent( AdResult result, string placementId ) { }

        protected abstract string GetAdMediationName();

        public void Destroy()
        {
            if ( delayFetchCoroutine != null )
                CoroutineManager.StopCoroutine( delayFetchCoroutine );
            DisconnectFromAds();
        }

        protected abstract void DisconnectFromAds();
    }
}