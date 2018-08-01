/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;
using System;
using GameSparks.Api.Requests;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> GetInitData(int appVersion, string appData)
        {
            // Fetch init data from server
            return new GSGetInitDataRequest().Send(appVersion, appData, OnGetInitDataSuccess);
        }

        void OnGetInitDataSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            appInfoModel.androidURL = response.ScriptData.GetString(GSBackendKeys.APP_ANDROID_URL);
            appInfoModel.iosURL = response.ScriptData.GetString(GSBackendKeys.APP_IOS_URL);

            // Check app version match with back end. Bail if there is mismatch.
            if (response.ScriptData.GetBoolean(GSBackendKeys.APP_VERSION_VALID) == false)
            {
                appInfoModel.appBackendVersionValid = false;
                return;
            }
            appInfoModel.appBackendVersionValid = true;

            GSData storeSettingsData = response.ScriptData.GetGSData(GSBackendKeys.SHOP_SETTINGS);
            FillStoreSettingsModel(storeSettingsData);

            GSData adsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.ADS_SETTINGS);
            FillAdsSettingsModel(adsSettingsData);

            GSData accountDetailsData = response.ScriptData.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            OnAccountDetailsSuccess(accountDetailsResponse);

            IPromise<bool> promise = storeService.Init(storeSettingsModel.getRemoteProductIds());
            if (promise != null)
            {
                promise.Then(OnStoreInit);
            }
        }

        private void OnStoreInit(bool success)
        {
            if (success) 
            {
                metaDataModel.store.remoteStoreAvailable = true;

                foreach (KeyValuePair<string, StoreItem> item in metaDataModel.store.items) 
                {
                    StoreItem storeItem = item.Value;
                    if (storeItem.remoteProductId != null) 
                    {
                        storeItem.remoteProductPrice = storeService.GetItemLocalizedPrice (storeItem.remoteProductId);
                    }
                }
            }
        }

        private void OnAccountDetailsSuccess(AccountDetailsResponse response)
        {
            playerModel.id = response.UserId;
            playerModel.tag = response.ScriptData.GetString(GSBackendKeys.TAG);
            playerModel.name = response.DisplayName;
            playerModel.countryId = response.Location.Country;

            playerModel.bucks = response.Currency2.Value;
            playerModel.eloScore = response.ScriptData.GetInt(GSBackendKeys.ELO_SCORE).Value;
            playerModel.adLifetimeImpressions = response.ScriptData.GetInt(GSBackendKeys.AD_LIFETIME_IMPRESSIONS).Value;

            playerModel.totalGamesWon = response.ScriptData.GetInt(GSBackendKeys.GAMES_WON).Value;
            playerModel.totalGamesLost = response.ScriptData.GetInt(GSBackendKeys.GAMES_LOST).Value;
            playerModel.totalGamesDrawn = response.ScriptData.GetInt(GSBackendKeys.GAMES_DRAWN).Value;

            // Populate inventory data
            IList<GSData> playerActiveInventory = response.ScriptData.GetGSDataList(GSBackendKeys.PLAYER_ACTIVE_INVENTORY);
            IOrderedDictionary<string, int> inventory = new OrderedDictionary<string, int>(); 
            GSParser.PopulateInventory(inventory, response.VirtualGoods);
            playerModel.inventory = inventory;
            GSParser.PopulateActiveInventory(playerModel, playerActiveInventory);

			// Populate friends data
			IList<GSData> friendsList = response.ScriptData.GetGSDataList(GSBackendKeys.FRIENDS);
			playerModel.friends = PopulateFriends(friendsList);

            //GSParser.LogPlayerInfo(playerModel);
        }

        private void FillAdsSettingsModel(GSData adsSettingsData)
        {
            adsSettingsModel.adsRewardIncrement = adsSettingsData.GetInt(GSBackendKeys.ADS_REWARD_INCREMENT).Value;
            adsSettingsModel.maxImpressionsPerSlot = adsSettingsData.GetInt(GSBackendKeys.ADS_MAX_IMPRESSIONS_PER_SLOT).Value;
            adsSettingsModel.slotHour = adsSettingsData.GetInt(GSBackendKeys.ADS_SLOT_HOUR).Value;
        }

        private void FillStoreSettingsModel(GSData storeSettingsData)
        {
            List<GSData> skinShopItemsData = storeSettingsData.GetGSDataList("skinShopItems");
            IOrderedDictionary<string, StoreItem> skinItems = PopulateSkinStoreItems(skinShopItemsData);

            List<GSData> currencyShopItemsData = storeSettingsData.GetGSDataList("coinsShopItems");
            IOrderedDictionary<string, StoreItem> currencyItems = PopulateCurrencyStoreItems(currencyShopItemsData);

            storeSettingsModel.Initialize();
            storeSettingsModel.Add(GSBackendKeys.ShopItem.SKIN_SHOP_TAG, skinItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.COINS_SHOP_TAG, currencyItems);
        }

        private IOrderedDictionary<string, StoreItem> PopulateSkinStoreItems(List<GSData> skinSettingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in skinSettingsData)
            {
                var item = new SkinStoreItem();
                GSParser.PopulateStoreItem(item, itemData);
                GSData properties = GSParser.GetVGoodProperties(itemData);
                if (properties != null)
                {
                    item.unlockAtLevel = GSParser.GetSafeInt(properties, GSBackendKeys.SHOP_ITEM_UNLOCKATLEVEL);
                }

                items.Add(item.key, item);
            }

            return items;
        }

        private IOrderedDictionary<string, StoreItem> PopulateCurrencyStoreItems(List<GSData> currencySetingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in currencySetingsData)
            {
                var item = new CurrencyStoreItem();
                GSParser.PopulateStoreItem(item, itemData);

                item.currency2Payout = item.currency2Cost;
                item.currency2Cost = 0;

                GSData properties = GSParser.GetVGoodProperties(itemData);
                if (properties != null)
                {
                    item.promotionId = GSParser.GetSafeString(properties, GSBackendKeys.SHOP_ITEM_PROMOTION_ID);
                    item.bonusXpPercentage = GSParser.GetSafeFloat(properties, GSBackendKeys.SHOP_ITEM_BONUS_XP_PERCENTAGE);
                    item.hintsCount = GSParser.GetSafeInt(properties, GSBackendKeys.SHOP_ITEM_HINTS_COUNT);
                    item.lossRecoveryPercentage = GSParser.GetSafeFloat(properties, GSBackendKeys.SHOP_ITEM_LOSS_RECOVERY_PERCENTAGE);
                    item.bonusAmount = GSParser.GetSafeInt(properties,GSBackendKeys.SHOP_ITEM_BONUS_AMOUNT);
                }

                items.Add(item.key, item);
            }

            return items;
        }

		private IDictionary<string, Friend> PopulateFriends(IList<GSData> friendsData)
		{
			IDictionary<string, Friend> friends = new Dictionary<string, Friend>();

			foreach (GSData friendData in friendsData)
			{
				var friend = new Friend();
				GSParser.PopulateFriend(friend, friendData);
			}

			GSParser.LogFriends(friends);

			return friends;
		}
    }

    #region REQUEST

    public class GSGetInitDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "GetInitData";
        const string ATT_APP_VERSION = "appVersion";
        const string ATT_APP_DATA = "appData";

        public IPromise<BackendResult> Send(int appVersion, string appData, Action<object> onSuccess)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.GET_INIT_DATA_REQUEST_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_APP_VERSION, appVersion)
                .SetEventAttribute(ATT_APP_DATA, appData)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
