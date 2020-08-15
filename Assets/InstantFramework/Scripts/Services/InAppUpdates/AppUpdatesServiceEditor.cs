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
    public class AppUpdatesServiceEditor : IAppUpdatesService
    {
        public bool updateLater { get; set; }

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
            // TODO: implement me 
        }

        public void Terminate()
        {
            // TODO: implement me
        }

        public bool IsUpdateAvailable()
        {
            return true;
        }

        public void GoToStore(string url)
        {
            // TODO: implement me
        }

        public void CheckForUpdate()
        {
            // TODO: implement me
        }

        public void OnUpdateAvailable(int availableVersionCode)
        {
            // TODO: implement me
        }

        public void StartUpdate(int availableVersionCode)
        {
            // TODO: implement me
        }

        public void OnUpdateDownloaded()
        {
            // TODO: implement me
        }
    }
}