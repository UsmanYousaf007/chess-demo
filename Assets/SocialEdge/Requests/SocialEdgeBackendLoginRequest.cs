/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace SocialEdge.Requests
{
    public class SocialEdgeBackendLoginResponse : SocialEdgeRequestResponse<LoginResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(LoginResult resultSuccess)
        {
            isSuccess = true;
            token = resultSuccess.EntityToken.EntityToken;
        }

        /// <summary>
        /// Build results on request failure
        /// </summary>
        public override void BuildFailure(PlayFabError resultFailure)
        {
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

        /// <summary>
        /// Constructor
        /// </summary>
        public SocialEdgeBackendLoginRequest()
        {
            // Mandatory call to base class
            Base(this);
        }


        /// <summary>
        /// Gets display name and avatar from server side
        /// </summary>
        public SocialEdgeBackendLoginRequest GetBasicInfo()
        {
            /*Server side call here to get random display name and avatar*/
            return this;
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
            //LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest
            //{
            //    AndroidDeviceId = GetDeviceId(),
            //    CreateAccount = true
            //};

            //LoginWithIOSDeviceIDRequest request2 = new LoginWithIOSDeviceIDRequest
            //{
            //    DeviceId = GetDeviceId(),
            //    CreateAccount = true
            //};



            LoginWithCustomIDRequest hrequest = new LoginWithCustomIDRequest
            {

                CustomId = "user0",
                CreateAccount = false
            };

            PlayFabClientAPI.LoginWithCustomID(hrequest, OnSuccess, OnFailure);
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

        private static string GetDeviceId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

    }
}
