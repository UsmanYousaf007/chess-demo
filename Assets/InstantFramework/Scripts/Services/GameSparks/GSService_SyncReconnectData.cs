/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using TurboLabz.TLUtils;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> SyncReconnectData(string challengeId)
        {
            return new GSSyncReconnectDataRequest().Send(challengeId, OnSyncDataSuccess);
        }

        private void OnSyncDataSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;

            string challengeId = response.ScriptData.GetString("challengeId");
            LogUtil.Log("SyncReconnectData: RESUME ChallengeID = " + challengeId, "cyan");
            if (challengeId != null && (appInfoModel.isReconnecting != DisconnectStates.LONG_DISCONNET))
            {
                if (chessboardModel.isValidChallenge(challengeId))
                {
                    LogUtil.Log("SyncReconnectData: PERFORM RESUME ChallengeID = " + challengeId, "cyan");
                    // Delete the current match and board
                    matchInfoModel.matches.Remove(challengeId);
                    chessboardModel.chessboards.Remove(challengeId);

                    GSData challengeData = response.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                    ParseChallengeData(challengeId, challengeData);
                }
            }
        }
    }

    #region REQUEST

    public class GSSyncReconnectDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SyncReconnectData";

        public IPromise<BackendResult> Send(string challengeId, Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;

            new LogEventRequest()
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute("challengeId", challengeId)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
