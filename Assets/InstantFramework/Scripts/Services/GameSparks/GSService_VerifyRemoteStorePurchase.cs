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

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        // Dispatch Signals
        [Inject] public UpdatePurchasedStoreItemSignal updatePurchasedStoreItemSignal { get; set; }
        [Inject] public UpdatePurchasedBundleStoreItemSignal updatePurchasedBundleStoreItemSignal { get; set; }
        [Inject] public UpdateRemoveAdsSignal updateRemoveAdsDisplaySignal { get; set; }

        public IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionId, string purchaseReceipt)
        {
            return new GSVerifyRemoteStorePurchaseRequest(remoteProductId, transactionId, purchaseReceipt, 
                                                            OnVerifyRemoteStorePurchaseSuccess).Send();
        }

        private void OnVerifyRemoteStorePurchaseSuccess(LogEventResponse response)
        {
            var res = response.ScriptData.GetGSData("remotePurchase");

            // Process bucks
            int? bucks = res.GetInt("bucks");
            playerModel.bucks += bucks != null ? bucks.Value : 0;

            // Process goods
            string shopItemId = "";
            GSData boughtItem = res.GetGSData("boughtItem");
            if (boughtItem != null)
            {
                shopItemId = boughtItem.GetString("shortCode");
                if (playerModel.inventory.ContainsKey(shopItemId))
                {
                    playerModel.inventory[shopItemId] = playerModel.inventory[shopItemId] + 1;
                }
                else
                {
                    playerModel.inventory.Add(shopItemId, 1); 
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

                    updatePurchasedStoreItemSignal.Dispatch(metaDataModel.store.items[shortCode]);
                }

                if (res.ContainsKey(GSBackendKeys.PlayerDetails.REMOVE_ADS_TIMESTAMP))
                {
                    playerModel.removeAdsTimeStamp = res.GetLong(GSBackendKeys.PlayerDetails.REMOVE_ADS_TIMESTAMP).Value;
                }
                if (res.ContainsKey(GSBackendKeys.PlayerDetails.REMOVE_ADS_TIMEPERIOD))
                {
                    playerModel.removeAdsTimePeriod = res.GetInt(GSBackendKeys.PlayerDetails.REMOVE_ADS_TIMEPERIOD).Value;
                }
            }

            updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);
            updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());

            if (bundledGoods != null)
            {
                StoreVO storeVO = new StoreVO();
                storeVO.playerModel = playerModel;
                storeVO.storeSettingsModel = metaDataModel;

                updatePurchasedBundleStoreItemSignal.Dispatch(storeVO, metaDataModel.store.items[shopItemId]);
            }

            if (!playerModel.HasAdsFreePeriod(metaDataModel.adsSettings))
            {
                updateRemoveAdsDisplaySignal.Dispatch(null, false);
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

        public GSVerifyRemoteStorePurchaseRequest(string remoteProductId, string transactionId, string purchaseReceipt,
                                                    Action<LogEventResponse> onSuccess)
        {
            transactionIdRecord = transactionId;

            key = "VerifyRemoteStorePurchase";
            request.SetEventAttribute("remoteProductId", remoteProductId);
            //request.SetEventAttribute("transactionID", transactionID);
            //request.SetEventAttribute("purchaseReceipt", purchaseReceipt);
            errorCode = BackendResult.VERIFY_REMOTE_STORE_PURCHASE_FAILED;

            // Do not modify below
            this.onSuccess = onSuccess;
        }
    }

    #endregion
}
