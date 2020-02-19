using System;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HUF.Utils
{
    public static class HLogs
    {
        /// <summary>
        /// Logs message in console when build is set to DEBUG
        /// </summary>
        [Obsolete( "Use " + nameof(HLogPrefix) + " based method" )]
        public static void LogFormatDebug(string logMessage, params object[] args)
        {
            if ( Debug.isDebugBuild )
                Debug.LogFormat( logMessage, args );
        }

        /// <summary>
        /// Logs message in console when build is set to DEBUG
        /// </summary>
        [Obsolete("Use "+nameof(HLogPrefix)+" based method")]
        public static void Log(object message)
        {
            if ( Debug.isDebugBuild )
                Debug.Log( message );
        }

        /// <summary>
        /// Logs message in console when build is set to DEBUG
        /// </summary>
        [Obsolete( "Use " + nameof(HLogPrefix) + " based method" )]
        public static void Log(object message, Object context)
        {
            if( Debug.isDebugBuild )
                Debug.Log( message, context );
        }
    }
}