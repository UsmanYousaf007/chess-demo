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

        public static void PopulateStoreItem(StoreItem item, GSData itemData)
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

            item.state = state == null ? StoreItem.State.ENABLED : StoreItem.State.DISABLED;
            item.key = itemData.GetString(GSBackendKeys.SHOP_ITEM_ID);
            item.type = itemData.GetString(GSBackendKeys.SHOP_ITEM_TYPE) == "VGOOD" ? StoreItem.Type.VGOOD : StoreItem.Type.CURRENCY;
            item.kind = kind ?? unrecognized;
            item.tier = tier ?? defaultTier;
            item.displayName = itemData.GetString(GSBackendKeys.SHOP_ITEM_DISPLAYNAME);
            item.description = itemData.GetString(GSBackendKeys.SHOP_ITEM_DESCRIPTION);
            item.currency1Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY1COST);
            item.currency2Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY2COST);
            item.maxQuantity = GetSafeInt(itemData,GSBackendKeys.SHOP_ITEM_MAX_QUANTITY);
            item.remoteProductId = GSParser.GetSafeString(itemData, GSBackendKeys.SHOP_ITEM_STORE_PRODUCT_ID, null);

            LogUtil.Log("********** PopulateShopItem: " + item.key);
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

        public static void GetActiveInventory(ref string activeSkinId,
            ref string activeAvatarId,
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
                        activeSkinId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATAR_SHOP_TAG)
                    {
                        activeAvatarId = itemId;
                    }
                }
            }
        }

        public static void PopulateActiveInventory(IPlayerModel playerModel,
            IList<GSData> activeInventoryData)
        {
            if (activeInventoryData != null && activeInventoryData.Count > 0)
            {
                string activeSkinId = "";
                string activeAvatarId = "";

                GetActiveInventory(ref activeSkinId, ref activeAvatarId, activeInventoryData);

                playerModel.activeSkinId = activeSkinId;
                playerModel.activeAvatarId = activeAvatarId;
            }
        }

        public static void LogPlayerInfo(IPlayerModel playerModel)
        {
            LogUtil.Log("******************** BEGIN PLAYER INFO ********************");

            // Player Private Profile
            LogUtil.Log("********** playerModel.id" + " " + playerModel.id);
            LogUtil.Log("********** playerModel.tag" + " " + playerModel.tag);
            LogUtil.Log("********** playerModel.name" + " " + playerModel.name);
            LogUtil.Log("********** playerModel.countryId" + " " + playerModel.countryId);
            LogUtil.Log("********** playerModel.profilePicture" + " " + playerModel.profilePic);
            LogUtil.Log("********** playerModel.totalGamesWon" + " " + playerModel.totalGamesWon);
            LogUtil.Log("********** playerModel.totalGamesLost" + " " + playerModel.totalGamesLost);
            LogUtil.Log("********** playerModel.totalGamesDrawn" + " " + playerModel.totalGamesDrawn);

            // Player Public Profile
            //PublicProfile publicProfile { get; }

            // Currency 
            LogUtil.Log("********** playerModel.bucks" + " " + playerModel.bucks);

            // League & Rating
            LogUtil.Log("********** playerModel.eloScore" + " " + playerModel.eloScore);

            // The keys of the dictionary are the IDs of the rooms.
            //IDictionary<string, RoomRecord> roomRecords { get; set; }

            // Social
            //IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }


            // Ads Info
            LogUtil.Log("********** playerModel.adLifetimeImpressions" + " " + playerModel.adLifetimeImpressions);
            LogUtil.Log("********** playerModel.adSlotImpressions" + " " + playerModel.adSlotImpressions);
            LogUtil.Log("********** playerModel.adSlotId" + " " + playerModel.adSlotId);

            // Inventory
            foreach (KeyValuePair<string, int> item in playerModel.inventory)
            {
                LogUtil.Log("********** playerModel.inventory " + item.Key + " Quantity: " + item.Value);
            }
            LogUtil.Log("********** playerModel.activeSkinId" + " " + playerModel.activeSkinId);
            LogUtil.Log("********** playerModel.activeAvatarId" + " " + playerModel.activeAvatarId);

            LogUtil.Log("******************** END PLAYER INFO ********************");
        }
    }
}
