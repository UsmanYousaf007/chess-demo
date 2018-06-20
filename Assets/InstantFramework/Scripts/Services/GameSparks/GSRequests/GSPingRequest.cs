/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-20 15:49:59 UTC+05:00
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

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class GSPingRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<LogEventResponse> successCallback;

        public IPromise<BackendResult> Send(Action<LogEventResponse> successCallback)
        {
            this.successCallback = successCallback;
            new LogEventRequest().SetEventKey(GSEventData.Ping.EVT_KEY)
                                 .SetEventAttribute(GSEventData.Ping.ATT_KEY_CLIENT_SEND_TIMESTAMP, TimeUtil.unixTimestampMilliseconds)
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
            DispatchResponse(BackendResult.PING_REQUEST_FAILED);
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
        }
    }
}