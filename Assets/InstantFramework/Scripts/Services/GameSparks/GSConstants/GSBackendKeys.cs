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
        public const string EVENT_DATA = "eventData";
        public const string EVENT_DAY_NUMBER = "eventdayNo";
        public const string POWERUP_USAGE_PERCENTAGE = "powerupUsagePercentage";
        public const string STORE_IOS = "ios";
        public const string STORE_ANDROID = "android";
        public const string CONTACT_SUPPORT_URL = "contactSupportURL";
        public const string IS_MANDATORY_UPDATE = "isMandatoryUpdate";
        public const string APP_UPDATE_FLAG = "appUpdateFlag";
        public const string NTH_WINS_APP_RATE_APP = "nthWinsRateApp";
        public const string GAMES_PLAYED_TODAY = "todayGamesCount";
        public const string MAINTENANCE_WARNING_TIMESTAMP = "maintenanceWarningTimeStamp";
        public const string SESSION_DURATION_FOR_GDPR = "sessionDurationForGDPRinMinutes";

        //game Settings
        public const string GAME_SETTINGS = "gameSettings";
        public const string MAX_LONG_MATCH_COUNT = "maxLongMatchCount";
        public const string MAX_FRIENDS_COUNT = "maxFriendsCount";
        public const string FACEBOOK_CONNECT_REWARD = "facebookConnectReward";
        public const string MAX_RECENTLY_COMPLETED_MATCH_COUNT = "maxRecentlyCompletedMatchCount";
        public const string MAX_COMMUNITY_MATECHES = "maxCommunityMatches";
        public const string PREMIUM = "premium";
        public const string HINTS_ALLOWED = "hintsAllowedPerGame";
        public const string DAILY_NOTIFICATION_DEADLINE_HOUR = "dailyNotificationDeadlineHour";
        public const string DEFAULT_SUBSCRIPTION_KEY = "defaultSubscriptionKey";
        public const string MATCHMAKING_RANDOM_RANGE = "matchmakingRandomRange";
        public const string BETTING_INCREMENTS = "bettingIncrements";
        public const string BET_INCREMENT_BY_GAMES_PLAYED = "defaultBetIncrementByGamesPlayed";
        public const string POWER_MODE_FREE_HINTS = "powerModeFreeHints";
        public const string MATCH_COINS_MULTIPLYER = "matchCoinsMultiplyer";
        public const string OPPONENT_HIGHER_ELO_CAP = "opponentHigherEloCap";
        public const string OPPONENT_LOWER_ELO_CAP = "opponentLowerEloCap";

        public const string HUUUGE_SERVER_VERIFICATION_ENABLED = "huuugeServerVerificationEnabled";
        //free hint Settings
        public const string FREE_HINT_THRESHOLDS = "freeHintThresholds";
        public const string ADV_THRESHOLDS = "advantage";
        public const string HINTS_PURCHASED_THRESHOLDS = "hintsPurchased";

        // Shop settings
        public const string SHOP_SETTINGS = "shopSettings";
        public const string SHOP_ITEM_TAGS = "tags";
        public const string SHOP_ITEM_ID = "shortCode";
        public const string SHOP_ITEM_TYPE = "type";
        public const string SHOP_ITEM_DISPLAYNAME = "name";
        public const string SHOP_ITEM_DESCRIPTION = "description";
        public const string SHOP_ITEM_CURRENCY1COST = "currency1Cost";
        public const string SHOP_ITEM_CURRENCY2COST = "currency2Cost";
        public const string SHOP_ITEM_CURRENCY3COST = "currency3Cost";
        public const string SHOP_ITEM_CURRENCY4COST = "currency4Cost";
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
        public const string OPPONENT_ELO_CHANGE = "opponentEloChange";
        public const string CHAT = "chat";
        public const string DEFAULT_ITEMS_ADDED = "isDefaultItemsAdded";
        public const string REFUND_GEMS_ADDED = "refundGemsAdded";

        // Lessons
        public const string LESSONS_MAPPING = "lessonsMapping";
        public static string GetLessonKey(string text)
        {
            switch (text)
            {
                case "How to Play": return "PieceMovement";
                case "Ending the Game": return "ChangeTheme";
                case "Building an Opening Strategy": return "BuildOpeningStrategy";
                case "Advanced Openings": return "AdvOpening";
                case "Tactics": return "Tactics";
                case "Advanced Tactics": return "AdvTactics";
                case "Checkmating your Opponent": return "CheckMatingOppo";
                case "Advanced Scenarios": return "AdvScenarios";
            }
            return "";
        }

        // Downloadables
        public const string DOWNLOADBLES = "downloadables";
        public const string DOWNLOADABLE_LAST_MODIFIED = "lastModified";
        public const string DOWNLOADABLE_SHORT_CODE = "shortCode";
        public const string DOWNALOADABLE_SIZE = "size";
        public const string DOWNLOADABLE_URL = "url";

        // Leagues
        public const string LEAGUE_SETTINGS = "leagueSettings";

        // Promotions
        public const string PROMOTION_SETTINGS = "promotionSettingsV3";

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
        public const string SHOW_PREGAME_AD_TOURNAMENT = "showPregameTournament";
        public const string SHOW_INGAME_AD_CPU = "showInGameCPU";
        public const string SHOW_INGAME_AD_30MIN = "showInGame30Min";
        public const string SHOW_INGAME_AD_CLASSIC = "showInGameClassic";
        public const string MINUTES_BETWEEN_INGAME_AD = "minutesBetweenIngameAds";
        public const string MINUTES_LEFT_DISABLE_TOURNAMENT_ADS = "minutesLeftDisableTournamentPregame";
        public const string MINUTES_ELAPSED_DISABLE_30MIN_INGAME_ADS = "minutesElapsedDisable30MinInGame";
        public const string ENABLE_BANNER_ADS = "enableBannerAds";
        public const string REMOVE_INTER_ADS = "removeInterAdsOnPurchase";
        public const string REMOVE_RV = "removeRVOnPurchase";

        public const string MIN_PLAY_DAYS_REQUIRED = "minPlayDaysRequired";
        public const string MIN_PURCHASES_REQUIRED = "minPurchasesRequired";
        public const string PREMIUM_TIMER_COOLDOWN_TIME = "premiumTimerCooldownTimeInMin";
        public const string FREEMIUM_TIMER_COOLDOWN_TIME = "freemiumTimerCooldownTimeInMin";
        public const string MIN_GEMS_REQUIRED_FOR_RV = "minGemsRequiredforRV";

        public const string AD_PLACEMENTS = "adPlacements";
        // AB Test Settings
        public const string AB_TEST_ADS_SETTINGS = "abTestAds";

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

        // Tournaments
        public const string JOINED_TOURNAMENTS = "tournaments";
        public const string LIVE_TOURNAMENTS = "liveTournaments";

        // Inbox
        public const string INBOX = "inbox";
        public const string INBOX_COUNT = "inboxCount";

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
            private const string APPLE = "AP";

            private static readonly IDictionary<string, ExternalAuthType> externalIdMap =
                new Dictionary<string, ExternalAuthType>() {
                { FACEBOOK, ExternalAuthType.FACEBOOK },
                { GOOGLE_PLAY, ExternalAuthType.GOOGLE_PLAY },
                { APPLE, ExternalAuthType.APPLE}
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
            public const string BET_VALUE = "betValue";
            public const string POWER_MODE = "powerMode";
            public const string FREE_HINTS = "freeHints";
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
            public const string VIDEO_LESSONS_BASE_URL = "videoLessonsBaseURL";

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
            public const string VIDEO_LESSON_SHOP_ITEMS = "videoLessonShopItems";
            public const string GEMPACK_SHOP_ITEMS = "gemPackShopItems";
            public const string SPECIALPACK_SHOP_ITEMS = "specialPackShopItems";
            public const string SPECIALITEM_SHOP_ITEMS = "specialItemShopItems";
            public const string SPECIALITEM_POINTS_ITEMS = "specialItemPointItems";

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

            public const string VIDEO_LESSON_SHOP_TAG = "VideoLesson";

            public const string FEATURE_REMOVEAD_PERM_SHOP_TAG = "FeatureRemoveAdsPerm";
            public const string FEATURE_REMOVEAD_30_SHOP_TAG = "FeatureRemoveAds30";

            public const string SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG = "SpecialBundleUltimate";
            public const string SPECIAL_BUNDLE_STANDARD_SHOP_TAG = "SpecialBundleStandard";
            public const string SPECIAL_BUNDLE_NOADSFOREVER_SHOP_TAG = "SpecialBundleNoAdsForever";

            public const string GEMPACK_SHOP_TAG = "GemPack";
            public const string SPECIALPACK_SHOP_TAG = "SpecialPack";
            public const string SPECIALITEM_SHOP_TAG = "SpecialItem";
            public const string SPECIALITEM_POINTS_TAG = "Points";

            public const string SUBSCRIPTION_SHOP_TAG = "Subscription";
            public const string SUBSCRIPTION_ANNUAL_SHOP_TAG = "SubscriptionAnnual";
            public const string SUBSCRIPTION_ANNUAL_SALE_TAG = "SubscriptionAnnualSale";

            public const string SHOP_ITEM_TIER_COMMON = "Common";
            public const string SHOP_ITEM_TIER_RARE = "Rare";
            public const string SHOP_ITEM_TIER_EPIC = "Epic";
            public const string SHOP_ITEM_TIER_LEGENDARY = "Legendary";

            public const string SHOP_ITEM_TYPE_CURRENCY = "CURRENCY";
            public const string SHOP_ITEM_TYPE_VGOOD = "VGOOD";

            public const string ALL_THEMES_PACK = "SpecialPackAllThemes";
            public const string ALL_LESSONS_PACK = "SpecialPackAllLessons";
            public const string REMOVE_ADS_PACK = "SpecialPackRemoveAds";
            public const string SALE_REMOVE_ADS_PACK = "SalePackRemoveAds";

            public const string SPECIAL_ITEM_GEMS_BOOSTER = "SpecialItemGemsBooster";
            public const string SPECIAL_ITEM_TICKET = "SpecialItemTicket";
            public const string SPECIAL_ITEM_HINT = "SpecialItemHint";
            public const string SPECIAL_ITEM_KEY = "SpecialItemKey";
            public const string SPECIAL_ITEM_RATING_BOOSTER = "SpecialItemRatingBooster";
            public const string INVENTORY_SETTINGS_REWARDED_VIDEO_COST = "inventorySettings";
            public const string DEFAULT_ITEMS_V1 = "DefaultOwnedItemsV1";
            public const string SPECIAL_ITEM_REWARD_DOUBLER = "SpecialItem2xReward";
            public const string DEFAULT_ITEMS_V2 = "DefaultOwnedItemsV2";
            public const string FULL_GAME_ANALYSIS = "FullGameAnalysis";

            public const string SPECIAL_BUNDLE_WELCOME = "SpecialBundleWelcome";
            public const string SPECIAL_BUNDLE_ELITE = "SpecialBundleElite";
            public const string SPECIAL_BUNDLE_GOLDEN = "SpecialBundleGolden";
            public const string SPECIAL_BUNDLE_EMERALD = "SpecialBundleEmerald";
            public const string SPECIAL_BUNDLE_RUBY = "SpecialBundleRuby";
            public const string SPECIAL_BUNDLE_DIAMOND = "SpecialBundleDiamond";
            public const string SPECIAL_BUNDLE_GRAND_MASTER = "SpecialBundleGrandMaster";

            public static string GetOfferItemKey(string text)
            {
                switch (text)
                {
                    case "Go completely Ads free": return "NoAds";
                    case "Unlock 40 professional chess lessons": return "Lessons";
                    case "Access unlimited Chess Sets": return "Theme";
                    case "Access unlimited game powerups": return "Powerups";
                    case "Access unlimited daily games": return "Games";
                    case "Increase your friends limit": return "Friends";
                    case "Free daily Tournament Ticket": return "Tickets";
                }
                return "";
            }
        }

        public static class ClaimReward
        {
            public const string REWARD_INFO = "reward";
            public const string BUCKS = "bucks";
            public const string AD_LIFETIME_IMPRESSIONS = "adLifetimeImpressions";

            public const string TYPE_AD_BUCKS = "rewardAdBucks";

            public const string TYPE_MATCH_WIN = "rewardMatchWin";
            public const string TYPE_MATCH_WIN_AD = "rewardMatchWinAd";
            public const string TYPE_MATCH_RUNNERUP_WIN = "rewardMatchRunnerUp";
            public const string TYPE_MATCH_RUNNERUP_WIN_AD = "rewardMatchRunnerUpWinAd";
            public const string TYPE_PROMOTION = "rewardMatchPromotional";
            public const string TYPE_BOOST_RATING = "ratingBoostTier1";
            public const string TYPE_GIFT = "giftReward";
            public const string TYPE_DAILY = "dailyReward";
            public const string TYPE_LOBBY_CHEST = "chestCoinsReward";
            public const string TYPE_COINS_PURCHASE = "coinPurchaseReward";
            public const string CLAIM_REWARD_TYPE = "claimRewardType";
            public const string TYPE_PERSONALISED_ADS_GEM = "personalisedAdsGemReward";
            public const string TYPE_FREE_FULL_GAME_ANALYSIS = "freeFullGameAnalysis";
            public const string TYPE_POWERPLAY = "powerPlayReward";
            public const string TYPE_RV_RATING_BOOSTER = "ratingBoosterReward";
            public const string TYPE_LOBBY_CHEST_V2 = "chestGemsReward";
            public const string TYPE_RV_ANALYSIS = "analysisReward";

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
            public const string FLAG_MASK = "flagMask";
            public const string UPLOADED_PIC_ID = "uploadedPicId";
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

        public static class Tournament
        {
            public const string TOURNAMENT_KEY = "tournament";
            public const string TOURNAMENT_ID = "tournamentId";
            public const string TYPE = "type";
            public const string SHORT_CODE = "shortCode";
            public const string ACTIVE = "active";
            public const string NAME = "name";
            public const string RANK = "rank";
            public const string GRAND_PRIZE = "grandPrize";
            public const string START_TIME = "startTime";
            public const string DURATION = "duration";
            public const string WAIT_TIME = "waitTime";
            public const string ENTRIES = "entries";
            public const string SCORE = "score";
            public const string MATCHES_PLAYED_COUNT = "matchesPlayedCount";
            public const string REWARDS = "rewards";
            public const string CONCLUDED = "concluded";
            public const string TOURNAMENT_MATCH_SCORE = "tournamentScore";
            public const string TOURNAMENT_MATCH_WIN_TIME_BONUS = "winTimeBonus";
        }

        public static class TournamentReward
        {
            public const string TYPE = "type";
            public const string QUANTITY = "quantity";
            public const string TROPHIES = "trophies";
            public const string CHEST_TYPE = "chestType";
            public const string GEMS = "gems";
            public const string HINTS = "hints";
            public const string RATING_BOOSTERS = "ratingBoosters";
            public const string MIN_RANK = "minRank";
            public const string MAX_RANK = "maxRank";
        }

        public static class TournamentsOp
        {
            public const string ERROR = "error";
            public const string TOURNAMENT = "tournament";
            public const string LIVE_TOURNAMENT = "liveTournament";
            public const string TOURNAMENTS = "tournaments";
            public const string LIVE_TOURNAMENTS = "liveTournaments";
        }

        public static class InBoxOp
        {
            public const string ERROR = "error";
            public const string GET = "get";
            public const string COLLECT = "collect";
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
            public const string LEAGUE = "league";
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
            public const string UPLOADED_PIC_ID = "uploadedPicId";
            public const string LAST_WATCHED_VIDEO = "lastWatchedVideoId";
            public const string GEMS = "gems";
            public const string TROPHIES = "trophies";
            public const string TROPHIES2 = "trophies2";
            public const string LEAGUE = "league";
            public const string COINS = "coins";
            public const string CHEST_UNLOCK_TIMESTAMP = "chestUnlockTimestamp";
            public const string RV_UNLOCK_TIMESTAMP = "rvUnlockTimestamp";
            public const string DYNAMIC_BUNDLE_SHORT_CODE = "dynamicBundleShortCode";
            public const string DYNAMIC_GEM_SPOT_BUNDLE = "dynamicGemSpotBundle";
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
            public const string RATING_BOOST = "ratingBoostTier1Reward";
            public const string PERSONALISED_ADS_GEM = "personalisedAdsGemReward";
            public const string FREE_FULL_GAME_ANALYSIS = "freeFullGameAnalysis";
            public const string RV_RATING_BOOST = "ratingBoosterReward";
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
