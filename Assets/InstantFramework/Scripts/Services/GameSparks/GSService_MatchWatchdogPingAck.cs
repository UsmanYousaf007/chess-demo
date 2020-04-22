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
        public IPromise<BackendResult> MatchWatchdogPingAck(string currentTurnPlayerId, string challengerId, string challengedId, string challengeId, int moveCount)
        {
            var context = new GSFrameworkRequestContext
            {
                currentViewId = navigatorModel.currentViewId
            };

            return new GSMatchWatchdogPingAckRequest(context).Send(currentTurnPlayerId, challengerId, challengedId, challengeId, moveCount);
        }
    }

    #region REQUEST

    public class GSMatchWatchdogPingAckRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "MatchWatchdogPingAck";

        public GSMatchWatchdogPingAckRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string currentTurnPlayerId, string challengerId, string challengedId, string challengeId, int moveCount)
        {
            this.errorCode = BackendResult.WATCHDOG_PING_ACK_FAILED;

            new LogEventRequest()
                .SetEventAttribute("currentTurnPlayerId", currentTurnPlayerId)
                .SetEventAttribute("challengerId", challengerId)
                .SetEventAttribute("challengedId", challengedId)
                .SetEventAttribute("challengeId", challengeId)
                .SetEventAttribute("moveCount", moveCount)
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
