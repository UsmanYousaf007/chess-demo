using System;
using HUF.Ads.Implementation;
using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.API
{
    public static class HAds
    {
        static HBanner banner;
        static HInterstitial interstitial;
        static HRewarded rewarded;
        static IAdsService service;

        public static event UnityAction OnAdsServiceInitialized; 
        
        /// <summary>
        /// Provides access to Banner ads API - show ad, subscribe for events etc
        /// </summary>
        [PublicAPI] public static HBanner Banner => banner ?? (banner = new HBanner(Service));
        
        /// <summary>
        /// Provides access to Interstitial ads API - show ad, subscribe for events etc
        /// </summary>
        [PublicAPI] public static HInterstitial Interstitial => interstitial ?? (interstitial = new HInterstitial(Service));
        
        /// <summary>
        /// Provides access to Rewarded ads API - show ad, subscribe for events etc
        /// </summary>
        [PublicAPI] public static HRewarded Rewarded => rewarded ?? (rewarded = new HRewarded(Service));
        
        static IAdsService Service 
        {
            get
            {
                if (service == null)
                {
                    service = new AdsService();
                    service.RegisterToInitializationEvent(AdsServiceInitialized);
                }

                return service;
            }
        }

        static void AdsServiceInitialized()
        {
            OnAdsServiceInitialized.Dispatch();
        }
        
        /// <summary>
        /// Sets consent for gathering user data during ad display.
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        [PublicAPI]
        public static void CollectSensitiveData(bool consentStatus)
        {
            Service.CollectSensitiveData(consentStatus);
        }

        [PublicAPI]
        [Obsolete("Use `CollectSensitiveData` instead.")]
        public static void SetConsent(bool consentStatus)
        {
            CollectSensitiveData(consentStatus);
        }

        [PublicAPI]
        public static bool IsAdsServiceInitialized()
        {
            return Service.IsServiceInitialized;
        }
    }
}