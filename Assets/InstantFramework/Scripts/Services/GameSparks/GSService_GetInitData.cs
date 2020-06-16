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
            return new GSGetInitDataRequest(GetRequestContext()).Send(appVersion, appData, OnGetInitDataSuccess);
        }

        void OnGetInitDataSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            appInfoModel.androidURL = response.ScriptData.GetString(GSBackendKeys.APP_ANDROID_URL);
            appInfoModel.iosURL = response.ScriptData.GetString(GSBackendKeys.APP_IOS_URL);
            appInfoModel.appBackendVersionValid = response.ScriptData.GetBoolean(GSBackendKeys.APP_VERSION_VALID).Value;
            appInfoModel.contactSupportURL = response.ScriptData.GetString(GSBackendKeys.CONTACT_SUPPORT_URL);

            GSData gsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.GAME_SETTINGS);
            FillGameSettingsModel(gsSettingsData);

            // Check app version match with back end. Bail if there is mismatch.
            if (appInfoModel.appBackendVersionValid == false)
            {
                return;
            }

            // Check if game maintenance mode is On
            if (settingsModel.maintenanceFlag == true)
            {
                return;
            }

            appInfoModel.rateAppThreshold = response.ScriptData.GetInt(GSBackendKeys.APP_RATE_APP_THRESHOLD).Value;
            appInfoModel.onlineCount = Int32.Parse(response.ScriptData.GetString(GSBackendKeys.APP_ONLINE_COUNT));

            GSData storeSettingsData = response.ScriptData.GetGSData(GSBackendKeys.SHOP_SETTINGS);
            FillStoreSettingsModel(storeSettingsData);

            GSData adsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.ADS_SETTINGS);
            FillAdsSettingsModel(adsSettingsData);

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);

            GSData chatData = response.ScriptData.GetGSData(GSBackendKeys.CHAT);
            FillChatModel(chatData);

            GSData rewardsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.Rewards.REWARDS_SETTINGS);
            FillRewardsSettingsModel(rewardsSettingsData);

            storeAvailableSignal.Dispatch(false);

            ParseActiveChallenges(response.ScriptData);

            // Parse active quick match
            string challengeId = response.ScriptData.GetString("challengeId");
            LogUtil.Log("GSInitData: RESUME ChallengeID = " + challengeId, "cyan");
            if (challengeId != null)
            {
                GSData challengeData = response.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                ParseChallengeData(challengeId, challengeData);
                // The matchInfoModel.activeChallengeId is retained for the session and maintained by the client so it 
                // need not be set from the server. Do not set activeChallengeId here.
            }

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
                        storeItem.remoteProductPrice = storeService.GetItemLocalizedPrice(storeItem.remoteProductId);
                        storeItem.remoteProductCurrencyCode = storeService.GetItemCurrencyCode(storeItem.remoteProductId);
                        storeItem.productPrice = storeService.GetItemPrice(storeItem.remoteProductId);
                    }
                }

                storeAvailableSignal.Dispatch(true);
            }

            if (playerModel.HasSubscription())
            {
                settingsModel.maxLongMatchCount = settingsModel.maxLongMatchCountPremium;
                settingsModel.maxFriendsCount = settingsModel.maxFriendsCountPremium;

                LogUtil.Log("======= max match count " + settingsModel.maxLongMatchCount + " friends count " + settingsModel.maxFriendsCount);
            }

            setDefaultSkinSignal.Dispatch();

            if (playerModel.subscriptionExipryTimeStamp > 0)
            {
                getInitDataCompleteSignal.Dispatch();
            }
        }

        private void FillPlayerDetails(GSData playerDetailsData)
        {
            playerModel.id = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.PLAYER_ID);
            playerModel.creationDate = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.CREATION_DATE).Value;
            playerModel.tag = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.TAG);
            playerModel.name = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.DISPLAY_NAME);
            playerModel.countryId = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.COUNTRY_ID);
            playerModel.bucks = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.BUCKS).Value;
            playerModel.eloScore = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.ELO_SCORE).Value;
            playerModel.isPremium = playerDetailsData.GetBoolean(GSBackendKeys.PlayerDetails.IS_PREMIUM).Value;
            playerModel.totalGamesWon = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.GAMES_WON).Value;
            playerModel.totalGamesLost = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.GAMES_LOST).Value;
            playerModel.totalGamesDrawn = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.GAMES_DRAWN).Value;
            playerModel.adLifetimeImpressions = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.AD_LIFETIME_IMPRESSIONS).Value;
            playerModel.removeAdsTimeStamp = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.REMOVE_ADS_TIMESTAMP).Value;
            playerModel.removeAdsTimePeriod = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.REMOVE_ADS_TIMEPERIOD).Value;
            playerModel.editedName = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.EDITED_NAME);
            playerModel.isFBConnectRewardClaimed = playerDetailsData.GetBoolean(GSBackendKeys.PlayerDetails.IS_FACEBOOK_REWARD_CLAIMED).Value;
            playerModel.cpuPowerupUsedCount = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.CPU_POWERUP_USED_COUNT).Value;

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.SUBSCRIPTION_EXPIRY_TIMESTAMP))
            {
                playerModel.subscriptionExipryTimeStamp = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.SUBSCRIPTION_EXPIRY_TIMESTAMP).Value;
            }

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.SUBSCRIPTION_TYPE))
            {
                playerModel.subscriptionType = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.SUBSCRIPTION_TYPE);
            }

            // Split name to first and last initial
            // TODO: split in View
            //playerModel.name = FormatUtil.SplitFirstLastNameInitial(playerModel.name);

            IOrderedDictionary<string, int> inventory = new OrderedDictionary<string, int>();
            GSData inventoryData = playerDetailsData.GetGSData(GSBackendKeys.PlayerDetails.INVENTORY);
            GSParser.PopulateInventory(inventory, inventoryData);
            playerModel.inventory = inventory;

            // Populate inventory data
            IList<GSData> playerActiveInventoryData = playerDetailsData.GetGSDataList(GSBackendKeys.PlayerDetails.PLAYER_ACTIVE_INVENTORY);
            GSParser.PopulateActiveInventory(playerModel, playerActiveInventoryData);

            // Populate friends data
            GSData friendsList = playerDetailsData.GetGSData(GSBackendKeys.FRIENDS);
            PopulateFriends(playerModel.friends, friendsList);
            GSData blockedList = playerDetailsData.GetGSData(GSBackendKeys.BLOCKED);
            PopulateFriends(playerModel.blocked, blockedList, true);

            GSParser.PopulateAdsRewardData(playerModel, playerDetailsData);
            LogUtil.Log(string.Format("Found ads reward data index {0} current {1} required {2}", playerModel.rewardIndex, playerModel.rewardCurrentPoints, playerModel.rewardPointsRequired));

            GSParser.LogPlayerInfo(playerModel);
            GSParser.LogFriends("friends", playerModel.friends);
        }

        private void FillAdsSettingsModel(GSData adsSettingsData)
        {
            adsSettingsModel.slotHour = adsSettingsData.GetInt(GSBackendKeys.ADS_SLOT_HOUR).Value;
            adsSettingsModel.freeNoAdsPeriod = adsSettingsData.GetInt(GSBackendKeys.ADS_FREE_NO_ADS_PERIOD).Value;
            adsSettingsModel.globalCap = adsSettingsData.GetInt(GSBackendKeys.ADS_GLOBAL_CAP).Value;
            adsSettingsModel.rewardedVideoCap = adsSettingsData.GetInt(GSBackendKeys.ADS_REWARDED_VIDEO_CAP).Value;
            adsSettingsModel.interstitialCap = adsSettingsData.GetInt(GSBackendKeys.ADS_INTERSTITIAL_CAP).Value;
            adsSettingsModel.resignCap = adsSettingsData.GetInt(GSBackendKeys.RESIGN_CAP).Value;
            adsSettingsModel.minutesForVictoryInternalAd = GSParser.GetSafeFloat(adsSettingsData, GSBackendKeys.MINUTES_VICTORY_AD);
            adsSettingsModel.autoSubscriptionDlgThreshold = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.AUTO_SUBSCRIPTION_THRESHOLD);
            adsSettingsModel.daysPerAutoSubscriptionDlgThreshold = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.DAYS_PER_AUTO_SUBSCRIPTION_THRESHOLD);
            adsSettingsModel.sessionsBeforePregameAd = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.SESSIONS_BEFORE_PREGAME_AD);
            adsSettingsModel.maxPregameAdsPerDay = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.MAX_PREGAME_ADS_PER_DAY);
            adsSettingsModel.waitForPregameAdLoadSeconds = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.WAIT_PREGAME_AD_LOAD_SECONDS);
            adsSettingsModel.intervalsBetweenPregameAds = GSParser.GetSafeFloat(adsSettingsData, GSBackendKeys.INTERVALS_BETWEEN_PREGAME_ADS);
            adsSettingsModel.showPregameInOneMinute = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.SHOW_PREGAME_AD_ONE_MINUTE);
        }

        private void FillRewardsSettingsModel(GSData rewardsSettingsData)
        {
            rewardsSettingsModel.facebookConnectReward = rewardsSettingsData.GetInt(GSBackendKeys.Rewards.FACEBOOK_CONNECT_REWARD).Value;
            rewardsSettingsModel.failSafeCoinReward = rewardsSettingsData.GetInt(GSBackendKeys.Rewards.FAIL_SAFE_COIN_REWARD).Value;
        }

        private void FillGameSettingsModel(GSData gsSettingsData)
        {
            settingsModel.maxLongMatchCount = gsSettingsData.GetInt(GSBackendKeys.MAX_LONG_MATCH_COUNT).Value;
            settingsModel.maxFriendsCount = gsSettingsData.GetInt(GSBackendKeys.MAX_FRIENDS_COUNT).Value;
            settingsModel.maxRecentlyCompletedMatchCount = gsSettingsData.GetInt(GSBackendKeys.MAX_RECENTLY_COMPLETED_MATCH_COUNT).Value;
            settingsModel.maxCommunityMatches = gsSettingsData.GetInt(GSBackendKeys.MAX_COMMUNITY_MATECHES).Value;

            settingsModel.maintenanceFlag = gsSettingsData.GetBoolean(GSBackendKeys.MAINTENANCE_FLAG).Value;
            settingsModel.updateMessage = gsSettingsData.GetString(GSBackendKeys.UPDATE_MESSAGE);
            settingsModel.maintenanceMessage = gsSettingsData.GetString(GSBackendKeys.MAINTENANCE_MESSAGE);
            settingsModel.maintenanceWarningFlag = gsSettingsData.GetBoolean(GSBackendKeys.MAINTENANCE_WARNING_FLAG).Value;
            settingsModel.maintenanceWarningMessege = gsSettingsData.GetString(GSBackendKeys.MAINTENANCE_WARNING_MESSEGE);
            settingsModel.maintenanceWarningBgColor = gsSettingsData.GetString(GSBackendKeys.MAINTENANCE_WARNING_BG_COLOR);

            if (gsSettingsData.ContainsKey(GSBackendKeys.PREMIUM))
            {
                var premiumData = gsSettingsData.GetGSData(GSBackendKeys.PREMIUM);
                settingsModel.maxLongMatchCountPremium = premiumData.GetInt(GSBackendKeys.MAX_LONG_MATCH_COUNT).Value;
                settingsModel.maxFriendsCountPremium = premiumData.GetInt(GSBackendKeys.MAX_FRIENDS_COUNT).Value;
            }

#if UNITY_IOS
            GSData storeData = gsSettingsData.GetGSData(GSBackendKeys.STORE_IOS);  
#elif UNITY_ANDROID
            GSData storeData = gsSettingsData.GetGSData(GSBackendKeys.STORE_ANDROID);
#endif
            settingsModel.minimumClientVersion = storeData.GetString(GSBackendKeys.MINIMUM_CLIENT_VERSION);
            settingsModel.updateReleaseBannerMessage = storeData.GetString(GSBackendKeys.UPDATE_RELEASE_BANNER_MESSAGE);
            settingsModel.manageSubscriptionURL = storeData.GetString(GSBackendKeys.MANAGE_SUBSCRIPTION_URL);
            LogUtil.Log("======= manage subscription url " + settingsModel.manageSubscriptionURL);
        }

        private void FillStoreSettingsModel(GSData storeSettingsData)
        {
            List<GSData> skinShopItemsData = storeSettingsData.GetGSDataList(GSBackendKeys.ShopItem.SKIN_SHOP_ITEMS);
            IOrderedDictionary<string, StoreItem> skinItems = PopulateStoreItems(skinShopItemsData);

            List<GSData> subscriptionItemsData = storeSettingsData.GetGSDataList(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_ITEMS);
            IOrderedDictionary<string, StoreItem> subscriptionItems = PopulateCurrencyStoreItems(subscriptionItemsData);

            List<GSData> powerUpHintShopItemsData = storeSettingsData.GetGSDataList(GSBackendKeys.ShopItem.POWERUP_HINT_SHOP_ITEMS);
            IOrderedDictionary<string, StoreItem> powerUpHintItems = PopulateStoreItems(powerUpHintShopItemsData);

            List<GSData> powerUpHindsightShopItemsData = storeSettingsData.GetGSDataList(GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_ITEMS);
            IOrderedDictionary<string, StoreItem> powerUpHindsightItems = PopulateStoreItems(powerUpHindsightShopItemsData);

            storeSettingsModel.Add(GSBackendKeys.ShopItem.SKIN_SHOP_TAG, skinItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG, subscriptionItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.POWERUP_HINT_SHOP_TAG, powerUpHintItems);
            storeSettingsModel.Add(GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_TAG, powerUpHindsightItems);
        }

        private IOrderedDictionary<string, StoreItem> PopulateStoreItems(List<GSData> itemSettingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in itemSettingsData)
            {
                var item = new StoreItem();
                GSParser.PopulateStoreItem(item, itemData);
                items.Add(item.key, item);
            }

            return items;
        }

        private IOrderedDictionary<string, StoreItem> PopulateCurrencyStoreItems(List<GSData> currencySetingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in currencySetingsData)
            {
                var item = new StoreItem();
                GSParser.PopulateStoreItem(item, itemData);

                item.currency2Payout = item.currency2Cost;
                item.currency2Cost = 0;

                items.Add(item.key, item);
            }

            return items;
        }

        private IOrderedDictionary<string, StoreItem> PopulateFeatureStoreItems(List<GSData> featureSetingsData)
        {
            IOrderedDictionary<string, StoreItem> items = new OrderedDictionary<string, StoreItem>();

            foreach (GSData itemData in featureSetingsData)
            {
                var item = new StoreItem();
                GSParser.PopulateStoreItem(item, itemData);
                items.Add(item.key, item);
            }

            return items;
        }

        private void PopulateFriends(IDictionary<string, Friend> targetList, GSData targetData, bool isBlocked = false)
        {
            targetList.Clear();

            foreach (KeyValuePair<string, object> obj in targetData.BaseData)
            {
                GSData friendData = (GSData)obj.Value;
                string friendId = obj.Key;
                Friend friend = null;

                if (!isBlocked)
                {
                    friend = LoadFriend(friendId, friendData);
                }
                else
                {
                    friend = LoadBlocked(friendId, friendData);
                }

                targetList.Add(friendId, friend);
            }
        }

        public Friend LoadFriend(string friendId, GSData friendData)
        {
            Friend friend = new Friend();
            friend.playerId = friendId;
            friend.publicProfile = new PublicProfile();
            GSParser.ParseFriend(friend, friendData, friendId);

            return friend;
        }

        public Friend LoadBlocked(string friendId, GSData friendData)
        {
            var friend = new Friend();
            friend.playerId = friendId;
            friend.gamesDrawn = GSParser.GetSafeInt(friendData, GSBackendKeys.Friend.GAMES_DRAWN);
            friend.gamesLost = GSParser.GetSafeInt(friendData, GSBackendKeys.Friend.GAMES_LOST);
            friend.gamesWon = GSParser.GetSafeInt(friendData, GSBackendKeys.Friend.GAMES_WON);
            friend.friendType = GSParser.GetSafeString(friendData, GSBackendKeys.Friend.TYPE, GSBackendKeys.Friend.TYPE_COMMUNITY);
            friend.lastMatchTimestamp = GSParser.GetSafeLong(friendData, GSBackendKeys.Friend.LAST_MATCH_TIMESTAMP);
            friend.flagMask = GSParser.GetSafeLong(friendData, GSBackendKeys.Friend.FLAG_MASK);
            friend.publicProfile = new PublicProfile();
            friend.publicProfile.name = friendData.GetString(GSBackendKeys.PublicProfile.NAME);
            return friend;
        }

        void FillChatModel(GSData chatData)
        {
            foreach (KeyValuePair<string, object> pair in chatData.BaseData)
            {
                GSData messageData = (GSData)pair.Value;

                ChatMessage msg = new ChatMessage();
                msg.senderId = messageData.GetString(GSBackendKeys.Chat.SENDER_ID);
                msg.recipientId = playerModel.id;
                msg.text = messageData.GetString(GSBackendKeys.Chat.TEXT);
                msg.timestamp = messageData.GetLong(GSBackendKeys.Chat.TIMESTAMP).Value;
                msg.guid = pair.Key;

                receiveChatMessageSignal.Dispatch(msg, true);
            }
        }
    }

    #region REQUEST

    public class GSGetInitDataRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "GetInitData";
        const string ATT_APP_VERSION = "appVersion";
        const string ATT_APP_DATA = "appData";

        public GSGetInitDataRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(int appVersion,
                                            string appData,
                                            Action<object> onSuccess)
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
