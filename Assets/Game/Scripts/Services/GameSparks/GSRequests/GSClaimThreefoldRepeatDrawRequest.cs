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

namespace TurboLabz.Gamebet
{
    public class GSClaimThreefoldRepeatDrawRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();

        public IPromise<BackendResult> Send(string challengeId)
        {
            GSRequestSession.instance.AddRequest(this);

            new LogChallengeEventRequest().SetChallengeInstanceId(challengeId)
                                          .SetEventKey(GSEventData.ClaimThreefoldRepeatDraw.EVT_KEY)
                                          .Send(OnSuccess, OnFailure);

            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnSuccess(LogChallengeEventResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.SUCCESS); 
            }
        }

        private void OnFailure(LogChallengeEventResponse response)
        {
            if (!isExpired)
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
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
            GSRequestSession.instance.RemoveRequest(this);
        }
    }
}
