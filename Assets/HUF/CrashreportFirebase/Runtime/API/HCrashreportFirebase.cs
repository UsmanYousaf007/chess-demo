using System;
using Firebase.Crashlytics;
using HUF.CrashreportFirebase.Runtime.Implementation;
using HUF.InitFirebase.Runtime.API;
using HUF.InitFirebase.Runtime.Config;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.CrashreportFirebase.Runtime.API
{
    public static class HCrashreportFirebase
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HCrashreportFirebase) );

        /// <summary>
        /// Returns whether Firebase Crashlytics is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => HInitFirebase.IsInitialized;

        /// <summary>
        /// Initializes Firebase Crashlytics.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( HInitFirebase.IsInitialized )
            {
                HLog.Log( logPrefix, "Service initialized" );
                return;
            }

            if ( !HConfigs.HasConfig<HFirebaseConfig>() || HConfigs.GetConfig<HFirebaseConfig>().AutoInit )
            {
                HLog.Log( logPrefix, "Service depends solely on Firebase. Initializing Firebase instead." );
                HInitFirebase.Init();
                return;
            }

            HLog.Log( logPrefix, "Service depends solely on Firebase which will be manually initialized." );
        }

        /// <summary>
        /// Sets a custom key.
        /// Custom keys help you get the specific state of your app leading up to a crash.
        /// You can associate arbitrary key/value pairs with your crash reports, and see them in the Firebase console.
        /// When called multiple times, new values for existing keys will update the value, and only the most current value is captured when a crash is recorded.
        /// </summary>
        [PublicAPI]
        public static void SetCustomKey( string key, string value )
        {
            Crashlytics.SetCustomKey( key, value );
        }

        /// <summary>
        /// Logs a custom message.
        /// Crashlytics associates the logs with the crash data and makes them visible in the Firebase console.
        /// Logged messages are associated with the crash data and are visible in the Firebase Crashlytics dashboard when viewing a specific crash.
        /// </summary>
        [PublicAPI]
        public static void Log( string message )
        {
            Crashlytics.Log( message );
        }

        /// <summary>
        /// Sets user id.
        /// Anonymously identifies users in the crash reports. It is possible to clear the value by setting it to a blank string.
        /// This value is displayed in the Firebase Crashlytics dashboard when viewing a specific crash.
        /// </summary>
        [PublicAPI]
        public static void SetUserId( string identifier )
        {
            Crashlytics.SetUserId( identifier );
        }

        /// <summary>
        /// Logs non-fatal exceptions.
        /// </summary>
        [PublicAPI]
        public static void LogException( Exception ex )
        {
            Crashlytics.LogException( ex );
        }

        /// <summary>
        /// Sets if the crash data will be collected.
        /// By default, Firebase Crashlytics automatically collects crash reports for all your app's users.
        /// To give users more control over the data they send, you can enable opt-in reporting instead.
        /// To do this, you have to disable automatic collection and initialize Crashlytics only for opt-in users.
        /// </summary>
        [PublicAPI]
        public static void SetCollectionEnabled( bool enable )
        {
            Crashlytics.IsCrashlyticsCollectionEnabled = enable;
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            var hasConfig = HConfigs.HasConfig<FirebaseCrashlyticsConfig>();

            if ( hasConfig && HConfigs.GetConfig<FirebaseCrashlyticsConfig>().AutoInit )
            {
                Init();
            }
        }
    }
}