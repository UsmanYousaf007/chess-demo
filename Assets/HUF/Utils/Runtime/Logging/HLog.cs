using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace HUF.Utils.Runtime.Logging
{
    public static class HLog
    {
        static bool canLogMessages = Config != null &&
                                     ( Debug.isDebugBuild && !Config.DisableHLogsOnDebugBuilds || Config.CanLogOnProd );

        static bool canLogTime = Config != null && Config.ShowTimeInNativeLogs;
        static HLogConfig config;

        static string TimeString => $"[{DateTime.UtcNow:T.ToString(\"HH:mm:ss\")}]";

        static HLogConfig Config
        {
            get
            {
                if ( config == null && HConfigs.HasConfig<HLogConfig>() )
                {
                    config = HConfigs.GetConfig<HLogConfig>();
                }

                return config;
            }
        }

#if UNITY_EDITOR
        public static void RefreshConfig()
        {
            config = null;
            config = GetLogConfig();
        }

        static HLogConfig GetLogConfig()
        {
            var files = AssetDatabase.FindAssets( $"t:{nameof(HLogConfig)}" );

            if ( files.Length == 0 )
                return null;

            var configPath = files.Select( AssetDatabase.GUIDToAssetPath )
                .FirstOrDefault( s => s.Contains( "Resources/HUFConfigs/" ) );

            if ( configPath.IsNullOrEmpty() )
                return null;

            return AssetDatabase.LoadAssetAtPath<HLogConfig>( configPath );
        }
#endif

        /// <summary>
        /// Logs a message in the console.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogError(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Error );
        }

        /// <summary>
        /// Logs a message in the console.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogWarning(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Warning );
        }

        /// <summary>
        /// Logs a message that is not filtered out like HBI ID but only on debug.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogImportant(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Log, null, Debug.isDebugBuild && (Config == null || !Config.DisableHLogsOnDebugBuilds));
        }

        /// <summary>
        /// Logs a message that is not filtered out like Ads adapters status.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogAlways(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Log, null, !Debug.isDebugBuild || Config != null && !Config.DisableHLogsOnDebugBuilds );
        }

        /// <summary>
        /// Logs a message in the console when the build is set to DEBUG.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Log(
            HLogPrefix prefixSource,
            string message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            if ( type == LogType.Log && !isNotFiltered )
            {
                if ( !canLogMessages )
                    return;

                if ( Config != null && Config.IsFilteringLogs )
                {
                    bool match = Regex.IsMatch( prefixSource.Prefix,
                        Config.RegexFilter,
                        Config.IgnoreCaseInRegex ? RegexOptions.IgnoreCase : RegexOptions.None );

                    //Equals to: ( !Config.InvertFilter && !match ) || ( Config.InvertFilter && match )
                    if ( Config.InvertFilter == match )
                        return;
                }
            }

            switch ( type )
            {
                case LogType.Error:
                    Debug.LogError( FormatMessage( prefixSource.Prefix, message, type ), context );
                    break;
                case LogType.Warning:
                    Debug.LogWarning( FormatMessage( prefixSource.Prefix, message, type ), context );
                    break;
                default:
                    Debug.Log( FormatMessage( prefixSource.Prefix, message, type ), context );
                    break;
            }
#if UNITY_IOS && !UNITY_EDITOR
            if (config != null && config.IOSNativeLogs)
                HUFiOSSendNativeLog( FormatMessage( prefixSource.Prefix, message, type ) );
#endif
        }

        /// <summary>
        /// Generates log message formatted in HUF manner.
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        static string FormatMessage( string prefix, string message, LogType type )
        {
#if UNITY_EDITOR
            switch ( type )
            {
                case LogType.Error:
                    return $"<color=#c70000><b>[{prefix}]</b></color> {message}";
                case LogType.Warning:
                    return $"<color=#c77700><b>[{prefix}]</b></color> {message}";
                default:
                    return $"<color=#6f8a91><b>[{prefix}]</b></color> {message}";
            }
#else
            return ( canLogTime ? $"[{DateTime.UtcNow:T.ToString(\"HH:mm:ss\")}]" : String.Empty ) +
                   $"[{prefix}] {message}";
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void HUFiOSSendNativeLog(string message);
#endif
    }
}