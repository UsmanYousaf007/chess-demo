 /// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-04 15:34:22 UTC+05:00
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

using TurboLabz.Patches;

namespace TurboLabz.Gamebet
{
    public class GSFindMatchRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<ChallengeStartedMessage> successCallback;

        public IPromise<BackendResult> Send(string groupId, Action<ChallengeStartedMessage> successCallback)
        {
            GSRequestSession.instance.AddRequest(this);
            this.successCallback = successCallback;
            AddListeners();

            new LogEventRequest().SetEventKey(GSEventData.FindMatch.EVT_KEY)
                                 .SetEventAttribute(GSEventData.FindMatch.ATT_KEY_MATCH_GROUP, groupId)
                                 .Send((response) => {}, OnFailure);

            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            RemoveListeners();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnFailure(LogEventResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.MATCHMAKING_REQUEST_FAILED);
            }
        }

        private void AddListeners()
        {
            ChallengeStartedMessage.Listener += OnChallengeStarted;
        }

        private void RemoveListeners()
        {
            ChallengeStartedMessage.Listener -= OnChallengeStarted;
        }

        private void OnChallengeStarted(ChallengeStartedMessage message) 
        {
            // Ignore the message if it is recurring.
            if (GSRecurringMessagePatch.isMessageRecurring(message, "GSFindMatchRequest"))
            {
                return;
            }

            if (!isExpired)
            {
                successCallback(message);
                DispatchResponse(BackendResult.SUCCESS);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            RemoveListeners();
            promise.Dispatch(result);
            GSRequestSession.instance.RemoveRequest(this);
        }
    }
}
