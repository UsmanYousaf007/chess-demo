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
        public const string SHOP_ITEM_PROMOTION_ID = "promotionId";
        public const string SHOP_ITEM_BONUS_XP_PERCENTAGE = "bonusXpPercentage";
        public const string SHOP_ITEM_HINTS_COUNT = "hintsCount";
        public const string SHOP_ITEM_LOSS_RECOVERY_PERCENTAGE = "lossRecoveryPercentage";
        public const string SHOP_ITEM_BONUS_AMOUNT = "bonusAmount";
		public const string SHOP_ITEM_STORE_PRODUCT_ID = "googlePlayProductId";

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

        // Ad Settings
        public const string ADS_SETTINGS = "adsSettings";
        public const string ADS_MAX_IMPRESSIONS_PER_SLOT = "ADS_MAX_IMPRESSIONS_PER_SLOT";
        public const string ADS_SLOT_HOUR = "ADS_SLOT_HOUR";
        public const string ADS_REWARD_INCREMENT = "ADS_REWARD_INCREMENT";
        public const string ADS_FREE_NO_ADS_PERIOD = "ADS_FREE_NO_ADS_PERIOD";

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

        // PING KEYS
        public const string CLIENT_SEND_TIMESTAMP = "clientSendTimestamp";
        public const string SERVER_RECEIPT_TIMESTAMP = "serverReceiptTimestamp";

        // Message ExtCodes
        public const string START_GAME_MESSAGE = "StartGameMessage";
        public const string NEW_FRIEND_MESSAGE = "NewFriendMessage";

        // Feature Shop Items
        public const string SHOP_ITEM_FEATURE_REMOVE_ADS = "FeatureRemoveAds";

        // Mesage data keys
        public const string GAME_START_TIME = "gameStartTime";
        public const string CHALLENGE_ID = "challengeId";

        // Other (TODO: Organize these)
        public const string CHALLENGER_ID = "challengerId";
        public const string CHALLENGED_ID = "challengedId";

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
            public const string MATCH_DATA_KEY = "matchData";
            public const string PROFILE = "profile";
            public const string PROFILE_NAME = "name";
            public const string PROFILE_COUNTRY_ID = "countryId";
            public const string PROFILE_ELO_SCORE = "eloScore";
            public const string PROFILE_EXTERNAL_IDS = "externalIds";
            public const string BOT_ID = "botId";
            public const string GAME_START_TIME = "gameStartTime";
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

            public const string SKIN_SHOP_TAG = "Skin";
            public const string COINS_SHOP_TAG = "CoinPack";
            public const string FEATURE_SHOP_TAG = "Feature";

            public const string SHOP_ITEM_TIER_COMMON = "Common";
            public const string SHOP_ITEM_TIER_RARE = "Rare";
            public const string SHOP_ITEM_TIER_EPIC = "Epic";
            public const string SHOP_ITEM_TIER_LEGENDARY = "Legendary";

            public const string SHOP_ITEM_TYPE_CURRENCY = "CURRENCY";
            public const string SHOP_ITEM_TYPE_VGOOD = "VGOOD";
        }

        public static class ClaimReward
        {
            public const string REWARD_INFO = "rewardInfo";
            public const string BUCKS = "bucks";
            public const string AD_LIFETIME_IMPRESSIONS = "adLifetimeImpressions";
            
            public const string TYPE_AD_BUCKS = "rewardAdBucks";
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
		}

		public static class FriendsOp
		{
			public const string FRIENDS = "friends";
			public const string BLOCKED = "blocked";
            public const string COMMUNITY = "community";
            public const string ADD = "add";
            public const string BLOCK = "block";
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
			public const string GAMES_WON = "gamesWon";
			public const string GAMES_LOST = "gamesLost";
			public const string GAMES_DRAWN = "gamesDrawn";
			public const string AD_LIFETIME_IMPRESSIONS = "adLifetimeImpressions";
			public const string PLAYER_ACTIVE_INVENTORY = "playerActiveInventory";
			public const string INVENTORY = "inventory";
			public const string FRIENDS = "friends";
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
            public const string ACCEPT_STATUS_ACCEPTED = "accepted";
            public const string ACCEPT_STATUS_DECLINED = "declined";
            public const string WINNER_ID = "winnerId";
            public const string ELO_CHANGE = "eloChange";
        }
    }

    // TODO: Organize these like the event data keys, consider class renaming
    // TODO: Look at the ErrorKey being sent from the server and wire it up to our error screen
}
