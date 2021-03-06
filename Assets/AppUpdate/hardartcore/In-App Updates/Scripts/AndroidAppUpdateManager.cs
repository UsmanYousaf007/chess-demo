using System;
using System.Collections;
using UnityEngine;

namespace UpdateManager
{
    public static class AndroidAppUpdateManager
    {
        #region Events
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
        public static Action<bool> IsUpdateAvailableOnStore;
        #endregion
        
        //public static AndroidUpdateManager Instance { get; private set; }

        public static bool isInDebugMode = false;

        private static string unityPlayer = "com.unity3d.player.UnityPlayer";

        [Header("Update Type")]
        [SerializeField]
        private static UpdateType updateType = UpdateType.FLEXIBLE;

        private static AndroidJavaObject _currentActivity;
        private static AndroidJavaObject _inAppUpdateManager;

        class OnUpdateListener : AndroidJavaProxy
        {
            public OnUpdateListener() : base("com.hardartcore.update.OnUpdateListener") { }

            // Invoked when Google Play Services returns a response 
            // availableVersionCode - versionCode of the your app newest update
            // isUpdateAvailable - indicate that update is available
            // isUpdateTypeAllowed - indicate if the update type is allowed (flexible or immediate)
            // versionStalenessDays - days that have passed since Google Play Store learns of an update
            // updatePriority - update priority, integer value between 0 and 5 with 0 being the default and 5 being the highest priority 
            // To set the priority for an update, use inAppUpdatePriority field under Edits.tracks.releases in the Google Play Developer API.
            public void onUpdate(int availableVersionCode, bool isUpdateAvailable, bool isUpdateTypeAllowed, int versionStalenessDays, int updatePriority)
            {
                {
                    if (IsUpdateAvailableOnStore != null)
                    {
                        Debug.Log("AndroidAppUpdateManager OnUpdate availableVersionCode: " + availableVersionCode.ToString() + ",isUpdateAvailable: " + isUpdateAvailable.ToString());
                        IsUpdateAvailableOnStore.Invoke(isUpdateAvailable);
                    }
                }
            }

            // Triggered when the update is being downloaded
            // bytesDownloaded - indicate the bytes downloaded by now
            // totalBytesToDownload - indicate the update's size
            public void onUpdateDownloading(long bytesDownloaded, long totalBytesToDownload)
            {
                if (OnUpdateDownloading != null)
                {
                    OnUpdateDownloading.Invoke(bytesDownloaded, totalBytesToDownload);
                }
            }

            // Invoked when the update is downloaded
            public void onUpdateDownloaded()
            {
                if (OnUpdateDownloaded != null)
                {
                    OnUpdateDownloaded.Invoke();
                }
            }

            // Invoked when error is received
            public void onFailure(string error)
            {
                if (OnErrorReceived != null)
                {
                    OnErrorReceived.Invoke(error);
                }
            }
        }

        public static void Init()
        {
            if (Application.isMobilePlatform)
            {
                // Get UnityPlayer Activity
                GetCurrentAndroidActivity();

                // Initialize Android App Update Manager
                // By default UpdateType is set to FLEXIBLE
                _inAppUpdateManager = new AndroidJavaObject("com.hardartcore.update.AndroidUpdateManager", _currentActivity);
                if (updateType != UpdateType.FLEXIBLE)
                {
                    _inAppUpdateManager.Call("setUpdateTypeImmediate");
                }
                // If isInDebugMode = true will print logs in LogCat
                // for better debugging
                _inAppUpdateManager.Call("setDebugMode", isInDebugMode);
                // Add update listener
                _inAppUpdateManager.Call("addOnUpdateListener", new OnUpdateListener());
            }
        }

        // You should call this function after OnUpdateListener.onUpdate(isUpdateAvailable, isUpdateTypeAllowed)
        // and only if both isUpdateAvailable and isUpdateTypeAllowed are true 
        public static void StartUpdate()
        {
            _inAppUpdateManager.Call("startUpdate");
        }

        // You should call this function after OnUpdateListener.onUpdateDownloaded()
        // to start the instalation of the update
        public static void CompleteUpdate()
        {
            _inAppUpdateManager.Call("completeUpdate");
        }

        // You can check if there is an update using this when returning back to the game
        // from background.
        public static void CheckForAnUpdate()
        {
            _inAppUpdateManager.Call("checkForAnUpdate");
        }

        // Get current UnityPlayerActivity object from UnityPlayer class
        private static AndroidJavaObject GetCurrentAndroidActivity()
        {
            if (_currentActivity == null)
            {
                _currentActivity = new AndroidJavaClass(unityPlayer).GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _currentActivity;
        }
    }

}