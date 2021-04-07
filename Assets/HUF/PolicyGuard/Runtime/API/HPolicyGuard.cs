using System;
using HUF.Ads.Runtime.API;
using HUF.Analytics.Runtime.API;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.PolicyGuard.Runtime.Implementations;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.PolicyGuard.Runtime.API
{
    public static class HPolicyGuard
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HPolicyGuard) );

        static PolicyGuardConfig config;
        static PolicyGuardService service;

#pragma warning disable 0067
        /// <summary>
        /// Raised when the GDPR window appears.
        /// </summary>
        [PublicAPI]
        public static event Action OnGDPRPopupShowed;

        /// <summary>
        /// Raised when the user closes the GDPR pop-up with a result.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnGDPRPopupClosed;

        /// <summary>
        /// Raised when the Personalized Ads pop-up appears.
        /// </summary>
        [PublicAPI]
        public static event Action OnPersonalizedAdsPopupShowed;

        /// <summary>
        /// Raised when the user closes the Personalized Ads pop-up with a result.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnPersonalizedAdsPopupClosed;

        /// <summary>
        /// Raised when the in game App Tracking Transparency pop-up appears.
        /// </summary>
        [PublicAPI]
        public static event Action OnATTPopupShowed;

        /// <summary>
        /// Raised when the user closes the in game App Tracking Transparency pop-up with a result.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnATTPopupClosed;

        /// <summary>
        /// Raised when the Native App Tracking Transparency pop-up appears.
        /// </summary>
        [PublicAPI]
        public static event Action OnATTNativePopupShowed;

        /// <summary>
        /// Raised when the user closes the native App tracking transparency pop-up with a result.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnATTNativePopupClosed;

        /// <summary>
        /// Raised when the analytics consent changes.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnAnalyticsConsentChanged;

        /// <summary>
        /// Raised when the ads consent changes.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnAdsConsentChanged;

        /// <summary>
        /// Raised when all policy pop-ups have been shown.
        /// </summary>
        [PublicAPI]
        public static event Action OnEndCheckingPolicy;

#if UNITY_IOS
        /// <summary>
        /// Raised when the ATT consent changes.
        /// </summary>
        [PublicAPI]
        public static event Action<AppTrackingTransparencyBridge.AuthorizationStatus> OnATTConsentChanged;
#endif

        /// <summary>
        /// Raised when the personalized ads consent changes.
        /// </summary>
        [PublicAPI]
        public static event Action<bool> OnPersonalizedAdsConsentChanged;

        /// <summary>
        /// Checks if the service is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => service != null;

#pragma warning restore 0067

        /// <summary>
        /// Initialize Policy Guard Service and start checking policies if needed.
        /// Should be called only if an AutoInit option is disabled.
        /// </summary>
        [PublicAPI]
        public static void Initialize()
        {
            if ( service != null || GetConfig() == null )
                return;

            service = new PolicyGuardService();
            service.OnGDPRPopupShowed += () => { OnGDPRPopupShowed.Dispatch(); };
            service.OnGDPRPopupCloses += status => { OnGDPRPopupClosed.Dispatch( status ); };
            service.OnPersonalizedAdsPopupShowed += () => { OnPersonalizedAdsPopupShowed.Dispatch(); };
            service.OnPersonalizedAdsPopupCloses += status => { OnPersonalizedAdsPopupClosed.Dispatch( status ); };
            service.OnATTPopupShowed += () => { OnATTPopupShowed.Dispatch(); };
            service.OnATTPopupCloses += status => { OnATTPopupClosed.Dispatch( status ); };
            service.OnATTNativePopupShowed += () => { OnATTNativePopupShowed.Dispatch(); };
            service.OnEndCheckingPolicy += () => { OnEndCheckingPolicy.Dispatch(); };
            HAnalytics.OnCollectSensitiveDataSet += status => { OnAnalyticsConsentChanged.Dispatch( status ); };
            HAds.OnCollectSensitiveDataSet += status => { OnAdsConsentChanged.Dispatch( status ); };
#if UNITY_IOS
            service.OnATTNativePopupClosed += status =>
            {
                OnATTNativePopupClosed.Dispatch( status == AppTrackingTransparencyBridge.AuthorizationStatus.Authorized );
            };

            AppTrackingTransparencyBridge.OnAuthorizationStatusChanged += status =>
            {
                OnATTConsentChanged.Dispatch( status );
            };
#endif
            HAds.OnPersonalizedAdsConsentChanged += status => { OnPersonalizedAdsConsentChanged.Dispatch( status ); };
            service.CheckFlow();
        }

        /// <summary>
        /// Shows an ATT pop-up if it was never shown.
        /// The Flow is dependent on the config setting and can show the Pre-opt in pop-up before the native one.
        /// Should be called only if <see cref="PolicyGuardConfig.useAutomatedFlow"/> option is disabled.
        /// </summary>
        [PublicAPI]
        public static bool TryShowATT()
        {
            if ( service == null )
            {
                HLog.LogError( logPrefix, "Service is not initialized" );
                return false;
            }

            return service.TryShowATT();
        }

        /// <summary>
        /// Shows a GDPR pop-up with personalized ads if it was never shown.
        /// In case of the ATT not displayed or not accepted, the GDPR pop-up without the ads consent will be shown.
        /// Should be called only if <see cref="PolicyGuardConfig.useAutomatedFlow"/> option is disabled.
        /// </summary>
        [PublicAPI]
        public static bool TryShowGDPRWithAds()
        {
            if ( service == null )
            {
                HLog.LogError( logPrefix, "Service is not initialized" );
                return false;
            }

            return service.TryShowGDPRWithAds();
        }

        /// <summary>
        /// Shows a GDPR pop-up if it was never shown.
        /// Should be called only if <see cref="PolicyGuardConfig.useAutomatedFlow"/> option is disabled.
        /// </summary>
        [PublicAPI]
        public static bool TryShowGDPR()
        {
            if ( service == null )
            {
                HLog.LogError( logPrefix, "Service is not initialized" );
                return false;
            }

            return service.TryShowGDPR();
        }

        /// <summary>
        /// Shows a Personalized Ads pop-up. On iOS it is required to have the ATT authorized.
        /// </summary>
        [PublicAPI]
        public static bool TryShowPersonalizedAdsPopup()
        {
            if ( service == null )
            {
                HLog.LogError( logPrefix, "Service is not initialized" );
                return false;
            }

            return service.TryShowPersonalizedAdsPopup();
        }

#if UNITY_IOS
        /// <summary>
        /// Raised when an AppTrackingTransparency authorization status is changed.
        /// <para>See <see cref="AppTrackingTransparencyBridge.AuthorizationStatus"/> for list of statuses.</para>
        /// </summary>
        [PublicAPI]
        public static event Action<AppTrackingTransparencyBridge.AuthorizationStatus> OnATTStatusChanged
        {
            add => AppTrackingTransparencyBridge.OnAuthorizationStatusChanged += value;
            remove => AppTrackingTransparencyBridge.OnAuthorizationStatusChanged -= value;
        }

        /// <summary>
        /// Checks AppTrackingTransparency status. When checked for the first time causes a system pop-up to appear.
        /// </summary>
        /// <param name="callback">A nullable callback with an authorization status.</param>
        [PublicAPI]
        public static void CheckATTStatus( Action<AppTrackingTransparencyBridge.AuthorizationStatus> callback )
        {
            AppTrackingTransparencyBridge.CheckAuthorizationStatus( callback );
        }

        /// <summary>
        /// Checks if the AppTrackingTransparency system pop-up was ever displayed.
        /// </summary>
        [PublicAPI]
        public static bool WasATTPopupDisplayed()
        {
            return AppTrackingTransparencyBridge.HasDoneInitialRequest;
        }
#endif

        /// <summary>
        /// Checks if a user did authorize the AppTrackingTransparency consent.
        /// </summary>
        [PublicAPI]
        public static bool IsATTAuthorized()
        {
#if UNITY_IOS
            return AppTrackingTransparencyBridge.IsATTAuthorized;
#else
            return true;
#endif
        }

        /// <summary>
        /// Checks if the ads consent can be changed. On iOS is related to an ATT Authorization status.
        /// </summary>
        /// <returns>Provides information about the possibility to change ads consent.</returns>
        [PublicAPI]
        public static bool CanChangeAdsConsent()
        {
            return IsATTAuthorized();
        }

        /// <summary>
        /// Checks if there is a pop-up shown by the service.
        /// </summary>
        /// <returns>Informs if any policy pop-up is currently shown.</returns>
        [PublicAPI]
        public static bool IsPolicyFlowRunning()
        {
            return IsATTAuthorized();
        }

        /// <summary>
        /// Checks if there is a pop-up shown by the service.
        /// </summary>
        /// <returns>Informs if any policy pop-up is currently shown.</returns>
        [PublicAPI]
        public static bool IsPolicyFlowEnded()
        {
            return service != null && service.IsCheckFlowEnded;
        }

#if UNITY_IOS
        /// <summary>
        /// Opens an  iOS setting of the game, for a user to change the ATT permission.
        /// </summary>
        [PublicAPI]
        public static void OpenATTNativeSettings()
        {
            AppTrackingTransparencyBridge.OpenATTSettings();
        }
#endif

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
        static void AutoInit()
        {
            if ( GetConfig() != null && config.AutoInit )
            {
                Initialize();
            }
        }

        static PolicyGuardConfig GetConfig()
        {
            if ( config == null )
            {
                if ( !HConfigs.HasConfig<PolicyGuardConfig>() )
                {
                    HLog.LogError( logPrefix, "Policy Guard Config is missing" );
                    return null;
                }

                config = HConfigs.GetConfig<PolicyGuardConfig>();
            }

            return config;
        }
    }
}