/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using GameSparks.Core;

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public static class GSParser
    {
        public static string GetSafeString(GSData item, string key, string defaultVal = "unassigned")
        {
            if (key == null) return defaultVal;
            return item.ContainsKey(key) ? item.GetString(key) : defaultVal;
        }

        public static int GetSafeInt(GSData item, string key, int defaultVal = 0)
        {
            if (key == null) return defaultVal;
            return item.ContainsKey(key) ? item.GetInt(key).Value : defaultVal;
        }

        public static float GetSafeFloat(GSData item, string key, float defaultVal = 0.0f)
        {
            if (key == null) return defaultVal;
            return item.ContainsKey(key) ? item.GetFloat(key).Value : defaultVal;
        }

        public static long GetSafeLong(GSData item, string key, long defaultVal = 0)
        {
            if (key == null) return defaultVal;
            return item.ContainsKey(key) ? item.GetLong(key).Value : defaultVal;
        }

        public static bool GetSafeBool(GSData item, string key, bool defaultVal = false)
        {
            if (key == null) return defaultVal;
            return item.ContainsKey(key) ? item.GetBoolean(key).Value : defaultVal;
        }

        public static GSData GetVGoodProperties(GSData itemData, string propertyKey)
        {
            GSData propertySet = itemData.GetGSData(GSBackendKeys.SHOP_ITEM_PROPERTY_SET);
            if (propertySet != null)
            {
                GSData properties = propertySet.GetGSData(propertyKey);
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
                found = knownTags[i] !=null && tags.Contains(knownTags[i]);        // TODO: check full words
                if (!found)
                    i++;
            }

            return found ? knownTags[i]: null;
        }

        public static void ParseBundledGoods(StoreItem item, IList<GSData> bundleData)
        {
            foreach (GSData bundledGoodData in bundleData)
            {
                int qty = bundledGoodData.GetInt(GSBackendKeys.SHOP_ITEM_QUANTITY).Value;
                string itemKey = bundledGoodData.GetString(GSBackendKeys.SHOP_ITEM_ID);
                item.bundledItems.Add(itemKey, qty);
            }
        }

        public static void PopulateStoreItem(StoreItem item, GSData itemData)
        {
            const string unrecognized = "unrecognized";

            string[] tagKinds = {
                GSBackendKeys.ShopItem.SKIN_SHOP_TAG,
                GSBackendKeys.ShopItem.COINS_SHOP_TAG,
                GSBackendKeys.ShopItem.FEATURE_SHOP_TAG,
                GSBackendKeys.ShopItem.SAFE_MOVE_SHOP_TAG,
                GSBackendKeys.ShopItem.HINT_SHOP_TAG,
                GSBackendKeys.ShopItem.HINDSIGHT_SHOP_TAG,
                GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_TAG,
                GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_TAG,
                GSBackendKeys.ShopItem.POWERUP_SAFEMOVE_SHOP_TAG,
                GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_TAG,
                GSBackendKeys.ShopItem.GEMPACK_SHOP_TAG,
                GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG,
                GSBackendKeys.ShopItem.SPECIAL_BUNDLE_SHOP_TAG,
                GSBackendKeys.ShopItem.SUBSCRIPTION_TAG,
                GSBackendKeys.ShopItem.SPECIALITEM_POINTS_TAG
            };

            //string[] tagState = {
            //    "Disabled"
            //};

            List <string> tagsList = itemData.GetStringList(GSBackendKeys.SHOP_ITEM_TAGS);
            string tags = tagsList[0];

           // string tags = itemData.GetString(GSBackendKeys.SHOP_ITEM_TAGS);
            string kind = SearchTags(tags, tagKinds);
            //string state = SearchTags(tags, tagState);

            bool disabledState = itemData.GetBoolean(GSBackendKeys.SHOP_ITEM_DISABLED).Value;
            item.state = disabledState == false ? StoreItem.State.ENABLED : StoreItem.State.DISABLED;
            item.key = itemData.GetString(GSBackendKeys.SHOP_ITEM_ID);
            item.type = itemData.GetString(GSBackendKeys.SHOP_ITEM_TYPE) == "VGOOD" ? StoreItem.Type.VGOOD : StoreItem.Type.CURRENCY;
            item.kind = kind ?? unrecognized;
            item.displayName = itemData.GetString(GSBackendKeys.SHOP_ITEM_DISPLAYNAME);
            item.description = itemData.GetString(GSBackendKeys.SHOP_ITEM_DESCRIPTION);
            item.currency1Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY1COST);
            item.currency2Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY2COST);
            item.currency3Cost = GetSafeInt(itemData, GSBackendKeys.SHOP_ITEM_CURRENCY3COST);
            item.maxQuantity = GetSafeInt(itemData,GSBackendKeys.SHOP_ITEM_MAX_QUANTITY);

#if UNITY_IOS
            item.remoteProductId = GetSafeString(itemData, GSBackendKeys.SHOP_ITEM_IOS_STORE_PRODUCT_ID, null);
#else
            item.remoteProductId = GetSafeString(itemData, GSBackendKeys.SHOP_ITEM_STORE_PRODUCT_ID, null);
#endif

            var skinPropertyData = GetVGoodProperties(itemData, GSBackendKeys.SHOP_ITEM_SKIN_PROPERTY);

            if (skinPropertyData != null)
            {
                item.skinIndex = GetSafeInt(skinPropertyData, GSBackendKeys.SHOP_ITEM_SKIN_INDEX);
                item.pointsRequired = GetSafeInt(skinPropertyData, GSBackendKeys.SHOP_ITEM_SKIN_POINTS);
                LogUtil.Log(string.Format("Found Skin property for {0} index {1} points {2}", item.key, item.skinIndex, item.pointsRequired));
            }

            if (itemData.ContainsKey(GSBackendKeys.SHOP_ITEM_PROPERTY_SET))
            {
                var propertySet = itemData.GetGSData(GSBackendKeys.SHOP_ITEM_PROPERTY_SET);
                if (propertySet != null)
                {
                    
                }
            }

            // Check for payouts if item is currency type
            if (item.type == StoreItem.Type.CURRENCY)
            {
                item.currency1Payout = item.currency1Cost;
                item.currency2Payout = item.currency2Cost;
                item.currency3Payout = item.currency3Cost;

                item.currency1Cost = 0;
                item.currency2Cost = 0;
                item.currency3Cost = 0;
            }

            IList<GSData> bundleData = itemData.GetGSDataList(GSBackendKeys.SHOP_ITEM_STORE_BUNDLED_GOODS);
            if (bundleData !=  null)
            {
                item.bundledItems = new Dictionary<string, int>();
                ParseBundledGoods(item, bundleData);
            }

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

        public static void PopulateAdsRewardData(IPlayerModel playerModel, GSData data)
        {
            var rewardType = GetSafeString(data, GSBackendKeys.ClaimReward.CLAIM_REWARD_TYPE);
            if (rewardType == GSBackendKeys.ClaimReward.TYPE_BOOST_RATING)
            {
                playerModel.eloScore += GetSafeInt(data, GSBackendKeys.Rewards.RATING_BOOST);
            }
            else
            {
                playerModel.rewardQuantity = GetSafeInt(data, GSBackendKeys.PlayerDetails.REWARD_QUANITY);

                var adsRewardData = data.GetGSData(GSBackendKeys.PlayerDetails.ADS_REWARD_DATA);
                if (adsRewardData != null)
                {
                    playerModel.rewardIndex = GetSafeInt(adsRewardData, GSBackendKeys.PlayerDetails.REWARD_INDEX);
                    playerModel.rewardShortCode = adsRewardData.GetString(GSBackendKeys.PlayerDetails.REWARD_SHORT_CODE);
                    playerModel.rewardCurrentPoints = GetSafeFloat(adsRewardData, GSBackendKeys.PlayerDetails.REWARD_CURRENT_POINTS);
                    playerModel.rewardPointsRequired = GetSafeFloat(adsRewardData, GSBackendKeys.PlayerDetails.REWARD_REQUIRED_POINTS);
                }
            }
        }

        public static void GetActiveInventory(ref string activeSkinId,
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
                }
            }
        }

        public static void PopulateActiveInventory(IPlayerModel playerModel, IList<GSData> activeInventoryData)
        {
            if (activeInventoryData != null && activeInventoryData.Count > 0)
            {
                foreach (GSData item in activeInventoryData)
                {
                    string itemId = item.GetString("shopItemKey");
                    string itemKind = item.GetString("kind");

                    LogUtil.Log("*******PopulateActiveInventory itemId : " + itemId + " itemKind : " + itemKind);

                    if (itemId == "unassigned")
                    {
                        continue;
                    }

                    if (itemKind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                    {
                        playerModel.activeSkinId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATAR_TAG)
                    {
                        playerModel.avatarId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATAR_BG_COLOR_TAG)
                    {
                        playerModel.avatarBgColorId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_TAG)
                    {
                        if (!playerModel.videos.ContainsKey(itemId))
                        {
                            playerModel.videos.Add(itemId, new Video(itemId, item.GetFloat("progress")));
                        }
                    }
                }
            }
        }

        public static void PopulateActiveInventory(PublicProfile publicProfile, IList<GSData> activeInventoryData)
        {
            if (activeInventoryData != null && activeInventoryData.Count > 0)
            {
                foreach (GSData item in activeInventoryData)
                {
                    string itemId = item.GetString("shopItemKey");
                    string itemKind = item.GetString("kind");

                    LogUtil.Log("!!!!!!!!! PopulatePublicProfile PopulateActiveInventory itemId : " + itemId + " itemKind : " + itemKind);

                    if (itemId == "unassigned")
                    {
                        continue;
                    }

                    if (itemKind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                    {
                        //playerModel.activeSkinId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATAR_TAG)
                    {
                        publicProfile.avatarId = itemId;
                    }
                    else if (itemKind == GSBackendKeys.ShopItem.AVATAR_BG_COLOR_TAG)
                    {
                        publicProfile.avatarBgColorId = itemId;
                    }
                }
            }
        }

        public static void PopulatePublicProfile(PublicProfile publicProfile, GSData publicProfileData, string playerId)
		{
            publicProfile.playerId = playerId;
			publicProfile.name = publicProfileData.GetString(GSBackendKeys.PublicProfile.NAME);
			publicProfile.countryId = publicProfileData.GetString(GSBackendKeys.PublicProfile.COUNTRY_ID);
			publicProfile.eloScore = publicProfileData.GetInt(GSBackendKeys.PublicProfile.ELO_SCORE).Value;
            publicProfile.isOnline = publicProfileData.GetBoolean(GSBackendKeys.PublicProfile.IS_ONLINE).Value;
            publicProfile.isSubscriber = GetSafeBool(publicProfileData, GSBackendKeys.PublicProfile.IS_SUBSCRIBER);
           // publicProfile.name = FormatUtil.SplitFirstLastNameInitial(publicProfile.name);

            publicProfile.totalGamesWon = publicProfileData.GetInt(GSBackendKeys.PublicProfile.TOTAL_GAMES_WON).Value;
            publicProfile.totalGamesLost = publicProfileData.GetInt(GSBackendKeys.PublicProfile.TOTAL_GAMES_LOST).Value;

            long creationDateUTC = publicProfileData.GetLong(GSBackendKeys.PublicProfile.CREATION_DATE).Value;
            DateTime creationDateTime = TimeUtil.ToDateTime(creationDateUTC);
            publicProfile.creationDate = creationDateTime.ToLocalTime().ToLongDateString();
            publicProfile.creationDateShort = creationDateTime.ToLocalTime().ToString("d MMM yyyy");

            long lastSeenDateUTC = publicProfileData.GetLong(GSBackendKeys.PublicProfile.LAST_SEEN).Value;
            publicProfile.lastSeenDateTime = TimeUtil.ToDateTime(lastSeenDateUTC);
            publicProfile.lastSeen = publicProfile.lastSeenDateTime.ToLocalTime().ToLongDateString();

            IList<GSData> activeInventoryData = publicProfileData.GetGSDataList(GSBackendKeys.PublicProfile.PLAYER_ACTIVE_INVENTORY);
            PopulateActiveInventory(publicProfile, activeInventoryData);

            GSData externalIds = publicProfileData.GetGSData(GSBackendKeys.PublicProfile.EXTERNAL_IDS);
			IDictionary<ExternalAuthType, ExternalAuth> auths = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);

            publicProfile.uploadedPicId = publicProfileData.GetString(GSBackendKeys.Friend.UPLOADED_PIC_ID);

            if (auths.ContainsKey(ExternalAuthType.FACEBOOK))
			{
				ExternalAuth facebookAuthData = auths[ExternalAuthType.FACEBOOK];
				publicProfile.facebookUserId = facebookAuthData.id;
			}
		}

        public static void ParseFriend(Friend friend, GSData friendData, string friendId)
		{
            friend.gamesDrawn = GetSafeInt(friendData, GSBackendKeys.Friend.GAMES_DRAWN);
			friend.gamesLost = GetSafeInt(friendData, GSBackendKeys.Friend.GAMES_LOST);
			friend.gamesWon = GetSafeInt(friendData, GSBackendKeys.Friend.GAMES_WON);
            friend.friendType = GetSafeString(friendData, GSBackendKeys.Friend.TYPE, GSBackendKeys.Friend.TYPE_COMMUNITY);
            friend.lastMatchTimestamp = GetSafeLong(friendData, GSBackendKeys.Friend.LAST_MATCH_TIMESTAMP);
            friend.flagMask = GetSafeLong(friendData, GSBackendKeys.Friend.FLAG_MASK);

            GSData publicProfileData = friendData.GetGSData(GSBackendKeys.Friend.PUBLIC_PROFILE);
            PopulatePublicProfile(friend.publicProfile, publicProfileData, friendId);
		}

        public static void ParseIboxMessageRewards(GSData rewardsData, Dictionary<string, int> rewards)
        {
            foreach (KeyValuePair<string, Object> obj in rewardsData.BaseData)
            {
                string itemShortCode = obj.Key;
                var qtyVar = obj.Value;
                int qtyInt = Int32.Parse(qtyVar.ToString());
                TLUtils.LogUtil.Log("+++++====>" + itemShortCode + " qty: " + qtyInt.ToString());
                rewards.Add(itemShortCode, qtyInt);
            }
        }

        public static void ParseInboxMessage(InboxMessage msg, GSData data)
        {
            msg.id = data.GetString("id");
            msg.type = data.GetString("type");
            msg.heading = data.GetString("heading");
            msg.subHeading = data.GetString("body");
            msg.timeStamp = data.GetLong("time").Value;
            msg.chestType = GetSafeString(data, "chestType");
            msg.tournamentType = GetSafeString(data, "tournamentType");
            msg.league = GetSafeString(data, "league");

            GSData rewardsData = data.GetGSData("reward");
            ParseIboxMessageRewards(rewardsData, msg.rewards);
        }

        public static void LogPublicProfile(PublicProfile publicProfile)
		{
			LogUtil.Log("********** publicProfile.name" + " " + publicProfile.name);
			LogUtil.Log("********** publicProfile.countryId" + " " + publicProfile.countryId);
			LogUtil.Log("********** publicProfile.eloScore" + " " + publicProfile.eloScore);
            LogUtil.Log("********** publicProfile.facebookUserId" + " " + publicProfile.facebookUserId);
		}

		public static void LogFriend(Friend friend)
		{
			LogUtil.Log("********** friend.gamesDrawn" + " " + friend.gamesDrawn);
			LogUtil.Log("********** friend.gamesLost" + " " + friend.gamesLost);
			LogUtil.Log("********** friend.gamesWon" + " " + friend.gamesWon);
            LogUtil.Log("********** friend.lastMatchTimestamp" + " " + friend.lastMatchTimestamp);

			LogPublicProfile(friend.publicProfile);
		}

		public static void LogFriends(string title, IDictionary<string, Friend> friends)
		{
			LogUtil.Log("********** " + title);
			foreach (KeyValuePair<string, Friend> friend in friends)
			{
				LogFriend(friend.Value);
			}
		}

        public static void LogInboxMessage(InboxMessage msg)
        {
            TLUtils.LogUtil.Log("<<---------- Inbox Message <<----------");
            TLUtils.LogUtil.Log("id = " + msg.id);
            TLUtils.LogUtil.Log("type = " + msg.type);
            TLUtils.LogUtil.Log("heading = " + msg.heading);
            TLUtils.LogUtil.Log("subHeading = " + msg.subHeading);
            TLUtils.LogUtil.Log("timeStamp = " + msg.timeStamp);

            TLUtils.LogUtil.Log("rewards:");
            foreach (KeyValuePair<string, int> item in msg.rewards)
            {
                TLUtils.LogUtil.Log("shortCode = " + item.Key + "qty = " + item.Value);
            }

            TLUtils.LogUtil.Log("<<---------- Inbox Message End <<----------");
        }

        public static void LogPlayerInfo(IPlayerModel playerModel)
        {
            LogUtil.Log("******************** BEGIN PLAYER INFO ********************");

            // Player Private Profile
            LogUtil.Log("********** playerModel.id" + " " + playerModel.id);
            LogUtil.Log("********** playerModel.creationDate" + " " + playerModel.creationDate);
            LogUtil.Log("********** playerModel.tag" + " " + playerModel.tag);
            LogUtil.Log("********** playerModel.name" + " " + playerModel.name);
            LogUtil.Log("********** playerModel.countryId" + " " + playerModel.countryId);
            LogUtil.Log("********** playerModel.totalGamesWon" + " " + playerModel.totalGamesWon);
            LogUtil.Log("********** playerModel.totalGamesLost" + " " + playerModel.totalGamesLost);
            LogUtil.Log("********** playerModel.totalGamesDrawn" + " " + playerModel.totalGamesDrawn);
            LogUtil.Log("********** playerModel.removeAdsTimeStamp" + " " + playerModel.removeAdsTimeStamp);
            LogUtil.Log("********** playerModel.removeAdsTimePeriod" + " " + playerModel.removeAdsTimePeriod);
            LogUtil.Log("********** playerModel.removeAdsTimePeriod" + " " + playerModel.cpuPowerupUsedCount);
            LogUtil.Log("********** playerModel.uploadedPicId" + " " + playerModel.uploadedPicId);
            // Player Public Profile
            //PublicProfile publicProfile { get; }

            // Currency 
            LogUtil.Log("********** playerModel.bucks" + " " + playerModel.bucks);

            // League & Rating
            LogUtil.Log("********** playerModel.eloScore" + " " + playerModel.eloScore);


            LogUtil.Log("********** playerModel.isPremium" + " " + playerModel.isPremium);

            // The keys of the dictionary are the IDs of the rooms.
            //IDictionary<string, RoomRecord> roomRecords { get; set; }

            // Social
            //IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }


            // Ads Info
            LogUtil.Log("********** playerModel.adLifetimeImpressions" + " " + playerModel.adLifetimeImpressions);

            // Inventory
            foreach (KeyValuePair<string, int> item in playerModel.inventory)
            {
                LogUtil.Log("********** playerModel.inventory " + item.Key + " Quantity: " + item.Value);
            }
            LogUtil.Log("********** playerModel.activeSkinId" + " " + playerModel.activeSkinId);

            LogUtil.Log("******************** END PLAYER INFO ********************");
        }
    }
}
