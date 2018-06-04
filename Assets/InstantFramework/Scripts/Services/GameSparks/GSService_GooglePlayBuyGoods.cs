
using GameSparks.Api.Responses;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult, string> GooglePlayBuyGoods(
            string transactionID,
            string currencyCode,                                    // The ISO 4217 currency code representing the real-world currency used for this transaction.
            string signature,                                       // The value obtained from data.getStringExtra(“INAPP_DATA_SIGNATURE”);
            string signedData,                                      // The value obtained from data.getStringExtra(“INAPP_PURCHASE_DATA”)
            int subUnitPrice)                                       // The price of this purchase
        {
            return new GSGooglePlayBuyGoodsRequest().Send(transactionID, currencyCode, signature, signedData, subUnitPrice, OnGooglePlayBuyGoodsSuccess);
        }

        private void OnGooglePlayBuyGoodsSuccess(BuyVirtualGoodResponse response)
        {
			LogUtil.Log("********* OnGooglePlayBuyGoodsSuccess.. ");

            // Bought bucks
            if (response.Currency2Added.HasValue)
            {
                playersModel.bucks += response.Currency2Added.Value;
                LogUtil.Log("********* GooglePlayBuyGoods BUCKS ADDED ---> " + response.Currency2Added.Value);
            }
        }
    }
}
