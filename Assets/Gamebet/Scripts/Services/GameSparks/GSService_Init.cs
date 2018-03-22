/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-16 02:54:18 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        // Models
        [Inject] public ILeagueSettingsModel leagueSettingsModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public ITitleSettingsModel titleSettingsModel { get; set; }
        [Inject] public IShopSettingsModel shopSettingsModel { get; set; }
        [Inject] public IInventoryModel inventoryModel { get; set; }
        [Inject] public IForgeSettingsModel forgeSettingsModel { get; set; }

        public IPromise<BackendResult> GetInitData()
        {
            return new GSGetInitDataRequest().Send(OnGetInitDataSuccess);
        }

        private void OnGetInitDataSuccess(LogEventResponse response)
        {
            IList<GSData> leagueSettingsData = response.ScriptData.GetGSDataList(GSBackendKeys.LEAGUE_SETTINGS);
            FillLeagueSettingsModel(leagueSettingsData);

            GSData levelSettingsData = response.ScriptData.GetGSData(GSBackendKeys.LEVEL_SETTINGS);
            FillLevelSettingsModel(levelSettingsData);

            IList<GSData> roomSettingsData = response.ScriptData.GetGSDataList(GSBackendKeys.ROOM_SETTINGS);
            FillRoomSettingsModel(roomSettingsData);

            GSData shopSettingsData = response.ScriptData.GetGSData(GSBackendKeys.SHOP_SETTINGS);
            FillShopSettingsModel(shopSettingsData);

            IList<GSData> forgeItemSettingsData = response.ScriptData.GetGSDataList(GSBackendKeys.FORGE_ITEM_SETTINGS);
            FillForgeItemSettingsModel(forgeItemSettingsData);

            IList<GSData> lootBoxesData = response.ScriptData.GetGSDataList(GSBackendKeys.LOOT_BOXES_PLAYER_LOOT);
            IList<GSData> activeInventoryData = response.ScriptData.GetGSDataList(GSBackendKeys.PLAYER_ACTIVE_INVENTORY);

            GSData accountDetailsData = response.ScriptData.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            GSData inventoryData = accountDetailsData.GetGSData(GSBackendKeys.ACCOUNT_VIRTUALGOODS);

            FillInventoryData(inventoryData, lootBoxesData, activeInventoryData);

            // Call the same method as for successful retrieval of account
            // details since we process the account details data in exactly the
            // same manner.
            OnAccountDetailsSuccess(accountDetailsResponse);

            CheckAndHandleMatchResume(response);

            // Start Unity InApp Purchase service object 
            InAppPurchase.CreateInAppPurchaseObject(shopSettingsModel);

            //InAppPurchase.instance.Initialize(shopSettingsModel);
            //InAppPurchase.instance.BuyProduct("Coins1");
            //AvatarItemContainer X = AvatarItemContainer.Load("AvatarVodoo");
            //X.Unload();
            //AvatarBorderContainer Y = AvatarBorderContainer.Load("AvatarGold");
            //ChatEmotesContainer Z = ChatEmotesContainer.Load();
            //ChatEmote H = Z.GetItem("ChatEmoteAngry");
            //LogUtil.Log("ITEM LOADED --------------------->  " + H.sprite);
            //BuyVirtualGoods(1, 1, "LootBoxTier1");

            //GrantForgedItem("ForgeCardSkinArmy"); 

            //SellForgeCards("ForgeCardSkinArmy", 1);

            //ClaimLoot("520322228836");
            //UpdateActiveInventory("SkinArmy", "AvatarBeachBoy", "AvatarBorderCommonBronze");
        }

        private void FillForgeItemSettingsModel(IList<GSData> forgeItemSettingsData)
        {
            IOrderedDictionary<string, ForgeItem> settings = new OrderedDictionary<string, ForgeItem>();
            foreach (GSData forgeItemData in forgeItemSettingsData)
            {
                ForgeItem forgeItem = new ForgeItem();
                forgeItem.forgeItemKey = forgeItemData.GetString(GSBackendKeys.FORGE_ITEM_ITEM_KEY);
                forgeItem.sellCoins = forgeItemData.GetInt(GSBackendKeys.FORGE_ITEM_SELL_COINS).Value;
                forgeItem.requiredQuantity = forgeItemData.GetInt(GSBackendKeys.FORGE_ITEM_REQUIRED_QUANTITY).Value;
                string forgeCardKey = forgeItemData.GetString(GSBackendKeys.FORGE_ITEM_CARD_KEY);

                LogUtil.Log("forgeItem.forgeItemKey: " + forgeItem.forgeItemKey + " forgeCardKey:" + forgeCardKey , "red" );

                settings.Add(forgeCardKey, forgeItem);
            }

            forgeSettingsModel.forgeItems = settings;
        }

        private void FillInventoryData(GSData inventoryData, IList<GSData> lootBoxesData, IList<GSData> playerActiveInventory)
        {
            IOrderedDictionary<string, int> allShopItems = new OrderedDictionary<string, int>(); 
            IList<LootBox> lootBoxItems = new List<LootBox>();

            GSParser.PopulateInventory(allShopItems, inventoryData);
            GSParser.PopulateLootBoxes(lootBoxItems, lootBoxesData);

            inventoryModel.allShopItems = allShopItems;
            inventoryModel.lootBoxItems = lootBoxItems;

            GSParser.PopulateActiveInventory(inventoryModel, playerActiveInventory);

            GSParser.LogInventory(allShopItems);
            GSParser.LogLootBoxes(lootBoxItems);
            GSParser.LogActiveInventory(inventoryModel);
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

        private void FillShopSettingsModel(GSData shopSettingsData)
        {
            List<GSData> skinShopItemsData = shopSettingsData.GetGSDataList("skinShopItems");
            PopulateSkinShopItems(skinShopItemsData);

            List<GSData> currencyShopItemsData = shopSettingsData.GetGSDataList("coinsShopItems");
            PopulateCurrencyShopItems(currencyShopItemsData);

            List<GSData> avatarShopItemsData = shopSettingsData.GetGSDataList("avatarShopItems");
            PopulateAvatarShopItems(avatarShopItemsData);

            List<GSData> chatpackShopItemsData = shopSettingsData.GetGSDataList("chatpackShopItems");
            PopulateChatapackShopItems(chatpackShopItemsData);

            List<GSData> avatarBorderShopItemsData = shopSettingsData.GetGSDataList("avatarBorderShopItems");
            PopulateAvatarBorderShopItems(avatarBorderShopItemsData);

            List<GSData> forgeCardShopItemsData = shopSettingsData.GetGSDataList("forgeCardShopItems");
            PopulateForgeCardShopItems(forgeCardShopItemsData);

            List<GSData> lootBoxShopItemsData = shopSettingsData.GetGSDataList("lootBoxShopItems");
            PopulateLootBoxShopItems(lootBoxShopItemsData);

            shopSettingsModel.allShopItems = new OrderedDictionary<string, ShopItem>();

            foreach (KeyValuePair<string, SkinShopItem> shopItem in shopSettingsModel.skinShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            }
            foreach (KeyValuePair<string, CurrencyShopItem> shopItem in shopSettingsModel.currencyShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            }        
            foreach (KeyValuePair<string, AvatarShopItem> shopItem in shopSettingsModel.avatarShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            }         
            foreach (KeyValuePair<string, ChatpackShopItem> shopItem in shopSettingsModel.chatpackShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            } 
            foreach (KeyValuePair<string, AvatarBorderShopItem> shopItem in shopSettingsModel.avatarBorderShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            } 
            foreach (KeyValuePair<string, ForgeCardShopItem> shopItem in shopSettingsModel.forgeCardShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            } 
            foreach (KeyValuePair<string, LootBoxShopItem> shopItem in shopSettingsModel.lootBoxShopItems)
            {
                shopSettingsModel.allShopItems.Add(shopItem.Key, shopItem.Value);
            } 
        }

        private void PopulateSkinShopItems(List<GSData> skinSettingsData)
        {
            IOrderedDictionary<string, SkinShopItem> items = new OrderedDictionary<string, SkinShopItem>();

            foreach (GSData itemData in skinSettingsData)
            {
                var item = new SkinShopItem();
                GSParser.PopulateShopItem(item, itemData);
                GSData properties = GSParser.GetVGoodProperties(itemData);
                if (properties != null)
                {
                    item.unlockAtLevel = GSParser.GetSafeInt(properties, GSBackendKeys.SHOP_ITEM_UNLOCKATLEVEL);
                }

                items.Add(item.id, item);
            }

            shopSettingsModel.skinShopItems = items;
        }

        private void PopulateCurrencyShopItems(List<GSData> currencySetingsData)
        {
            IOrderedDictionary<string, CurrencyShopItem> items = new OrderedDictionary<string, CurrencyShopItem>();

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

                items.Add(item.id, item);
            }

            shopSettingsModel.currencyShopItems = items;
        }

        private void PopulateAvatarShopItems(List<GSData> avatarSettingData)
        {
            IOrderedDictionary<string, AvatarShopItem> items = new OrderedDictionary<string, AvatarShopItem>();

            foreach (GSData itemData in avatarSettingData)
            {
                var item = new AvatarShopItem();
                GSParser.PopulateShopItem(item, itemData);

                items.Add(item.id, item);
            }

            shopSettingsModel.avatarShopItems = items;
        }

        private void PopulateAvatarBorderShopItems(List<GSData> avatarBorderSettingData)
        {
            IOrderedDictionary<string, AvatarBorderShopItem> items = new OrderedDictionary<string, AvatarBorderShopItem>();

            foreach (GSData itemData in avatarBorderSettingData)
            {
                var item = new AvatarBorderShopItem();
                GSParser.PopulateShopItem(item, itemData);

                items.Add(item.id, item);
            }

            shopSettingsModel.avatarBorderShopItems = items;
        }

        private void PopulateChatapackShopItems(List<GSData> chatpackSettingsData)
        {
            IOrderedDictionary<string, ChatpackShopItem> items = new OrderedDictionary<string, ChatpackShopItem>();

            foreach (GSData itemData in chatpackSettingsData)
            {
                var item = new ChatpackShopItem();
                GSParser.PopulateShopItem(item, itemData);

                items.Add(item.id, item);
            }

            shopSettingsModel.chatpackShopItems = items;
        }

        private void PopulateForgeCardShopItems(List<GSData> forgeCardSettingsData)
        {
            IOrderedDictionary<string, ForgeCardShopItem> items = new OrderedDictionary<string, ForgeCardShopItem>();

            foreach (GSData itemData in forgeCardSettingsData)
            {
                var item = new ForgeCardShopItem();
                GSParser.PopulateShopItem(item, itemData);

                items.Add(item.id, item);
            }

            shopSettingsModel.forgeCardShopItems = items;
        }

        private void PopulateLootBoxShopItems(List<GSData> lootBoxSettingsData)
        {
            IOrderedDictionary<string, LootBoxShopItem> items = new OrderedDictionary<string, LootBoxShopItem>();

            foreach (GSData itemData in lootBoxSettingsData)
            {
                var item = new LootBoxShopItem();
                GSParser.PopulateShopItem(item, itemData);

                string itemDescriptionsString = GSParser.GetSafeString(itemData, GSBackendKeys.SHOP_ITEM_DESCRIPTION);
                string[] itemDescriptinsList = itemDescriptionsString.Split(';');
                item.itemDescriptions = new List<string>();
                for (int i = 0; i < itemDescriptinsList.Length; i++)
                {
                    item.itemDescriptions.Add(itemDescriptinsList[i]);
                }

                string data = itemData.GetString("tags").Trim('\"');
                string[] fields = data.Split(',');

                item.weightTier = fields[1];

                items.Add(item.id, item);
            }

            shopSettingsModel.lootBoxShopItems = items;
        }
    }
}
