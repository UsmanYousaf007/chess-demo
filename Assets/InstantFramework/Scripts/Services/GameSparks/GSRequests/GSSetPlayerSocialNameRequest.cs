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
    public class GSSetPlayerSocialNameRequest : GSFrameworkRequest
    {
        private Action<LogEventResponse> successCallback;

        public IPromise<BackendResult> Send(string name, Action<LogEventResponse> successCallback)
        {
            this.successCallback = successCallback;
           
            string eventKey = GSEventData.SetPlayerSocialName.EVT_KEY;
            string attributeKey = GSEventData.SetPlayerSocialName.ATT_KEY_NAME;

            new LogEventRequest().SetEventKey(eventKey)
                                 .SetEventAttribute(attributeKey, name)
                                 .Send(OnSuccess, OnFailure);

            return promise;
        }

        private void OnSuccess(LogEventResponse response)
        {
            successCallback(response);
            DispatchResponse(BackendResult.SUCCESS);
        }

        private void OnFailure(LogEventResponse response)
        {
            DispatchResponse(BackendResult.SET_PLAYER_SOCIAL_NAME_FAILED);
        }

        private void DispatchResponse(BackendResult result)
        {  
            Dispatch(result);
        }
    }
}
