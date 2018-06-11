/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> GetInitData(int clientVersion)
        {
            return new GSGetInitDataRequest().Send(clientVersion, OnGetInitDataSuccess);
        }

        private void OnGetInitDataSuccess(LogEventResponse response)
        {
            appInfoModel.androidURL = response.ScriptData.GetString(GSBackendKeys.APP_ANDROID_URL);
            appInfoModel.iosURL = response.ScriptData.GetString(GSBackendKeys.APP_IOS_URL);

            // Check app version match with back end. Bail if there is mismatch.
            if (response.ScriptData.GetBoolean(GSBackendKeys.APP_VERSION_VALID) == false)
            {
                appInfoModel.appVersionValid = false;
                return;
            }
            appInfoModel.appVersionValid = true;

            GSData storeSettingsData = response.ScriptData.GetGSData(GSBackendKeys.SHOP_SETTINGS);
            FillStoreSettingsModel(storeSettingsData);

            GSData adsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.ADS_SETTINGS);
            FillAdsSettingsModel(adsSettingsData);

            GSData accountDetailsData = response.ScriptData.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            OnAccountDetailsSuccess(accountDetailsResponse);

            //CheckAndHandleMatchResume(response); // TODO: game dependency to remove or move

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

        private void FillAdsSettingsModel(GSData adsSettingsData)
        {
            adsSettingsModel.adsRewardIncrement = adsSettingsData.GetInt(GSBackendKeys.ADS_REWARD_INCREMENT).Value;
            adsSettingsModel.maxImpressionsPerSlot = adsSettingsData.GetInt(GSBackendKeys.ADS_MAX_IMPRESSIONS_PER_SLOT).Value;
            adsSettingsModel.slotMinutes = adsSettingsData.GetInt(GSBackendKeys.ADS_SLOT_MINUTES).Value;
        }
            
        private void FillStoreSettingsModel(GSData storeSettingsData)
        {
            List<GSData> skinShopItemsData = storeSettingsData.GetGSDataList("skinShopItems");
            IOrderedDictionary<string, StoreItem> skinItems = PopulateSkinStoreItems(skinShopItemsData);

            List<GSData> currencyShopItemsData = storeSettingsData.GetGSDataList("coinsShopItems");
            IOrderedDictionary<string, StoreItem> currencyItems = PopulateCurrencyStoreItems(currencyShopItemsData);

            List<GSData> avatarShopItemsData = storeSettingsData.GetGSDataList("avatarShopItems");
            IOrderedDictionary<string, StoreItem> avatarItems = PopulateAvatarStoreItems(avatarShopItemsData);

            storeSettingsModel.Initialize();
            storeSettingsModel.Add(GSBackendKeys.ShopItem.SKIN_SHOP_TAG, skinItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.COINS_SHOP_TAG, currencyItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.AVATAR_SHOP_TAG, avatarItems);
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
            
        private IOrderedDictionary<string, StoreItem> PopulateAvatarStoreItems(List<GSData> avatarSettingData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in avatarSettingData)
            {
                var item = new AvatarStoreItem();
                GSParser.PopulateStoreItem(item, itemData);

                items.Add(item.key, item);
            }

            return items;
        }
    }
}
