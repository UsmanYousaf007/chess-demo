using UnityEngine;
using UnityEngine.UI;
using UpdateManager;

namespace DemoUpdateManager {
    public class UiManager : MonoBehaviour {

#if UNITY_ANDROID
        public AndroidUpdateManager androidUpdateManager;
        public Text statusText;

        private void OnEnable () {
            // Subscribe for events from AndroidUpdateManager
            AndroidUpdateManager.OnUpdateAvailable += OnUpdateAvailable;
            AndroidUpdateManager.OnUpdateDownloading += OnUpdateDownloading;
            AndroidUpdateManager.OnUpdateDownloaded += OnUpdateDownloaded;
        }

        private void OnDisable () {
            // Unsubscribe for events from AndroidUpdateManager
            AndroidUpdateManager.OnUpdateAvailable -= OnUpdateAvailable;
            AndroidUpdateManager.OnUpdateDownloading -= OnUpdateDownloading;
            AndroidUpdateManager.OnUpdateDownloaded -= OnUpdateDownloaded;
        }

        private void Start () {
            androidUpdateManager.CheckForAnUpdate ();
        }

        private void OnUpdateAvailable (int availableVersionCode) {
            // Start the process of downloading the update
            androidUpdateManager.StartUpdate ();
        }

        private void OnUpdateDownloading (long bytesDownloaded, long totalBytesToDownload) {
            statusText.text = "Downloaded: " + bytesDownloaded.ToString () + " from a total of: " + totalBytesToDownload.ToString ();
        }

        private void OnUpdateDownloaded () {
            // Update downloaded, install it : )
            statusText.text = "Status: Update downloaded";
            androidUpdateManager.CompleteUpdate ();
        }
#endif

    }
}