﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> CreateLongMatch(string opponentId, bool isRanked)
        {
            return new GSCreateLongMatchRequest().Send(opponentId, isRanked, OnCreateLongMatchResponse);
        }

        private void OnCreateLongMatchResponse(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            Debug.Log("-------ABORT:REASON----------");
            if (response != null &&
                response.ScriptData != null && 
                response.ScriptData.ContainsKey(GSBackendKeys.Match.ABORT_KEY))
            {
                string reason = response.ScriptData.GetString(GSBackendKeys.Match.ABORT_KEY);

                Debug.Log("-------ABORT:REASON----------"+ reason);
                matchInfoModel.createLongMatchAborted = true;
                if(reason == "LimitReached")
                {
                    matchInfoModel.createLongMatchAbortReason = CreateLongMatchAbortReason.LimitReached;
                }else if(reason == "Pending match already exists.")
                {
                    matchInfoModel.createLongMatchAbortReason = CreateLongMatchAbortReason.Pending;
                }else
                {
                    matchInfoModel.createLongMatchAbortReason = CreateLongMatchAbortReason.CreateFailed;
                }
            }
        }
    }

    #region REQUEST

    public class GSCreateLongMatchRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "CreateLongMatch";
        const string ATT_OPPONENT_ID = "opponentId";
        const string ATT_IS_RANKED = "isRanked";

        public IPromise<BackendResult> Send(string opponentId, bool isRanked, Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.CREATE_LONG_MATCH_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_OPPONENT_ID, opponentId)
                .SetEventAttribute(ATT_IS_RANKED, GSFormat.GetBool(isRanked))
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
