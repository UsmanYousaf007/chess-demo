/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> ConsumeVirtualGood(GSRequestData jsonData)
        {
            //return new GSConsumeVirtualGoodRequest().Send(quantity, shortCode, OnConsumeVirtualGoodSuccess);
            return new GSConsumeVirtualGood().Send(jsonData, OnConsumeVirtualGoodSuccess);
        }

        public class GSConsumeVirtualGood : GSFrameworkRequest
        {
            const string SHORT_CODE = "ConsumeVirtualGood";
            const string ATT_JSON_DATA = "jsonData";

            public IPromise<BackendResult> Send(GSRequestData jsonData, Action<object> onSuccess)
            {
                this.onSuccess = onSuccess;
                this.errorCode = BackendResult.CONSUME_VIRTUAL_GOOD_FAILED;

                new LogEventRequest()
                    .SetEventKey(SHORT_CODE)
                    .SetEventAttribute(ATT_JSON_DATA, jsonData)
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }
        }

        private void OnConsumeVirtualGoodSuccess(object r)
        {
           LogEventResponse response = (LogEventResponse)r;

            if(response != null && response.ScriptData != null)
            {
                int quantity = response.ScriptData.GetInt("quantity").Value;
                string shopItemId = response.ScriptData.GetString("shortCode");

                TLUtils.LogUtil.LogNullValidation(shopItemId, "shopItemId");

                if (shopItemId != null && playerModel.inventory.ContainsKey(shopItemId))
                {
                    int count = playerModel.inventory[shopItemId] - quantity;
                    playerModel.inventory[shopItemId] = count;
                }

            }
        
        }
    }

    //#region REQUEST

    //public class GSConsumeVirtualGoodRequest : GSFrameworkRequest
    //{
        
    //    public IPromise<BackendResult> Send(                      
    //        int quantity,                                         // The number of items to consume
    //        string shortCode,                                     // The short code of the virtual good to be consumed
    //        Action<object> onSuccess)
    //    {
    //        this.onSuccess = onSuccess;
    //        this.errorCode = BackendResult.CONSUME_VIRTUAL_GOOD_FAILED;

    //        new ConsumeVirtualGoodRequest()                
    //            .SetQuantity(quantity)
    //            .SetShortCode(shortCode)
    //            .Send(OnRequestSuccess, OnRequestFailure);

    //        return promise;
    //    }
    //}

    //#endregion
}
