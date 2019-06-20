/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Core;
using TurboLabz.Chess;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {

        [Inject] public PlayerTimerExpiredSignal playerTimerExpiredSignal { get; set; }
        [Inject] public OpponentTimerExpiredSignal opponentTimerExpiredSignal { get; set; }

        public IPromise<BackendResult> ResumeMatchData()
        {
            return new GSResumeMatchDataRequest().Send(OnResumeMatchDataSuccess);
        }

        private void OnResumeMatchDataSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            GSData challengeData = response.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            string challengeId = response.ScriptData.GetString("challengeId");

            // Reset this challenge because it is going to be re-loaded
            matchInfoModel.matches.Remove(challengeId);
            chessboardModel.chessboards.Remove(challengeId);

            playerTimerExpiredSignal.Dispatch();
            opponentTimerExpiredSignal.Dispatch();

            ParseChallengeData(challengeId, challengeData);

            RunTimeControlVO vo;
                vo.pauseAfterSwap = false;
                vo.waitingForOpponentToAccept = false;
                vo.playerJustAcceptedOnPlayerTurn = false;
                runTimeControlSignal.Dispatch(vo);
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
