using HUF.Ads.API;
using HUF.Analytics.API;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using HUF.Utils.PlayerPrefs;
using HUF.Utils.Runtime.Logging;
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

                if ( HPlayerPrefs.GetBool( config.CustomPersonalizedAdsKey, false ) )
                {
                    HAds.CollectSensitiveData( true );
                    return true;
                }

                return false;
            }
        }


        public static void SetPersonalisedAds(bool value)
        {
            var adsConsent = HAds.GetGDPRConsent();

            if (adsConsent.HasValue)
            {
                HAds.CollectSensitiveData(value);
            }
            else
            {
                HPlayerPrefs.SetBool(config.CustomPersonalizedAdsKey, value);
            }
            
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            FetchConfig();
            if( ValidateConfig() && config.AutoInit && !IsPolicyAccepted )
            {
                Initialize();
            }
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

            if( !IsInitialized && config != null && config.Prefab != null )
            {
                canvas = Object.Instantiate( config.Prefab );
                view = canvas.GetComponent<GDPRView>();
                OnPolicyAccepted += Dispose;
                OnPolicyWindowShow.Dispatch();

                var analyticsEvent = AnalyticsEvent.Create("gdpr_displayed")
                    .ST1("launch")
                    .ST2("gdpr");
                HAnalytics.LogEvent(analyticsEvent);
            }
        }

        /// <summary>
        /// This method is called when user click on OK button.
        /// It should be connected to button out of the box.
        /// </summary>
        [PublicAPI]
        public static void AcceptPolicy()
        {
            HAnalytics.CollectSensitiveData( true );
            HAds.CollectSensitiveData( view.AdsConsentToggle );
            OnPolicyAccepted.Dispatch();

            var analyticsEvent = AnalyticsEvent.Create("gdpr_accepted")
                   .ST1("launch")
                   .ST2("gdpr");
            HAnalytics.LogEvent(analyticsEvent);
        }

        /// <summary>
        /// Destroys GDPR object.
        /// </summary>
        [PublicAPI]
        public static void Dispose()
        {
            if( IsInitialized && config.DestroyOnAccept )
            {
                Object.Destroy( canvas );
            }

            OnPolicyAccepted -= Dispose;
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
    }
}
