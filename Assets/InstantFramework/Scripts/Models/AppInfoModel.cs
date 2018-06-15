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

        enum subVersionIndex
        {
            MAJOR = 0,
            MINOR = 1,
            BACKEND = 2
        }

        public void Reset()
        {
            appBackendVersionValid = false;
            iosURL = "";
            androidURL = "";

            string[] version = Application.version.Split('.');
            appBackendVersion = int.Parse(version[(int)subVersionIndex.BACKEND]);
        }
    }
}
