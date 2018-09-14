﻿/// @license Propriety <http://license.url>
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
        public IPromise<BackendResult> Unregister(string challengeId)
        {
            return new GSUnregisterRequest().Send(challengeId, OnUnregisterSuccess);
        }

        private void OnUnregisterSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response.ScriptData != null)
            {
                ParseActiveChallenges(response.ScriptData);
            }
        }
    }


    #region REQUEST

    public class GSUnregisterRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "Unregister";
        const string ATT_CHALLENGE_ID = "challengeId";

        public IPromise<BackendResult> Send(string challengeId, Action<object> onSuccess)
        {
            this.errorCode = BackendResult.UNREGISTER_FAILED;
            this.onSuccess = onSuccess;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
