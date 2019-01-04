/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> ConsumeVirtualGood(int quantity, string shortCode)
        {
            return new GSConsumeVirtualGoodRequest().Send(quantity, shortCode, OnConsumeVirtualGoodSuccess);
        }

        private void OnConsumeVirtualGoodSuccess(object r)
        {
            ConsumeVirtualGoodResponse response = (ConsumeVirtualGoodResponse)r;
            int quantity = response.ScriptData.GetInt("quantity").Value;
            string shopItemId = response.ScriptData.GetString("shortCode");

            if (playerModel.inventory.ContainsKey(shopItemId))
            {
                int count = playerModel.inventory[shopItemId] - quantity;
                playerModel.inventory[shopItemId] = count;
            }
        }
    }

    #region REQUEST

    public class GSConsumeVirtualGoodRequest : GSFrameworkRequest
    {
        
        public IPromise<BackendResult> Send(                      
            int quantity,                                         // The number of items to consume
            string shortCode,                                     // The short code of the virtual good to be consumed
            Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.CONSUME_VIRTUAL_GOOD_FAILED;

            new ConsumeVirtualGoodRequest()                
                .SetQuantity(quantity)
                .SetShortCode(shortCode)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
