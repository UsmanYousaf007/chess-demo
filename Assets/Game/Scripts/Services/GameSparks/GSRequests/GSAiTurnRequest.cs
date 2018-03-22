/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-16 17:01:55 UTC+05:00
/// 
/// @description
/// In the request classes for services the Send() method always returns a
/// promise with SomeResultType as a type parameter:
/// 
/// IPromise<BackendResult> Send()
/// IPromise<SocialServiceResult> Send()
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

namespace TurboLabz.Gamebet
{
    public class GSAiTurnRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();

        public IPromise<BackendResult> Send(string challengeId,
                                            string from,
                                            string to,
                                            string promotion)
        {
            GSRequestSession.instance.AddRequest(this);

            new LogEventRequest().SetEventKey(GSEventData.AiTurn.EVT_KEY)
                                 .SetEventAttribute(GSEventData.AiTurn.ATT_KEY_CHALLENGE_ID, challengeId)
                                 .SetEventAttribute(GSEventData.AiTurn.ATT_KEY_FROM, from)
                                 .SetEventAttribute(GSEventData.AiTurn.ATT_KEY_TO, to)
                                 .SetEventAttribute(GSEventData.AiTurn.ATT_KEY_PROMOTION, promotion)
                                 .Send(OnSuccess, OnFailure);

            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnSuccess(LogEventResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.SUCCESS); 
            }
        }

        private void OnFailure(LogEventResponse response)
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
                    DispatchResponse(BackendResult.AI_TAKE_TURN_REQUEST_FAILED);
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
