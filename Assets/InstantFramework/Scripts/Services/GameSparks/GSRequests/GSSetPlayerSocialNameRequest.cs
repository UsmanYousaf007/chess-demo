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
        const string SHORT_CODE = "SetPlayerSocialName";
        const string ATT_NAME = "name";

        private Action<LogEventResponse> onSuccess;

        public IPromise<BackendResult> Send(string name, Action<LogEventResponse> onSuccess)
        {
            this.onSuccess = onSuccess;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_NAME, name)
                                 .Send(OnSuccess, OnFailure);

            return promise;
        }

        private void OnSuccess(LogEventResponse response)
        {
            if (IsActive())
            {
                onSuccess(response);
            }

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
