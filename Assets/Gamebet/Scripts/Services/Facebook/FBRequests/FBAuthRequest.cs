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

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class FBAuthRequest
    {
        private IPromise<FacebookResult, string> promise = new Promise<FacebookResult, string>();
        private IEnumerator loginTimeoutCR;

        // TODO(mubeeniqbal): According to the below comment the check might be
        // wrong. Maybe the right check is
        // !UNITY_EDITOR && (UNITY_ANDROID && UNITY_IOS)
        // Not sure though. Need to check.
        //
        // The code inside the preprocessor check is only needed when the app
        // runs on a device. This is because the app goes into background on a
        // device but not when running on Unity editor.
        #if !UNITY_EDITOR
        // Utilities
        private IAppEventDispatcher appEventDispatcher = new AppEventDispatcher();
        private IRoutineRunner routineRunner = new NormalRoutineRunner();

        public FBAuthRequest()
        {
            appEventDispatcher.appResumedSignal.AddOnce(StartLoginTimeout);
        }

        private void StartLoginTimeout()
        {
            LogUtil.Log(this.GetType().Name + ": StartLoginTimeout()", "yellow");
            Assertions.Assert(loginTimeoutCR == null, "loginTimeoutCR must be null!");

            loginTimeoutCR = LoginTimeoutCR();
            routineRunner.StartCoroutine(loginTimeoutCR);
        }

        private void StopLoginTimeout()
        {
            LogUtil.Log(this.GetType().Name + ": StopLoginTimeout()", "yellow");
            Assertions.Assert(loginTimeoutCR != null, "loginTimeoutCR must not be null!");

            routineRunner.StopCoroutine(loginTimeoutCR);
            loginTimeoutCR = null;
        }

        private IEnumerator LoginTimeoutCR()
        {
            int frameCount = 0;

            while (frameCount <= FBSettings.LOGIN_TIMEOUT_FRAME_COUNT)
            {
                ++frameCount;
                yield return null;
            }

            LogUtil.Log(this.GetType().Name + ": Login Timeout!", "red");
            OnLoginCancelled();
            StopLoginTimeout();
        }
        #endif

        public IPromise<FacebookResult, string> Send()
        {
            Assertions.Assert(FB.IsInitialized, "Facebook must already be initialized!");

            Login();
            
            return promise;
        }

        private void Login()
        {
            if (FB.IsLoggedIn)
            {
                OnLoginSuccess();
            }
            else
            {
                List<string> permissions = new List<string>() {
                    FBPermissions.PUBLIC_PROFILE,
                    FBPermissions.USER_FRIENDS
                };

                FB.LogInWithReadPermissions(permissions, OnLogInWithReadPermissions);
            }
        }

        private void OnLogInWithReadPermissions(ILoginResult result)
        {
            #if !UNITY_EDITOR
            StopLoginTimeout();
            #endif

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
            Assertions.Assert(AccessToken.CurrentAccessToken != null, "CurrentAccessToken must not be null after logging in to Facebook!");

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
