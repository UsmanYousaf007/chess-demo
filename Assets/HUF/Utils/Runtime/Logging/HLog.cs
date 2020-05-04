using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HUF.Utils.Runtime.Logging
{
    public static class HLog
    {
        static HLogConfig config;

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
        /// Logs message in console
        /// </summary>
        public static void LogError(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Error );
        }

        /// <summary>
        /// Logs message in console
        /// </summary>
        public static void LogWarning(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Warning );
        }

        /// <summary>
        /// Logs message that is not filtered out like HBI id but only on debug 
        /// </summary>
        public static void LogImportant(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Log, null, Debug.isDebugBuild );
        }
        
        /// <summary>
        /// Logs message that is not filtered out like Ads adapters status
        /// </summary>
        public static void LogAlways(
            HLogPrefix prefixSource,
            string message )
        {
            Log( prefixSource, message, LogType.Log, null, true );
        }

        /// <summary>
        /// Logs message in console when build is set to DEBUG
        /// </summary>
        public static void Log(
            HLogPrefix prefixSource,
            string message,
            LogType type = LogType.Log,
            Object context = null,
            bool isNotFiltered = false )
        {
            if ( type == LogType.Log && !isNotFiltered )
            {
                if ( ( Debug.isDebugBuild || !CanLogOnProd() ) && Config == null )
                    return;

                if ( Config.IsFilteringLogs && !Regex.IsMatch( prefixSource.Prefix,
                    Config.RegexFilter,
                    Config.IgnoreCaseInRegex ? RegexOptions.IgnoreCase : RegexOptions.None ) )
                    return;
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
        }

        /// <summary>
        /// Generates log message formatted in HUF manner
        /// </summary>
        static string FormatMessage( string prefix, string message, LogType type )
        {
#if UNITY_EDITOR
            switch ( type )
            {
                case LogType.Error:
                    return $"<color=\"#c70000\"><b>[{prefix}]</b></color> {message}";
                case LogType.Warning:
                    return $"<color=\"#c77700\"><b>[{prefix}]</b></color> {message}";
                default:
                    return $"<color=\"#6f8a91\"><b>[{prefix}]</b></color> {message}";
            }
#else
            return $"[{prefix}] {message}";
#endif
        }

        static bool CanLogOnProd()
        {
            return !Debug.isDebugBuild && Config != null && Config.CanLogOnProd;
        }
    }
}