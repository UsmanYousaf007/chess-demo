using System;
using System.Collections;
using System.Linq;
using HUF.Analytics.Runtime.API;
#if HUF_ANALYTICS_APPSFLYER
using HUF.AnalyticsAppsFlyer.Runtime.API;
#endif
using HUF.AnalyticsHBI.Runtime.Configs;
#if !HUF_ANALYTICS_HBI_DUMMY
using HUF.AnalyticsHBI.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
#else
using HBIAnalyticsService = HUF.Analytics.Runtime.Implementation.DummyAnalyticsService;
using HUF.Analytics.Runtime.Implementation;
#endif
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.NetworkRequests;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.AnalyticsHBI.Runtime.API
{
    public static class HAnalyticsHBI
    {
        internal const string HBI_LAST_PLAYER_ATTRIBUTES = "HBILastPlayerAttributes";
        const string HDS_REVENUE_LEVEL_CHANGED = "hds_revenue_level_changed";
        const string REVENUE_LEVEL = "revenue_level";
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HAnalyticsHBI) );

        static HBIAnalyticsService service;
        static HBIAnalyticsConfig config;

        /// <summary>
        /// Session ID will reset if the app is minimized for such duration of minutes.
        /// </summary>
        public static int minutesToResetSessionId = 5;

        /// <summary>
        /// Raised when player revenue level changes.
        /// </summary>
        [PublicAPI]
        public static event Action<int> OnRevenueLevelChanged;

        /// <summary>
        /// Returns whether HAnalyticsHBI is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; } = false;

        /// <summary>
        /// Gets Session ID from HDS service.
        /// </summary>
        [PublicAPI]
        public static string SessionId
        {
            get
            {
                if ( CheckInitialization() )
                    return service.SessionId;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets User ID from HDS service.
        /// </summary>
        [PublicAPI]
        public static string UserId
        {
            get
            {
                if ( CheckInitialization() )
                    return service.UserId;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets cached player attributes.
        /// </summary>
        [PublicAPI]
        public static PlayerAttributes LastPlayerAttributes
        {
            get
            {
                if ( PlayerPrefs.HasKey( HBI_LAST_PLAYER_ATTRIBUTES ) )
                    return new PlayerAttributes( PlayerPrefs.GetString( HBI_LAST_PLAYER_ATTRIBUTES ) );

                return null;
            }
        }

        public static HBIAnalyticsConfig Config
        {
            get
            {
                if ( config == null && HConfigs.HasConfig<HBIAnalyticsConfig>() )
                {
                    config = HConfigs.GetConfig<HBIAnalyticsConfig>();
                }

                return config;
            }
        }

        /// <summary>
        /// Initializes the HDS analytics service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( IsInitialized )
                return;

#if !HUF_ANALYTICS_HBI_DUMMY
            service = new HBIAnalyticsService();
#else
            service = new HBIAnalyticsService( AnalyticsServiceName.HBI );
#endif
            IsInitialized = HAnalytics.TryRegisterService( service );

            if ( IsInitialized )
            {
#if HUF_ANALYTICS_APPSFLYER
                LogEventToAppsFlyerWhenRevenueLevelChanges();
#endif
                RepeatedlyDownloadPlayerAttributes();
            }
        }

        /// <summary>
        /// Gets player attributes.
        /// </summary>
        [PublicAPI]
        public static void GetPlayerAttributes( Action<PlayerAttributesResponse> response )
        {
            if ( CheckInitialization() )
                HDSPlayerAttributesService.GetPlayerAttributes( res => { response.Dispatch( res ); } );
        }

        internal static void HandleRevenueLevelChanged( int revenueLevel )
        {
            OnRevenueLevelChanged.Dispatch( revenueLevel );
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
#if !HUF_ANALYTICS_HBI_DUMMY
            if ( HConfigs.HasConfig<HBIAnalyticsConfig>() && HConfigs.GetConfig<HBIAnalyticsConfig>().AutoInit )
#endif
            {
                Init();
            }
        }

        static bool CheckInitialization()
        {
            if ( service != null && service.IsInitialized )
                return true;

            HLog.LogError( logPrefix, "Service not initialized." );
            return false;
        }

#if HUF_ANALYTICS_APPSFLYER
        static void LogEventToAppsFlyerWhenRevenueLevelChanges()
        {
            OnRevenueLevelChanged += revenueLevel =>
            {
                var analyticsEvent = AnalyticsEvent.Create( HDS_REVENUE_LEVEL_CHANGED ).ST1( HDS_REVENUE_LEVEL_CHANGED )
                    .Parameter( REVENUE_LEVEL, revenueLevel );
                HAnalytics.LogEvent( analyticsEvent, AnalyticsServiceName.APPS_FLYER );
            };
        }
#endif

        static void RepeatedlyDownloadPlayerAttributes()
        {
            CoroutineManager.StartCoroutine(
                RepeatedlyDownloadPlayerAttributesCoroutine( Config.RefreshPlayerAttributesDistanceInSeconds ) );

            IEnumerator RepeatedlyDownloadPlayerAttributesCoroutine( float downloadTimeDistance )
            {
                var waitForSeconds = new WaitForSeconds( downloadTimeDistance );

                while ( true )
                {
                    GetPlayerAttributes( null );
                    yield return waitForSeconds;
                }
            }
        }
    }
}