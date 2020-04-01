using System;
using HUF.Ads.Implementation;
using HUF.Utils.Extensions;
using HUF.Utils.PlayerPrefs;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Ads.API
{
    public static class HAds
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAds) );
        
        const string ADS_CONSENT_SENSITIVE_DATA = "HUFAdsConsentSensitiveData";
        static HBanner banner;
        static HInterstitial interstitial;
        static HRewarded rewarded;
        static IAdsService service;

        static event UnityAction<bool> OnCollectSensitiveDataSetEvent;

        /// <summary>
        /// Occurs when called CollectSensitiveData or SetConsent
        /// </summary>
        [PublicAPI]
        public static event UnityAction<bool> OnCollectSensitiveDataSet
        {
            add => OnCollectSensitiveDataSetEvent += value;
            remove => OnCollectSensitiveDataSetEvent -= value;
        }

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
            bool? consent = GetGDPRConsent();

            if (consent.HasValue)
            {
                CollectSensitiveData(consent.Value);
            }
        }

        /// <summary>
        /// Sets consent for gathering user data during ad display.
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        [PublicAPI]
        public static void CollectSensitiveData(bool consentStatus)
        {
            bool? previousValue = GetGDPRConsent();
            HPlayerPrefs.SetBool(ADS_CONSENT_SENSITIVE_DATA, consentStatus);
            Service.CollectSensitiveData(consentStatus);

            if(!previousValue.HasValue || previousValue.Value != consentStatus)
            {
                OnCollectSensitiveDataSetEvent?.Invoke(consentStatus);
            }
        }

        /// <summary>
        /// Returns consent for gathering user data during ad display.
        /// <returns>If no consent is set returns null. Returns consent value otherwise </returns>
        /// </summary>
        [PublicAPI]
        public static bool? GetGDPRConsent()
        {
            if (!HPlayerPrefs.HasKey(ADS_CONSENT_SENSITIVE_DATA))
            {
                return null;
            }

            return HPlayerPrefs.GetBool(ADS_CONSENT_SENSITIVE_DATA);
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