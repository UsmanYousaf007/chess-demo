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
        public IPromise<BackendResult> CreateLongMatch(string opponentId)
        {
            return new GSCreateLongMatchRequest().Send(opponentId, OnCreateLongMatchResponse);
        }

        private void OnCreateLongMatchResponse(object r)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response != null &&
                response.ScriptData != null && 
                response.ScriptData.ContainsKey(GSBackendKeys.Match.ABORT_KEY))
            {
                matchInfoModel.createLongMatchAborted = true;
            }
        }
    }

    #region REQUEST

    public class GSCreateLongMatchRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "CreateLongMatch";
        const string ATT_OPPONENT_ID = "opponentId";

        public IPromise<BackendResult> Send(string opponentId, Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.CREATE_LONG_MATCH_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_OPPONENT_ID, opponentId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
