/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:32:51 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using System;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> PlayerResign()
        {
            return new GSPlayerResignRequest().Send(matchInfoModel.activeMatch.challengeId);
        }
    }

    #region REQUEST

    public class GSPlayerResignRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "Resign";
        const string ATT_CHALLENGE_ID = "challengeId";

        public IPromise<BackendResult> Send(string challengeId)
        {
            this.errorCode = BackendResult.RESIGN_REQUEST_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
