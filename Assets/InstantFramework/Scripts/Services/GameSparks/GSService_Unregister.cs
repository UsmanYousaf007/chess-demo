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
        public IPromise<BackendResult> Unregister(string challengeId)
        {
            return new GSUnregisterRequest().Send(challengeId, OnUnregisterSuccess);
        }

        // TODO: move this logic into the unregister command
        private void OnUnregisterSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            ParseActiveChallenges(response.ScriptData);

            bool isFriendRemoved = response.ScriptData.GetBoolean(GSBackendKeys.IS_FRIEND_REMOVED).Value;
            string challengeId = response.ScriptData.GetString(GSBackendKeys.CHALLENGE_ID);
            string opponentId = response.ScriptData.GetString(GSBackendKeys.OPPONENT_ID);

            if (isFriendRemoved)
            {
                clearFriendSignal.Dispatch(opponentId);
                playerModel.friends.Remove(opponentId);
            }

            matchInfoModel.matches.Remove(challengeId);
            chessboardModel.chessboards.Remove(challengeId);
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
