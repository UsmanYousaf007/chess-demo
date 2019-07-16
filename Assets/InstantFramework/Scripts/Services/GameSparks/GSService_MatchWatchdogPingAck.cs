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
        public IPromise<BackendResult> MatchWatchdogPingAck(string currentTurnPlayerId, string challengerId, string challengedId, string challengeId)
        {
            return new GSMatchWatchdogPingAckRequest().Send(currentTurnPlayerId, challengerId, challengedId, challengeId);
        }
    }

    #region REQUEST

    public class GSMatchWatchdogPingAckRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "MatchWatchdogPingAck";

        public IPromise<BackendResult> Send(string currentTurnPlayerId, string challengerId, string challengedId, string challengeId)
        {
            new LogEventRequest()
                .SetEventAttribute("currentTurnPlayerId", currentTurnPlayerId)
                .SetEventAttribute("challengerId", challengerId)
                .SetEventAttribute("challengedId", challengedId)
                .SetEventAttribute("challengeId", challengeId)
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
