using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace HUF.Utils.Runtime.Logging
{
    public static class HLog
    {
        const int DEBUG_FRAME_OFFSET = 3;
        const int PRODUCTION_FRAME_OFFSET_IOS = 2;
        const int PRODUCTION_FRAME_OFFSET = 1;

        public static int stackFrame =
#if UNITY_EDITOR
            DEBUG_FRAME_OFFSET;
#else
    #if UNITY_IOS
            HUFIsIOSBuildConfigDebug() ? DEBUG_FRAME_OFFSET : PRODUCTION_FRAME_OFFSET_IOS;
    #else
        #if DEVELOPMENT_BUILD
            DEBUG_FRAME_OFFSET;
        #else
            PRODUCTION_FRAME_OFFSET;
        #endif
    #endif
#endif

        static bool canLogMessages = Config != null &&
                                     ( Debug.isDebugBuild && !Config.DisableHLogsOnDebugBuilds ||
                                       Config.CanLogOnProd );

        static bool canLogTime = Config != null && Config.ShowTimeInNativeLogs;
        static HLogConfig config;

        /// <summary>
        /// Correctly recognizes nature of the build for both Android and iOS.
        /// </summary>
        [PublicAPI]
        public static bool IsTrueDebugBuild
        {
            get
            {
#if UNITY_EDITOR
                return true;
#elif UNITY_IOS
                return HUFIsIOSBuildConfigDebug();
#else
#if DEVELOPMENT_BUILD
                return true;
#else
                return false;
#endif
#endif
            }
        }

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
            string message )
        {
            LogWithAutoPrefix( message, LogType.Error );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogError(
            HLogPrefix prefixSource,
            string message )
        {
            LogWithAutoPrefix( message, LogType.Error );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogError(
            HLogPrefix prefixSource,
            Func<string> message )
        {
            LogWithAutoPrefix( message, LogType.Error );
        }

        /// <summary>
        /// Logs the value of message in the console.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogError(
            Func<string> message )
        {
            LogWithAutoPrefix( message, LogType.Error );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogWarning(
            HLogPrefix prefixSource,
            string message )
        {
            LogWithAutoPrefix( message, LogType.Warning );
        }

        /// <summary>
        /// Logs a message in the console.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogWarning(
            string message )
        {
            LogWithAutoPrefix( message, LogType.Warning );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogWarning(
            HLogPrefix prefixSource,
            Func<string> message )
        {
            LogWithAutoPrefix( message, LogType.Warning );
        }

        /// <summary>
        /// Logs the value of message in the console.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogWarning(
            Func<string> message )
        {
            LogWithAutoPrefix( message, LogType.Warning );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogImportant(
            HLogPrefix prefixSource,
            string message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                Debug.isDebugBuild && ( Config == null || !Config.DisableHLogsOnDebugBuilds ) );
        }

        /// <summary>
        /// Logs a message that is not filtered out like HBI ID but only on debug.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogImportant(
            string message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                Debug.isDebugBuild && ( Config == null || !Config.DisableHLogsOnDebugBuilds ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogImportant(
            HLogPrefix prefixSource,
            Func<string> message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                Debug.isDebugBuild && ( Config == null || !Config.DisableHLogsOnDebugBuilds ) );
        }

        /// <summary>
        /// Logs the value of message that is not filtered out like HBI ID but only on debug.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogImportant(
            Func<string> message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                Debug.isDebugBuild && ( Config == null || !Config.DisableHLogsOnDebugBuilds ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogAlways(
            HLogPrefix prefixSource,
            string message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                !Debug.isDebugBuild || Config != null && !Config.DisableHLogsOnDebugBuilds );
        }

        /// <summary>
        /// Logs a message that is not filtered out like Ads adapters status.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogAlways(
            string message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                !Debug.isDebugBuild || Config != null && !Config.DisableHLogsOnDebugBuilds );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogAlways(
            HLogPrefix prefixSource,
            Func<string> message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                !Debug.isDebugBuild || Config != null && !Config.DisableHLogsOnDebugBuilds );
        }

        /// <summary>
        /// Logs the value of message that is not filtered out like Ads adapters status.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void LogAlways(
            Func<string> message )
        {
            LogWithAutoPrefix( message,
                LogType.Log,
                null,
                !Debug.isDebugBuild || Config != null && !Config.DisableHLogsOnDebugBuilds );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Log(
            HLogPrefix prefixSource,
            string message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            LogWithAutoPrefix( message, type, context, isNotFiltered );
        }

        /// <summary>
        /// Logs a message in the console when if the criteria from the config are met.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Log(
            string message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            LogWithAutoPrefix( message, type, context, isNotFiltered );
        }

        /// <summary>
        /// Logs the value of message in the console when if the criteria from the config are met.
        /// </summary>
        [PublicAPI]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Log(
            Func<string> message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            LogWithAutoPrefix( message, type, context, isNotFiltered );
        }

        [Obsolete]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static void Log(
            HLogPrefix prefixSource,
            Func<string> message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            LogWithAutoPrefix( message, type, context, isNotFiltered );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        static void LogWithAutoPrefix(
            string message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            ( string prefix, string suffix ) = GetPrefixAndSuffix();

            if ( IsMessageFilteredOut( prefix, type, isNotFiltered ) )
                return;

            LogMessage( prefix, $"{message}\nSource: {suffix}", type, context );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        static void LogWithAutoPrefix(
            Func<string> message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            ( string prefix, string suffix ) = GetPrefixAndSuffix();

            if ( IsMessageFilteredOut( prefix, type, isNotFiltered ) )
                return;

            LogMessage( prefix, $"{message()}\nSource: {suffix}", type, context );
        }

        static bool IsMessageFilteredOut( string prefix, LogType type, bool isNotFiltered )
        {
            if ( type == LogType.Log && !isNotFiltered )
            {
                if ( !canLogMessages )
                    return true;

                if ( Config != null && Config.IsFilteringLogs )
                {
                    bool match = Regex.IsMatch( prefix,
                        Config.RegexFilter,
                        Config.IgnoreCaseInRegex ? RegexOptions.IgnoreCase : RegexOptions.None );

                    //Equals to: ( !Config.InvertFilter && !match ) || ( Config.InvertFilter && match )
                    if ( Config.InvertFilter == match )
                        return true;
                }
            }

            return false;
        }

        static void LogMessage( string prefix,
            string message,
            LogType type = LogType.Log,
            Object context = null )
        {
            switch ( type )
            {
                case LogType.Error:
                    Debug.LogError( FormatMessage( prefix, message, type ), context );
                    break;
                case LogType.Warning:
                    Debug.LogWarning( FormatMessage( prefix, message, type ), context );
                    break;
                default:
                    Debug.Log( FormatMessage( prefix, message, type ), context );
                    break;
            }
#if UNITY_IOS && !UNITY_EDITOR
            if (config != null && config.IOSNativeLogs)
                HUFiOSSendNativeLog( FormatMessage( prefix, message, type ) );
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

        static (string, string) GetPrefixAndSuffix()
        {
            StackFrame[] stacks = new StackFrame[stackFrame + 1];
            MethodBase[] methods = new MethodBase[stackFrame + 1];

            for ( int i = 0; i <= stackFrame; i++ )
            {
                stacks[i] = new StackFrame( i );
                methods[i] = stacks[i].GetMethod();
            }

            var caller = new StackFrame( stackFrame ).GetMethod();

            if ( caller == null )
            {
#if HUF_TESTS
                Debug.LogError( $"No stack method!\n{Environment.StackTrace}" );
#endif
                return ( "Unknown", new StackFrame( stackFrame ).ToString() );
            }

            var callerType = caller.DeclaringType;

            if ( callerType != null )
            {
                var fullPath = callerType.ToString();

                return ( $"{ExtractCoreNamespace( callerType.Namespace )}{ExtractCorePrefix( fullPath )}",
                    $"{fullPath}:{caller.Name}" );
            }

            return ( caller.Name, caller.ToString() );
        }

        static string ExtractCoreNamespace( string fullNamespace )
        {
            if ( fullNamespace.IsNullOrEmpty() )
                return string.Empty;

            int trim = 0;

            for ( int i = 0; i < fullNamespace.Length; i++ )
            {
                if ( fullNamespace[i] != '.' )
                    continue;

                if ( trim > 0 )
                {
                    return fullNamespace.Substring( 0, i );
                }

                trim = i;
            }

            return fullNamespace;
        }

        static string ExtractCorePrefix( string fullName )
        {
            int last = fullName.LastIndexOf( '+' );
            int first = fullName.LastIndexOf( '.' );

            if ( first < 0 )
                return string.Empty;

            if ( last < 0 )
                last = fullName.Length;
            int length = last - first;

            if ( length <= 0 )
                return string.Empty;

            return fullName.Substring( first, length ).TrimEnd( ']' );
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
        public static extern bool HUFIsIOSBuildConfigDebug();

        [DllImport("__Internal")]
        static extern void HUFiOSSendNativeLog(string message);
#endif
    }
}
