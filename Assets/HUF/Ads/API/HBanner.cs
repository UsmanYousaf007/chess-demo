using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.API
{
    public class HBanner
    {
        /// <summary>
        /// This callback is called immediately after banner is shown on the screen.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnShown;

        /// <summary>
        /// This callback is called when banner failed to load and show.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnFailed;

        /// <summary>
        /// This callback is called after user clicks on a banner.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnClicked;
        
        /// <summary>
        /// This callback is called immediately after banner is hide from screen.
        /// </summary>
        [PublicAPI]
        public event UnityAction<IBannerCallbackData> OnHidden;

        readonly IAdsService service;

        internal HBanner(IAdsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Use this to register your banner ad provider. <para />
        /// Could be used to add own Provider implementation.
        /// </summary>
        /// <param name="provider">Banner ad provider</param>
        [PublicAPI]
        public void RegisterAdProvider(IBannerAdProvider provider)
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
        }

        /// <summary>
        /// Use this to show ad banner.
        /// First Placement with the "Banner" type found in the Config will be used
        /// </summary>
        /// <param name="position">Optional parameter with position on screen where banner will be displayed. <para />
        /// By default is set as BannerPosition.BottomCenter.</param>
        /// <returns>Return TRUE if show operation is finished with success. <para />
        /// This does not indicate that banner is shown. <para />
        /// Use <see cref="OnShown"/> to indicate presence of the banner.</returns>
        [PublicAPI]
        public bool Show(BannerPosition position = BannerPosition.BottomCenter)
        {
            return service.ShowBanner(position);
        }
        
        /// <summary>
        /// Use this to show ad banner with given placement id
        /// </summary>
        /// <param name="placementId">Your placement id</param>
        /// <param name="position">Optional parameter with position on screen where banner will be displayed. <para />
        /// By default is set as BannerPosition.BottomCenter.</param>
        /// <returns>Return TRUE if show operation is finished with success. <para />
        /// This does not indicate that banner is shown. <para />
        /// Use <see cref="OnShown"/> to indicate presence of the banner.</returns>
        [PublicAPI]
        public bool Show(string placementId, BannerPosition position = BannerPosition.BottomCenter)
        {
            return service.ShowBanner(placementId, position);
        }

        /// <summary>
        /// Use this to hide ad banner
        /// Use <see cref="OnHidden"/> to indicate that banner is hidden.
        /// </summary>
        [PublicAPI]
        public void Hide()
        {
            service.HideBanner();
        }

        /// <summary>
        /// Call this function to get current banner ad provider name.
        /// </summary>
        /// <returns>Banner ad provider name</returns>
        [PublicAPI]
        public string GetAdProviderName()
        {
            return service.BannerAdProvider.ProviderId;
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