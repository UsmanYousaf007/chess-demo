using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace UpdateManager
{
    public static class IOSUpdateManager
    {
        // Used to get an event when the update is available
        public static event Action<int> OnUpdateAvailable;
        // Used to receive an event when downloading the event
        // about bytes downloaded
        public static event Action<long, long> OnUpdateDownloading;
        // Used to receive an event when an error is received
        public static event Action<string> OnErrorReceived;
        // Used to get an event when the update is downloaded
        public static event Action OnUpdateDownloaded;
        // Used to know if update is available or not
        public static event Action<bool> IsUpdateAvailableOnStore;

        [DllImport("__Internal")]
        private static extern bool IsNewVersionPresent();
        [DllImport("__Internal")]
        private static extern void GoToAppStore();
        [DllImport("__Internal")]
        private static extern void Init();

        public static void Initialize()
        {
            Init();
        }

        public static void CheckForUpdate()
        {
            int updateAvailable = IsNewVersionPresent() ? 1 : 0;
            OnUpdateAvailable.Invoke(updateAvailable);
        }

        public static void GoToStore()
        {
            GoToAppStore();
        }
    }
}