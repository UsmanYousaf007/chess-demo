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
        public static GDPRStatus IsPolicyAccepted
        {
            get
            {
                if( config == null || !config.UsePlayerPrefs )
                {
                    return GDPRStatus.DECLINED;
                }

                return (GDPRStatus)PlayerPrefs.GetInt( config.PlayerPrefsKey ) ;
            }

            set
            {
                PlayerPrefs.SetInt(config.PlayerPrefsKey, (int)value);
            }
        }
        
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

                var analyticsEvent = AnalyticsEvent.Create("gdpr_displayed")
                    .ST1("launch")
                    .ST2("gdpr");
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
            OnPolicyAccepted?.Invoke();

            if( config.UsePlayerPrefs )
            {
                IsPolicyAccepted = GDPRStatus.ACCEPTED;
            }

            var analyticsEvent = AnalyticsEvent.Create("gdpr_accepted")
                    .ST1("launch")
                    .ST2("gdpr");
            HAnalytics.LogEvent(analyticsEvent);
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
                return;
            }
            
            config = HConfigs.GetConfig<GDPRConfig>();
            if( config != null && config.AutoInit && !(IsPolicyAccepted == GDPRStatus.ACCEPTED || IsPolicyAccepted == GDPRStatus.TURNED_OFF) )
            {
                Create();
            }
        }
    }

    public enum GDPRStatus
    {
        ACCEPTED = 1,
        DECLINED = 2,
        TURNED_OFF = 3
    }
}
