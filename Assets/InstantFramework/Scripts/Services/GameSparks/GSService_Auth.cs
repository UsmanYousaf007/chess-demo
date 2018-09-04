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
            return new GSAuthGuestRequest().Send();
        }

        public IPromise<BackendResult> AuthFacebook(string accessToken)
        {
            return new GSAuthFacebookRequest().Send(accessToken, onFacebookAuthSuccess);
        }

        private void onFacebookAuthSuccess(object r)
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
        }
    }

    #region FACEBOOK AUTH REQUEST

    public class GSAuthFacebookRequest : GSFrameworkRequest
    {
        public IPromise<BackendResult> Send(string accessToken, Action<object> onFacebookAuthSuccess)
        {
            this.onSuccess = onFacebookAuthSuccess;
            this.errorCode = BackendResult.AUTH_FACEBOOK_REQUEST_FAILED;

            new FacebookConnectRequest()
                .SetAccessToken(accessToken)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion

    #region GUEST AUTH REQUEST

    public class GSAuthGuestRequest : GSFrameworkRequest
    {
        public IPromise<BackendResult> Send()
        {
            this.errorCode = BackendResult.AUTH_GUEST_REQUEST_FAILED;

            new DeviceAuthenticationRequest().Send(OnRequestSuccess, OnRequestFailure);
            return promise;
        }
    }

    #endregion

    #region SET SOCIAL PLAYER NAME

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

    #endregion
}
