/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System.Collections.Generic;
using GameSparks.Core;
using strange.extensions.promise.impl;
using System;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode)
        {
            return new GSBuyVirtualGoodsRequest(GetRequestContext()).Send(currencyType, quantity, shortCode, OnBuyVirtualGoodsSuccess);
        }

        private void OnBuyVirtualGoodsSuccess(object r, Action<object>a)
        {
            BuyVirtualGoodResponse response = (BuyVirtualGoodResponse)r;

            // Consume bucks
            if (response.CurrencyType == 2 && response.CurrencyConsumed.HasValue)
            {
                playerModel.bucks -= response.CurrencyConsumed.Value;
            }
            // Consume gems
            else if (response.CurrencyType == 3 && response.CurrencyConsumed.HasValue)
            {
                playerModel.gems -= response.CurrencyConsumed.Value;
            }

            GSEnumerable<BuyVirtualGoodResponse._Boughtitem> virtualGoods = response.BoughtItems;
            foreach (var v in virtualGoods)
            {
                int quantity = (int) v.Quantity.Value;
                string shopItemId = v.ShortCode;

                int count = 0;
                
                TLUtils.LogUtil.LogNullValidation(shopItemId, "shopItemId");
                
                if (shopItemId == null)
                {
                    return;
                }
                if (playerModel.inventory.ContainsKey(shopItemId))
                {
                    count = playerModel.inventory[shopItemId] + quantity;
                    playerModel.inventory[shopItemId] = count;
                }
                else
                {
                    playerModel.inventory.Add(shopItemId, quantity); 
                }
            }
        }
    }

    #region REQUEST

    public class GSBuyVirtualGoodsRequest : GSFrameworkRequest
    {
        public GSBuyVirtualGoodsRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(                      
            long currencyType,                                    // Which virtual currency to use. (1 to 6)
            int quantity,                                         // The number of items to purchase
            string shortCode,                                     // The short code of the virtual good to be purchased
            Action<object, Action<object>> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.BUY_VIRTUAL_GOOD_FAILED;

            new BuyVirtualGoodsRequest()  
                .SetCurrencyType(currencyType)                  
                .SetQuantity(quantity)
                .SetShortCode(shortCode)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
