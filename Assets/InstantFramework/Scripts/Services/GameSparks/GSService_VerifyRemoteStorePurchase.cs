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
        [Inject] public UpdateRemoveAdsSignal updateRemoveAdsDisplaySignal { get; set; }
        [Inject] public RemoveLobbyPromotionSignal removeLobbyPromotionSignal { get; set; }
        [Inject] public ReportLobbyPromotionAnalyticSingal reportLobbyPromotionAnalyticSingal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

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

                switch (shopItemId)
                {
                    case GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG:
                        reportLobbyPromotionAnalyticSingal.Dispatch(LobbyPromotionKeys.ULTIMATE_BANNER, AnalyticsEventId.banner_utlimate_bundle_purchase_success);
                        removeLobbyPromotionSignal.Dispatch(LobbyPromotionKeys.ULTIMATE_BANNER);
                        break;

                    case GSBackendKeys.ShopItem.SPECIAL_BUNDLE_NOADSFOREVER_SHOP_TAG:
                        reportLobbyPromotionAnalyticSingal.Dispatch(LobbyPromotionKeys.ADS_BANNER, AnalyticsEventId.banner_ad_bundle_purchase_success);
                        removeLobbyPromotionSignal.Dispatch(LobbyPromotionKeys.ADS_BANNER);
                        break;
                }

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
            errorCode = BackendResult.VERIFY_REMOTE_STORE_PURCHASE_FAILED;

            var jsonData = new GSRequestData().AddString("remoteProductId", remoteProductId).
                                               AddJSONStringAsObject("purchaseReceipt", purchaseReceipt);
            request.SetEventAttribute("jsonData", jsonData);

            // Do not modify below
            this.onSuccess = onSuccess;
        }
    }

    #endregion
}
