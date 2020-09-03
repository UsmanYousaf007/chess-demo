using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

namespace UpdateManager
{
    public static class EditorUpdateManager
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
        //public static event Action<bool> IsUpdateAvailableOnStore;
        public static Action<bool> IsUpdateAvailableOnStore;


        public static void Initialize()
        {

        }

        public static IEnumerator CheckForUpdate()
        {
            Debug.Log("IOSAPPUPDATEMANAGER CheckForUpdate called");
            bool updateAvailable = false;
            //bool updateAvailable = true;
            Debug.Log("IOSAPPUPDATEMANAGER IsNewVersionPresent result: " + updateAvailable.ToString());
            yield return null;
            IsUpdateAvailableOnStore.Invoke(updateAvailable);
        }

        public static void GoToStore()
        {
            
        }
    }
}