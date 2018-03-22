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

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        
        public IPromise<BackendResult> CheckGameVersion()
        {
            return new GSAppVersionInfoRequest().Send(OnGetAppVersionInfo);
        }

        private void OnGetAppVersionInfo(LogEventResponse response)
        {
            appInfoModel.appVersion = response.ScriptData.GetInt(GSBackendKeys.APP_VERSION_NUMBER).Value;
        }
    }
}
