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
        // Models
        [Inject] public ILeagueSettingsModel leagueSettingsModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel  { get; set; }

		// Services
		[Inject] public IStoreService storeService { get; set; }

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
            
            IList<GSData> leagueSettingsData = response.ScriptData.GetGSDataList(GSBackendKeys.LEAGUE_SETTINGS);
            FillLeagueSettingsModel(leagueSettingsData);

            GSData levelSettingsData = response.ScriptData.GetGSData(GSBackendKeys.LEVEL_SETTINGS);
            FillLevelSettingsModel(levelSettingsData);

            IList<GSData> roomSettingsData = response.ScriptData.GetGSDataList(GSBackendKeys.ROOM_SETTINGS);
            FillRoomSettingsModel(roomSettingsData);

            GSData shopSettingsData = response.ScriptData.GetGSData(GSBackendKeys.SHOP_SETTINGS);
            FillStoreSettingsModel(shopSettingsData);

            GSData accountDetailsData = response.ScriptData.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            OnAccountDetailsSuccess(accountDetailsResponse);

            //CheckAndHandleMatchResume(response); // TODO: game dependency to remove

            List<string> storeProductIds = storeSettingsModel.getRemoteProductIds();
            storeService.Init(storeProductIds);
        }

        private void FillLeagueSettingsModel(IList<GSData> leagueSettingsData)
        {
            IOrderedDictionary<string, LeagueInfo> settings = new OrderedDictionary<string, LeagueInfo>();

            foreach (GSData leagueInfoData in leagueSettingsData)
            {
                GSData prize1Data = leagueInfoData.GetGSData(GSBackendKeys.LEAGUE_INFO_PRIZE_1);
                LeaguePrize prize1;
                prize1.currency1 = prize1Data.GetLong(GSBackendKeys.PRIZE_CURRENCY_1).Value;
                prize1.currency2 = prize1Data.GetLong(GSBackendKeys.PRIZE_CURRENCY_2).Value;

                GSData prize2Data = leagueInfoData.GetGSData(GSBackendKeys.LEAGUE_INFO_PRIZE_2);
                LeaguePrize prize2;
                prize2.currency1 = prize2Data.GetLong(GSBackendKeys.PRIZE_CURRENCY_1).Value;
                prize2.currency2 = prize2Data.GetLong(GSBackendKeys.PRIZE_CURRENCY_2).Value;

                GSData prize3Data = leagueInfoData.GetGSData(GSBackendKeys.LEAGUE_INFO_PRIZE_3);
                LeaguePrize prize3;
                prize3.currency1 = prize3Data.GetLong(GSBackendKeys.PRIZE_CURRENCY_1).Value;
                prize3.currency2 = prize3Data.GetLong(GSBackendKeys.PRIZE_CURRENCY_2).Value;

                LeagueInfo leagueInfo;
                leagueInfo.id = leagueInfoData.GetString(GSBackendKeys.LEAGUE_INFO_ID);
                leagueInfo.startLevel = leagueInfoData.GetInt(GSBackendKeys.LEAGUE_INFO_START_LEVEL).Value;
                leagueInfo.endLevel = leagueInfoData.GetInt(GSBackendKeys.LEAGUE_INFO_END_LEVEL).Value;
                leagueInfo.prize1 = prize1;
                leagueInfo.prize2 = prize2;
                leagueInfo.prize3 = prize3;

                settings.Add(leagueInfo.id, leagueInfo);
            }

            leagueSettingsModel.settings = settings;
        }

        private void FillLevelSettingsModel(GSData levelSettingsData)
        {
            int maxLevel = (int)levelSettingsData.GetInt(GSBackendKeys.MAX_LEVEL);
            levelSettingsModel.maxLevel = maxLevel;
        }

        private void FillRoomSettingsModel(IList<GSData> roomSettingsData)
        {
            IOrderedDictionary<string, RoomSetting> settings = new OrderedDictionary<string, RoomSetting>();

            foreach (GSData roomInfoData in roomSettingsData)
            {
                RoomSetting roomInfo = new RoomSetting();

                roomInfo.id = roomInfoData.GetString(GSBackendKeys.ROOM_INFO_ID);
                roomInfo.groupId = roomInfoData.GetString(GSBackendKeys.ROOM_INFO_GROUP_ID);
                roomInfo.unlockAtLevel = roomInfoData.GetInt(GSBackendKeys.ROOM_INFO_UNLOCK_AT_LEVEL).Value;
                roomInfo.gameDuration = roomInfoData.GetLong(GSBackendKeys.ROOM_INFO_GAME_DURATION).Value;
                roomInfo.wager = roomInfoData.GetLong(GSBackendKeys.ROOM_INFO_WAGER).Value;
                roomInfo.prize = roomInfoData.GetLong(GSBackendKeys.ROOM_INFO_PRIZE).Value;
                roomInfo.drawPrize = roomInfoData.GetLong(GSBackendKeys.ROOM_INFO_DRAW_PRIZE).Value;
                roomInfo.victoryXp = roomInfoData.GetInt(GSBackendKeys.ROOM_INFO_VICTORY_XP).Value;
                roomInfo.winsForTrophy = roomInfoData.GetInt(GSBackendKeys.ROOM_INFO_WINS_FOR_TROPHY).Value;
                roomInfo.trophiesForRoomTitle1 = roomInfoData.GetInt(GSBackendKeys.ROOM_INFO_TROPHIES_FOR_ROOM_TITLE_1).Value;
                roomInfo.trophiesForRoomTitle2 = roomInfoData.GetInt(GSBackendKeys.ROOM_INFO_TROPHIES_FOR_ROOM_TITLE_2).Value;
                roomInfo.trophiesForRoomTitle3 = roomInfoData.GetInt(GSBackendKeys.ROOM_INFO_TROPHIES_FOR_ROOM_TITLE_3).Value;
                roomInfo.roomDescription = roomInfoData.GetString(GSBackendKeys.ROOM_DESCRIPTION);
                roomInfo.roomStartTime = roomInfoData.GetLong(GSBackendKeys.ROOM_START_TIME).Value;
                roomInfo.roomDuration = 120000;
                roomInfo.roomBuffer = 60000;
                //roomInfo.roomDuration = roomInfoData.GetInt(GSBackendKeys.ROOM_DURATION).Value;

                settings.Add(roomInfo.id, roomInfo);
            }

            roomSettingsModel.settings = settings;
        }
            
        private void FillStoreSettingsModel(GSData shopSettingsData)
        {
            List<GSData> skinShopItemsData = shopSettingsData.GetGSDataList("skinShopItems");
            IOrderedDictionary<string, StoreItem> skinItems = PopulateSkinShopItems(skinShopItemsData);

            List<GSData> currencyShopItemsData = shopSettingsData.GetGSDataList("coinsShopItems");
            IOrderedDictionary<string, StoreItem> currencyItems = PopulateCurrencyShopItems(currencyShopItemsData);

            //List<GSData> avatarShopItemsData = shopSettingsData.GetGSDataList("avatarShopItems");
            //IOrderedDictionary<string, ShopItem> avatarItems = PopulateAvatarShopItems(avatarShopItemsData);

            storeSettingsModel.Initialize();
            storeSettingsModel.Add(GSBackendKeys.ShopItem.SKIN_SHOP_TAG, skinItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.COINS_SHOP_TAG, currencyItems);
            //storeSettingsModel.Add(GSBackendKeys.ShopItem.AVATAR_SHOP_TAG, avatarItems);
        }

        private IOrderedDictionary<string, StoreItem> PopulateSkinShopItems(List<GSData> skinSettingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in skinSettingsData)
            {
                var item = new SkinShopItem();
                GSParser.PopulateShopItem(item, itemData);
                GSData properties = GSParser.GetVGoodProperties(itemData);
                if (properties != null)
                {
                    item.unlockAtLevel = GSParser.GetSafeInt(properties, GSBackendKeys.SHOP_ITEM_UNLOCKATLEVEL);
                }

                items.Add(item.key, item);
            }

            return items;
        }

        private IOrderedDictionary<string, StoreItem> PopulateCurrencyShopItems(List<GSData> currencySetingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in currencySetingsData)
            {
                var item = new CurrencyShopItem();
                GSParser.PopulateShopItem(item, itemData);

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

        /*
        private IOrderedDictionary<string, ShopItem> PopulateAvatarShopItems(List<GSData> avatarSettingData)
        {
            IOrderedDictionary<string, ShopItem> items = new OrderedDictionary<string, ShopItem>();

            foreach (GSData itemData in avatarSettingData)
            {
                var item = new AvatarShopItem();
                GSParser.PopulateShopItem(item, itemData);

                items.Add(item.id, item);
            }

            return items;
        }
        */

    }
}
