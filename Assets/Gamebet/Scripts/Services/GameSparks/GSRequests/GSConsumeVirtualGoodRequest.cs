/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor Khawaja <noor.khawaja@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-23-01 14:45:19 UTC+05:00
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

namespace TurboLabz.Gamebet
{
    public class GSConsumeVirtualGoodRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<ConsumeVirtualGoodResponse> successCallback;

        public IPromise<BackendResult> Send(                     
            int quantity,                                         // The number of items to consume
            string shortCode,                                     // The short code of the virtual good to be consumed
            Action<ConsumeVirtualGoodResponse> successCallback)
        {
            GSRequestSession.instance.AddRequest(this);
            this.successCallback = successCallback;

            new ConsumeVirtualGoodRequest()
                .SetQuantity(quantity)
                .SetShortCode(shortCode)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnSuccess(ConsumeVirtualGoodResponse response)
        {
            if (!isExpired)
            {
                successCallback(response);
                DispatchResponse(BackendResult.SUCCESS);
            }
        }

        private void OnFailure(ConsumeVirtualGoodResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.CONSUME_VIRTUAL_GOOD_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
            GSRequestSession.instance.RemoveRequest(this);
        }
    }
}

