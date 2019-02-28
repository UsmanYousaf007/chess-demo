﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppInfoModel : IAppInfoModel
    {
        public int appBackendVersion  { get; set; }
        public bool appBackendVersionValid { get; set; }
        public string iosURL { get; set; }
        public string androidURL { get; set; }
        public int rateAppThreshold { get; set; }
        public int onlineCount { get; set; }

        enum subVersionIndex
        {
            MAJOR = 0,
            MINOR = 1,
            BACKEND = 2
        }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            string[] version = Application.version.Split('.');
            appBackendVersion = int.Parse(version[(int)subVersionIndex.BACKEND]);

            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            appBackendVersionValid = false;
            iosURL = "";
            androidURL = "";
            rateAppThreshold = 0;
            onlineCount = 0;
        }
    }
}
