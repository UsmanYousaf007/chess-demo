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
        static readonly HLogPrefix prefix = new HLogPrefix( nameof( HGenericGDPR ) );

        static GameObject canvas;
        static GDPRConfig config;
        static GDPRView view;

        /// <summary>
        /// Use this event to perform required actions when policy window appears.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnPolicyWindowShow;
        
        /// <summary>
        /// Use this event to perform required actions when user accept policy.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnPolicyAccepted;

        /// <summary>
        /// Returns TRUE if GDPR prefab with view exist on scene.
        /// </summary>
        public static bool IsInitialized => view != null;

        /// <summary>
        /// Returns GDPR prefab instance with view that exists on scene.
        /// </summary>
        public static GameObject ViewCanvas => canvas;
        
        /// <summary>
        /// Returns information whether player accept policy.
        /// If consent is not found, getter will check for custom prefs key and try to parse it from int value.
        /// <returns> Return TRUE if player has analytics policy accepted. </returns>
        /// </summary>
        public static bool IsPolicyAccepted
        {
            get
            {
                var analyticsConsent = HAnalytics.GetGDPRConsent();

                if ( analyticsConsent.HasValue )
                {
                    return analyticsConsent.Value;
                }

                if (config == null)
                {
                    FetchConfig();
                }

                if ( HPlayerPrefs.GetBool( config.CustomGDPRKey, false ) )
                {
                    HAnalytics.CollectSensitiveData( true );
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns information whether player accept personalized ads consent.
        /// If consent is not found, getter will check for custom prefs key and try to parse it from int value.
        /// <returns> Return TRUE if player has ads consent or if player prefs usage is disabled in config. </returns>
        /// </summary>
        public static bool IsPersonalizedAdsAccepted
        {
            get
            {
                var adsConsent = HAds.GetGDPRConsent();

                if ( adsConsent.HasValue )
                {
                    return adsConsent.Value;
                }

                if (config == null)
                {
                    FetchConfig();
                }

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
            FetchConfig();
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
        /// This method will fetch GDPR config and instantiate prefab.
        /// Should be called only if you have AutoInit option disabled.
        /// </summary>
        [PublicAPI]
        public static void Initialize()
        {
            if ( !ValidateConfig() )
            {
                FetchConfig();
            }

            if ( !IsInitialized && config != null )
            {
                Initialize( config.Prefab );
            }
        }

        /// <summary>
        /// This method will fetch GDPR config and instantiate prefab.
        /// Should be called only if you have AutoInit option disabled.
        /// Please notice that in this case, prefab in config must be set to None.
        /// </summary>
        [PublicAPI]
        public static void Initialize( GameObject prefab )
        {
            if ( !ValidateConfig() )
            {
                FetchConfig();
            }

            if ( IsInitialized || config == null || prefab == null )
            {
                HLog.LogError( prefix, "Unable to instantiate GDPR prefab." );
                return;
            }

            if ( config.EnableCountryCheck && !IsGDPRRequiredForCurrentCountry() )
            {
                HLog.LogImportant( prefix, "GDPR is not required for current country." );
                ForceAcceptPolicy();
                return;
            }
            
            canvas = Object.Instantiate( prefab );
            view = canvas.GetComponent<GDPRView>();

            if ( !IsInitialized )
            {
                HLog.LogError( prefix, "GDPR prefab doesn't have required GDPR View component." );
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
        /// This method is called when user click on OK button.
        /// It should be connected to button out of the box.
        /// </summary>
        [PublicAPI]
        public static void AcceptPolicy()
        {
            if ( !IsInitialized )
            {
                HLog.LogError( prefix, "Unable to accept policy, GDPR view is not initialized." );
                return;
            }
            
            HAnalytics.CollectSensitiveData( true );
            HAds.CollectSensitiveData( view.AdsConsentToggle );

            var analyticsEvent = AnalyticsEvent.Create("gdpr_accepted").ST1("launch").ST2("gdpr");
            HAnalytics.LogEvent(analyticsEvent);

            OnPolicyAccepted.Dispatch();
        }

        /// <summary>
        /// Destroys GDPR object.
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

        private static void FetchConfig()
        {
            if ( !HConfigs.HasConfig<GDPRConfig>() )
            {
                return;
            }

            config = HConfigs.GetConfig<GDPRConfig>();
        }

        private static bool ValidateConfig()
        {
            if ( config == null || !HConfigs.HasConfig<GDPRConfig>() )
            {
                HLog.LogError( prefix, "Missing GDPR config (check HUFConfigs directory)." );
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
