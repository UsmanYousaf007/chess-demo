using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public class HRewarded
    {
        /// <summary>
        /// This callback is called after rewarded ad has finished or been interrupted by user. <para />
        /// Reward should be given only when result of IAdCallbackData is set as AdResult.Completed
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnEnded;

        /// <summary>
        /// This callback is called when user clicks an rewarded ad.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnClicked;

        /// <summary>
        /// This callback is called when rewarded ad is ready to be displayed.
        /// It will be triggered as a response to <see cref="Fetch"/> method.
        /// If fetching was successful, then callback is called with AdResult.Completed, if failed - AdResult.Failed 
        /// </summary>
        [PublicAPI]
        public event UnityAction<IAdCallbackData> OnFetched;

        readonly IAdsService service;

        internal HRewarded( IAdsService service )
        {
            this.service = service;
        }

        /// <summary>
        /// Use this to register your rewarded ad provider. <para />
        /// Could be used to add own Provider implementation.
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
        /// Use this to show rewarded ad.
        /// First Placement with the "Rewarded" type found in the Config will be used
        /// </summary>
        /// <returns>Whether ad shown operation has started successfully or not</returns>
        [PublicAPI]
        public bool TryShow()
        {
            return service.TryShowRewarded();
        }

        /// <summary>
        /// Use this to show rewarded ad with given placement id
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        /// <returns>Whether ad shown operation has started successfully or not</returns>
        [PublicAPI]
        public bool TryShow( string placementId )
        {
            return service.TryShowRewarded( placementId );
        }

        /// <summary>
        /// Use this to check whether rewarded ad is ready to play or not.
        /// First Placement with the "Rewarded" type found in the Config will be used
        /// </summary>
        /// <returns>Status of ad</returns>
        [PublicAPI]
        public bool IsReady()
        {
            return service.IsRewardedReady();
        }

        /// <summary>
        /// Use this to check whether a rewarded ad with given placement id is ready to play or not.
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        /// <returns>Status of ad</returns>
        [PublicAPI]
        public bool IsReady( string placementId )
        {
            return service.IsRewardedReady( placementId );
        }

        /// <summary>
        /// Use this to fetch rewarded ad.
        /// First Placement with the "Rewarded" type found in the Config will be used
        /// Although fetch might not be required by some Ads implementations we highly recommend to do so
        /// </summary>
        [PublicAPI]
        public void Fetch()
        {
            service.FetchRewarded();
        }

        /// <summary>
        /// Use this to fetch rewarded ad with given placement id
        /// Although fetch might not be required by some Ads implementations we highly recommend to do so
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        [PublicAPI]
        public void Fetch( string placementId )
        {
            service.FetchRewarded( placementId );
        }

        /// <summary>
        /// Call this function to get current Rewarded ad provider name.
        /// </summary>
        /// <returns>Rewarded ad provider name</returns>
        [PublicAPI]
        public string GetAdProviderName()
        {
            return service.RewardedAdProvider.ProviderId;
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