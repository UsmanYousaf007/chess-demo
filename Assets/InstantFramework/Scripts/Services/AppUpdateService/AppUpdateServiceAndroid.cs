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
    public class AppUpdatesServiceAndroid : IAppUpdateService
    {
        //public bool updateLater { get; set; }

        [Inject] public AppUpdateSignal appUpdateSignal { get; set; }

        private bool updateAvailable;
        private int appVersion;
        // Listen to signals
        //[Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        //[PostConstruct]
        //public void PostConstruct()
        //{
        //modelsResetSignal.AddListener(Reset);
        //}

        //private void Reset()
        //{
        //    updateLater = false;
        //}


        public void Init(int appVersion)
        {
            this.appVersion = appVersion;
            AndroidAppUpdateManager.Init();
            AndroidAppUpdateManager.IsUpdateAvailableOnStore += OnIsUpdateAvailableResult;
            
        }

        public void Terminate()
        {
            AndroidAppUpdateManager.IsUpdateAvailableOnStore -= OnIsUpdateAvailableResult;

        }

        public bool IsUpdateAvailable()
        {
            return updateAvailable;
        }

        public void GoToStore(string url)
        {
            Application.OpenURL(url);
        }

        public void CheckForUpdate()
        {
            AndroidAppUpdateManager.CheckForAnUpdate();
        }

        public void OnIsUpdateAvailableResult(bool isUpdateAvailable)
        {
            this.updateAvailable = isUpdateAvailable;
            appUpdateSignal.Dispatch(isUpdateAvailable);
        }

        public void StartUpdate(int availableVersionCode)
        {
            // Start the process of downloading the update
            AndroidAppUpdateManager.StartUpdate();
        }

        public void OnUpdateDownloading(long bytesDownloaded, long totalBytesToDownload)
        {
            //statusText.text = "Downloaded: " + bytesDownloaded.ToString() + " from a total of: " + totalBytesToDownload.ToString();
        }

        public void OnUpdateDownloaded()
        {
            // Update downloaded, install it : )
            AndroidAppUpdateManager.CompleteUpdate();
        }
    }
}

