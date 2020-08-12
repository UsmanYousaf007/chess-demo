/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using UnityEngine;
using UpdateManager;

namespace TurboLabz.InstantFramework
{
    public class InAppUpdatesService : IInAppUpdatesService
    {
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        bool isCritical = false;

#if UNITY_IOS

        public void Init()
        {
#if !UNITY_EDITOR
            IOSUpdateManager.Initialize();
#endif
        }

        public bool IsUpdateAvailable()
        {
#if !UNITY_EDITOR
            return IOSUpdateManager.IsUpdateAvailable();
#else
            return true;
#endif
        }

        public void GoToAppStore()
        {
            IOSUpdateManager.GoToAppStore();
        }

#endif

#if UNITY_ANDROID
        private bool updateAvailable = false;

        public void Init()
        {
            AndroidUpdateManager.Init();

            // Subscribe for events from AndroidUpdateManager
            AndroidUpdateManager.OnUpdateAvailable += OnUpdateAvailable;
            AndroidUpdateManager.OnUpdateDownloading += OnUpdateDownloading;
            AndroidUpdateManager.OnUpdateDownloaded += OnUpdateDownloaded;
        }

        public bool IsUpdateAvailable()
        {
            return updateAvailable;
        }

        public void CheckForUpdate()
        {
            AndroidUpdateManager.CheckForAnUpdate();
        }

        public void DisableListeners()
        {
            // Unsubscribe for events from AndroidUpdateManager
            AndroidUpdateManager.OnUpdateAvailable -= OnUpdateAvailable;
            AndroidUpdateManager.OnUpdateDownloading -= OnUpdateDownloading;
            AndroidUpdateManager.OnUpdateDownloaded -= OnUpdateDownloaded;
        }

        public void OnUpdateAvailable(int availableVersionCode)
        {
            // set updateAvailable to true
            updateAvailable = true;
        }

        public void StartUpdate(int availableVersionCode)
        {
            // Start the process of downloading the update
            AndroidUpdateManager.StartUpdate();
        }

        public void OnUpdateDownloading(long bytesDownloaded, long totalBytesToDownload)
        {
            //statusText.text = "Downloaded: " + bytesDownloaded.ToString() + " from a total of: " + totalBytesToDownload.ToString();
        }

        public void OnUpdateDownloaded()
        {
            // Update downloaded, install it : )
            AndroidUpdateManager.CompleteUpdate();
        }
#endif
    }
}