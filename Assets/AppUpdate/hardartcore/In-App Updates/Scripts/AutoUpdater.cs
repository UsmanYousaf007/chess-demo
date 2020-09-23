﻿using UnityEngine;

namespace UpdateManager {
    [RequireComponent (typeof (AndroidUpdateManager))]
    public class AutoUpdater : MonoBehaviour {

        private AndroidUpdateManager _updateManager;

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

        private void Awake () {
            _updateManager = GetComponent<AndroidUpdateManager> ();
            // This line trigger the check for the update.
            // If you want to check for an update in another place, you should
            // move this function call.
            _updateManager.CheckForAnUpdate ();
        }

        private void OnUpdateAvailable (int availableVersionCode) {
            // Start the process of downloading the update
            _updateManager.StartUpdate ();
        }

        private void OnUpdateDownloading (long bytesDownloaded, long totalBytesToDownload) {
            // Update an UI element with a proper message about the download process
        }

        private void OnUpdateDownloaded () {
            // Update downloaded, install it : )
            _updateManager.CompleteUpdate ();
        }

    }
}