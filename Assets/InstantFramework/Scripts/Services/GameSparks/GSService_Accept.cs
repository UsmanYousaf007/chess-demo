/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> Accept(string challengeId)
        {
            return new GSAcceptRequest(GetRequestContext()).Send(challengeId);
        }
    }

    #region REQUEST

    public class GSAcceptRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "Accept";
        const string ATT_CHALLENGE_ID = "challengeId";

        public GSAcceptRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string challengeId)
        {
            this.errorCode = BackendResult.ACCEPT_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
