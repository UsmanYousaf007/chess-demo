using System;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.API
{
    public class HBanner
    {
        /// <summary>
        /// Raised immediately after a banner is shown on the screen.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnShown;

        /// <summary>
        /// Raised when a banner fails to load and show.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnFailed;

        /// <summary>
        /// Raised after a user clicks on a banner.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnClicked;
        
        /// <summary>
        /// Raised immediately after a banner is hidden from the screen.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnHidden;

        readonly IAdsService service;

        internal HBanner(IAdsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Registers a banner ad provider. <para />
        /// Can be used to add own Provider implementation.
        /// </summary>
        /// <param name="provider">Banner ad provider</param>
        [PublicAPI]
        public IAdsService RegisterAdProvider(IBannerAdProvider provider)
        {
            if (service.BannerAdProvider != null)
            {
                service.BannerAdProvider.OnBannerShown -= AdShown;
                service.BannerAdProvider.OnBannerFailed -= AdFailed;
                service.BannerAdProvider.OnBannerClicked -= AdClicked;
                service.BannerAdProvider.OnBannerHidden -= AdHidden;
            }
            service.BannerAdProvider = provider;
            service.BannerAdProvider.OnBannerShown += AdShown;
            service.BannerAdProvider.OnBannerFailed += AdFailed;
            service.BannerAdProvider.OnBannerClicked += AdClicked;
            service.BannerAdProvider.OnBannerHidden += AdHidden;

            return service;
        }

        /// <summary>
        /// Shows a banner ad.
        /// First Placement with the "Banner" type found in the Config will be used.
        /// </summary>
        /// <param name="position">An optional parameter with an on-screen position where the banner should be displayed. <para />
        /// By default it is set to BannerPosition.BottomCenter.</param>
        /// <returns>Returns TRUE if a show operation is successful. <para />
        /// This does not indicate that the banner is shown.
        /// Subscribe to <see cref="OnShown"/> event to confirm presence of the banner.</returns>
        [PublicAPI]
        public bool Show(BannerPosition position = BannerPosition.BottomCenter)
        {
            return service.ShowBanner(position);
        }
        
        /// <summary>
        /// Shows ad banner with a given placement ID.
        /// </summary>
        /// <param name="placementId">The placement ID</param>
        /// <param name="position">An optional parameter with an on-screen position where the banner should be displayed. <para />
        /// By default it is set to BannerPosition.BottomCenter.</param>
        /// <returns>Returns TRUE if a show operation is successful. <para />
        /// This does not indicate that the banner is shown.
        /// Subscribe to <see cref="OnShown"/> event to confirm presence of the banner.</returns>
        [PublicAPI]
        public bool Show(string placementId, BannerPosition position = BannerPosition.BottomCenter)
        {
            return service.ShowBanner(placementId, position);
        }

        /// <summary>
        /// Hides the banner ad.
        /// Subscribe to <see cref="OnHidden"/> event to confirm presence of the banner.
        /// </summary>
        [PublicAPI]
        public void Hide()
        {
            service.HideBanner();
        }

        /// <summary>
        /// Gets a current banner ad Mediator name.
        /// </summary>
        /// <returns>Banner ad provider name</returns>
        [PublicAPI]
        [Obsolete("Use `GetAdMediatorName` instead.")]
        public string GetAdProviderName()
        {
            return GetAdMediatorName();
        }

        /// <summary>
        /// Gets a current banner ad Mediator name.
        /// </summary>
        /// <returns>Banner ad provider name</returns>
        [PublicAPI]
        public string GetAdMediatorName()
        {
            return service?.BannerAdProvider == null ? "UNKNOWN" : service.BannerAdProvider.ProviderId;
        }

        void AdShown(IBannerCallbackData data)
        {
            OnShown.Dispatch(data);
        }

        void AdFailed(IBannerCallbackData data)
        {
            OnFailed.Dispatch(data);
        }

        void AdClicked(IBannerCallbackData data)
        {
            OnClicked.Dispatch(data);
        }
        
        void AdHidden(IBannerCallbackData data)
        {
            OnHidden.Dispatch(data);
        }
    }
}