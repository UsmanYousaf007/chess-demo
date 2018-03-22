﻿
using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> IOSBuyVirtualGoods(
            string currencyCode,                                    // The ISO 4217 currency code representing the real-world currency used for this transaction.
            string receipt,                                         // The receipt obtained from SKPaymentTransaction. transactionReceipt
            bool sandbox,                                           // Should the sandbox account be used
            int subUnitPrice)                                       // The price of this purchase
        {
            return new GSIOSBuyVirtualGoodsRequest().Send(currencyCode, receipt, sandbox, subUnitPrice, OnIOSBuyVirtualGoodsSuccess);
        }

        private void OnIOSBuyVirtualGoodsSuccess(BuyVirtualGoodResponse response)
        {
            GSData records = response.ScriptData.GetGSData("TODO");
        }
    }
}
