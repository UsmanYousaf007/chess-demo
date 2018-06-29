/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-14 15:13:25 UTC+05:00
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
using GameSparks.Core;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class GSAiResignRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SetShardFlag2";
        const string ATT_CHALLENGE_ID = "challengeId";

        public IPromise<BackendResult> Send(string challengeId)
        {
            new LogEventRequest().SetEventKey(SHORT_CODE)
                                 .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                                 .Send(OnSuccess, OnFailure);

            return promise;
        }

        private void OnSuccess(LogEventResponse response)
        {
            DispatchResponse(BackendResult.SUCCESS);
        }

        private void OnFailure(LogEventResponse response)
        {
            if (response.Errors.GetString(GSBackendKeys.CHALLENGE_INSTANCE_ID) == 
                GSBackendKeys.COMPLETED_CHALLENGE_ID)
            {
                DispatchResponse(BackendResult.CHALLENGE_COMPLETE);
            }
            else
            {
                DispatchResponse(BackendResult.AI_RESIGN_REQUEST_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            Dispatch(result);
        }
    }
}
