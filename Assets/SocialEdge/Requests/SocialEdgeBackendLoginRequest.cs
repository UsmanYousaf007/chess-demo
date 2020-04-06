/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeBackendLoginResponse : SocialEdgeRequestResponse
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(object resultSuccess)
        {
            LoginResult lr = (LoginResult)resultSuccess;
            isSuccess = true;
            token = lr.EntityToken.EntityToken;
        }

        /// <summary>
        /// Build results on request failure
        /// </summary>
        public override void BuildFailure(object resultFailure)
        {
            PlayFabError pf = (PlayFabError)resultFailure;
            isSuccess = false;
        }
    }

    /// <summary>
    /// Backend login to server request
    /// </summary>
    public class SocialEdgeBackendLoginRequest : SocialEdgeRequest<SocialEdgeBackendLoginRequest, SocialEdgeBackendLoginResponse>
    {
        // Request parameters section
        public string userName;

        public SocialEdgeBackendLoginRequest()
        {
            request = this;
        }

        /// <summary>
        /// Set login username
        /// </summary>
        public SocialEdgeBackendLoginRequest SetUserName(string displayName)
        {
            userName = displayName;
            return this;
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        public override void Send()
        {
            hrequest = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID((LoginWithCustomIDRequest)hrequest, OnSuccess, OnFailure);
        }

        private void OnSuccess(LoginResult result)
        {
            response.BuildSuccess(result);
            actionSuccess?.Invoke(response);
        }

        private void OnFailure(PlayFabError error)
        {
            response.BuildFailure(error);
            actionFailure?.Invoke(response);
        }
    }
}
