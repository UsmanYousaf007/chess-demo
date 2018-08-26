/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:32:51 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using System;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> ClaimFiftyMoveDraw()
        {
            return new GSClaimFiftyMoveDrawRequest().Send(matchInfoModel.activeChallengeId);
        }
    }

    #region REQUEST

    public class GSClaimFiftyMoveDrawRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "ClaimFiftyMoveDraw";

        public IPromise<BackendResult> Send(string challengeId)
        {
            this.errorCode = BackendResult.CLAIM_FIFTY_MOVE_DRAW_FAILED;

            new LogChallengeEventRequest().SetChallengeInstanceId(challengeId)
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
