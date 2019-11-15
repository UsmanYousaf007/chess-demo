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
            return new GSAuthGuestRequest().Send(OnGuestAuthSuccess);
        }

        public IPromise<BackendResult> AuthFacebook(string accessToken, bool existingPlayer)
        {
            return new GSAuthFacebookRequest().Send(accessToken, existingPlayer, (existingPlayer == true ? (Action<object>)null : onFacebookAuthSuccess));
        }

        public IPromise<BackendResult> AuthEmail(string email, string password, bool existingPlayer)
        {
            return new GSAuthEmailResquest().Send(email, password, existingPlayer, (existingPlayer == true ? (Action<object>)null : onEmailAuthSuccess));
        }

        private void OnGuestAuthSuccess(object r)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.newUser = (bool)response.NewPlayer;
        }

        private void onFacebookAuthSuccess(object r)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.id = response.UserId;

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);
        }

        private void onEmailAuthSuccess(object r)
        {
            AuthenticationResponse response = (AuthenticationResponse)r;
            playerModel.id = response.UserId;

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);
        }

        public IPromise<BackendResult> SetPlayerSocialName(string name)
        {
            return new GSSetPlayerSocialNameRequest().Send(name, OnSetPlayerSocialNameSuccess);
        }

        private void OnSetPlayerSocialNameSuccess(object r)
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
        public IPromise<BackendResult> Send(string accessToken, bool existingPlayer, Action<object> onFacebookAuthSuccess)
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

    public class GSAuthEmailResquest : GSFrameworkRequest
    {
        public IPromise<BackendResult> Send(string email, string password, bool existingPlayer, Action<object> onEmailAuthSuccess)
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
        public IPromise<BackendResult> Send(Action<object> onSuccess)
        {
            this.errorCode = BackendResult.AUTH_GUEST_REQUEST_FAILED;
            this.onSuccess = onSuccess;

            new DeviceAuthenticationRequest().Send(OnRequestSuccess, OnRequestFailure);
            return promise;
        }
    }

    public class GSSetPlayerSocialNameRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SetPlayerSocialName";
        const string ATT_NAME = "name";

        public IPromise<BackendResult> Send(string name, Action<object> onSuccess)
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
