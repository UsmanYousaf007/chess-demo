/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class GSIOSBuyGoodsRequest : GSRequest
    {
        private string transaction;
        private IPromise<BackendResult, string> promise = new Promise<BackendResult, string>();
        private Action<BuyVirtualGoodResponse> successCallback;

        public IPromise<BackendResult, string> Send(
            string transactionId,
            string currencyCode,                                    // The ISO 4217 currency code representing the real-world currency used for this transaction.
            string receipt,                                         // The receipt obtained from SKPaymentTransaction. transactionReceipt
            bool sandbox,                                           // Should the sandbox account be used
            int subUnitPrice,                                       // The price of this purchase
            Action<BuyVirtualGoodResponse> successCallback)
        {
            transaction = transactionId;
            GSRequestSession.Instance.AddRequest(this);
            this.successCallback = successCallback;
            new IOSBuyGoodsRequest()
                .SetCurrencyCode(currencyCode)
                .SetReceipt(receipt)
                .SetSandbox(sandbox)
                .SetSubUnitPrice(subUnitPrice)
                //.SetUniqueTransactionByPlayer(uniqueTransactionByPlayer)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE, transaction);
        }

        private void OnSuccess(BuyVirtualGoodResponse response)
        {
            if (!isExpired)
            {
                successCallback(response);
                DispatchResponse(BackendResult.SUCCESS);
            }
        }

        private void OnFailure(BuyVirtualGoodResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.BUY_VIRTUAL_GOOD_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result, transaction);
            GSRequestSession.Instance.RemoveRequest(this);
        }
    }
}
