/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeCloudScriptResponse : SocialEdgeRequestResponse<ExecuteCloudScriptResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(ExecuteCloudScriptResult resultSuccess)
        {

            isSuccess = true;
            token = resultSuccess.Request.AuthenticationContext.EntityToken;
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
    public class SocialEdgeCloudScriptRequest : SocialEdgeRequest<SocialEdgeCloudScriptRequest, SocialEdgeCloudScriptResponse>
    {
        // Request parameters section
        private string _functionName { get; set; }
        private string[] _args { get; set; }
        public SocialEdgeCloudScriptRequest(string functionName)
        {
            // Mandatory call to base class
            Base(this);
            _functionName = functionName;
        }


        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new ExecuteCloudScriptRequest 
            {
                FunctionName= _functionName
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
