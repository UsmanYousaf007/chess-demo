using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public class HInterstitial
    {
        /// <summary>
        /// This callback is called after interstitial ad has finished or been interrupted by user.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnEnded;

        /// <summary>
        /// This callback is called when user clicks an interstitial ad.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnClicked;

        /// <summary>
        /// This callback is called when interstitial ad is ready to be displayed.
        /// It will be triggered as a response to <see cref="Fetch"/> method.
        /// If fetching was successful, then callback is called with AdResult.Completed, if failed - AdResult.Failed
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnFetched;

        readonly IAdsService service;

        internal HInterstitial(IAdsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Use this to register your interstitial ad provider. <para />
        /// Could be used to add own Provider implementation.
        /// </summary>
        /// <param name="provider">Interstitial ad provider</param>
        [PublicAPI]
        public IAdsService RegisterAdProvider(IInterstitialAdProvider provider)
        {
            if (service.InterstitialAdProvider != null)
            {
                service.InterstitialAdProvider.OnInterstitialEnded -= AdEnded;
                service.InterstitialAdProvider.OnInterstitialFetched -= AdFetched;
                service.InterstitialAdProvider.OnInterstitialClicked -= AdClicked;
            }
            
            service.InterstitialAdProvider = provider;
            service.InterstitialAdProvider.OnInterstitialEnded += AdEnded;
            service.InterstitialAdProvider.OnInterstitialFetched += AdFetched;
            service.InterstitialAdProvider.OnInterstitialClicked += AdClicked;

            return service;
        }

        /// <summary>
        /// Use this to show interstitial ad.
        /// First Placement with the "Interstitial" type found in the Config will be used
        /// </summary>
        /// <returns>Whether ad shown operation has started successfully or not</returns>
        [PublicAPI]
        public bool TryShow()
        {
            return service.TryShowInterstitial();
        }
        
        /// <summary>
        /// Use this to show interstitial ad with given placement id
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        /// <returns>Whether ad shown operation has started successfully or not</returns>
        [PublicAPI]
        public bool TryShow(string placementId)
        {
            return service.TryShowInterstitial(placementId);
        }

        /// <summary>
        /// Use this to check whether your interstitial ad is ready to play or not.
        /// First Placement with the "Interstitial" type found in the Config will be used
        /// </summary>
        /// <returns>Status of ad</returns>
        [PublicAPI]
        public bool IsReady()
        {
            return service.IsInterstitialReady();
        }
        
        /// <summary>
        /// Use this to check whether the interstitial ad with given placement id is ready to play or not
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        /// <returns>Status of ad</returns>
        [PublicAPI]
        public bool IsReady(string placementId)
        {
            return service.IsInterstitialReady(placementId);
        }

        /// <summary>
        /// Use this to fetch interstitial ad.
        /// First Placement with the "Interstitial" type found in the Config will be used
        /// Although fetch might not be required by some Ads implementations we highly recommend to do so
        /// </summary>
        [PublicAPI]
        public void Fetch()
        {
            service.FetchInterstitial();
        }
        
        /// <summary>
        /// Use this to fetch interstitial ad with given placement id
        /// Although fetch might not be required by some Ads implementations we highly recommend to do so
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        [PublicAPI]
        public void Fetch(string placementId)
        {
            service.FetchInterstitial(placementId);
        }

        /// <summary>
        /// Call this function to get current Interstitial ad provider name.
        /// </summary>
        /// <returns>Interstitial ad provider name</returns>
        [PublicAPI]
        public string GetAdProviderName()
        {
            return service.InterstitialAdProvider.ProviderId;
        }

        void AdEnded(IAdCallbackData data)
        {
            OnEnded.Dispatch(data);
        }

        void AdFetched(IAdCallbackData data)
        {
            OnFetched.Dispatch(data);
        }

        void AdClicked(IAdCallbackData data)
        {
            OnClicked.Dispatch(data);
        }
    }
}