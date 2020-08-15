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
    public class AppUpdatesServiceAndroid : IAppUpdatesService
    {
        public bool updateLater { get; set; }

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }

        private bool updateAvailable = false;

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            updateLater = false;
        }


        public void Init()
        {
            AndroidAppUpdateManager.Init();

            // Subscribe for events from AndroidUpdateManager
            AndroidAppUpdateManager.OnUpdateAvailable += OnUpdateAvailable;
            AndroidAppUpdateManager.OnUpdateDownloading += OnUpdateDownloading;
            AndroidAppUpdateManager.OnUpdateDownloaded += OnUpdateDownloaded;
        }

        public void Terminate()
        {
            // Unsubscribe for events from AndroidUpdateManager
            AndroidAppUpdateManager.OnUpdateAvailable -= OnUpdateAvailable;
            AndroidAppUpdateManager.OnUpdateDownloading -= OnUpdateDownloading;
            AndroidAppUpdateManager.OnUpdateDownloaded -= OnUpdateDownloaded;
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

        public void OnUpdateAvailable(int availableVersionCode)
        {
            // set updateAvailable to true
            updateAvailable = true;
            /*navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);
            var vo = new ConfirmDlgVO
            {
                title = "Update",
                desc = "Update available",
                yesButtonText = "Ok",
                onClickYesButton = delegate
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                }
            };
            updateConfirmDlgSignal.Dispatch(vo);*/
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