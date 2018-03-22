/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-16 19:35:32 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Common
{
    // Don't use the class name Logger since it conflicts with Unity's Logger
    // class.
    public static class LogUtil
    {
        private const int INDENT_SPACE_COUNT = 4;

        private static int indentLevel = 0;

        private static int indentSpaces
        {
            get { return INDENT_SPACE_COUNT * indentLevel; }
        }

        public static void Log(object message)
        {
            Debug.Log(message);
        }

        public static void Log(object message, string color)
        {
            Debug.Log("<color=" + color + ">" + message + "</color>");
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        public static void LogWarning(object message, string color)
        {
            Debug.LogWarning("<color=" + color + ">" + message + "</color>");
        }

        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        public static void LogAssertion(object message)
        {
            Debug.LogAssertion(message);
        }

        public static void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void LogMethodStart(object message)
        {
            Log(new string(' ', indentSpaces) + message + " {", "green");
            ++indentLevel;
        }

        public static void LogMethodEnd(object message)
        {
            --indentLevel;
            Log(new string(' ', indentSpaces) + "} // " + message, "red");
        }

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Verbose(object message)
        {
            Debug.Log("<color=grey>[VERBOSE]</color> " + message);
        }
    }
}
