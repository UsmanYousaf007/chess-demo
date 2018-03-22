
using GameSparks.Api.Responses;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> GooglePlayBuyVirtualGoods(
            string currencyCode,                                    // The ISO 4217 currency code representing the real-world currency used for this transaction.
            string signature,                                       // The value obtained from data.getStringExtra(“INAPP_DATA_SIGNATURE”);
            string signedData,                                      // The value obtained from data.getStringExtra(“INAPP_PURCHASE_DATA”)
            int subUnitPrice)                                       // The price of this purchase
        {
            return new GSGooglePlayBuyVirtualGoodsRequest().Send(currencyCode, signature, signedData, subUnitPrice, OnGooglePlayBuyVirtualGoodsSuccess);
        }

        private void OnGooglePlayBuyVirtualGoodsSuccess(BuyVirtualGoodResponse response)
        {
            // Bought bucks
            if (response.Currency2Added.HasValue)
            {
                playersModel.currency2 += response.Currency2Added.Value;
                LogUtil.Log("********* GooglePlayBuyVirtualGoods BUCKS ADDED ---> " + response.Currency2Added.Value);
            }
        }
    }
}
