/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using System;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> RegisterChallenge(string challengeId)
        {
            return new GSRegisterChallengeRequest().Send(challengeId);
        }
    }

    #region REQUEST

    public class GSRegisterChallengeRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "Register";
        const string ATT_CHALLENGE_ID = "challengeId";

        public IPromise<BackendResult> Send(string challengeId)
        {
            this.errorCode = BackendResult.REGISTER_CHALLENGE_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
