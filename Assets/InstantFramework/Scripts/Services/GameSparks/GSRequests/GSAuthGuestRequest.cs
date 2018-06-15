/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-20 13:19:39 UTC+05:00
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

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class GSAuthGuestRequest : GSRequestDispatcher
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();

        public IPromise<BackendResult> Send()
        {
            TLUtils.LogUtil.Log("AUTH ...", "yellow");

            GSRequestSession.Instance.AddRequest(this);
            new DeviceAuthenticationRequest().Send(OnSuccess, OnFailure);
            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnSuccess(AuthenticationResponse response)
        {
            if (!isExpired)
            {
                TLUtils.LogUtil.Log("AUTH SUCCESS", "yellow");

                DispatchResponse(BackendResult.SUCCESS);   
            }
        }

        private void OnFailure(AuthenticationResponse response)
        {
            if (!isExpired)
            {
                TLUtils.LogUtil.Log("AUTH FAIL", "red");

                DispatchResponse(BackendResult.AUTH_GUEST_REQUEST_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
            GSRequestSession.Instance.RemoveRequest(this);
        }
    }
}
