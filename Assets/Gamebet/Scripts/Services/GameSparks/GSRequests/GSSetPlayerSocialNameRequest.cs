/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-07 21:47:49 UTC+05:00
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

using System;
using System.Collections.Generic;

using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.Gamebet
{
    public class GSSetPlayerSocialNameRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<LogEventResponse> successCallback;

        public IPromise<BackendResult> Send(string name, Action<LogEventResponse> successCallback)
        {
            GSRequestSession.instance.AddRequest(this);
            this.successCallback = successCallback;
           
            string eventKey = GSEventData.SetPlayerSocialName.EVT_KEY;
            string attributeKey = GSEventData.SetPlayerSocialName.ATT_KEY_NAME;

            new LogEventRequest().SetEventKey(eventKey)
                                 .SetEventAttribute(attributeKey, name)
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
                successCallback(response);
                DispatchResponse(BackendResult.SUCCESS);
            }
        }

        private void OnFailure(LogEventResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.SET_PLAYER_SOCIAL_NAME_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
            GSRequestSession.instance.RemoveRequest(this);
        }
    }
}
