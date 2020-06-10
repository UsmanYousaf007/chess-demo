/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeGetInventoryResponse : SocialEdgeRequestResponse<GetUserInventoryResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(GetUserInventoryResult resultSuccess)
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
    public class SocialEdgeGetInventoryRequest : SocialEdgeRequest<SocialEdgeGetInventoryRequest, SocialEdgeGetInventoryResponse>
    {
        // Request parameters section
        public string storeId { get; set; }
        public string catalogueVersion { get; set; }
        public SocialEdgeGetInventoryRequest(string storeId, string catalogueVersion)
        {
            Base(this);
            this.storeId = storeId;
            this.catalogueVersion = catalogueVersion;
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(GetUserInventoryResult result)
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
