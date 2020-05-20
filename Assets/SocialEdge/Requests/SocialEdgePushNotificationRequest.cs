/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgePushNotificationResponse : SocialEdgeRequestResponse<ExecuteCloudScriptResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(ExecuteCloudScriptResult resultSuccess)
        {

            isSuccess = true;
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
    public class SocialEdgePushNotificationRequest : SocialEdgeRequest<SocialEdgePushNotificationRequest, SocialEdgePushNotificationResponse>
    {
        // Request parameters section
        private string receiverId { get; set; }
        private string funcName { get; set; }

        public SocialEdgePushNotificationRequest()
        {
            // Mandatory call to base class
            Base(this);
        }

        public SocialEdgePushNotificationRequest SetFunction(string funcName)
        {
            this.funcName = funcName;
            return this;
        }

        public SocialEdgePushNotificationRequest SetReceiver(string receiverId)
        {
            this.receiverId = receiverId;
            return this;
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send() {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = funcName,
                FunctionParameter = receiverId
            };

            PlayFabClientAPI.ExecuteCloudScript(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(ExecuteCloudScriptResult result)
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
