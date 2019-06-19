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
        public IPromise<BackendResult> ResumeMatchData()
        {
            return new GSResumeMatchDataRequest().Send(OnResumeMatchDataSuccess);
        }

        private void OnResumeMatchDataSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            ParseActiveChallenges(response.ScriptData);
        }
    }

    #region REQUEST

    public class GSResumeMatchDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "ResumeMatchData";

        public IPromise<BackendResult> Send(Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.RESUME_MATCH_DATA_FAILED;

            new LogEventRequest()
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
