/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

/// @description
/// These are the server backend keys. These keys remain the same on the server
/// and the client side.

using System.Collections.Generic;

using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    // TODO: Organize framework/game keys in a better way
    // TODO: Consider using customer gamesparks SDK for requests

    public static partial class GSBackendKeys
    {
        // App Version Settings
        public const string APP_VERSION_VALID = "appVersionValid";
        public const string APP_ANDROID_URL = "androidURL";
        public const string APP_IOS_URL = "iosURL";
        public const string APP_RATE_APP_THRESHOLD = "rateAppThreshold";
        public const string APP_ONLINE_COUNT = "onlineCount";
        public const string MAINTENANCE_FLAG = "maintenanceFlag";
        public const string UPDATE_MESSAGE = "updateMessage";
        public const string MAINTENANCE_MESSAGE = "maintenanceMessage";
        public const string MINIMUM_CLIENT_VERSION = "minimumClientVersion";
        public const string MAINTENANCE_WARNING_FLAG = "maintenanceWarningFlag";
        public const string MAINTENANCE_WARNING_MESSEGE = "maintenanceWarningMessage";
        public const string MAINTENANCE_WARNING_BG_COLOR = "maintenanceWarningBgColor";
        public const string UPDATE_RELEASE_BANNER_MESSAGE = "gameUpdateBannerMsg";
        public const string MANAGE_SUBSCRIPTION_URL = "manageSubscriptionURL";
        public const string EVENT_DATA          = "eventData";
        public const string EVENT_DAY_NUMBER    = "eventdayNo";
        public const string POWERUP_USAGE_PERCENTAGE = "powerupUsagePercentage";
        public const string STORE_IOS = "ios";
        public const string STORE_ANDROID = "android";
        public const string CONTACT_SUPPORT_URL = "contactSupportURL";

        //game Settings
        public const string GAME_SETTINGS = "gameSettings";
        public const string MAX_LONG_MATCH_COUNT = "maxLongMatchCount";
        public const string MAX_FRIENDS_COUNT = "maxFriendsCount";
        public const string FACEBOOK_CONNECT_REWARD = "facebookConnectReward";
        public const string MAX_RECENTLY_COMPLETED_MATCH_COUNT = "maxRecentlyCompletedMatchCount";
        public const string MAX_COMMUNITY_MATECHES = "maxCommunityMatches";
        public const string PREMIUM = "premium";
        
        // Shop settings
        public const string SHOP_SETTINGS = "shopSettings";
        public const string SHOP_ITEM_TAGS = "tags";
        public const string SHOP_ITEM_ID = "shortCode";
        public const string SHOP_ITEM_TYPE = "type";
        public const string SHOP_ITEM_DISPLAYNAME = "name";
        public const string SHOP_ITEM_DESCRIPTION = "description";
        public const string SHOP_ITEM_CURRENCY1COST = "currency1Cost";
        public const string SHOP_ITEM_CURRENCY2COST = "currency2Cost";
        public const string SHOP_ITEM_MAX_QUANTITY = "maxQuantity";
        public const string SHOP_ITEM_UNLOCKATLEVEL = "unlockAtLevel";
        public const string SHOP_ITEM_CURRENCY1PAYOUT = "currency1Cost";
		public const string SHOP_ITEM_STORE_PRODUCT_ID = "googlePlayProductId";
        public const string SHOP_ITEM_IOS_STORE_PRODUCT_ID = "iosAppStoreProductId";
        public const string SHOP_ITEM_STORE_BUNDLED_GOODS = "bundledGoods";
        public const string SHOP_ITEM_QUANTITY = "qty";
        public const string SHOP_ITEM_DISABLED = "disabled";
        public const string SHOP_ITEM_PROPERTY_SET = "propertySet";
        public const string SHOP_ITEM_SKIN_PROPERTY = "SkinProperty";
        public const string SHOP_ITEM_SKIN_INDEX = "skinIndex";
        public const string SHOP_ITEM_SKIN_POINTS = "pointsRequired";

        // Account details
        public const string PLAYER_DETAILS = "playerDetails";
        public const string ACCOUNT_VIRTUALGOODS = "virtualGoods";
        public const string PLAYER_ACTIVE_INVENTORY = "playerActiveInventory";
		public const string FRIENDS = "friends";
		public const string BLOCKED = "blocked";
        public const string UPDATED_STATS = "updatedStats";
        public const string FRIEND = "friend";
        public const string OPPONENT_ID = "opponentId";
        public const string OPPONENT_ELO = "opponentElo";
        public const string CHAT = "chat";

        // Ad Settings
        public const string ADS_SETTINGS = "adsSettings";
        public const string ADS_MAX_IMPRESSIONS_PER_SLOT = "ADS_MAX_IMPRESSIONS_PER_SLOT";
        public const string ADS_SLOT_HOUR = "ADS_SLOT_HOUR";
        public const string ADS_REWARD_INCREMENT = "ADS_REWARD_INCREMENT";
        public const string ADS_FREE_NO_ADS_PERIOD = "ADS_FREE_NO_ADS_PERIOD";
        public const string ADS_GLOBAL_CAP = "adsGlobalCap";
        public const string ADS_REWARDED_VIDEO_CAP = "adsRewardedVideoCap";
        public const string ADS_INTERSTITIAL_CAP = "adsInterstitialCap";
        public const string RESIGN_CAP = "resignCap";
        public const string MINUTES_VICTORY_AD = "minutesForVictoryInteralAd";
        public const string AUTO_SUBSCRIPTION_THRESHOLD = "autoSubscriptionDlgThreshold";
        public const string DAYS_PER_AUTO_SUBSCRIPTION_THRESHOLD = "daysPerAutoSubscriptionDlgThreshold";
        public const string SESSIONS_BEFORE_PREGAME_AD = "sessionsBeforePregameAd";
        public const string MAX_PREGAME_ADS_PER_DAY = "maxPregameAdsPerDay";
        public const string WAIT_PREGAME_AD_LOAD_SECONDS = "waitForPregameAdLoadSeconds";
        public const string INTERVALS_BETWEEN_PREGAME_ADS = "intervalsBetweenPregameAds";
        public const string SHOW_PREGAME_AD_ONE_MINUTE = "showPregameOneMinute";

        // Coin pack purchase
        public const string COINSPACK_COINS1_BOUGHT = "coins1Bought";

        // Player data keys
        public const string IS_SOCIAL_NAME_SET = "isSocialNameSet";
        public const string DISPLAY_NAME = "displayName";
        public const string TAG = "tag";
        public const string ELO_SCORE = "eloScore";
        public const string AD_LIFETIME_IMPRESSIONS = "adLifetimeImpressions";
        public const string GAMES_WON = "gamesWon";
        public const string GAMES_LOST = "gamesLost";
        public const string GAMES_DRAWN = "gamesDrawn";
        public const string GAMES_ABANDONED = "gamesAbandoned";
        public const string GAMES_PLAYED = "gamesPlayed";
        public const string AVATAR_ID = "avatarId";

        //Altas names
        public const string DEFAULT_AVATAR_ALTAS_NAME = "DefaultAvatar";


        // PING KEYS
        public const string CLIENT_SEND_TIMESTAMP = "clientSendTimestamp";
        public const string SERVER_RECEIPT_TIMESTAMP = "serverReceiptTimestamp";

        // Message ExtCodes
        public const string START_GAME_MESSAGE = "StartGameMessage";
        public const string NEW_FRIEND_MESSAGE = "NewFriendMessage";
        public const string NEW_COMMUNITY_FRIEND_MESSAGE = "NewCommnunityFriendMessage";
        public const string ONLINE_STATUS_FRIEND_MESSAGE = "OnlineStatusFriendMessage";
        public const string MATCH_WATCHDOG_PING_MESSAGE = "MatchWatchdogPingMessage";
        public const string MATCH_WATCHDOG_OPPONENT_PINGED_MESSAGE = "MatchWatchdogOpponentPingedMessage";
        public const string MATCH_WATCHDOG_OPPONENT_ACKNOWLEDGED_MESSAGE = "MatchWatchdogOpponentAcknowlegedMessage";
        public const string AI_TAKE_TURN_FAILED_MESSAGE = "AiTakeTurnFailedMessage";

        // Feature Shop Items
        public const string SHOP_ITEM_FEATURE_REMOVE_ADS_PERM = "FeatureRemoveAdsPerm";

        // Mesage data keys
        public const string GAME_START_TIME = "gameStartTime";
        public const string CHALLENGE_ID = "challengeId";

        // Other (TODO: Organize these)
        public const string CHALLENGER_ID = "challengerId";
        public const string CHALLENGED_ID = "challengedId";
        public const string IS_FRIEND_REMOVED = "isFriendRemoved";
        public const string UNREGISTER_STATUS = "status";

        // External auth types
        public static class Auth
        {
            private const string FACEBOOK = "FB";
            private const string GOOGLE_PLAY = "GP";

            private static readonly IDictionary<string, ExternalAuthType> externalIdMap =
                new Dictionary<string, ExternalAuthType>() {
                { FACEBOOK, ExternalAuthType.FACEBOOK },
                { GOOGLE_PLAY, ExternalAuthType.GOOGLE_PLAY }
            };

            public static IDictionary<ExternalAuthType, ExternalAuth> GetExternalAuthentications(GSData externalIds)
            {
                IDictionary<ExternalAuthType, ExternalAuth> externalAuthentications = new Dictionary<ExternalAuthType, ExternalAuth>();
                IDictionary<string, object> externalIdsBaseData = externalIds.BaseData;

                foreach (KeyValuePair<string, object> e in externalIdsBaseData)
                {
                    ExternalAuthType type = externalIdMap[e.Key];
                    ExternalAuth data;
                    data.id = (string)e.Value;

                    externalAuthentications.Add(type, data);
                }

                return externalAuthentications;
            }
        }

        // Match data
        public static class ChallengeData
        {
            public const string CHALLENGE_DATA_KEY = "ChallengeData";
            public const string CHALLENGE_ID = "challengeId";
            public const string MATCH_DATA_KEY = "matchData";
            public const string PROFILE = "profile";
            public const string PROFILE_NAME = "name";
            public const string PROFILE_COUNTRY_ID = "countryId";
            public const string PROFILE_ELO_SCORE = "eloScore";
            public const string PROFILE_EXTERNAL_IDS = "externalIds";
            public const string BOT_ID = "botId";
            public const string GAME_START_TIME = "gameStartTime";
            public const string GAME_END_TIME = "gameEndTime";
            public const string OPPONENT_ELO_SCORE = "opponentEloScore";
            public const string BOT_DIFFICULTY = "botDifficulty";
        }

        // Resume
        public static class ResumeData
        {
            public const string KEY = "resumeData";
            public const string MATCH_DATA = "matchData";
            public const string CHALLENGE_ID = "challengeId";
            public const string CHALLENGER_ID = "challengerId";
            public const string CHALLENGED_ID = "challengedId";
        }

        public static class AdInfo
        {
            public const string KEY = "adInfo";
            public const string LEVEL = "level";
            public const string REWARD = "reward";
            public const string REWARD_CURRENCY_2 = "bucks";
        }

        public static class ShopItem
        {
            public const string SHOP_SETTINGS = "shopSettings";
            public const string SKIN_SHOP_ITEMS = "skinShopItems";
            public const string COINS_SHOP_ITEMS = "coinsShopItems";
            public const string FEATURE_SHOP_ITEMS = "featureShopItems";
            public const string SAFE_MOVE_SHOP_ITEMS = "safeMoveShopItems";
            public const string HINT_SHOP_ITEMS = "hintShopItems";
            public const string HINDSIGHT_SHOP_ITEMS = "hindsightShopItems";
            public const string SPECIAL_BUNDLE_SHOP_ITEMS = "specialBundleShopItems";
            public const string POWERUP_HINT_SHOP_ITEMS = "powerUpHintShopItems";
            public const string POWERUP_HINDSIGHT_SHOP_ITEMS = "powerUpHindsightShopItems";
            public const string POWERUP_SAFEMOVE_SHOP_ITEMS = "powerUpSafeMoveShopItems";
            public const string SUBSCRIPTION_SHOP_ITEMS = "subscriptionShopItems";

            public const string SKIN_SHOP_TAG = "Skin";
            public const string COINS_SHOP_TAG = "CoinPack";
            public const string FEATURE_SHOP_TAG = "Feature";
            public const string SAFE_MOVE_SHOP_TAG = "SafeMovePack";
            public const string HINT_SHOP_TAG = "HintPack";
            public const string HINDSIGHT_SHOP_TAG = "HindsightPack";
            public const string SPECIAL_BUNDLE_SHOP_TAG = "SpecialBundle";
            public const string AVATAR_TAG = "Avatar";
            public const string AVATAR_BG_COLOR_TAG = "AvatarBgColor";
            public const string SUBSCRIPTION_TAG = "subscription";

            public const string POWERUP_HINT_SHOP_TAG = "PowerUpHint";
            public const string POWERUP_HINDSIGHT_SHOP_TAG = "PowerUpHindsight";
            public const string POWERUP_SAFEMOVE_SHOP_TAG = "PowerUpSafeMove";

            public const string FEATURE_REMOVEAD_PERM_SHOP_TAG = "FeatureRemoveAdsPerm";
            public const string FEATURE_REMOVEAD_30_SHOP_TAG = "FeatureRemoveAds30";

            public const string SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG = "SpecialBundleUltimate";
            public const string SPECIAL_BUNDLE_STANDARD_SHOP_TAG = "SpecialBundleStandard";
            public const string SPECIAL_BUNDLE_NOADSFOREVER_SHOP_TAG = "SpecialBundleNoAdsForever";

            public const string SUBSCRIPTION_SHOP_TAG = "Subscription";
            public const string SUBSCRIPTION_ANNUAL_SHOP_TAG = "SubscriptionAnnual";

            public const string SHOP_ITEM_TIER_COMMON = "Common";
            public const string SHOP_ITEM_TIER_RARE = "Rare";
            public const string SHOP_ITEM_TIER_EPIC = "Epic";
            public const string SHOP_ITEM_TIER_LEGENDARY = "Legendary";

            public const string SHOP_ITEM_TYPE_CURRENCY = "CURRENCY";
            public const string SHOP_ITEM_TYPE_VGOOD = "VGOOD";

            public static string GetOfferItemKey(string text)
            {
                switch (text)
                {
                    case "No Ads": return "NoAds";
                    case "Unlimited Learning Tools Usage": return "LearningTools";
                    case "All Themes Unlocked": return "UnlockAllThemes";
                    case "Premium Display Picture Frame": return "PremiumBorder";
                    case "Increased Friends Limit": return "IncreasedFriendsLimit";
                    case "More Simultaneous Classic Matches": return "MoreMatches";
                }
                return "";
            }
        }

        public static class ClaimReward
        {
            public const string REWARD_INFO = "rewardInfo";
            public const string BUCKS = "bucks";
            public const string AD_LIFETIME_IMPRESSIONS = "adLifetimeImpressions";
            
            public const string TYPE_AD_BUCKS = "rewardAdBucks";

            public const string TYPE_MATCH_WIN = "rewardMatchWin";
            public const string TYPE_MATCH_WIN_AD = "rewardMatchWinAd";
            public const string TYPE_MATCH_RUNNERUP_WIN = "rewardMatchRunnerUp";
            public const string TYPE_MATCH_RUNNERUP_WIN_AD = "rewardMatchRunnerUpWinAd";
            public const string TYPE_PROMOTION = "rewardMatchPromotional";

            public const string NONE = "none";
        }

		public static class Friend
		{
			public const string PLAYER_ID = "playerId";
			public const string TYPE = "type";
			public const string PERMISSION = "permission";
			public const string CHALLENGE_ID = "challengeId";
			public const string GAMES_WON = "gamesWon";
			public const string GAMES_LOST = "gamesLost";
			public const string GAMES_DRAWN = "gamesDrawn";
			public const string PUBLIC_PROFILE = "publicProfile";
            public const string FRIEND_ID = "friendId";
            public const string IS_ONLINE = "isOnline";
            public const string TYPE_SOCIAL = "social";
            public const string TYPE_COMMUNITY = "community";
            public const string TYPE_FAVOURITE = "favourite";
            public const string LAST_MATCH_TIMESTAMP = "lastMatchTimestamp";
            public const string REMOVED_FROM_RECENT_PLAYED = "removedFromRecentPlayed";
        }

		public static class FriendsOp
		{
			public const string FRIENDS = "friends";
			public const string BLOCKED = "blocked";
            public const string COMMUNITY = "community";
            public const string ADD = "add";
            public const string BLOCK = "block";
            public const string SEARCH = "search";
            public const string STATUS = "status";
            public const string UNBLOCK = "unblock";
        }

        public static class OfferDrawOp
        {
            public const string OFFER_DRAW = "offerDraw";
            public const string REJECT_OFFER_DRAW = "rejectOfferDraw";
            public const string ACCEPT_OFFER_DRAW = "accepttOfferDraw";
        }

        public static class PublicProfile
		{
			public const string PROFILE_NAME = "name";
			public const string PROFILE_COUNTRY_ID = "countryId";
			public const string PROFILE_ELO_SCORE = "eloScore";
			public const string PROFILE_EXTERNAL_IDS = "externalIds";

			public const string NAME = "name";
			public const string COUNTRY_ID = "countryId";
			public const string ELO_SCORE = "eloScore";
			public const string PLAYER_ACTIVE_INVENTORY = "playerActiveInventory";
			public const string EXTERNAL_IDS = "externalIds";
            public const string IS_ONLINE = "isOnline";
            public const string CREATION_DATE = "creationDate";
            public const string LAST_SEEN = "lastSeen";
            public const string TOTAL_GAMES_WON = "totalGamesWon";
            public const string TOTAL_GAMES_LOST = "totalGamesLost";
            public const string IS_SUBSCRIBER = "isSubscriber";
        }

        public static class  PlayerDetails
		{
			public const string PLAYER_ID = "playerId";
            public const string CREATION_DATE = "creationDate";
			public const string TAG = "tag";
			public const string DISPLAY_NAME = "displayName";
			public const string COUNTRY_ID = "countryId";
			public const string BUCKS = "bucks";
			public const string ELO_SCORE = "eloScore";
            public const string IS_PREMIUM = "isPremium";
            public const string GAMES_WON = "gamesWon";
			public const string GAMES_LOST = "gamesLost";
			public const string GAMES_DRAWN = "gamesDrawn";
			public const string AD_LIFETIME_IMPRESSIONS = "adLifetimeImpressions";
			public const string PLAYER_ACTIVE_INVENTORY = "playerActiveInventory";
			public const string INVENTORY = "inventory";
			public const string FRIENDS = "friends";
            public const string REMOVE_ADS_TIMESTAMP = "removeAdsTimeStamp";
            public const string REMOVE_ADS_TIMEPERIOD = "removeAdsTimePeriod";
            public const string EDITED_NAME = "editedName";
            public const string IS_FACEBOOK_REWARD_CLAIMED = "isFBConnectRewardClaimed";
            public const string CPU_POWERUP_USED_COUNT = "cpuPowerupUsedCount";
            public const string SUBSCRIPTION_EXPIRY_TIMESTAMP = "subscriptionExpiryTime";
            public const string SUBSCRIPTION_TYPE = "subscriptionType";
            public const string REWARD_INDEX = "rewardIndex";
            public const string REWARD_SHORT_CODE = "shortCode";
            public const string REWARD_QUANITY = "addedAmount";
            public const string REWARD_CURRENT_POINTS = "currentPoints";
            public const string REWARD_REQUIRED_POINTS = "pointsRequired";
            public const string ADS_REWARD_DATA = "adsRewardData";
        }

        public static class Match
        {
            public const string ABORT_KEY = "abort";
            public const string ACTIVE_CHALLENGES = "activeChallenges";
            public const string SHORT_CODE = "shortCode";
            public const string QUICK_MATCH_SHORT_CODE = "Standard";
            public const string LONG_MATCH_SHORT_CODE = "LongPlay";
            public const string CHALLENGER_ID = "challengerId";
            public const string CHALLENGED_ID = "challengedId";
            public const string ACCEPT_STATUS_KEY = "status";
            public const string ACCEPT_STATUS_NEW = "new";
            public const string ACCEPT_STATUS_CANCELED = "canceled";
            public const string ACCEPT_STATUS_ACCEPTED = "accepted";
            public const string ACCEPT_STATUS_DECLINED = "declined";
            public const string WINNER_ID = "winnerId";
            public const string ELO_CHANGE = "eloChange";
            public const string CREATE_TIME = "createTime";
            public const string IS_RANKED = "isRanked";
            public const string DURATION = "gameDuration";
        }

        public static class Chat
        {
            public const string CHAT_EXT_CODE = "ChatMessage";
            public const string SENDER_ID = "senderId";
            public const string TEXT = "text";
            public const string TIMESTAMP = "timestamp";
            public const string GUID = "guid";
            public const string CHAT_DATA = "chatData";
        }

        public static class Rewards
        {
            public const string REWARDS_SETTINGS = "rewardsSettings";
            public const string MATCH_WIN_REWARD = "matchWinReward";
            public const string MATCH_WIN_AD_REWARD = "matchWinAdReward";
            public const string MATCH_RUNNER_UP_REWARD = "matchRunnerUpReward";
            public const string MATCH_RUNNER_UP_AD_REWARD = "matchRunnerUpAdReward";
            public const string FACEBOOK_CONNECT_REWARD = "facebookConnectReward";
            public const string FAIL_SAFE_COIN_REWARD   = "failSafe";
            public const string POWERUP_COIN_VALUE = "powerUpCoinsValue";
            public const string COEFFICIENT_WIN_VIDEO = "winVideo";
            public const string COEFFICIENT_WIN_INTERSITIAL = "winIntersitial";
            public const string COEFFICIENT_LOSE_VIDEO = "loseVideo";
            public const string COEFFICIENT_LOSE_INTERSITIAL = "loseIntersitial";
            

        }

        public static class PowerUp
        {
            public const string SAFE_MOVE = "PowerUpSafeMove";
            public const string HINT = "PowerUpHint";
            public const string HINDSIGHT = "PowerUpHindsight";
        }
    }

    // TODO: Organize these like the event data keys, consider class renaming
    // TODO: Look at the ErrorKey being sent from the server and wire it up to our error screen
}
