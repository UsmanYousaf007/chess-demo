/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-10-24 16:06:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using GameSparks.Core;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public static class GSParser
    {
        public static IDictionary<string, RoomRecord> ParseRoomRecords(
            GSData records,
            IOrderedDictionary<string, RoomSetting> roomSettings)
        {
            IDictionary<string, RoomRecord> roomRecords = new Dictionary<string, RoomRecord>();

            // Since the room records coming from the server contain records
            // only for those rooms which the user has played at least once so
            // we are making sure here that we fill in the model for the rooms
            // that are left out on the server. Ideally the server should've
            // provided room records for all the rooms.
            //
            // For this reason we've commented this code block and have written
            // another one further down which fills in the model for the missing
            // rooms data.
            //
            // TODO: Make the server send data for all the rooms regardless of
            // if they've been played or not and modify the code below for that.
            // We have to check if we really should be doing that.
            /*foreach (KeyValuePair<string, object> e in records.BaseData)
            {
                string roomId = e.Key;
                GSData roomRecord = (GSData)e.Value;

                RoomRecord record;
                record.id = roomRecord.GetString(GSBackendKeys.ROOM_ID);
                record.gamesWon = (int)roomRecord.GetInt(GSBackendKeys.ROOM_GAMES_WON);
                record.gamesLost = (int)roomRecord.GetInt(GSBackendKeys.ROOM_GAMES_LOST);
                record.gamesDrawn = (int)roomRecord.GetInt(GSBackendKeys.ROOM_GAMES_DRAWN);
                record.trophiesWon = (int)roomRecord.GetInt(GSBackendKeys.ROOM_TROPHIES_WON);
                record.roomTitleId = roomRecord.GetString(GSBackendKeys.ROOM_ROOM_TITLE_ID);

                roomRecords.Add(roomId, record);
            }*/

            IDictionary<string, object> recordsBase = records.BaseData;

            foreach (KeyValuePair<string, RoomSetting> room in roomSettings)
            {
                RoomSetting roomInfo = room.Value;
                string roomId = roomInfo.id;
                RoomRecord record;

                if (recordsBase.ContainsKey(roomId))
                {
                    GSData roomRecord = (GSData)recordsBase[roomId];

                    record.id = roomRecord.GetString(GSBackendKeys.ROOM_ID);
                    record.gamesWon = roomRecord.GetInt(GSBackendKeys.ROOM_GAMES_WON).Value;
                    record.gamesLost = roomRecord.GetInt(GSBackendKeys.ROOM_GAMES_LOST).Value;
                    record.gamesDrawn = roomRecord.GetInt(GSBackendKeys.ROOM_GAMES_DRAWN).Value;
                    record.trophiesWon = roomRecord.GetInt(GSBackendKeys.ROOM_TROPHIES_WON).Value;
                    record.roomTitleId = roomRecord.GetString(GSBackendKeys.ROOM_ROOM_TITLE_ID);
                }
                else
                {
                    record.id = roomId;
                    record.gamesWon = 0;
                    record.gamesLost = 0;
                    record.gamesDrawn = 0;
                    record.trophiesWon = 0;
                    record.roomTitleId = null;
                }

                roomRecords.Add(roomId, record);
            }

            return roomRecords;
        }

        public static string GetSafeString(GSData item, string key, string defaultVal = "unassigned")
        {
            return item.ContainsKey(key) ? item.GetString(key) : defaultVal;
        }

        public static int GetSafeInt(GSData item, string key, int defaultVal = 0)
        {
            return item.ContainsKey(key) ? item.GetInt(key).Value : defaultVal;
        }

        public static float GetSafeFloat(GSData item, string key, float defaultVal = 0.0f)
        {
            return item.ContainsKey(key) ? item.GetFloat(key).Value : defaultVal;
        }

        public static GSData GetVGoodProperties(GSData itemData)
        {
            GSData propertySet = itemData.GetGSData("propertySet");
            if (propertySet != null)
            {
                GSData properties = propertySet.GetGSData("properties");
                if (properties != null)
                {
                    return properties;
                }
            }

            return null;
        }

        public static string SearchTags(string tags, string[] knownTags)
        {
            int i = 0;
            bool found = false;

            while (!found && i < knownTags.Length)
            {
                found = tags.Contains(knownTags[i]);        // TODO: check full words
                if (!found)
                    i++;
            }

            return found ? knownTags[i]: null;
        }

        public static void PopulateShopItem(ShopItem item, GSData itemData)
        {
            const string unrecognized = "unrecognized";
            const string defaultTier = GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON;
 
            string[] tagTiers = { 
                GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON, 
                GSBackendKeys.ShopItem.SHOP_ITEM_TIER_RARE, 
                GSBackendKeys.ShopItem.SHOP_ITEM_TIER_EPIC, 
                GSBackendKeys.ShopItem.SHOP_ITEM_TIER_LEGENDARY 
            };
                
            string[] tagKinds = {
                GSBackendKeys.ShopItem.SKIN_SHOP_TAG,
                GSBackendKeys.ShopItem.AVATARBORDER_SHOP_TAG,       // Border must come before AVATAR_SHOP_TAG
                GSBackendKeys.ShopItem.AVATAR_SHOP_TAG,             // TODO: Make this independent of order
                GSBackendKeys.ShopItem.COINS_SHOP_TAG
            };

            string[] tagState = {
                "Disabled"
            };

            string tags = itemData.GetString(GSBackendKeys.SHOP_ITEM_TAGS);
            string kind = SearchTags(tags, tagKinds);
            string tier = SearchTags(tags, tagTiers);
            string state = SearchTags(tags, tagState);

            item.state = state ?? "Enabled";
            item.id = itemData.GetString(GSBackendKeys.SHOP_ITEM_ID);
            item.type = itemData.GetString(GSBackendKeys.SHOP_ITEM_TYPE);
            item.kind = kind ?? unrecognized;
            item.tier = tier ?? defaultTier;
            item.displayName = itemData.GetString(GSBackendKeys.SHOP_ITEM_DISPLAYNAME);
            item.description = itemData.GetString(GSBackendKeys.SHOP_ITEM_DESCRIPTION);
            item.currency1Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY1COST);
            item.currency2Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY2COST);
            item.maxQuantity = GetSafeInt(itemData,GSBackendKeys.SHOP_ITEM_MAX_QUANTITY);
            item.storeProductId = GSParser.GetSafeString(itemData, GSBackendKeys.SHOP_ITEM_STORE_PRODUCT_ID, null);

            LogUtil.Log("********** PopulateShopItem: " + item.id);
        }

        public static void PopulateInventory(IOrderedDictionary<string, int> items, GSData inventoryData)
        {
            if (inventoryData != null && inventoryData.BaseData.Count > 0)
            {
                char[] trimChars = { '{', '}' }; 
                string data = inventoryData.JSON.Trim(trimChars);
                string[] fields = data.Split(',');

                foreach (string field in fields)
                {
                    string[] valuePair = field.Split(':');
                    string itemId = valuePair[0].Trim('\"');
                    int quantity = int.Parse(valuePair[1]);

                    items.Add(itemId, quantity);
                }
            }
        }

        public static void GetActiveInventory(ref string activeChessSkinsId,
            ref string activeAvatarsId,
            ref string activeAvatarsBorderId,
            IList<GSData> activeInventoryData)
        {
            if (activeInventoryData != null && activeInventoryData.Count > 0)
            {
                foreach (GSData item in activeInventoryData)
                {
                    string itemId = item.GetString("shopItemKey");
                    string itemKind = item.GetString("kind");

                    if (itemId == "unassigned")
                    {
                        continue;
                    }

                    if (itemKind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                    {
                        activeChessSkinsId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATAR_SHOP_TAG)
                    {
                        activeAvatarsId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATARBORDER_SHOP_TAG)
                    {
                        activeAvatarsBorderId = itemId;
                    }
                }
            }
        }

        public static void PopulateActiveInventory(IInventoryModel inventoryModel,
            IList<GSData> activeInventoryData)
        {
            if (activeInventoryData != null && activeInventoryData.Count > 0)
            {
                string activeChessSkinsId = "";
                string activeAvatarsId = "";
                string activeAvatarsBorderId = "";
                GetActiveInventory(ref activeChessSkinsId,
                    ref activeAvatarsId,
                    ref activeAvatarsBorderId,
                    activeInventoryData);

                inventoryModel.activeChessSkinId = activeChessSkinsId;
                inventoryModel.activeAvatarsId = activeAvatarsId;
                inventoryModel.activeAvatarsBorderId = activeAvatarsBorderId;
            }
        }

        public static void LogInventory(IOrderedDictionary<string, int> items)
        {
            LogUtil.Log("---------------- LOG INVENTORY ------------------");
            foreach (KeyValuePair<string, int> item in items)
            {
                LogUtil.Log("Inventory Item: " + item.Key + " Quantity: " + item.Value);
            }
        }

        public static void LogActiveInventory(IInventoryModel inventoryModel)
        {
            LogUtil.Log("---------------- LOG ACTIVE INVENTORY ------------------");
            LogUtil.Log("Active Inventory Item: " + inventoryModel.activeChessSkinId);
            LogUtil.Log("Active Inventory Item: " + inventoryModel.activeAvatarsId);
            LogUtil.Log("Active Inventory Item: " + inventoryModel.activeAvatarsBorderId);
        }

    }
}
