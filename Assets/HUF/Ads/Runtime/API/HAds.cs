using System;
using HUF.Ads.Runtime.Implementation;
using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using JetBrains.Annotations;
using UnityEngine;

#if HUF_POLICY_GUARD
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Implementations;
#endif

namespace HUF.Ads.Runtime.API
{
    public static class HAds
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAds) );
        
        const string ADS_CONSENT_SENSITIVE_DATA = "HUFAdsConsentSensitiveDataV2";
        static HBanner banner;
        static HInterstitial interstitial;
        static HRewarded rewarded;
        static IAdsService service;
        static bool isTrackingATT;

        static event Action<bool> OnCollectSensitiveDataSetEvent;


        /// <summary>
        /// Occurs when called CollectSensitiveData or SetConsent
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnCollectSensitiveDataSet
        {
            add => OnCollectSensitiveDataSetEvent += value;
            remove => OnCollectSensitiveDataSetEvent -= value;
        }

        /// <summary>
        /// Raised when the ads service completes initialization.
        /// </summary>
        [PublicAPI]
        public static event Action OnAdsServiceInitialized;

        /// <summary>
        /// Raised when the personalized ads consent changes state.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnPersonalizedAdsConsentChanged;

        /// <summary>
        /// Provides access to Banner ads API - show ad, subscribe for events, etc.
        /// </summary>
        [PublicAPI] public static HBanner Banner => banner ?? (banner = new HBanner(Service));

        /// <summary>
        /// Provides access to Interstitial ads API - show ad, subscribe for events, etc.
        /// </summary>
        [PublicAPI] public static HInterstitial Interstitial => interstitial ?? (interstitial = new HInterstitial(Service));

        /// <summary>
        /// Provides access to Rewarded ads API - show ad, subscribe for events, etc.
        /// </summary>
        [PublicAPI] public static HRewarded Rewarded => rewarded ?? (rewarded = new HRewarded(Service));

        static IAdsService Service
        {
            get
            {
                if ( service != null )
                    return service;

                service = new AdsService();
                service.RegisterToInitializationEvent(AdsServiceInitialized);

                return service;
            }
        }

        /// <summary>
        /// Sets consent for gathering user data during ad display. On iOS ATT needs to be Authorized.
        /// </summary>
        /// <param name="consentStatus">Status of consent</param>
        [PublicAPI]
        public static void CollectSensitiveData(bool consentStatus)
        {
#if UNITY_IOS
            if ( !HPolicyGuard.IsATTAuthorized() )
            {
                if ( consentStatus == false )
                {
                    HLog.LogWarning( logPrefix, "ATT is not Authorized. Ads consent cannot be set" );
                    return;
                }

                HPolicyGuard.OpenATTNativeSettings();
#if UNITY_EDITOR
                HPolicyGuard.CheckATTStatus( status =>
                {
                    if ( status == AppTrackingTransparencyBridge.AuthorizationStatus.Authorized )
                        CollectSensitiveData( true );
                    else
                    {
                        HLog.LogWarning( logPrefix, "ATT is not Authorized. Ads consent cannot be set" );
                    }
                });
#endif
                return;
            }
#endif

            bool? previousValue = HasConsent();
            HPlayerPrefs.SetBool(ADS_CONSENT_SENSITIVE_DATA, consentStatus);

            RefreshPersonalizedAds();

            if(!previousValue.HasValue || previousValue.Value != consentStatus)
            {
                OnCollectSensitiveDataSetEvent.Dispatch(consentStatus);
                OnPersonalizedAdsConsentChanged.Dispatch( HasPersonalizedAdConsent() == true);
            }
        }

        /// <summary>
        /// Returns consent set for ads mediation to show (or not) personalized ads. On iOS ATT needs to be Authorized.
        /// <returns>If no consent is set returns null. Returns consent value otherwise. </returns>
        /// </summary>
        [PublicAPI]
        public static bool? HasPersonalizedAdConsent()
        {
            if (!HPlayerPrefs.HasKey(ADS_CONSENT_SENSITIVE_DATA))
            {
                return null;
            }

            return HPlayerPrefs.GetBool(ADS_CONSENT_SENSITIVE_DATA) && HPolicyGuard.IsATTAuthorized();
        }

        /// <summary>
        /// Returns consent set for ads mediation to show (or not) personalized ads.
        /// <returns>If no consent is set returns null. Returns consent value otherwise. </returns>
        /// </summary>
        [PublicAPI]
        public static bool? HasConsent()
        {
            if (!HPlayerPrefs.HasKey(ADS_CONSENT_SENSITIVE_DATA))
            {
                return null;
            }

            return HPlayerPrefs.GetBool(ADS_CONSENT_SENSITIVE_DATA);
        }

        /// <summary>
        /// Checks the service initialization status
        /// </summary>
        /// <returns>Service initialization status</returns>
        [PublicAPI]
        public static bool IsAdsServiceInitialized()
        {
            return Service.IsServiceInitialized;
        }

        /// <summary>
        /// Refreshes personalized ads consent. Consent differs between platforms.
        /// For example on iOS App Tracking Transparency pop-up needs to be authorized with ads consent.
        /// </summary>
        [PublicAPI]
        public static void RefreshPersonalizedAds()
        {
            Service.CollectSensitiveData(HasPersonalizedAdConsent() == true);
        }

        /// <summary>
        /// Checks if ads consent can be changed. On iOS is related to ATT Authorization status.
        /// </summary>
        /// <returns>Provides information about the possibility to change ads consent.</returns>
        [PublicAPI]
        public static bool CanChangeAdsConsent()
        {
            return HPolicyGuard.IsATTAuthorized();
        }

        static void AdsServiceInitialized()
        {
            OnAdsServiceInitialized.Dispatch();
            RefreshPersonalizedAds();
        }

#if UNITY_IOS
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
        static void StartCheckingATTConsent()
        {
            if ( isTrackingATT )
                return;

            HPolicyGuard.OnATTStatusChanged += status =>
            {
                OnPersonalizedAdsConsentChanged.Dispatch( HasPersonalizedAdConsent() == true );
                RefreshPersonalizedAds();
            };
            isTrackingATT = true;
        }
#endif
    }
}