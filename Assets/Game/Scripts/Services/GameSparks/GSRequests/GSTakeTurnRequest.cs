/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author #AUTHOR# <#AUTHOR_EMAIL#>
/// @company #COMPANY# <#COMPANY_URL#>
/// @date #DATE#
/// 
/// @description
/// In the request classes for services the Send() method always returns a
/// promise with BackendResult as a type parameter:
/// 
/// IPromise<BackendResult> Send()
/// 
/// We can return more data using more type parameters but if the returned type
/// is specific to the service itself then we need to shield the world outside
/// the service to not receive service specific type parameters. For that
/// purpose we use a callback as a parameter to the Send() method e.g.:
/// 
/// IPromise<BackendResult> Send(Action<SomeServiceSpecificType> callback)
/// 
/// instead of doing this
/// 
/// IPromise<BackendResult, SomeServiceSpecificType> Send()
/// 
/// However these would be valid:
/// 
/// IPromise<BackendResult, string> Send()
/// IPromise<BackendResult, SomeGenericType> Send()

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using GameSparks.Core;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class GSTakeTurnRequest : GSFrameworkRequest
    {
        public IPromise<BackendResult> Send(string challengeId,
                                            string from,
                                            string to,
                                            string promotion,
                                            int claimFiftyMoveDraw,
                                            int claimThreefoldRepeatDraw,
                                            int rejectThreefoldRepeatDraw)
        {
            new LogChallengeEventRequest().SetChallengeInstanceId(challengeId)
                                          .SetEventKey(GSEventData.TakeTurn.EVT_KEY)
                                          .SetEventAttribute(GSEventData.TakeTurn.ATT_KEY_FROM, from)
                                          .SetEventAttribute(GSEventData.TakeTurn.ATT_KEY_TO, to)
                                          .SetEventAttribute(GSEventData.TakeTurn.ATT_KEY_PROMOTION, promotion)
                                          .SetEventAttribute(GSEventData.TakeTurn.ATT_KEY_CLAIM_FIFTY_MOVE_DRAW, claimFiftyMoveDraw)
                                          .SetEventAttribute(GSEventData.TakeTurn.ATT_KEY_CLAIM_THREEFOLD_REPEAT_DRAW, claimThreefoldRepeatDraw)
                                          .SetEventAttribute(GSEventData.TakeTurn.ATT_KEY_REJECT_THREEFOLD_REPEAT_DRAW, rejectThreefoldRepeatDraw)
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
                DispatchResponse(BackendResult.TAKE_TURN_REQUEST_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            Dispatch(result);
        }
    }
}
