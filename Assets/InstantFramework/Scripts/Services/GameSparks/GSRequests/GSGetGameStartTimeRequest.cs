/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-13 20:08:07 UTC+05:00
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

using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class GSGetGameStartTimeRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<ScriptMessage> successCallback;

        public IPromise<BackendResult> Send(string challengeId, Action<ScriptMessage> successCallback)
        {
            this.successCallback = successCallback;
            AddListeners();

            string eventKey = GSEventData.GetGameStartTime.EVT_KEY;
            string attributeKey = GSEventData.GetGameStartTime.ATT_KEY_CHALLENGE_ID;

            new LogEventRequest().SetEventKey(eventKey)
                                 .SetEventAttribute(attributeKey, challengeId)
                                 .Send(null, OnFailure);

            return promise;
        }

        private void OnFailure(LogEventResponse response)
        {
            DispatchResponse(BackendResult.GET_GAME_START_TIME_REQUEST_FAILED);
        }

        private void AddListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
        }

        private void RemoveListeners()
        {
            ScriptMessage.Listener -= OnScriptMessage;
        }

        private void OnScriptMessage(ScriptMessage message) 
        {
            if (message.ExtCode == GSBackendKeys.START_GAME_MESSAGE)
            {
                successCallback(message);
                DispatchResponse(BackendResult.SUCCESS);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            RemoveListeners();
            promise.Dispatch(result);
        }
    }
}
