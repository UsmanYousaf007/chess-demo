/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-09-13 17:54:23 UTC+05:00
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

using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class FBInitRequest
    {
        private IPromise<FacebookResult> promise = new Promise<FacebookResult>();

        public IPromise<FacebookResult> Send()
        {
            Assertions.Assert(!FB.IsInitialized, "Facebook must not already be initialized!");

            FB.Init(OnInit);
            return promise;
        }

        private void OnInit()
        {
            if (FB.IsInitialized)
            {
                DispatchResponse(FacebookResult.SUCCESS);
            }
            else
            {
                OnInitFailure();
            }
        }

        private void OnInitFailure()
        {
            LogUtil.Log(this.GetType().Name + ": Facebook init failure", "red");
            DispatchResponse(FacebookResult.FAILURE);
        }

        private void DispatchResponse(FacebookResult result)
        {
            promise.Dispatch(result);
        }
    }
}
