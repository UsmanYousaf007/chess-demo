/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppInfoModel : IAppInfoModel
    {
        public string appVersion { get; set; }
        public string appBackendVersion  { get; set; }

        public bool appVersionValid { get; set; }
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
            appVersion = "";  
            appBackendVersion = "";
            appVersionValid = false;
            iosURL = "";
            androidURL = "";
        }

        public void RetrieveAppVersion()
        {
            string[] version = Application.version.Split('.');

            appVersion = version[(int)subVersionIndex.MAJOR] + "." + version[(int)subVersionIndex.MINOR];
            appBackendVersion = version[(int)subVersionIndex.BACKEND];
        }
    }
}
