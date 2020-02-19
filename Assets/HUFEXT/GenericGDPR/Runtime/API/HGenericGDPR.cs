using HUF.Analytics.API;
using HUF.Utils.Configs.API;
using HUFEXT.GenericGDPR.Runtime.Implementation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.GenericGDPR.Runtime.API
{
    public static class HGenericGDPR
    {
        /// <summary>
        /// Use this event to perform required actions when user
        /// accept policy.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnPolicyAccepted;

        static GameObject canvas;
        static GenericGDPRView view;
        static GDPRConfig config;

        /// <summary>
        /// Returns information about GDPR window status.
        /// </summary>
        public static bool IsInitialized => view != null;

        /// <summary>
        /// Returns information whether player accept policy.
        /// <returns> Return TRUE if player has policy accepted or if player prefs usage is disabled in config. <para/>
        /// </summary>
        public static bool IsPolicyAccepted => ValidateConfig() &&
                                               PlayerPrefs.GetInt( config.PolicyAcceptedKey ) == 1;

        /// <summary>
        /// Returns information whether player accept personalized ads consent.
        /// <returns> Return TRUE if player has ads consent or if player prefs usage is disabled in config. <para/>
        /// </summary>
        public static bool IsPersonalizedAdsAccepted => ValidateConfig() &&
                                                        PlayerPrefs.GetInt( config.PersonalizedAdsKey ) == 1;

        /// <summary>
        /// Use this event to get information about panel being close
        /// at any point.
        /// </summary>
        [PublicAPI]
        public static void Create()
        {
            if( config == null && HConfigs.HasConfig<GDPRConfig>() )
            {
                config = HConfigs.GetConfig<GDPRConfig>();
            }

            if( !IsInitialized && config != null && config.Prefab != null )
            {
                canvas = Object.Instantiate( config.Prefab );
                view = canvas.GetComponentInChildren<GenericGDPRView>();
                view.Init();
                OnPolicyAccepted += Dispose;

                var analyticsEvent = AnalyticsEvent.Create("gdpr_displayed").ST1("launch").ST2("gdpr");
                HAnalytics.LogEvent(analyticsEvent);

            }
        }

        /// <summary>
        /// Use this event to get information about panel being close
        /// at any point.
        /// </summary>
        [PublicAPI]
        public static void AcceptPolicy()
        {
            if( config.UsePlayerPrefs )
            {
                PlayerPrefs.SetInt( config.PolicyAcceptedKey, 1 );
                PlayerPrefs.SetInt( config.PersonalizedAdsKey, view.AdsConsent ? 1 : 0 );
            }

            var analyticsEvent = AnalyticsEvent.Create("gdpr_accepted").ST1("launch").ST2("gdpr");
            HAnalytics.LogEvent(analyticsEvent);


            OnPolicyAccepted?.Invoke();
        }

        /// <summary>
        /// Use this event to get information about panel being close
        /// at any point.
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if( !HConfigs.HasConfig<GDPRConfig>() )
            {
                Debug.LogError( "[GenericGDPR] Missing GDPR config (check HUFConfigs directory)." );
                return;
            }

            config = HConfigs.GetConfig<GDPRConfig>();
            if( config != null && config.AutoInit && !IsPolicyAccepted )
            {
                Create();
            }
        }

        private static bool ValidateConfig()
        {
            if( config == null )
            {
                if ( !HConfigs.HasConfig<GDPRConfig>() )
                {
                    Debug.LogError( "[GenericGDPR] Missing GDPR config (check HUFConfigs directory)." );
                    return false;
                }

                config = HConfigs.GetConfig<GDPRConfig>();
            }
            return config != null && config.UsePlayerPrefs;
        }

        public static void SetPersonalizedAdsAccepted(bool value)
        {
            PlayerPrefs.SetInt(config.PersonalizedAdsKey, value ? 1 : 0);
        }
    }
}
