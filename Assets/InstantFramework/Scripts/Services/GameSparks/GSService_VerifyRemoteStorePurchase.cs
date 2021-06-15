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
        long subscriptionExpiryTimeStamp;
        string subscriptionType;

        public IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionId, string purchaseReceipt, long expiryTimeStamp, string subscriptionType)
        {
            subscriptionExpiryTimeStamp = expiryTimeStamp;
            this.subscriptionType = subscriptionType;
            return new GSVerifyRemoteStorePurchaseRequest(remoteProductId, transactionId, purchaseReceipt, expiryTimeStamp, subscriptionType,
                                                            OnVerifyRemoteStorePurchaseSuccess).Send();
        }

        private void OnVerifyRemoteStorePurchaseSuccess(LogEventResponse response)
        {
            var res = response.ScriptData.GetGSData("remotePurchase");

            // Process gems
            int? gems = res.GetInt(GSBackendKeys.PlayerDetails.GEMS);
            playerModel.gems += gems != null ? gems.Value : 0;

            // Process coins
            long? coins = res.GetLong(GSBackendKeys.PlayerDetails.COINS);
            playerModel.coins += coins != null ? coins.Value : 0;

            // Parse dynamic bundle
            var dynamicBundleShortCode = res.GetString(GSBackendKeys.PlayerDetails.DYNAMIC_BUNDLE_SHORT_CODE);
            playerModel.dynamicBundleToDisplay = dynamicBundleShortCode;

            // Parse gem spot bundle data
            var dynamicSpotPurchaseBundleData = res.GetGSData(GSBackendKeys.PlayerDetails.DYNAMIC_GEM_SPOT_BUNDLE);
            GSParser.ParseDynamicSpotPurchaseBundle(playerModel.dynamicGemSpotBundle, dynamicSpotPurchaseBundleData);

            // Process goods
            GSData boughtItem = res.GetGSData("boughtItem");
            if (boughtItem != null)
            {
                string shopItemId = boughtItem.GetString("shortCode");

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
                    case GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SHOP_TAG:
                    case GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SALE_TAG:
                        playerModel.subscriptionExipryTimeStamp = subscriptionExpiryTimeStamp;
                        playerModel.renewDate = TimeUtil.ToDateTime(subscriptionExpiryTimeStamp).ToShortDateString();
                        playerModel.subscriptionType = subscriptionType;
                        break;
                }

                updatePurchasedStoreItemSignal.Dispatch(metaDataModel.store.items[shopItemId]);
            }

            // Process bundled goods
            IList<GSData> bundledGoods = res.GetGSDataList("bundledGoods");
            if (bundledGoods != null)
            {
                foreach (GSData item in bundledGoods)
                {
                    string shortCode = item.GetString("shortCode");
                    int qty = item.GetInt("qty").Value;
                    if (playerModel.inventory.ContainsKey(shortCode))
                    {
                        playerModel.inventory[shortCode] = playerModel.inventory[shortCode] + qty;
                    }
                    else
                    {
                        playerModel.inventory.Add(shortCode, qty);
                    }
                }
            }

            updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            updateBundleSignal.Dispatch();
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

        public GSVerifyRemoteStorePurchaseRequest(string remoteProductId, string transactionId, string purchaseReceipt, long expiryTimeStamp, string subscriptionType,
                                                    Action<LogEventResponse> onSuccess)
        {
            transactionIdRecord = transactionId;
            key = "VerifyRemoteStorePurchase";
            errorCode = BackendResult.VERIFY_REMOTE_STORE_PURCHASE_FAILED;

            var jsonData = new GSRequestData().AddString("remoteProductId", remoteProductId)
                                              .AddJSONStringAsObject("purchaseReceipt", purchaseReceipt)
                                              .AddNumber("expiryTimeStamp", expiryTimeStamp)
                                              .AddString("subscriptionType", subscriptionType);

            request.SetEventAttribute("jsonData", jsonData);

            // Do not modify below
            this.onSuccess = onSuccess;
        }
    }

    #endregion
}
