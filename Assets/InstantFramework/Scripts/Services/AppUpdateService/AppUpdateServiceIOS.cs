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
    public class AppUpdatesServiceIOS : IAppUpdateService
    {
        [Inject] public AppUpdateSignal appUpdateSignal { get; set; }

        private bool updateAvailable;
        private int appVersion;
        //public bool updateLater { get; set; }

        // Listen to signals
        //[Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        //[PostConstruct]
        //public void PostConstruct()
        //{
        //    modelsResetSignal.AddListener(Reset);
        //}

        //private void Reset()
        //{
        //    updateLater = false;
        //}

        public void Init(int appVersion)
        {
            this.appVersion = appVersion;
            IOSUpdateManager.Initialize();
            IOSUpdateManager.IsUpdateAvailableOnStore += OnIsUpdateAvailableResult;
        }

        public void Terminate()
        {
            IOSUpdateManager.IsUpdateAvailableOnStore -= OnIsUpdateAvailableResult;
        }

        public void CheckForUpdate()
        {
            IOSUpdateManager.CheckForUpdate();
        }

        public void OnIsUpdateAvailableResult(bool isUpdateAvailable)
        {
            this.updateAvailable = isUpdateAvailable;
            appUpdateSignal.Dispatch(isUpdateAvailable);
        }

        public bool IsUpdateAvailable()
        {
            return updateAvailable;
        }

        public void GoToStore(string url)
        {
            Application.OpenURL(url);
        }

        public void StartUpdate(int availableVersionCode)
        {
            // TODO: implement me
        }

        public void OnUpdateDownloaded()
        {
            // TODO: implement me
        }

        public void OnUpdateDownloading(long a, long b)
        {
            // TODO: implement me
        }
    }
}

