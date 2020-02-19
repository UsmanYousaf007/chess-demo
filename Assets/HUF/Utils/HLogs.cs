using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HUF.Utils.Runtime.Logging
{
    public static class HLog
    {
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
        /// Logs message in console when build is set to DEBUG
        /// </summary>
        public static void Log(
            HLogPrefix prefixSource,
            string message,
            LogType type = LogType.Log,
            Object context = null )
        {
            if ( !Debug.isDebugBuild && type == LogType.Log )
                return;

            switch ( type )
            {
                case LogType.Error:
                    Debug.LogError( FormatMessage( prefixSource.Prefix, message ), context );
                    break;
                case LogType.Warning:
                    Debug.LogWarning( FormatMessage( prefixSource.Prefix, message ), context );
                    break;
                default:
                    Debug.Log( FormatMessage( prefixSource.Prefix, message ), context );
                    break;
            }
        }

        /// <summary>
        /// Generates log message formatted in HUF manner
        /// </summary>
        static string FormatMessage( string prefix, string message )
        {
#if UNITY_EDITOR
            return $"<color=\"#6f8a91\"><b>[{prefix}]</b></color> {message}";
#else
            return $"[{prefix}] {message}";
#endif
        }
    }
}