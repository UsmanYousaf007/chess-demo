/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using PlayFab;
using PlayFab.ClientModels;

namespace SocialEdge.Requests
{
    public class SocialEdgePurchaseItemResponse : SocialEdgeRequestResponse<PurchaseItemResult, PlayFabError>
    {
        public string token;

        /// <summary>
        /// Build results on request success
        /// </summary>
        public override void BuildSuccess(PurchaseItemResult resultSuccess)
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
    public class SocialEdgePurchaseItemRequest : SocialEdgeRequest<SocialEdgePurchaseItemRequest, SocialEdgePurchaseItemResponse>
    {
        // Request parameters section
        public string storeId { get; set; }
        public string itemId { get; set; }
        public string currency { get; set; }
        public int price { get; set; }
        public SocialEdgePurchaseItemRequest()
        {
            Base(this);
        }

        public SocialEdgePurchaseItemRequest(string storeId, string itemId, string currency, int price)
        {
            Base(this);
            this.storeId = storeId;
            this.itemId = itemId;
            this.currency = currency;
            this.price = price;
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        ///
        public override void Send()
        {
            var request = new PlayFab.ClientModels.PurchaseItemRequest
            {
                ItemId = itemId,
                Price = price,
                VirtualCurrency = currency,
                StoreId = storeId
            };


            PlayFabClientAPI.PurchaseItem(request, OnSuccess, OnFailure);
        }

        private void OnSuccess(PurchaseItemResult result)
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
