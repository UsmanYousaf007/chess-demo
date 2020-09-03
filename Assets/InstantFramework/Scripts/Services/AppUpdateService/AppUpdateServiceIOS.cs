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
    public class AppUpdateServiceIOS : IAppUpdateService
    {
        [Inject] public AppUpdateSignal appUpdateSignal { get; set; }
        private IRoutineRunner routineRunner;

        public void Init()
        {
#if !UNITY_EDITOR && (UNITY_IOS)
            routineRunner = new NormalRoutineRunner();
            IOSUpdateManager.Initialize();
            IOSUpdateManager.IsUpdateAvailableOnStore += OnIsUpdateAvailableResult;
#endif
        }

        public void Terminate()
        {
            #if !UNITY_EDITOR && (UNITY_IOS)
#endif
        }

        public void CheckForUpdate()
        {
            #if !UNITY_EDITOR && (UNITY_IOS)
            routineRunner.StartCoroutine(IOSUpdateManager.CheckForUpdate());
#endif
        }

        public void OnIsUpdateAvailableResult(bool isUpdateAvailable)
        {
            #if !UNITY_EDITOR && (UNITY_IOS)
            appUpdateSignal.Dispatch(isUpdateAvailable);
#endif
        }

        public void GoToStore(string url)
        {
            Application.OpenURL(url);
        }

        public void StartUpdate(int availableVersionCode)
        {
        }

        public void OnUpdateDownloaded()
        {
        }

        public void OnUpdateDownloading(long a, long b)
        {
        }

    }
}

