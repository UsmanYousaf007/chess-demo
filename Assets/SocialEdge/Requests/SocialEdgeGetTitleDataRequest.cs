/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeGetTitleDataResponse : SocialEdgeRequestResponse<GetTitleDataResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(GetTitleDataResult resultSuccess)
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
    public class SocialEdgeGetTitleDataRequest : SocialEdgeRequest<SocialEdgeGetTitleDataRequest, SocialEdgeGetTitleDataResponse>
    {
        // Request parameters section
        public SocialEdgeGetTitleDataRequest()
        {
            Base(this);
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new GetTitleDataRequest();

            PlayFabClientAPI.GetTitleData(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(GetTitleDataResult result)
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
