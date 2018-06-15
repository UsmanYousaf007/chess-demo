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
            return new AuthGuestRequest().Send();
        }

        public IPromise<BackendResult> AuthFacebook(string accessToken)
        {
            return new AuthFacebookRequest().Send(accessToken);
        }

        public IPromise<BackendResult> SetPlayerSocialName(string name)
        {
            return new GSSetPlayerSocialNameRequest().Send(name, OnSetPlayerSocialNameSuccess);
        }

        private void OnSetPlayerSocialNameSuccess(LogEventResponse response)
        {
            playerModel.name = response.ScriptData.GetString(GSBackendKeys.DISPLAY_NAME);
        }
    }

    #region FACEBOOK AUTH REQUEST

    public class AuthFacebookRequest
    {
        readonly IPromise<BackendResult> promise = new Promise<BackendResult>();

        public IPromise<BackendResult> Send(string accessToken)
        {
            new FacebookConnectRequest().SetAccessToken(accessToken)
                .SetSyncDisplayName(true)
                .SetSwitchIfPossible(true)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        void OnSuccess(AuthenticationResponse response)
        {
            promise.Dispatch(BackendResult.SUCCESS);
        }

        void OnFailure(AuthenticationResponse response)
        {
            promise.Dispatch(BackendResult.AUTH_FACEBOOK_REQUEST_FAILED);
        }
    }

    #endregion

    #region GUEST AUTH REQUEST

    public class AuthGuestRequest
    {
        readonly IPromise<BackendResult> promise = new Promise<BackendResult>();

        public IPromise<BackendResult> Send()
        {
            new DeviceAuthenticationRequest().Send(OnSuccess, OnFailure);

            return promise;
        }

        void OnSuccess(AuthenticationResponse response)
        {
            promise.Dispatch(BackendResult.SUCCESS);
        }

        void OnFailure(AuthenticationResponse response)
        {
            promise.Dispatch(BackendResult.AUTH_GUEST_REQUEST_FAILED);
        }
    }

    #endregion
}
