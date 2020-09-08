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
    public class AppUpdateServiceAndroid : IAppUpdateService
    {
        private IRoutineRunner routineRunner;

        //Signals
        [Inject] public AppUpdateSignal appUpdateSignal { get; set; }
       
        public void Init()
        {
            routineRunner = new NormalRoutineRunner();
            AndroidAppUpdateManager.Init();
            AndroidAppUpdateManager.IsUpdateAvailableOnStore += OnIsUpdateAvailableResult;
        }

        public void Terminate()
        {
            AndroidAppUpdateManager.IsUpdateAvailableOnStore -= OnIsUpdateAvailableResult;

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
            appUpdateSignal.Dispatch(isUpdateAvailable);
        }

        public void StartUpdate(int availableVersionCode)
        {
            AndroidAppUpdateManager.StartUpdate();
        }

        public void OnUpdateDownloading(long bytesDownloaded, long totalBytesToDownload)
        {
        }

        public void OnUpdateDownloaded()
        {
            AndroidAppUpdateManager.CompleteUpdate();
        }
    }
}

