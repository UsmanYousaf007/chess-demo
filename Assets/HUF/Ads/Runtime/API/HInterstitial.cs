using System;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public class HInterstitial
    {
        /// <summary>
        /// Raised after an interstitial ad finishes or gets interrupted.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnEnded;

        /// <summary>
        /// Raised when user clicks an interstitial ad.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnClicked;

        /// <summary>
        /// Raised when an interstitial ad is ready to be displayed, with <see cref="AdResult"/>.
        /// It is triggered as a response to <see cref="Fetch"/> method.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnFetched;

        readonly IAdsService service;

        internal HInterstitial(IAdsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Registers an interstitial ad provider.<para />
        /// Can be used to add own Provider implementation.
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
        /// Shows an interstitial ad.
        /// First Placement with the "Interstitial" type found in the Config will be used.
        /// </summary>
        /// <returns>Whether ad shown operation has started successfully.</returns>
        [PublicAPI]
        public bool TryShow()
        {
            return service.TryShowInterstitial();
        }
        
        /// <summary>
        /// Shows an interstitial ad with a given placement ID.
        /// </summary>
        /// <param name="placementId">The placement ID.</param>
        /// <returns>Whether ad shown operation has started successfully.</returns>
        [PublicAPI]
        public bool TryShow(string placementId)
        {
            return service.TryShowInterstitial(placementId);
        }

        /// <summary>
        /// Checks whether an interstitial ad is ready to play.
        /// First Placement with the "Interstitial" type found in the Config will be used.
        /// </summary>
        /// <returns>Status of an ad.</returns>
        [PublicAPI]
        public bool IsReady()
        {
            return service.IsInterstitialReady();
        }
        
        /// <summary>
        /// Checks whether the interstitial ad with a given placement ID is ready to play.
        /// </summary>
        /// <param name="placementId">The placement ID.</param>
        /// <returns>Status of ad.</returns>
        [PublicAPI]
        public bool IsReady(string placementId)
        {
            return service.IsInterstitialReady(placementId);
        }

        /// <summary>
        /// Fetches an interstitial ad.
        /// First Placement with the "Interstitial" type found in the Config will be used.
        /// Although a fetch might not be required by some Ad implementations it is highly recommend.
        /// </summary>
        [PublicAPI]
        public void Fetch()
        {
            service.FetchInterstitial();
        }
        
        /// <summary>
        /// Fetches an interstitial ad with a given placement ID.
        /// Although a fetch might not be required by some Ad implementations it is highly recommend.
        /// </summary>
        /// <param name="placementId">The placement ID.</param>
        [PublicAPI]
        public void Fetch(string placementId)
        {
            service.FetchInterstitial(placementId);
        }

        /// <summary>
        /// Gets the current interstitial ad mediator name.
        /// </summary>
        /// <returns>The interstitial ad mediator name</returns>
        [PublicAPI]
        [Obsolete("Use `GetAdMediatorName` instead.")]
        public string GetAdProviderName()
        {
            return service.InterstitialAdProvider.ProviderId;
        }

        /// <summary>
        /// Gets the current interstitial ad mediator name.
        /// </summary>
        /// <returns>The interstitial ad mediator name</returns>
        [PublicAPI]
        public string GetAdMediatorName()
        {
            return service?.BannerAdProvider == null ? "UNKNOWN" : service.BannerAdProvider.ProviderId;
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