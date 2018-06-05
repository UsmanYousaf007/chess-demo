/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class GSAuthFacebookRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();

        public IPromise<BackendResult> Send(IFacebookService facebookService)
        {
            GSRequestSession.Instance.AddRequest(this);
            facebookService.Auth().Then(OnFacebookAuth);
            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnFacebookAuth(FacebookResult result, string accessToken)
        {
            if (result == FacebookResult.SUCCESS)
            {
                new FacebookConnectRequest().SetAccessToken(accessToken)
                    .SetSyncDisplayName(true)
                    .SetSwitchIfPossible(true)
                    .Send(OnSuccess, OnFailure);
            }
            else if (result == FacebookResult.CANCELLED)
            {
                LogUtil.Log(this.GetType().Name + ": Facebook authentication cancelled.", "red");
                DispatchResponse(BackendResult.AUTH_FACEBOOK_REQUEST_CANCELLED);
            }
            else
            {
                LogUtil.Log(this.GetType().Name + ": Facebook authentication failed.", "red");
                DispatchResponse(BackendResult.AUTH_FACEBOOK_REQUEST_FAILED);
            }
        }

        private void OnSuccess(AuthenticationResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.SUCCESS);   
            }
        }

        private void OnFailure(AuthenticationResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.AUTH_FACEBOOK_REQUEST_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {
            promise.Dispatch(result);
            GSRequestSession.Instance.RemoveRequest(this);
        }
    }
}
