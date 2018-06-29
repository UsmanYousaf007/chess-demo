/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-10 11:34:43 UTC+05:00
///
/// @description
/// [add_description_here]

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using GameSparks.Core;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class GSClaimThreefoldRepeatDrawRequest : GSFrameworkRequest
    {
        public IPromise<BackendResult> Send(string challengeId)
        {

            new LogChallengeEventRequest().SetChallengeInstanceId(challengeId)
                .SetEventKey(GSEventData.ClaimThreefoldRepeatDraw.EVT_KEY)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        private void OnSuccess(LogChallengeEventResponse response)
        {
            DispatchResponse(BackendResult.SUCCESS); 
        }

        private void OnFailure(LogChallengeEventResponse response)
        {
            if (response.Errors.GetString(GSBackendKeys.CHALLENGE_INSTANCE_ID) == 
                GSBackendKeys.COMPLETED_CHALLENGE_ID)
            {
                DispatchResponse(BackendResult.CHALLENGE_COMPLETE);
            }
            else
            {
                DispatchResponse(BackendResult.CLAIM_THREEFOLD_REPEAT_DRAW_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            Dispatch(result);
        }
    }
}
