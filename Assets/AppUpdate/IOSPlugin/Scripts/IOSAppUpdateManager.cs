using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace UpdateManager
{
    public static class IOSUpdateManager
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool IsNewVersionPresent();
        [DllImport("__Internal")]
        private static extern void GoToAppStore();
        [DllImport("__Internal")]
        private static extern void Init();
#endif

        public static void Initialize()
        {
#if UNITY_IOS
            Init();
#endif
        }

        public static bool IsUpdateAvailable()
        {
#if UNITY_IOS
            return IsNewVersionPresent();
#else
            return true;
#endif
        }

        public static void GoToStore()
        {
#if UNITY_IOS
            GoToAppStore();

#endif
        }
    }
}