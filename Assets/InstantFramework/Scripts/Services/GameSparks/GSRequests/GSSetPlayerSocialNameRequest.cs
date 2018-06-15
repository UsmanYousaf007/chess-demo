/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class GSSetPlayerSocialNameRequest : GSRequestDispatcher
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<LogEventResponse> successCallback;

        public IPromise<BackendResult> Send(string name, Action<LogEventResponse> successCallback)
        {
            GSRequestSession.Instance.AddRequest(this);
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
            GSRequestSession.Instance.RemoveRequest(this);
        }
    }
}
