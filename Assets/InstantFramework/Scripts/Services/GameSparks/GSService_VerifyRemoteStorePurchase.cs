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

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }
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
            GSData boughtItem = res.GetGSData("boughtItem");
            if (boughtItem != null)
            {
                string shopItemId = boughtItem.GetString("shortCode");
                if (playersModel.inventory.ContainsKey(shopItemId))
                {
                    playersModel.inventory[shopItemId] = playersModel.inventory[shopItemId] + 1;
                }
                else
                {
                    playersModel.inventory.Add(shopItemId, 1); 
                }
            }

            updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);
            updateRemoveAdsDisplaySignal.Dispatch(playersModel.OwnsVGood(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS));
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
