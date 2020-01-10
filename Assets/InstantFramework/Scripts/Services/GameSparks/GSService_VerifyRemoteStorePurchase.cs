/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;
using System;
using GameSparks.Api.Requests;
using strange.extensions.promise.impl;
using GameSparks.Core;
using System.Collections.Generic;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        // Dispatch Signals
        [Inject] public UpdatePurchasedStoreItemSignal updatePurchasedStoreItemSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

        long subscriptionExpiryTimeStamp;

        public IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionId, string purchaseReceipt, long expiryTimeStamp)
        {
            subscriptionExpiryTimeStamp = expiryTimeStamp;
            return new GSVerifyRemoteStorePurchaseRequest(remoteProductId, transactionId, purchaseReceipt, expiryTimeStamp,
                                                            OnVerifyRemoteStorePurchaseSuccess).Send();
        }

        private void OnVerifyRemoteStorePurchaseSuccess(LogEventResponse response)
        {
            var res = response.ScriptData.GetGSData("remotePurchase");

            // Process goods
            GSData boughtItem = res.GetGSData("boughtItem");
            if (boughtItem != null)
            {
                string shopItemId = boughtItem.GetString("shortCode");

                if (navigatorModel.currentViewId == NavigatorViewId.STORE)
                {
                    analyticsService.Event(AnalyticsEventId.store_purchase_complete, AnalyticsParameter.item_id, shopItemId);
                }
                else if (navigatorModel.currentViewId == NavigatorViewId.SPOT_PURCHASE_DLG)
                {
                    analyticsService.Event(AnalyticsEventId.v1_spot_purchase_complete, AnalyticsParameter.item_id, shopItemId);
                }

                if (playerModel.inventory.ContainsKey(shopItemId))
                {
                    playerModel.inventory[shopItemId] = playerModel.inventory[shopItemId] + 1;
                }
                else
                {
                    playerModel.inventory.Add(shopItemId, 1); 
                }

                switch (shopItemId)
                {
                    case GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG:
                        playerModel.subscriptionExipryTimeStamp = subscriptionExpiryTimeStamp;
                        break;
                }

                updatePurchasedStoreItemSignal.Dispatch(metaDataModel.store.items[shopItemId]);
            }
        }
    }

    #region REQUEST

    public class GSVerifyRemoteStorePurchaseRequest
    {
        protected LogEventRequest request = new LogEventRequest();
        protected string key;
        protected BackendResult errorCode;
        protected Action<LogEventResponse> onSuccess;
        protected string transactionIdRecord;

        readonly IPromise<BackendResult, string> promise = new Promise<BackendResult, string>();

        public IPromise<BackendResult, string> Send()
        {
            request.SetEventKey(key).Send(OnSuccess, OnFailure);
            return promise;
        }

        void OnSuccess(LogEventResponse response)
        {
            onSuccess(response); 
            promise.Dispatch(BackendResult.SUCCESS, transactionIdRecord);
        }

        void OnFailure(LogEventResponse response)
        {
            promise.Dispatch(errorCode, transactionIdRecord);
        }

        public GSVerifyRemoteStorePurchaseRequest(string remoteProductId, string transactionId, string purchaseReceipt, long expiryTimeStamp,
                                                    Action<LogEventResponse> onSuccess)
        {
            transactionIdRecord = transactionId;
            key = "VerifyRemoteStorePurchase";
            errorCode = BackendResult.VERIFY_REMOTE_STORE_PURCHASE_FAILED;

            var jsonData = new GSRequestData().AddString("remoteProductId", remoteProductId)
                                              .AddJSONStringAsObject("purchaseReceipt", purchaseReceipt)
                                              .AddNumber("expiryTimeStamp", expiryTimeStamp);
            request.SetEventAttribute("jsonData", jsonData);

            // Do not modify below
            this.onSuccess = onSuccess;
        }
    }

    #endregion
}
