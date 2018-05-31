/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author #AUTHOR# <#AUTHOR_EMAIL#>
/// @company #COMPANY# <#COMPANY_URL#>
/// @date #DATE#
/// 
/// @description
/// #DESCRIPTION#

namespace TurboLabz.InstantFramework
{
    public interface IAppInfoModel
    {
        void Reset();
        int appVersion { get; set; }
        bool appVersionValid { get; set; }
        string iosURL { get; set; }
        string androidURL { get; set; }
    }
}
