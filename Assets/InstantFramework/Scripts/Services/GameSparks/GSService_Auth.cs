/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEditor;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public bool isAuthenticated
        {
            get { return GS.Authenticated; }
        }

        public IPromise<BackendResult> AuthGuest()
        {
            return new GSAuthGuestRequest(GetRequestContext()).Send(OnGuestAuthSuccess);
        }

        public IPromise<BackendResult> AuthFacebook(string accessToken, bool existingPlayer)
        {
            return new GSAuthFacebookRequest(GetRequestContext()).Send(accessToken, existingPlayer, (existingPlayer == true ? (Action<object, Action<object>>)null : onFacebookAuthSuccess));
        }

        public IPromise<BackendResult> AuthSignInWithApple(string authorizationCode, bool existingPlayer)
        {
            return new GSAuthSignInWithAppleRequest(GetRequestContext()).Send(authorizationCode, existingPlayer, (existingPlayer == true ? (Action<object, Action<object>>)null : onSignInWithAppleAuthSuccess));
        }

        public IPromise<BackendResult> AuthEmail(string email, string password, bool existingPlayer)
        {
            return new GSAuthEmailResquest(GetRequestContext()).Send(email, password, existingPlayer, (existingPlayer == true ? (Action<object, Action<object>>)null : onEmailAuthSuccess));
        }

        private void OnGuestAuthSuccess(object r, Action<object> a)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.newUser = (bool)response.NewPlayer;
        }

        private void onFacebookAuthSuccess(object r, Action<object> a)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.id = response.UserId;

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);
        }

        private void onSignInWithAppleAuthSuccess(object r, Action<object> a)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.id = response.UserId;

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);
        }

        private void onEmailAuthSuccess(object r, Action<object> a)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.id = response.UserId;

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);
        }

        public IPromise<BackendResult> SetPlayerSocialName(string name)
        {
            return new GSSetPlayerSocialNameRequest(GetRequestContext()).Send(name, OnSetPlayerSocialNameSuccess);
        }

        private void OnSetPlayerSocialNameSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;
            playerModel.name = response.ScriptData.GetString(GSBackendKeys.DISPLAY_NAME);

            if(String.IsNullOrEmpty(playerModel.editedName))
            {
                playerModel.name = FormatUtil.SplitFirstLastNameInitial(playerModel.name);
            }
            
        }
    }

    public class GSAuthFacebookRequest : GSFrameworkRequest
    {
        public GSAuthFacebookRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string accessToken, bool existingPlayer, Action<object, Action<object>> onFacebookAuthSuccess)
        {
            this.onSuccess = onFacebookAuthSuccess;
            this.errorCode = BackendResult.AUTH_FACEBOOK_REQUEST_FAILED;

            GSRequestData scriptData = new GSRequestData();
            scriptData.AddBoolean("existingPlayer", existingPlayer);

            new FacebookConnectRequest()
                .SetScriptData(scriptData)
                .SetAccessToken(accessToken)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    public class GSAuthSignInWithAppleRequest : GSFrameworkRequest
    {
        public GSAuthSignInWithAppleRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string authorizationCode, bool existingPlayer, Action<object, Action<object>> onSignInWithAppleAuthSuccess)
        {
            this.onSuccess = onSignInWithAppleAuthSuccess;
            this.errorCode = BackendResult.AUTH_SIGN_IN_WITH_APPLE_FAILED;

            GSRequestData scriptData = new GSRequestData();
            scriptData.AddBoolean("existingPlayer", existingPlayer);

            new SignInWithAppleConnectRequest()
                .SetScriptData(scriptData)
                .SetClientId("com.turbolabz.instantchess.ios")
                .SetAuthorizationCode(authorizationCode)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    public class GSAuthEmailResquest : GSFrameworkRequest
    {
        public GSAuthEmailResquest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string email, string password, bool existingPlayer, Action<object, Action<object>> onEmailAuthSuccess)
        {
            this.onSuccess = onEmailAuthSuccess;
            this.errorCode = BackendResult.AUTH_EMAIL_REQUEST_FAILED;

            GSRequestData scriptData = new GSRequestData();
            scriptData.AddBoolean("existingPlayer", existingPlayer);

            new AuthenticationRequest()
                .SetScriptData(scriptData)
                .SetPassword(password)
                .SetUserName(email)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    public class GSAuthGuestRequest : GSFrameworkRequest
    {
        public GSAuthGuestRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(Action<object, Action<object>> onSuccess)
        {
            this.errorCode = BackendResult.AUTH_GUEST_REQUEST_FAILED;
            this.onSuccess = onSuccess;

#if UNITY_ANDROID || UNITY_EDITOR
            var deviceId = SystemInfo.deviceUniqueIdentifier;
#elif UNITY_IOS
            var deviceId = KeyChain.BindGetKeyChainUser();
            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                KeyChain.BindSetKeyChainUser("0", deviceId);
            }
#endif

            new ExtendedDeviceAuthenticationRequest()
                .SetDeviceId(deviceId)
                .Send(OnRequestSuccess, OnRequestFailure);
            return promise;
        }
    }

    public class GSSetPlayerSocialNameRequest : GSFrameworkRequest
    {
        public GSSetPlayerSocialNameRequest(GSFrameworkRequestContext context) : base(context) { }

        const string SHORT_CODE = "SetPlayerSocialName";
        const string ATT_NAME = "name";

        public IPromise<BackendResult> Send(string name, Action<object, Action<object>> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.SET_PLAYER_SOCIAL_NAME_FAILED;

            new LogEventRequest()
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_NAME, name)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

}
