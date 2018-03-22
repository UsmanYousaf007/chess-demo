/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 15:36:16 UTC+05:00
/// 
/// @description
/// These are the server backend keys. These keys remain the same on the server
/// and the client side.

using System.Collections.Generic;

using GameSparks.Core;

namespace TurboLabz.Gamebet
{
    public static partial class GSBackendKeys
    {
        // App Version Settings
        public const string APP_VERSION_NUMBER = "appVersionNumber";

        // League settings
        public const string LEAGUE_SETTINGS = "leagueSettings";
        public const string LEAGUE_INFO_ID = "id";
        public const string LEAGUE_INFO_START_LEVEL = "startLevel";
        public const string LEAGUE_INFO_END_LEVEL = "endLevel";
        public const string LEAGUE_INFO_PRIZE_1 = "prize1";
        public const string LEAGUE_INFO_PRIZE_2 = "prize2";
        public const string LEAGUE_INFO_PRIZE_3 = "prize3";
        public const string PRIZE_CURRENCY_1 = "coins";
        public const string PRIZE_CURRENCY_2 = "bucks";

        // Room settings
        public const string ROOM_SETTINGS = "roomSettings";
        public const string ROOM_INFO_ID = "id";
        public const string ROOM_INFO_GROUP_ID = "groupId";
        public const string ROOM_INFO_UNLOCK_AT_LEVEL = "unlockAtLevel";
        public const string ROOM_INFO_GAME_DURATION = "gameDuration";
        public const string ROOM_INFO_WAGER = "wager";
        public const string ROOM_INFO_PRIZE = "prize";
        public const string ROOM_INFO_DRAW_PRIZE = "drawPrize";
        public const string ROOM_INFO_VICTORY_XP = "victoryXp";
        public const string ROOM_INFO_WINS_FOR_TROPHY = "winsForTrophy";
        public const string ROOM_INFO_TROPHIES_FOR_ROOM_TITLE_1 = "trophiesForRoomTitle1";
        public const string ROOM_INFO_TROPHIES_FOR_ROOM_TITLE_2 = "trophiesForRoomTitle2";
        public const string ROOM_INFO_TROPHIES_FOR_ROOM_TITLE_3 = "trophiesForRoomTitle3";
        public const string ROOM_DESCRIPTION = "description";
        public const string ROOM_START_TIME = "roomStartTime";
        public const string ROOM_DURATION = "roomDuration";

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

        // Account details
        public const string ACCOUNT_DETAILS = "accountDetails";
        public const string ACCOUNT_VIRTUALGOODS = "virtualGoods";
        public const string PLAYER_ACTIVE_INVENTORY = "playerActiveInventory";

        // Loot boxes
        public const string LOOT_BOXES_PLAYER_LOOT = "playerLootBoxes";
        public const string LOOT_BOXES_PLAYER_LOOT_OPENED = "playerLootBoxesOpened";

        // Coin pack purchase
        public const string COINSPACK_COINS1_BOUGHT = "coins1Bought";

        // Forge Item settings
        public const string FORGE_ITEM_SETTINGS = "forgeItemSettings";
        public const string FORGE_ITEM_ITEM_KEY = "forgeItemKey";
        public const string FORGE_ITEM_SELL_COINS = "sellCoins";
        public const string FORGE_ITEM_REQUIRED_QUANTITY = "requiredQuantity";
        public const string FORGE_ITEM_CARD_KEY = "forgeCardKey";

        // Forge cards and items
        public const string FORGE_CARDS_COINS_GAINED = "coins1Gained";
        public const string FORGE_CARDS_FORGE_ITEM_GRANTED = "forgedItemGranted";
        public const string FORGE_CARDS_SOLD_QUANTITY = "quantity";
        public const string FORGE_CARDS_CONSUMED_QUANTITY = "forgeCardConsumedQuantity";

        // Player data keys
        public const string IS_SOCIAL_NAME_SET = "isSocialNameSet";
        public const string DISPLAY_NAME = "displayName";
        public const string XP = "xp";
        public const string TAG = "tag";
        public const string LEVEL = "level";
        public const string CURRENCY_1_WINNINGS = "coinWinnings";
        public const string LEAGUE_ID = "leagueId";
        public const string ROOM_RECORDS = "roomRecords";

        // Level settings keys
        public const string LEVEL_SETTINGS = "levelSettings";
        public const string MAX_LEVEL = "maxLevel";
        public const string LEVEL_INFO = "levelInfo";
        public const string LEVEL_INFO_LEVEL = "level";
        public const string LEVEL_INFO_START_XP = "startXp";
        public const string LEVEL_INFO_END_XP = "endXp";
        public const string LEVEL_INFO_LEVEL_PROMOTION_REWARD_BUCKS = "levelPromotionRewardBucks";

        // Room data key
        public const string ROOM_ID = "id";
        public const string ROOM_GAMES_WON = "gamesWon";
        public const string ROOM_GAMES_LOST = "gamesLost";
        public const string ROOM_GAMES_DRAWN = "gamesDrawn";
        public const string ROOM_TROPHIES_WON = "trophiesWon";
        public const string ROOM_ROOM_TITLE_ID = "roomTitleId";

        // Message ExtCodes
        public const string START_GAME_MESSAGE = "StartGameMessage";
        public const string OPPONENT_DISCONNECTED_MESSAGE = "OpponentDisconnectedMessage";
        public const string OPPONENT_RECONNECTED_MESSAGE = "OpponentReconnectedMessage";

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

            public static IDictionary<ExternalAuthType, ExternalAuthData> GetExternalAuthentications(GSData externalIds)
            {
                IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications = new Dictionary<ExternalAuthType, ExternalAuthData>();
                IDictionary<string, object> externalIdsBaseData = externalIds.BaseData;

                foreach (KeyValuePair<string, object> e in externalIdsBaseData)
                {
                    ExternalAuthType type = externalIdMap[e.Key];
                    ExternalAuthData data;
                    data.id = (string)e.Value;

                    externalAuthentications.Add(type, data);
                }

                return externalAuthentications;
            }
        }

        // Match data
        public static class MatchData
        {
            public const string KEY = "matchData";
            public const string PROFILE = "profile";
            public const string PROFILE_NAME = "name";
            public const string PROFILE_LEVEL = "level";
            public const string PROFILE_LEAGUE_ID = "leagueId";
            public const string PROFILE_COUNTRY_ID = "countryId";
            public const string PROFILE_ELO_DIVISION = "eloDivision";
            public const string PROFILE_ELO_SCORE = "eloScore";
            public const string ROOM_RECORDS = "roomRecords";
            public const string PROFILE_EXTERNAL_IDS = "externalIds";
            public const string BOT_ID = "botId";
            public const string ROOM_ID = "roomId";
            public const string GAME_START_TIME = "gameStartTime";
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

        public static class LevelPromotions
        {
            public const string KEY = "levelPromotions";
            public const string FROM = "from";
            public const string TO = "to";
            public const string REWARD = "reward";
            public const string CURRENCY_2 = "bucks";
            public const string NEXT_LEAGUE_PROMOTION = "nextLeaguePromotion";
            public const string NEXT_LEAGUE_PROMOTION_ID = "id";
            public const string NEXT_LEAGUE_PROMOTION_START_LEVEL = "startLevel";
        }

        public static class LeaguePromotion
        {
            public const string KEY = "leaguePromotion";
            public const string FROM = "from";
            public const string TO = "to";
            public const string NEXT_LEAGUE_PROMOTION = "nextLeaguePromotion";
            public const string NEXT_LEAGUE_PROMOTION_ID = "id";
            public const string NEXT_LEAGUE_PROMOTION_START_LEVEL = "startLevel";
        }

        public static class TrophyPromotion
        {
            public const string KEY = "trophyPromotion";
            public const string FROM = "from";
            public const string TO = "to";
        }

        public static class RoomTitlePromotion
        {
            public const string KEY = "roomTitlePromotion";
            public const string FROM = "from";
            public const string TO = "to";
            public const string REWARD = "reward";
            public const string CURRENCY_2 = "bucks";
        }

        public static class AdInfo
        {
            public const string KEY = "adInfo";
            public const string LEVEL = "level";
            public const string REWARD = "reward";
            public const string REWARD_CURRENCY_1 = "coins";
            public const string REWARD_CURRENCY_2 = "bucks";
        }

        public static class ShopItem
        {
            public const string SHOP_SETTINGS = "shopSettings";
            public const string SKIN_SHOP_ITEMS = "skinShopItems";
            public const string AVATAR_SHOP_ITEMS = "avatarShopItems";
            public const string AVATARBORDER_SHOP_ITEMS = "avatarBorderShopItems";
            public const string CHATPACK_SHOP_ITEMS = "chatpackShopItems";
            public const string FORGECARD_SHOP_ITEMS = "forgeCardShopItems";
            public const string LOOTBOX_SHOP_ITEMS = "lootBoxShopItems";
            public const string COINS_SHOP_ITEMS = "coinsShopItems";

            public const string SKIN_SHOP_TAG = "Skin";
            public const string AVATAR_SHOP_TAG = "Avatar";
            public const string AVATARBORDER_SHOP_TAG = "AvatarBorder";
            public const string CHATPACK_SHOP_TAG = "ChatPack";
            public const string FORGECARD_SHOP_TAG = "ForgeCard";
            public const string LOOTBOX_SHOP_TAG = "LootBox";
            public const string COINS_SHOP_TAG = "CoinPack";

            public const string SHOP_ITEM_TIER_COMMON = "Common";
            public const string SHOP_ITEM_TIER_RARE = "Rare";
            public const string SHOP_ITEM_TIER_EPIC = "Epic";
            public const string SHOP_ITEM_TIER_LEGENDARY = "Legendary";

            public const string SHOP_ITEM_TYPE_CURRENCY = "CURRENCY";
            public const string SHOP_ITEM_TYPE_VGOOD = "VGOOD";

            public const string LOOT_BOX_TIER_1 = "LootBoxTier1";
            public const string LOOT_BOX_TIER_2 = "LootBoxTier2";
            public const string LOOT_BOX_TIER_3 = "LootBoxTier3";
            public const string LOOT_BOX_TIER_4 = "LootBoxTier4";
        }
    }

    // TODO: Organize these like the event data keys, consider class renaming
    // TODO: Look at the ErrorKey being sent from the server and wire it up to our error screen
}
