
using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> IOSBuyGoods(
            string currencyCode,                                    // The ISO 4217 currency code representing the real-world currency used for this transaction.
            string receipt,                                         // The receipt obtained from SKPaymentTransaction. transactionReceipt
            bool sandbox,                                           // Should the sandbox account be used
            int subUnitPrice)                                       // The price of this purchase
        {
            return new GSIOSBuyGoodsRequest().Send(currencyCode, receipt, sandbox, subUnitPrice, OnIOSBuyGoodsSuccess);
        }

		private void OnIOSBuyGoodsSuccess(BuyVirtualGoodResponse response)
        {
            GSData records = response.ScriptData.GetGSData("TODO");
        }
    }
}
