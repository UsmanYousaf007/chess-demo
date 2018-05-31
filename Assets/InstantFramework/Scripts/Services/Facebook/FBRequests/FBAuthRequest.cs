/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-29 19:18:41 UTC+05:00
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

using System.Collections;
using System.Collections.Generic;

using Facebook.Unity;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class FBAuthRequest
    {
        private IPromise<FacebookResult, string> promise = new Promise<FacebookResult, string>();
        private IEnumerator loginTimeoutCR;

        public IPromise<FacebookResult, string> Send()
        {
             // Login
            List<string> permissions = new List<string>() {
                FBPermissions.PUBLIC_PROFILE,
                FBPermissions.USER_FRIENDS
            };

            FB.LogInWithReadPermissions(permissions, OnLogInWithReadPermissions);
            
            return promise;
        }
            
        private void OnLogInWithReadPermissions(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                OnLoginSuccess();
            }
            else if (result.Cancelled)
            {
                OnLoginCancelled();
            }
            else
            {
                OnLoginFailure();
            }
        }

        private void OnLoginSuccess()
        {
            LogUtil.Log(this.GetType().Name + ": Facebook login successfull", "green");
            DispatchResponse(FacebookResult.SUCCESS, AccessToken.CurrentAccessToken.TokenString);
        }

        private void OnLoginFailure()
        {
            LogUtil.Log(this.GetType().Name + ": Facebook login failure", "red");
            DispatchResponse(FacebookResult.FAILURE, null);
        }

        private void OnLoginCancelled()
        {
            LogUtil.Log(this.GetType().Name + ": Facebook login cancelled", "red");
            DispatchResponse(FacebookResult.CANCELLED, null);
        }

        private void DispatchResponse(FacebookResult result, string accessToken)
        {
            promise.Dispatch(result, accessToken);
        }
    }
}
