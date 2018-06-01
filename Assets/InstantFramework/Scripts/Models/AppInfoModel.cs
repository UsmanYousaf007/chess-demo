/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class AppInfoModel : IAppInfoModel
    {
        public int appVersion { get; set; }
        public bool appVersionValid { get; set; }
        public string iosURL { get; set; }
        public string androidURL { get; set; }

        public void Reset()
        {
            appVersion = 0;  
            appVersionValid = false;
        }
    }
}
