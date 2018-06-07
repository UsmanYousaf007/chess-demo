/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult, string> IOSBuyGoods(
            string transactionID,
            string currencyCode,                                    // The ISO 4217 currency code representing the real-world currency used for this transaction.
            string receipt,                                         // The receipt obtained from SKPaymentTransaction. transactionReceipt
            bool sandbox,                                           // Should the sandbox account be used
            int subUnitPrice)                                       // The price of this purchase
        {
            return new GSIOSBuyGoodsRequest().Send(transactionID, currencyCode, receipt, sandbox, subUnitPrice, OnIOSBuyGoodsSuccess);
        }

		private void OnIOSBuyGoodsSuccess(BuyVirtualGoodResponse response)
        {
            LogUtil.Log("********* OnIOSBuyGoodsSuccess.. ");

            // Bought bucks
            if (response.Currency2Added.HasValue)
            {
                playersModel.bucks += response.Currency2Added.Value;
                LogUtil.Log("********* IOSBuyGoods BUCKS ADDED ---> " + response.Currency2Added.Value);
            }

        }
    }
}
