/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgeGetStoreItemsResponse : SocialEdgeRequestResponse<GetStoreItemsResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(GetStoreItemsResult resultSuccess)
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
    public class SocialEdgeGetStoreItemsRequest : SocialEdgeRequest<SocialEdgeGetStoreItemsRequest, SocialEdgeGetStoreItemsResponse>
    {
        // Request parameters section
        public string storeId { get; set; }
        public string catalogueVersion { get; set; }
        public SocialEdgeGetStoreItemsRequest(string storeId, string catalogueVersion)
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
            var request = new GetStoreItemsRequest
            {
                StoreId = storeId

            };

            PlayFabClientAPI.GetStoreItems(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(GetStoreItemsResult result)
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
