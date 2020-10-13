using System.Collections;
using HUF.Ads.Runtime.API;
using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using HUFEXT.CountryCode.Runtime.API;
using HUFEXT.GenericGDPR.Runtime.Views;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.GenericGDPR.Runtime.API
{
    public static class HGenericGDPR
    {
        const string CONFIG_ERROR = "Missing GDPR config (check HUFConfigs directory).";
        
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof( HGenericGDPR ) );

        static GameObject canvas;
        static GDPRConfig config;
        static GDPRView view;

        /// <summary>
        /// Raised when the GDPR window appears.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnPolicyWindowShow;
        
        /// <summary>
        /// Raised when the user accepts the GDPR policy.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnPolicyAccepted;

        /// <summary>
        /// Returns whether the GDPR prefab exists in the scene.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => view != null;

        /// <summary>
        /// Returns the GDPR prefab instance that exists in the scene.
        /// </summary>
        [PublicAPI]
        public static GameObject ViewCanvas => canvas;
        
        /// <summary>
        /// Returns whether the player has accepted the GDPR policy.
        /// If consent is not found, getter will check for the custom prefs key and try to parse it from int value.
        /// </summary>
        [PublicAPI]
        public static bool IsPolicyAccepted
        {
            get
            {
                var analyticsConsent = HAnalytics.GetGDPRConsent();

                if ( analyticsConsent.HasValue )
                {
                    return analyticsConsent.Value;
                }
                
                if ( config == null )
                    GetConfig();
                
                if ( HPlayerPrefs.GetBool( config.CustomGDPRKey, false ) )
                {
                    HAnalytics.CollectSensitiveData( true );
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns whether the player has consented to personalized ads.
        /// If consent is not found, getter will check for the custom prefs key and try to parse it from int value.
        /// </summary>
        [PublicAPI]
        public static bool IsPersonalizedAdsAccepted
        {
            get
            {
                var adsConsent = HAds.GetGDPRConsent();

                if ( adsConsent.HasValue )
                {
                    return adsConsent.Value;
                }

                if ( config == null )
                    GetConfig();
                    
                if ( HPlayerPrefs.GetBool( config.CustomPersonalizedAdsKey, false ) )
                {
                    HAds.CollectSensitiveData( true );
                    return true;
                }

                return false;
            }
            set
            {
                HAds.CollectSensitiveData(value);
                HPlayerPrefs.SetBool(config.CustomPersonalizedAdsKey, value);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoInit()
        {
            GetConfig();
            if( ValidateConfig() && config.AutoInit && !IsPolicyAccepted )
            {
                CoroutineManager.StartCoroutine( InitializeWithDelay() );
            }
        }

        static IEnumerator InitializeWithDelay()
        {
            yield return null;
            Initialize();
        }

        /// <summary>
        /// Fetches the GDPR config and instantiates the GDPR prefab.
        /// Should be called only if AutoInit option is disabled.
        /// </summary>
        [PublicAPI]
        public static void Initialize()
        {
            if ( !ValidateConfig() )
            {
                GetConfig();
            }

            if ( !IsInitialized && config != null )
            {
                Initialize( config.Prefab );
            }
        }

        /// <summary>
        /// Fetches the GDPR config and instantiates the custom GDPR prefab.
        /// Should be called only if AutoInit option is disabled.
        /// When using a custom prefab, the prefab in the config must be set to None.
        /// <param name="prefab">Custom GDPR window prefab</param>
        /// </summary>
        [PublicAPI]
        public static void Initialize( GameObject prefab )
        {
            if ( !ValidateConfig() )
            {
                GetConfig();
            }

            if ( IsInitialized || config == null || prefab == null )
            {
                HLog.LogError( logPrefix, "Unable to instantiate GDPR prefab." );
                return;
            }

            if ( config.EnableCountryCheck && !IsGDPRRequiredForCurrentCountry() )
            {
                HLog.LogImportant( logPrefix, "GDPR is not required for current country." );
                ForceAcceptPolicy();
                return;
            }
            
            canvas = Object.Instantiate( prefab );
            view = canvas.GetComponent<GDPRView>();

            if ( !IsInitialized )
            {
                HLog.LogError( logPrefix, "GDPR prefab doesn't have required GDPR View component." );
                Object.Destroy( canvas );
                canvas = null;
                view   = null;
                return;
            }

            if ( config.DestroyOnAccept )
            {
                OnPolicyAccepted += Dispose;
            }

            var analyticsEvent = AnalyticsEvent.Create("gdpr_displayed").ST1("launch").ST2("gdpr");
            HAnalytics.LogEvent(analyticsEvent);

            OnPolicyWindowShow.Dispatch();
        }
        
        /// <summary>
        /// Accepts the GDPR policy.
        /// It should be called out of the box, when the OK button of the prefab is clicked.
        /// </summary>
        [PublicAPI]
        public static void AcceptPolicy()
        {
            if ( !IsInitialized )
            {
                HLog.LogError( logPrefix, "Unable to accept policy, GDPR view is not initialized." );
                return;
            }
            
            HAnalytics.CollectSensitiveData( true );
            HAds.CollectSensitiveData( view.AdsConsentToggle );
            OnPolicyAccepted.Dispatch();
            var analyticsEvent = AnalyticsEvent.Create("gdpr_accepted").ST1("launch").ST2("gdpr");
            HAnalytics.LogEvent(analyticsEvent);
        }

        /// <summary>
        /// Destroys the GDPR object.
        /// </summary>
        [PublicAPI]
        public static void Dispose()
        {
            if( IsInitialized )
            {
                Object.Destroy( canvas );
                canvas = null;
                view = null;
            }

            if ( config.DestroyOnAccept )
            {
                OnPolicyAccepted -= Dispose;
            }
        }

        private static void GetConfig()
        {
            if ( !HConfigs.HasConfig<GDPRConfig>() )
            {
                HLog.LogError( logPrefix, CONFIG_ERROR );
                return;
            }

            config = HConfigs.GetConfig<GDPRConfig>();
        }

        private static bool ValidateConfig()
        {
            if ( config == null || !HConfigs.HasConfig<GDPRConfig>() )
            {
                HLog.LogError( logPrefix, CONFIG_ERROR );
                return false;
            }

            return true;
        }

        private static bool IsGDPRRequiredForCurrentCountry()
        {
            var country = HNativeCountryCode.GetCountryCode().Country.ToUpper();
            return config.ShowForCountries.Contains( country );
        }

        private static void ForceAcceptPolicy()
        {
            HAnalytics.CollectSensitiveData( true );
            HAds.CollectSensitiveData( config.DefaultPersonalizedAdsValue );
            OnPolicyAccepted.Dispatch();
        }
    }
}
