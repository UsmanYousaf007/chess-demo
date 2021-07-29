using System;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public class HRewarded
    {
        /// <summary>
        /// Raised after a rewarded ad finishes or gets interrupted.<para />
        /// A reward should be given only when the result of <see cref="IAdCallbackData"/> is <see cref="AdResult.Completed"/>.
        /// </summary>
        [PublicAPI]
        public event Action<IAdCallbackData> OnEnded;

        /// <summary>
        /// Raised when user clicks a rewarded ad.
        /// </summary>
        [PublicAPI]
        public event Action<IAdCallbackData> OnClicked;

        /// <summary>
        /// Raised when a rewarded ad has been loaded, with <see cref="AdResult"/>.
        /// It is triggered as a response to <see cref="Fetch"/> method.
        /// </summary>
        [PublicAPI]
        public event Action<IAdCallbackData> OnFetched;

        readonly IAdsService service;

        internal HRewarded( IAdsService service )
        {
            this.service = service;
        }

        /// <summary>
        /// Registers a rewarded ad provider.<para />
        /// Can be used to add own Provider implementation.
        /// </summary>
        /// <param name="provider">Rewarded ad provider</param>
        [PublicAPI]
        public IAdsService RegisterAdProvider( IRewardedAdProvider provider )
        {
            if ( service.RewardedAdProvider != null )
            {
                service.RewardedAdProvider.OnRewardedEnded -= AdEnded;
                service.RewardedAdProvider.OnRewardedFetched -= AdFetched;
                service.RewardedAdProvider.OnRewardedClicked -= AdClicked;
            }

            service.RewardedAdProvider = provider;
            service.RewardedAdProvider.OnRewardedEnded += AdEnded;
            service.RewardedAdProvider.OnRewardedFetched += AdFetched;
            service.RewardedAdProvider.OnRewardedClicked += AdClicked;
            return service;
        }

        /// <summary>
        /// Shows a rewarded ad.
        /// First Placement with the "Rewarded" type found in the Config will be used.
        /// </summary>
        /// <returns>Whether ad shown operation has started successfully.</returns>
        [PublicAPI]
        public bool TryShow()
        {
            return service.TryShowRewarded();
        }

        /// <summary>
        /// Shows a rewarded ad.
        /// </summary>
        /// <param name="placementId">The placement ID./param>
        /// <returns>Whether ad shown operation has started successfully</returns>
        [PublicAPI]
        public bool TryShow( string placementId )
        {
            return service.TryShowRewarded( placementId );
        }

        /// <summary>
        /// Checks whether a rewarded ad is ready to play.
        /// First Placement with the "Rewarded" type found in the Config will be used
        /// </summary>
        /// <returns>Status of ad</returns>
        [PublicAPI]
        public bool IsReady()
        {
            return service.IsRewardedReady();
        }

        /// <summary>
        /// Checks whether a rewarded ad with a given placement ID is ready to play
        /// </summary>
        /// <param name="placementId">The placement ID.</param>
        /// <returns>Status of ad</returns>
        [PublicAPI]
        public bool IsReady( string placementId )
        {
            return service.IsRewardedReady( placementId );
        }

        /// <summary>
        /// Fetches a rewarded ad.
        /// First Placement with the "Rewarded" type found in the Config will be used.
        /// Although a fetch might not be required by some Ad implementations it is highly recommend.
        /// </summary>
        [PublicAPI]
        public void Fetch()
        {
            service.FetchRewarded();
        }

        /// <summary>
        /// Fetches a rewarded ad with a given placement ID.
        /// Although a fetch might not be required by some Ad implementations it is highly recommend.
        /// </summary>
        /// <param name="placementId">The placement ID.</param>
        [PublicAPI]
        public void Fetch( string placementId )
        {
            service.FetchRewarded( placementId );
        }

        /// <summary>
        /// Gets the current rewarded ad mediator name.
        /// </summary>
        /// <returns>The rewarded ad mediator name</returns>
        [PublicAPI]
        [Obsolete("Use `GetAdMediatorName` instead.")]
        public string GetAdProviderName()
        {
            return service.RewardedAdProvider.ProviderId;
        }

        /// <summary>
        /// Gets the current rewarded ad mediator name.
        /// </summary>
        /// <returns>The rewarded ad mediator name</returns>
        [PublicAPI]
        public string GetAdMediatorName()
        {
            return service?.BannerAdProvider == null ? "UNKNOWN" : service.BannerAdProvider.ProviderId;
        }

        void AdEnded( IAdCallbackData data )
        {
            OnEnded.Dispatch( data );
        }

        void AdFetched( IAdCallbackData data )
        {
            OnFetched.Dispatch( data );
        }

        void AdClicked( IAdCallbackData data )
        {
            OnClicked.Dispatch( data );
        }
    }
}