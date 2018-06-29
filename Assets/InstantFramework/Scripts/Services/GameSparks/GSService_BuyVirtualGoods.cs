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
        [Inject] public IPlayerModel playersModel { get; set; }

        public IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode)
        {
            return new GSBuyVirtualGoodsRequest().Send(currencyType, quantity, shortCode, OnBuyVirtualGoodsSuccess);
        }

        private void OnBuyVirtualGoodsSuccess(BuyVirtualGoodResponse response)
        {
            // Consume bucks
            if (response.CurrencyType == 2 && response.CurrencyConsumed.HasValue)
            {
                playersModel.bucks -= response.CurrencyConsumed.Value;
            }

            GSEnumerable<BuyVirtualGoodResponse._Boughtitem> virtualGoods = response.BoughtItems;
            foreach (var v in virtualGoods)
            {
                //long quantity = v.Quantity.Value;
                string shopItemId = v.ShortCode;

                int count = 0;
                if (playersModel.inventory.ContainsKey(shopItemId))
                {
                    count = playersModel.inventory[shopItemId] + 1;
                    playersModel.inventory[shopItemId] = count;
                }
                else
                {
                    playersModel.inventory.Add(shopItemId, 1); 
                }
            }
        }
    }

    #region REQUEST

    public class GSBuyVirtualGoodsRequest : GSFrameworkRequest
    {
        Action<BuyVirtualGoodResponse> onSuccess;

        public IPromise<BackendResult> Send(                      
            long currencyType,                                    // Which virtual currency to use. (1 to 6)
            int quantity,                                         // The number of items to purchase
            string shortCode,                                     // The short code of the virtual good to be purchased
            Action<BuyVirtualGoodResponse> onSuccess)
        {
            this.onSuccess = onSuccess;

            new BuyVirtualGoodsRequest()  
                .SetCurrencyType(currencyType)                  
                .SetQuantity(quantity)
                .SetShortCode(shortCode)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        void OnSuccess(BuyVirtualGoodResponse response)
        {
            if (IsActive())
            {
                onSuccess(response);
            }

            Dispatch(BackendResult.SUCCESS);
        }

        void OnFailure(BuyVirtualGoodResponse response)
        {
            Dispatch(BackendResult.BUY_VIRTUAL_GOOD_FAILED);
        }
    }

    #endregion
}
