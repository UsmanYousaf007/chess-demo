using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace UpdateManager
{
    public static class IOSUpdateManager
    {
        //public static IOSUpdateManager Instance { get; private set; }
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool newVersionPresent();
        [DllImport("__Internal")]
        private static extern void goToAppStore();
        [DllImport("__Internal")]
        private static extern void Init();

        public static void Initialize()
        {
            Init();
        }

        public static bool IsUpdateAvailable()
        {
            return newVersionPresent();
        }

        public static void GoToAppStore()
        {
            goToAppStore();
        }
#endif
    }
}