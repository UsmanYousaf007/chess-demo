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
    public class AppUpdateServiceEditor : IAppUpdateService
    {
        private IRoutineRunner routineRunner;

        //Signals
        [Inject] public AppUpdateSignal appUpdateSignal { get; set; }

        public void Init()
        {
            routineRunner = new NormalRoutineRunner();
            EditorUpdateManager.IsUpdateAvailableOnStore += OnIsUpdateAvailableResult;
            CheckForUpdate();
        }

        public void Terminate()
        {
            EditorUpdateManager.IsUpdateAvailableOnStore -= OnIsUpdateAvailableResult;
        }

        public void GoToStore(string url)
        {
        }

        public void CheckForUpdate()
        {
            routineRunner.StartCoroutine(EditorUpdateManager.CheckForUpdate());
        }


        public void OnIsUpdateAvailableResult(bool isUpdateAvailable)
        {
            appUpdateSignal.Dispatch(isUpdateAvailable);
        }
    }
}