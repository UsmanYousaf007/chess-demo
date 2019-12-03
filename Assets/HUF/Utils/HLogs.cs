using UnityEngine;

namespace HUF.Utils
{
    public static class HLogs
    {
        /// <summary>
        /// Logs message in console when build is set to DEBUG
        /// </summary>
        public static void LogFormatDebug(string logMessage, params object[] args)
        {
            if (Debug.isDebugBuild)
                Debug.LogFormat(logMessage, args);
        }
    }
}