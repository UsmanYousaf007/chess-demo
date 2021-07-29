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
using System.Collections;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        bool isStoreAvailable;
        bool isInitComplete;
        public IPromise<BackendResult> GetInitData(int appVersion, string appData)
        {
            isStoreAvailable = false;
            isInitComplete = false;
            // Fetch init data from server
            return new GSGetInitDataRequest(GetRequestContext()).Send(appVersion, appData, OnGetInitDataSuccess);
        }

        void OnGetInitDataSuccess(object r, Action<object> a)
        {
            routineRunner = new NormalRoutineRunner();
            LogEventResponse response = (LogEventResponse)r;
            appInfoModel.androidURL = response.ScriptData.GetString(GSBackendKeys.APP_ANDROID_URL);
            appInfoModel.iosURL = response.ScriptData.GetString(GSBackendKeys.APP_IOS_URL);
            appInfoModel.appBackendVersionValid = response.ScriptData.GetBoolean(GSBackendKeys.APP_VERSION_VALID).Value;
            appInfoModel.contactSupportURL = response.ScriptData.GetString(GSBackendKeys.CONTACT_SUPPORT_URL);

#if UNITY_IOS
            appInfoModel.storeURL = appInfoModel.iosURL;  
#elif UNITY_ANDROID
            appInfoModel.storeURL = appInfoModel.androidURL;
#endif
            appInfoModel.isMandatoryUpdate = GSParser.GetSafeBool(response.ScriptData, GSBackendKeys.IS_MANDATORY_UPDATE);

            GSData gsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.GAME_SETTINGS);
            FillGameSettingsModel(gsSettingsData);

            // Check app version match with back end. Bail if there is mismatch.
            if (appInfoModel.appBackendVersionValid == false)
            {
                getInitDataCompleteSignal.Dispatch();
                return;
            }

            // Check if game maintenance mode is On
            if (settingsModel.maintenanceFlag == true)
            {
                getInitDataCompleteSignal.Dispatch();
                return;
            }
            
            appInfoModel.rateAppThreshold = response.ScriptData.GetInt(GSBackendKeys.APP_RATE_APP_THRESHOLD).Value;
            appInfoModel.onlineCount = Int32.Parse(response.ScriptData.GetString(GSBackendKeys.APP_ONLINE_COUNT));
            appInfoModel.nthWinsRateApp = GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.NTH_WINS_APP_RATE_APP);
            appInfoModel.gamesPlayedCount = GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.GAMES_PLAYED_TODAY);

            GSData lessonsData = response.ScriptData.GetGSData(GSBackendKeys.LESSONS_MAPPING);
            FillLessonsModel(lessonsData);

            GSData storeSettingsData = response.ScriptData.GetGSData(GSBackendKeys.SHOP_SETTINGS);
            FillStoreSettingsModel(storeSettingsData);
            storeAvailableSignal.Dispatch(false);
            //Debug.Log("ItemsPrices::FillStoreSettingsModel call completed: " + DateTime.Now);

            IPromise<bool> promise = storeService.Init(storeSettingsModel.getRemoteProductIds());
            if (promise != null)
            {

                promise.Then(OnStoreInit);
            }

            GSData adsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.ADS_SETTINGS);
            GSData adsABTestSettingsData = response.ScriptData.GetGSData(GSBackendKeys.AB_TEST_ADS_SETTINGS);

            if (Settings.ABTest.ADS_TEST_GROUP != Settings.ABTest.ADS_TEST_GROUP_DEFAULT && adsABTestSettingsData != null)
            {
                FillAdsSettingsModel(adsABTestSettingsData);
            }
            else
            {
                FillAdsSettingsModel(adsSettingsData);
            }

            GSData inventorySettingsData = response.ScriptData.GetGSData(GSBackendKeys.ShopItem.INVENTORY_SETTINGS_REWARDED_VIDEO_COST);
            FillInventorySettings(inventorySettingsData);

            GSData playerDetailsData = response.ScriptData.GetGSData(GSBackendKeys.PLAYER_DETAILS);
            FillPlayerDetails(playerDetailsData);

            if (playerModel.HasSubscription())
            {
                settingsModel.maxLongMatchCount = settingsModel.maxLongMatchCountPremium;
                settingsModel.maxFriendsCount = settingsModel.maxFriendsCountPremium;

                LogUtil.Log("======= max match count " + settingsModel.maxLongMatchCount + " friends count " + settingsModel.maxFriendsCount);
            }


            GSData chatData = response.ScriptData.GetGSData(GSBackendKeys.CHAT);
            FillChatModel(chatData);

            GSData rewardsSettingsData = response.ScriptData.GetGSData(GSBackendKeys.Rewards.REWARDS_SETTINGS);
            FillRewardsSettingsModel(rewardsSettingsData);

            GSData joinedTournamentsData = response.ScriptData.GetGSData(GSBackendKeys.JOINED_TOURNAMENTS);
            FillJoinedTournaments(joinedTournamentsData);

            List<GSData> liveTournamentsData = response.ScriptData.GetGSDataList(GSBackendKeys.LIVE_TOURNAMENTS);
            FillLiveTournaments(liveTournamentsData);

            List<GSData> downloadablesData = response.ScriptData.GetGSDataList(GSBackendKeys.DOWNLOADBLES);
            FillDownloadablesModel(downloadablesData);

            GSData leaguesData = response.ScriptData.GetGSData(GSBackendKeys.LEAGUE_SETTINGS);
            FillLeaguesModel(leaguesData);

            GSData promotionsData = response.ScriptData.GetGSData(GSBackendKeys.PROMOTION_SETTINGS);
            FillPromotionsData(promotionsData);

            tournamentsModel.lastFetchedTime = DateTime.UtcNow;

            inboxModel.inboxMessageCount = GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.INBOX_COUNT);
            GSData inBoxMessagesData = response.ScriptData.GetGSData("inbox");
            PopulateInboxModel(inBoxMessagesData);

            settingsModel.bettingIncrements = response.ScriptData.GetLongList(GSBackendKeys.BETTING_INCREMENTS);
            settingsModel.defaultBetIncrementByGamesPlayed = response.ScriptData.GetFloatList(GSBackendKeys.BET_INCREMENT_BY_GAMES_PLAYED);

            GSData freeHintSettingsData = response.ScriptData.GetGSData(GSBackendKeys.FREE_HINT_THRESHOLDS);
            ParseFreeHintSettings(freeHintSettingsData);

            GSData matchCoinsMultiplyerData = response.ScriptData.GetGSData(GSBackendKeys.MATCH_COINS_MULTIPLYER);
            FillMatchCoinsMultiplayerData(matchCoinsMultiplyerData);

            if (GSParser.GetSafeBool(response.ScriptData, GSBackendKeys.DEFAULT_ITEMS_ADDED))
            {
                SendDefaultItemsOwnedAnalytics();
            }

            if (response.ScriptData.ContainsKey(GSBackendKeys.REFUND_GEMS_ADDED))
            {
                var refundedGems = response.ScriptData.GetInt(GSBackendKeys.REFUND_GEMS_ADDED);
                if (refundedGems > 0)
                {
                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, (int)refundedGems, "refund", "old_inventory_items");
                }
            }

            updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            updateBundleSignal.Dispatch();

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
            isInitComplete = true;
            CheckCompletion();
            //routineRunner.StartCoroutine(OnInitDataComplete());
        }




        //private IEnumerator OnInitDataComplete()
        //{
        //    while (!isStoreAvailable)
        //    {
        //        yield return null;
        //    }

        //    setDefaultSkinSignal.Dispatch();
        //    getInitDataCompleteSignal.Dispatch();
        //    yield break;
        //}


        private void OnStoreInit(bool success)
        {
            //Debug.Log("ItemsPrices::OnStoreInit call time: " + DateTime.Now);
            isStoreAvailable = success;
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

            CheckCompletion();
        }

        private void CheckCompletion()
        {
            if (isInitComplete && isStoreAvailable)
            {
                setDefaultSkinSignal.Dispatch();
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
            playerModel.lastWatchedVideo = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.LAST_WATCHED_VIDEO);
            playerModel.uploadedPicId = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.UPLOADED_PIC_ID);
            playerModel.trophies = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.TROPHIES).Value;
            playerModel.trophies2 = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.TROPHIES2).Value;
            playerModel.league = playerDetailsData.GetInt(GSBackendKeys.PlayerDetails.LEAGUE).Value;
            playerModel.dynamicBundleToDisplay = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.DYNAMIC_BUNDLE_SHORT_CODE);

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.SUBSCRIPTION_EXPIRY_TIMESTAMP))
            {
                playerModel.subscriptionExipryTimeStamp = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.SUBSCRIPTION_EXPIRY_TIMESTAMP).Value;
            }

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.SUBSCRIPTION_TYPE))
            {
                playerModel.subscriptionType = playerDetailsData.GetString(GSBackendKeys.PlayerDetails.SUBSCRIPTION_TYPE);
            }

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.GEMS))
            {
                playerModel.gems = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.GEMS).Value;
            }

            playerModel.coins = GSParser.GetSafeLong(playerDetailsData, GSBackendKeys.PlayerDetails.COINS);

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.CHEST_UNLOCK_TIMESTAMP))
            {
                playerModel.chestUnlockTimestamp = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.CHEST_UNLOCK_TIMESTAMP).Value;
            }

            if (playerDetailsData.ContainsKey(GSBackendKeys.PlayerDetails.RV_UNLOCK_TIMESTAMP))
            {
                playerModel.rvUnlockTimestamp = playerDetailsData.GetLong(GSBackendKeys.PlayerDetails.RV_UNLOCK_TIMESTAMP).Value;
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

            GSData dynamicSpotPurchaseBundleData = playerDetailsData.GetGSData(GSBackendKeys.PlayerDetails.DYNAMIC_GEM_SPOT_BUNDLE);
            GSParser.ParseDynamicSpotPurchaseBundle(playerModel.dynamicGemSpotBundle, dynamicSpotPurchaseBundleData);
        }

        private void PopulateInboxModel(GSData inBoxMessagesData)
        {
            if (inBoxMessagesData != null)
            {
                Dictionary<string, InboxMessage> dict = new Dictionary<string, InboxMessage>();
                FillInbox(dict, inBoxMessagesData);
                inboxModel.lastFetchedTime = DateTime.UtcNow;
                inboxModel.items = dict;
                inboxAddMessagesSignal.Dispatch(); 
            }
            else
            {
                inboxEmptySignal.Dispatch();
            }
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
            adsSettingsModel.sessionsBeforePregameAd = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.SESSIONS_BEFORE_PREGAME_AD);
            adsSettingsModel.maxPregameAdsPerDay = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.MAX_PREGAME_ADS_PER_DAY);
            adsSettingsModel.waitForPregameAdLoadSeconds = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.WAIT_PREGAME_AD_LOAD_SECONDS);
            adsSettingsModel.intervalsBetweenPregameAds = GSParser.GetSafeFloat(adsSettingsData, GSBackendKeys.INTERVALS_BETWEEN_PREGAME_ADS);
            adsSettingsModel.showPregameInOneMinute = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.SHOW_PREGAME_AD_ONE_MINUTE);
            adsSettingsModel.autoSubscriptionDlgThreshold = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.AUTO_SUBSCRIPTION_THRESHOLD);
            adsSettingsModel.daysPerAutoSubscriptionDlgThreshold = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.DAYS_PER_AUTO_SUBSCRIPTION_THRESHOLD);
            adsSettingsModel.showPregameTournament = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.SHOW_PREGAME_AD_TOURNAMENT);
            adsSettingsModel.showInGameCPU = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.SHOW_INGAME_AD_CPU);
            adsSettingsModel.showInGame30Min = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.SHOW_INGAME_AD_30MIN);
            adsSettingsModel.showInGameClassic = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.SHOW_INGAME_AD_CLASSIC);
            adsSettingsModel.secondsBetweenIngameAds = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.MINUTES_BETWEEN_INGAME_AD) * 60;
            adsSettingsModel.secondsLeftDisableTournamentPregame = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.MINUTES_LEFT_DISABLE_TOURNAMENT_ADS) * 60;
            adsSettingsModel.secondsElapsedDisable30MinInGame = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.MINUTES_ELAPSED_DISABLE_30MIN_INGAME_ADS) * 60;
            adsSettingsModel.isBannerEnabled = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.ENABLE_BANNER_ADS, true);
            adsSettingsModel.minGemsRequiredforRV = GSParser.GetSafeInt(adsSettingsData, GSBackendKeys.MIN_GEMS_REQUIRED_FOR_RV);
            adsSettingsModel.adPlacements = adsSettingsData.GetStringList(GSBackendKeys.AD_PLACEMENTS);
            adsSettingsModel.removeInterAdsOnPurchase = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.REMOVE_INTER_ADS);
            adsSettingsModel.removeRVOnPurchase = GSParser.GetSafeBool(adsSettingsData, GSBackendKeys.REMOVE_RV);
        }

        private void FillRewardsSettingsModel(GSData rewardsSettingsData)
        {
            rewardsSettingsModel.facebookConnectReward = GSParser.GetSafeInt(rewardsSettingsData, GSBackendKeys.Rewards.FACEBOOK_CONNECT_REWARD);
            rewardsSettingsModel.failSafeCoinReward = GSParser.GetSafeInt(rewardsSettingsData, GSBackendKeys.Rewards.FAIL_SAFE_COIN_REWARD);
            rewardsSettingsModel.ratingBoostReward = GSParser.GetSafeInt(rewardsSettingsData, GSBackendKeys.Rewards.RATING_BOOST);
            rewardsSettingsModel.personalisedAdsGemReward = GSParser.GetSafeInt(rewardsSettingsData, GSBackendKeys.Rewards.PERSONALISED_ADS_GEM);
            rewardsSettingsModel.freeFullGameAnalysis = GSParser.GetSafeInt(rewardsSettingsData, GSBackendKeys.Rewards.FREE_FULL_GAME_ANALYSIS);
        }

        private void FillGameSettingsModel(GSData gsSettingsData)
        {
            settingsModel.maxLongMatchCount = gsSettingsData.GetInt(GSBackendKeys.MAX_LONG_MATCH_COUNT).Value;
            settingsModel.maxFriendsCount = gsSettingsData.GetInt(GSBackendKeys.MAX_FRIENDS_COUNT).Value;
            settingsModel.maxRecentlyCompletedMatchCount = gsSettingsData.GetInt(GSBackendKeys.MAX_RECENTLY_COMPLETED_MATCH_COUNT).Value;
            settingsModel.maxCommunityMatches = gsSettingsData.GetInt(GSBackendKeys.MAX_COMMUNITY_MATECHES).Value;
            settingsModel.hintsAllowedPerGame = GSParser.GetSafeInt(gsSettingsData, GSBackendKeys.HINTS_ALLOWED);
            settingsModel.maintenanceFlag = gsSettingsData.GetBoolean(GSBackendKeys.MAINTENANCE_FLAG).Value;
            settingsModel.appUpdateFlag = GSParser.GetSafeBool(gsSettingsData, GSBackendKeys.APP_UPDATE_FLAG);
            settingsModel.updateMessage = gsSettingsData.GetString(GSBackendKeys.UPDATE_MESSAGE);
            settingsModel.maintenanceMessage = gsSettingsData.GetString(GSBackendKeys.MAINTENANCE_MESSAGE);
            settingsModel.maintenanceWarningFlag = gsSettingsData.GetBoolean(GSBackendKeys.MAINTENANCE_WARNING_FLAG).Value;
            settingsModel.maintenanceWarningMessege = gsSettingsData.GetString(GSBackendKeys.MAINTENANCE_WARNING_MESSEGE);
            settingsModel.maintenanceWarningBgColor = gsSettingsData.GetString(GSBackendKeys.MAINTENANCE_WARNING_BG_COLOR);
            settingsModel.dailyNotificationDeadlineHour = GSParser.GetSafeInt(gsSettingsData, GSBackendKeys.DAILY_NOTIFICATION_DEADLINE_HOUR);
            settingsModel.defaultSubscriptionKey = GSParser.GetSafeString(gsSettingsData, GSBackendKeys.DEFAULT_SUBSCRIPTION_KEY, "Subscription");
            settingsModel.matchmakingRandomRange = GSParser.GetSafeInt(gsSettingsData, GSBackendKeys.MATCHMAKING_RANDOM_RANGE);
            settingsModel.powerModeFreeHints = GSParser.GetSafeInt(gsSettingsData, GSBackendKeys.POWER_MODE_FREE_HINTS);
            settingsModel.maintenanceWarningTimeStamp = GSParser.GetSafeLong(gsSettingsData, GSBackendKeys.MAINTENANCE_WARNING_TIMESTAMP);
            settingsModel.sessionDurationForGDPRinMinutes = GSParser.GetSafeInt(gsSettingsData, GSBackendKeys.SESSION_DURATION_FOR_GDPR, 15);

            settingsModel.opponentHigherEloCap = GSParser.GetSafeInt(gsSettingsData, GSBackendKeys.OPPONENT_HIGHER_ELO_CAP);

            GSData opponentLowerEloCapData = gsSettingsData.GetGSData(GSBackendKeys.OPPONENT_LOWER_ELO_CAP);

            if (opponentLowerEloCapData != null)
            {
                settingsModel.opponentLowerEloCapMin = GSParser.GetSafeInt(opponentLowerEloCapData, "min");
                settingsModel.opponentLowerEloCapMax = GSParser.GetSafeInt(opponentLowerEloCapData, "max");
            }

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
            settingsModel.isHuuugeServerValidationEnabled = GSParser.GetSafeBool(storeData, GSBackendKeys.HUUUGE_SERVER_VERIFICATION_ENABLED);
            LogUtil.Log("======= manage subscription url " + settingsModel.manageSubscriptionURL);
        }

        private void FillStoreSettingsModel(GSData storeSettingsData)
        {
            string[,] shopItemKeys =
            {
                { GSBackendKeys.ShopItem.SKIN_SHOP_ITEMS, GSBackendKeys.ShopItem.SKIN_SHOP_TAG },
                { GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_ITEMS,GSBackendKeys.ShopItem.SUBSCRIPTION_TAG },
                { GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_ITEMS, GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_TAG },
                { GSBackendKeys.ShopItem.GEMPACK_SHOP_ITEMS, GSBackendKeys.ShopItem.GEMPACK_SHOP_TAG },
                { GSBackendKeys.ShopItem.SPECIALPACK_SHOP_ITEMS, GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG },
                { GSBackendKeys.ShopItem.SPECIALITEM_SHOP_ITEMS, GSBackendKeys.ShopItem.SPECIALITEM_SHOP_TAG },
                { GSBackendKeys.ShopItem.SPECIAL_BUNDLE_SHOP_ITEMS, GSBackendKeys.ShopItem.SPECIAL_BUNDLE_SHOP_TAG },
                { GSBackendKeys.ShopItem.SPECIALITEM_POINTS_ITEMS, GSBackendKeys.ShopItem.SPECIALITEM_POINTS_TAG},
                { GSBackendKeys.ShopItem.COINS_SHOP_ITEMS, GSBackendKeys.ShopItem.COINS_SHOP_TAG}
            };

            int len = shopItemKeys.Length >> 1;
            for (int i = 0; i < len; i++)
            {
                List<GSData> itemsData = storeSettingsData.GetGSDataList(shopItemKeys[i, 0]);
                if (itemsData != null)
                {
                    IOrderedDictionary<string, StoreItem> items = PopulateStoreItems(itemsData);
                    storeSettingsModel.Add(shopItemKeys[i, 1], items);
                }
            }

            // Set URL for video lesson items
            string videoBaseUrl = storeSettingsData.GetString(GSBackendKeys.ShopItem.VIDEO_LESSONS_BASE_URL);
            List<StoreItem> videoLessons = storeSettingsModel.lists[GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_TAG];
            for (int i = 0; i < videoLessons.Count; i++)
            {
                videoLessons[i].videoUrl = videoBaseUrl + videoLessons[i].key + ".mp4";
            }
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

            var leagueAssets = tournamentsModel.GetLeagueSprites(friend.publicProfile.league.ToString());
            friend.publicProfile.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;

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

        private void FillLessonsModel(GSData lessonsData)
        {
            if (lessonsData != null)
            {
                foreach (var entry in lessonsData.BaseData)
                {
                    var topicDictionary = new OrderedDictionary<string, List<string>>();
                    var topics = entry.Value as GSData;

                    foreach (var topic in topics.BaseData)
                    {
                        var lessonsList = topic.Value as List<object>;

                        foreach (var lesson in lessonsList)
                        {
                            lessonsModel.lessonsMapping.Add(lesson.ToString(), topic.Key);
                        }

                        lessonsModel.topicsMapping.Add(topic.Key, entry.Key);
                    }
                }
            }
        }

        private void FillInventorySettings(GSData inventorySettingsData)
        {
            if (inventorySettingsData != null)
            {
                string[] inventoryItemKeys =
                {
                    GSBackendKeys.ShopItem.SPECIAL_ITEM_GEMS_BOOSTER,
                    GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER,
                    GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY,
                    GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT,
                    GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET
                };

                foreach (var key in inventoryItemKeys)
                {
                    settingsModel.inventorySpecialItemsRewardedVideoCost.Add(key, GSParser.GetSafeInt(inventorySettingsData, key));
                }
            }
        }

        private void FillJoinedTournaments(GSData joinedTournaments)
        {
            if (joinedTournaments != null)
            {
                List<JoinedTournamentData> joinedTournamentsList = new List<JoinedTournamentData>();

                foreach (KeyValuePair<string, object> pair in joinedTournaments.BaseData)
                {
                    var joinedTournament = tournamentsModel.GetJoinedTournament(pair.Key);

                    var tournamentGSData = pair.Value as GSData;
                    JoinedTournamentData newJoinedTournament = ParseJoinedTournament(tournamentGSData, pair.Key, joinedTournament);

                    joinedTournamentsList.Add(newJoinedTournament);
                }

                joinedTournamentsList.Sort((x, y) => x.endTimeUTCSeconds.CompareTo(y.endTimeUTCSeconds));

                tournamentsModel.joinedTournaments = joinedTournamentsList;
            }
        }

        private JoinedTournamentData ParseJoinedTournament(GSData tournamentGSData, string id, JoinedTournamentData savedJoinedTournament = null)
        {
            JoinedTournamentData joinedTournament = savedJoinedTournament == null ? new JoinedTournamentData() : savedJoinedTournament;
            joinedTournament.id = id;
            joinedTournament.shortCode = GSParser.GetSafeString(tournamentGSData, GSBackendKeys.Tournament.SHORT_CODE);
            joinedTournament.type = GSParser.GetSafeString(tournamentGSData, GSBackendKeys.Tournament.TYPE);
            joinedTournament.name = GSParser.GetSafeString(tournamentGSData, GSBackendKeys.Tournament.NAME);
            joinedTournament.rank = GSParser.GetSafeInt(tournamentGSData, GSBackendKeys.Tournament.RANK);
            joinedTournament.ended = GSParser.GetSafeBool(tournamentGSData, GSBackendKeys.Tournament.CONCLUDED);
            joinedTournament.matchesPlayedCount = GSParser.GetSafeInt(tournamentGSData, GSBackendKeys.Tournament.MATCHES_PLAYED_COUNT);
            joinedTournament.score = GSParser.GetSafeInt(tournamentGSData, GSBackendKeys.Tournament.SCORE);

            var grandPrizeGSData = tournamentGSData.GetGSData(GSBackendKeys.Tournament.GRAND_PRIZE);
            if (grandPrizeGSData != null)
            {
                TournamentReward grandPrize = ParseTournamentReward(grandPrizeGSData);

                joinedTournament.grandPrize = grandPrize;
            }

            var rewards = tournamentGSData.GetGSData(GSBackendKeys.Tournament.REWARDS);
            if (rewards != null)
            {
                var rewardsListForLeague = rewards.GetGSDataList(playerModel.league.ToString());
                for (int i = 0; i < rewardsListForLeague.Count; i++)
                {
                    TournamentReward reward = ParseTournamentReward(rewardsListForLeague[i]);
                    for (int j = reward.minRank; j <= reward.maxRank; j++)
                    {
                        if (joinedTournament.rewardsDict.ContainsKey(j))
                        {
                            joinedTournament.rewardsDict[j] = reward;
                        }
                        else
                        {
                            joinedTournament.rewardsDict.Add(j, reward);
                        }
                    }
                }

                if (joinedTournament.grandPrize == null)
                {
                    joinedTournament.grandPrize = joinedTournament.rewardsDict[1];
                }
            }

            var entries = tournamentGSData.GetGSDataList(GSBackendKeys.Tournament.ENTRIES);
            if (entries != null)
            {
                List<TournamentEntry> tournamentEntries = ParseTournamentEntries(entries);

                // Sort entries on score here, and Update rank
                SortTournamentEntries(tournamentEntries);

                joinedTournament.entries = tournamentEntries;

                string playerId = playerModel.id;

                // Rank and other entry updates
                for (int i = 0; i < tournamentEntries.Count; i++)
                {
                    tournamentEntries[i].rank = i + 1;

                    if (playerId == tournamentEntries[i].publicProfile.playerId)
                    {
                        joinedTournament.rank = tournamentEntries[i].rank;
                        joinedTournament.score = tournamentEntries[i].score;
                    }

                    if (tournamentEntries[i].publicProfile.leagueBorder == null)
                    {
                        var leagueAssets = tournamentsModel.GetLeagueSprites(tournamentEntries[i].publicProfile.league.ToString());
                        tournamentEntries[i].publicProfile.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;
                    }
                }
            }

            joinedTournament.startTimeUTC = GSParser.GetSafeLong(tournamentGSData, GSBackendKeys.Tournament.START_TIME);
            joinedTournament.durationMinutes = GSParser.GetSafeInt(tournamentGSData, GSBackendKeys.Tournament.DURATION);

            long concludeTimeUTC = joinedTournament.startTimeUTC + (joinedTournament.durationMinutes * 60 * 1000);
            joinedTournament.concludeTimeUTCSeconds = concludeTimeUTC / 1000;
            joinedTournament.endTimeUTCSeconds = joinedTournament.concludeTimeUTCSeconds;

            return joinedTournament;
        }

        private void SortTournamentEntries(List<TournamentEntry> entries)
        {
            TournamentEntry playerEntry = null;

            // Remove player entry
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].publicProfile.playerId == playerModel.id)
                {
                    playerEntry = entries[i];
                    entries.RemoveAt(i);

                    break;
                }
            }

            // Sort entries on score here
            entries.Sort((x, y) =>
                y.score.CompareTo(x.score));

            // Add back player entry
            bool inserted = false;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].score <= playerEntry.score)
                {
                    entries.Insert(i, playerEntry);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
            {
                entries.Add(playerEntry);
            }
        }

        private List<TournamentEntry> ParseTournamentEntries(List<GSData> entriesGSData)
        {
            List<TournamentEntry> tournamentEntries = new List<TournamentEntry>();
            for (int i = 0; i < entriesGSData.Count; i++)
            {
                GSData entryGSData = entriesGSData[i];

                TournamentEntry tournamentEntry = new TournamentEntry();
                tournamentEntry.rank = GSParser.GetSafeInt(entryGSData, GSBackendKeys.Tournament.RANK);
                tournamentEntry.score = GSParser.GetSafeInt(entryGSData, GSBackendKeys.Tournament.SCORE);
                tournamentEntry.matchesPlayedCount = GSParser.GetSafeInt(entryGSData, GSBackendKeys.Tournament.MATCHES_PLAYED_COUNT);

                tournamentEntry.score += tournamentEntry.randomScoreFactor;

                tournamentEntry.publicProfile = new PublicProfile();
                GSData publicProfileData = entryGSData.GetGSData(GSBackendKeys.Friend.PUBLIC_PROFILE);
                string entryId = GSParser.GetSafeString(entryGSData, GSBackendKeys.PlayerDetails.PLAYER_ID);
                GSParser.PopulatePublicProfile(tournamentEntry.publicProfile, publicProfileData, entryId);

                tournamentEntries.Add(tournamentEntry);
            }

            return tournamentEntries;
        }

        private void FillLiveTournaments(List<GSData> liveTournaments)
        {
            if (liveTournaments != null)
            {
                List<LiveTournamentData> openTournamentsList = new List<LiveTournamentData>();
                List<LiveTournamentData> upcomingTournamentsList = new List<LiveTournamentData>();

                for (int i = 0; i < liveTournaments.Count; i++)
                {
                    var tournamentGSData = liveTournaments[i].BaseData[GSBackendKeys.Tournament.TOURNAMENT_KEY] as GSData;
                    string shortCode = GSParser.GetSafeString(tournamentGSData, GSBackendKeys.Tournament.SHORT_CODE);

                    LiveTournamentData openTournament = null;
                    LiveTournamentData upcomingTournament = null;
                    LiveTournamentData liveTournamentData = null;
                    if (shortCode != null)
                    {
                        openTournament = tournamentsModel.GetOpenTournament(shortCode);
                        upcomingTournament = tournamentsModel.GetOpenTournament(shortCode);

                        liveTournamentData = openTournament != null ? openTournament : upcomingTournament;
                    }

                    LiveTournamentData liveTournament = ParseLiveTournament(tournamentGSData, liveTournamentData);

                    if (tournamentsModel.isTournamentOpen(liveTournament))
                    {
                        openTournamentsList.Add(liveTournament);
                    }
                    else
                    {
                        upcomingTournamentsList.Add(liveTournament);
                    }
                }

                openTournamentsList.Sort((x, y) => x.endTimeUTCSeconds.CompareTo(y.endTimeUTCSeconds));
                upcomingTournamentsList.Sort((x, y) => x.endTimeUTCSeconds.CompareTo(y.endTimeUTCSeconds));

                tournamentsModel.openTournaments = openTournamentsList;
                tournamentsModel.upcomingTournaments = upcomingTournamentsList;
            }
        }

        private LiveTournamentData ParseLiveTournament(GSData liveTournamentGSData, LiveTournamentData savedLiveTournamentData = null)
        {
            LiveTournamentData liveTournament = savedLiveTournamentData == null ? new LiveTournamentData() : savedLiveTournamentData;

            liveTournament.shortCode = GSParser.GetSafeString(liveTournamentGSData, GSBackendKeys.Tournament.SHORT_CODE);
            liveTournament.name = GSParser.GetSafeString(liveTournamentGSData, GSBackendKeys.Tournament.NAME);
            liveTournament.type = GSParser.GetSafeString(liveTournamentGSData, GSBackendKeys.Tournament.TYPE);
            liveTournament.active = GSParser.GetSafeBool(liveTournamentGSData, GSBackendKeys.Tournament.ACTIVE);
            liveTournament.firstStartTimeUTC = GSParser.GetSafeLong(liveTournamentGSData, GSBackendKeys.Tournament.START_TIME);
            liveTournament.durationMinutes = GSParser.GetSafeInt(liveTournamentGSData, GSBackendKeys.Tournament.DURATION);
            liveTournament.waitTimeMinutes = GSParser.GetSafeInt(liveTournamentGSData, GSBackendKeys.Tournament.WAIT_TIME);

            var grandPrizeGSData = liveTournamentGSData.GetGSData(GSBackendKeys.Tournament.GRAND_PRIZE);
            if (grandPrizeGSData != null)
            {
                TournamentReward grandPrize = ParseTournamentReward(grandPrizeGSData);

                liveTournament.grandPrize = grandPrize;
            }

            var rewards = liveTournamentGSData.GetGSData(GSBackendKeys.Tournament.REWARDS);
            if (rewards != null)
            {
                var rewardsListForLeague = rewards.GetGSDataList(playerModel.league.ToString());
                for (int i = 0; i < rewardsListForLeague.Count; i++)
                {
                    TournamentReward reward = ParseTournamentReward(rewardsListForLeague[i]);
                    for (int j = reward.minRank; j <= reward.maxRank; j++)
                    {
                        if (liveTournament.rewardsDict.ContainsKey(j))
                        {
                            liveTournament.rewardsDict[j] = reward;
                        }
                        else
                        {
                            liveTournament.rewardsDict.Add(j, reward);
                        }
                    }
                }
            }

            long waitTimeSeconds = liveTournament.waitTimeMinutes * 60;
            long durationSeconds = liveTournament.durationMinutes * 60;
            long firstStartTimeSeconds = liveTournament.firstStartTimeUTC / 1000;
            liveTournament.currentStartTimeUTCSeconds = tournamentsModel.CalculateCurrentStartTime(waitTimeSeconds, durationSeconds, firstStartTimeSeconds);

            long currentTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (currentTimeUTCSeconds < liveTournament.currentStartTimeUTCSeconds)
            {
                liveTournament.concludeTimeUTCSeconds = liveTournament.currentStartTimeUTCSeconds;
                liveTournament.endTimeUTCSeconds = liveTournament.concludeTimeUTCSeconds;
                liveTournament.concluded = false;
            }
            else
            {
                liveTournament.endTimeUTCSeconds = liveTournament.currentStartTimeUTCSeconds + durationSeconds;
                liveTournament.concludeTimeUTCSeconds = liveTournament.endTimeUTCSeconds - (TournamentConstants.BUFFER_TIME_MINS * 60);
                liveTournament.concluded = currentTimeUTCSeconds > liveTournament.concludeTimeUTCSeconds;
            }

            liveTournament.joined = false;

            return liveTournament;
        }

        private TournamentReward ParseTournamentReward(GSData rewardGSData)
        {
            if (rewardGSData != null) {
                TournamentReward reward = new TournamentReward();
                reward.trophies = GSParser.GetSafeInt(rewardGSData, GSBackendKeys.TournamentReward.TROPHIES);
                reward.chestType = GSParser.GetSafeString(rewardGSData, GSBackendKeys.TournamentReward.CHEST_TYPE);
                reward.gems = GSParser.GetSafeInt(rewardGSData, GSBackendKeys.TournamentReward.GEMS);
                reward.hints = GSParser.GetSafeInt(rewardGSData, GSBackendKeys.TournamentReward.HINTS);
                reward.ratingBoosters = GSParser.GetSafeInt(rewardGSData, GSBackendKeys.TournamentReward.RATING_BOOSTERS);
                reward.minRank = GSParser.GetSafeInt(rewardGSData, GSBackendKeys.TournamentReward.MIN_RANK);
                reward.maxRank = GSParser.GetSafeInt(rewardGSData, GSBackendKeys.TournamentReward.MAX_RANK);

                return reward;
            }

            return null;
        }


        private void ParseFreeHintSettings(GSData freeHintData)
        {
            settingsModel.advantageThreshold = GSParser.GetSafeInt(freeHintData, GSBackendKeys.ADV_THRESHOLDS);
            settingsModel.purchasedHintsThreshold = GSParser.GetSafeInt(freeHintData, GSBackendKeys.HINTS_PURCHASED_THRESHOLDS);
        }


        private void FillInbox(IDictionary<string, InboxMessage> targetList, GSData targetData)
        {
            targetList.Clear();

            GSData messages = targetData.GetGSData("messages");

            foreach (KeyValuePair<string, object> obj in messages.BaseData)
            {
                GSData data = (GSData)obj.Value;
                string id = obj.Key;
                InboxMessage msg = new InboxMessage();

                GSParser.ParseInboxMessage(msg, data);
                GSParser.LogInboxMessage(msg);

                targetList.Add(id, msg);
            }

        }

        private void FillLeaguesModel(GSData leaguesData)
        {
            leaguesModel.leagues.Clear();

            foreach (KeyValuePair<string, object> obj in leaguesData.BaseData)
            {
                GSData data = (GSData)obj.Value;
                string id = obj.Key;
                League league = new League();
                league.dailyReward = new Dictionary<string, int>();

                GSParser.ParseLeague(league, data);
                GSParser.LogLeague(league);

                leaguesModel.leagues.Add(id, league);
            }
        }

        private void FillPromotionsData(GSData promotionsData)
        {
            if (promotionsData != null)
            {
#if UNITY_IOS
                var promotionsPlatformData = promotionsData.GetGSData(GSBackendKeys.STORE_IOS);  
#elif UNITY_ANDROID
                var promotionsPlatformData = promotionsData.GetGSData(GSBackendKeys.STORE_ANDROID);
#endif
                foreach (var entry in promotionsPlatformData.BaseData)
                {
                    var sequence = entry.Value as List<object>;
                    var sequenceString = new List<string>();

                    foreach (var item in sequence)
                    {
                        sequenceString.Add(item.ToString());
                    }

                    promotionsService.promotionsSequence.Add(sequenceString);
                }
            }
        }

        private void FillDownloadablesModel(List<GSData> downloadablesData)
        {
            if (downloadablesData != null)
            {
                downloadablesModel.downloadableItems = new Dictionary<string, DownloadableItem>();
                foreach (var downloadable in downloadablesData)
                {
                    string downloadShortCode = downloadable.GetString(GSBackendKeys.DOWNLOADABLE_SHORT_CODE);
                    if (PlatformUtil.IsCurrentPlatformSuffixAppended(downloadShortCode))
                    {
                        DownloadableItem item = new DownloadableItem();
                        item.size = downloadable.GetInt(GSBackendKeys.DOWNALOADABLE_SIZE).Value;
                        item.downloadShortCode = downloadShortCode;
                        item.shortCode = downloadShortCode.RemovePlatfrom();
                        item.lastModified = downloadable.GetLong(GSBackendKeys.DOWNLOADABLE_LAST_MODIFIED).Value;
                        item.url = downloadable.GetString(GSBackendKeys.DOWNLOADABLE_URL);
                        item.bundle = downloadablesModel.GetBundleFromVersionCache(item.shortCode);
                        downloadablesModel.downloadableItems.Add(item.shortCode, item);
                    }
                }
            }
        }

        private void FillMatchCoinsMultiplayerData(GSData matchCoinsMultiplyerData)
        {
            if (matchCoinsMultiplyerData == null)
            {
                return;
            }

            if (settingsModel.matchCoinsMultiplayer == null)
            {
                settingsModel.matchCoinsMultiplayer = new Dictionary<string, float>();
            }

            UpdateMatchCoinsMultiplyerDictionary("coins_A", matchCoinsMultiplyerData, 2.0f);
            UpdateMatchCoinsMultiplyerDictionary("coins_B", matchCoinsMultiplyerData, 1.5f);
        }

        private void UpdateMatchCoinsMultiplyerDictionary(string key, GSData matchCoinsMultiplyerData, float defaultValue = 2.0f)
        {
            var value = GSParser.GetSafeFloat(matchCoinsMultiplyerData, key, defaultValue);

            if (settingsModel.matchCoinsMultiplayer.ContainsKey(key))
            {
                settingsModel.matchCoinsMultiplayer[key] = value;
            }
            else
            {
                settingsModel.matchCoinsMultiplayer.Add(key, value);
            }
        }

        private void SendDefaultItemsOwnedAnalytics()
        {
            string[] defaultItems = { GSBackendKeys.ShopItem.DEFAULT_ITEMS_V1, GSBackendKeys.ShopItem.DEFAULT_ITEMS_V2 };

            foreach (var defaultItem in defaultItems)
            {
                if (storeSettingsModel.items.ContainsKey(defaultItem))
                {
                    var storeItem = storeSettingsModel.items[defaultItem];

                    if (storeItem.bundledItems != null)
                    {
                        foreach (var item in storeItem.bundledItems)
                        {
                            var context = CollectionsUtil.GetContextFromString(item.Key);

                            if (context != AnalyticsContext.unknown)
                            {
                                analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, context.ToString(), item.Value, "new_player", "default");
                            }
                        }

                        if (storeItem.currency3Cost > 0)
                        {
                            analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, "gems", storeItem.currency3Cost, "new_player", "default");
                        }
                    }
                }
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
                                            Action<object, Action<object>> onSuccess)
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
