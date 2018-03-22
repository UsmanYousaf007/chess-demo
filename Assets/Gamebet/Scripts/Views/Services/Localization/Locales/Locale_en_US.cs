/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-05 16:43:41 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class Locale_en_US : ILocale
    {
        public IDictionary<string, string> data { get; private set; }

        public Locale_en_US()
        {
            data = new Dictionary<string, string>() {
                //
                // Client only values
                //

                #region Loading

                { LocalizationKey.LD_MESSAGE_LABEL, "Connecting to servers..." },

                #endregion

                #region RetryConnection

                { LocalizationKey.RC_ERROR_CODE, "Ooops! Something went wrong.\n Error Code: " },

                #endregion

                #region Auth

                { LocalizationKey.AUTH_SELECT_LOGIN_LABEL, "Select Login Type" },
                { LocalizationKey.AUTH_AUTH_FACEBOOK_BUTTON_LABEL, "LOGIN WITH FACEBOOK" },
                { LocalizationKey.AUTH_AUTH_GUEST_BUTTON_LABEL, "PLAY AS GUEST" },

                #endregion

                #region SetPlayerSocialName

                { LocalizationKey.SPSN_HEADING_LABEL, "Select Display Name" },

                #endregion

                #region Lobby

                { LocalizationKey.LOBBY_PLAY_BUTTON_LABEL, "PLAY" },
                { LocalizationKey.CPU_BUTTON_LABEL, "CPU" },
                { LocalizationKey.LEARN_BUTTON_LABEL, "LEARN" },
                { LocalizationKey.LOBBY_FREE_CURRENCY_1_BUTTON_LABEL, "FREE COINS" },
                { LocalizationKey.LOBBY_FEEDBACK_BUTTON_LABEL, "FEEDBACK" },
                { LocalizationKey.LOBBY_LEVEL, "lvl.{0}" },

                #endregion

                #region PlayerProfile

                { LocalizationKey.PP_LEVEL_LABEL, "Level {0}" },
                { LocalizationKey.PP_XP_MAX_LEVEL_LABEL, "MAX LEVEL" },
                { LocalizationKey.PP_XP_LABEL, "{0} / {1}" },
                { LocalizationKey.PP_CURRENCY_1_TITLE_LABEL, "Coins" },
                { LocalizationKey.PP_CURRENCY_2_TITLE_LABEL, "Bucks" },
                { LocalizationKey.PP_CURRENCY_1_WINNINGS_TITLE_LABEL, "Total Winnings" },
                { LocalizationKey.PP_WIN_RATE_TITLE_LABEL, "Win Rate" },
                { LocalizationKey.PP_WIN_RATE_LABEL, "{0:0.#}%" },
                { LocalizationKey.PP_GAMES_WON_TITLE_LABEL, "Games Won" },
                { LocalizationKey.PP_GAMES_WON_LABEL, "{0} of {1}" },
                { LocalizationKey.PP_GAMES_LOST_TITLE_LABEL, "Games Lost" },
                { LocalizationKey.PP_GAMES_LOST_LABEL, "{0} of {1}" },
                { LocalizationKey.PP_GAMES_DRAWN_TITLE_LABEL, "Games Drawn" },
                { LocalizationKey.PP_GAMES_DRAWN_LABEL, "{0} of {1}" },
                { LocalizationKey.PP_ROOM_TITLES_HEADER_LABEL, "Country Titles" },
                { LocalizationKey.PP_NO_ROOM_TITLES_LABEL, "Earn Country Titles by winning matches." },
                { LocalizationKey.PP_ROOM_DURATION_LABEL, "{0} {0:PLRL|Min|Mins}" },
                { LocalizationKey.PP_TROPHIES_WON_LABEL, "x{0}" },

                #endregion

                #region Shop

                { LocalizationKey.S_HEADING_LABEL, "Daily gift" },
                { LocalizationKey.S_COLLECTION_LABEL, "COLLECTION" },
                { LocalizationKey.S_LOOT_LABEL, "LOOT" },
                { LocalizationKey.S_LOOT_BOXES_LABEL, "Loot Boxes" },
                { LocalizationKey.S_AVATARS_LABEL, "Avatars" },
                { LocalizationKey.S_BORDER_LABEL, "Border" },
                { LocalizationKey.S_CHESS_SKINS_LABEL, "Chess Skins" },
                { LocalizationKey.S_CHAT_LABEL, "Chat" },
                { LocalizationKey.S_CURRENCY_LABEL, "Currency" },
                { LocalizationKey.S_FREE_CURRENCY_1_LABEL, "Free Coins" },
                { LocalizationKey.S_AVATARS_BORDERS_LABEL, "Avatars/Borders" },
                { LocalizationKey.S_BUCKS_COINS_LABEL, "Bucks/Coins" },

                { LocalizationKey.S_COMMON_LABEL, "Common" },
                { LocalizationKey.S_RARE_LABEL, "Rare" },
                { LocalizationKey.S_EPIC_LABEL, "Epic" },
                { LocalizationKey.S_LEGENDARY_LABEL, "Legendary" },

                { LocalizationKey.S_BUCKS_LABEL, "Bucks" },
                { LocalizationKey.S_COINS_LABEL, "Coins" },

                { LocalizationKey.S_OWNED_LABEL, "owned" },
                { LocalizationKey.S_BONUS_LABEL, "+{0} bonus" },

                #endregion

                #region ShopModals

                { LocalizationKey.S_M_BUY_FOR_LABEL, "Buy for" },
                { LocalizationKey.S_M_NOT_ENOUGH_BUCKS_LABEL, "Not enough Bucks" },
                { LocalizationKey.S_M_ADD_TO_COLLECTION_LABEL, "ADD TO COLLECTION" },
                { LocalizationKey.S_M_VIEW_COLLECTION_LABEL, "VIEW COLLECTION" },
                { LocalizationKey.S_M_YES_LABEL, "YES" },
                { LocalizationKey.S_M_NO_LABEL, "NO" },
                { LocalizationKey.S_M_OWNED_LABEL, "owned" },
                { LocalizationKey.S_M_CANCEL_LABEL, "CANCEL" },

                { LocalizationKey.S_M_PURCHASE_CHESS_SKIN_FOR_LABEL, "Purchase this chess skin for {0} Bucks?" },
                { LocalizationKey.S_M_BOUGHT_CHESS_SKIN_LABEL, "You have bought this chess skin!" },

                { LocalizationKey.S_M_PURCHASE_AVATARS_FOR_LABEL, "Purchase this Avatar for {0} Bucks?" },
                { LocalizationKey.S_M_BOUGHT_AVATARS_LABEL, "You have bought this Avatar." },

                { LocalizationKey.S_M_PURCHASE_AVATARS_BORDER_FOR_LABEL, "Purchase this Border for {0} Bucks?" },
                { LocalizationKey.S_M_BOUGHT_AVATARS_BORDER_LABEL, "You have bought this border." },

                { LocalizationKey.S_M_PURCHASE_CURRENCY_FOR_LABEL, "Purchase \"{0}\" for {1}?" },

                { LocalizationKey.S_M_ADD_TO_LOOT_LABEL, "ADD TO LOOT" },
                { LocalizationKey.S_M_BOUGHT_LOOT_BOX_LABEL, "You have bought loot box" },
                { LocalizationKey.S_M_BONUS_LABEL, "+{0} bonus" },

                #endregion

                #region Inventory

                { LocalizationKey.I_INFO_LABEL, "INFO" },
                { LocalizationKey.I_EQUIP_LABEL, "EQUIP" },
                { LocalizationKey.I_EQUIPPED_LABEL, "Equipped" },

                { LocalizationKey.I_DISMANTLE_DIALOGUE_LABEL, "Dismantle to get" },
                { LocalizationKey.I_BUILD_BUTTON_LABEL, "BUILD" },
                { LocalizationKey.I_DISMANTLE_BUTTON_LABEL, "DISMANTLE" },
                { LocalizationKey.I_COLLECT_BUTTON_LABEL, "COLLECT" },

                { LocalizationKey.I_DIVIDER_LABEL, "{0}/{1}" },
                { LocalizationKey.I_X_LABEL, "x{0}" },
                { LocalizationKey.I_NOT_ENOUGH_CARDS_LABEL, "Not enough cards" },
                { LocalizationKey.I_OWNED_Label, "Owned" },
                { LocalizationKey.I_EMPTY_LOOT_SCREEN_LABEL, "You dont have any item in loot screen" },

                #endregion

                #region Rooms

                { LocalizationKey.ROOMS_FREE_CURRENCY_1_BUTTON_LABEL, "FREE COINS" },
                { LocalizationKey.ROOMS_GAME_DURATION_1_TOGGLE_LABEL, "{0}m" },
                { LocalizationKey.ROOMS_GAME_DURATION_2_TOGGLE_LABEL, "{0}m" },
                { LocalizationKey.ROOMS_GAME_DURATION_3_TOGGLE_LABEL, "{0}m" },

                #endregion

                #region RoomCard

                { LocalizationKey.RC_ROOM_DURATION_LABEL, "{0}m" },
                { LocalizationKey.RC_PRIZE_TITLE_LABEL, "Prize" },
                { LocalizationKey.RC_ENTRY_FEE_TITLE_LABEL, "Fee:" },
                { LocalizationKey.RC_ZERO_TROPHY_COUNT_LABEL, "Get it!" },
                { LocalizationKey.RC_TROPHY_COUNT_LABEL, "x{0}" },
                { LocalizationKey.RC_WINS_NEEDED_TITLE_LABEL, "Wins" },
                { LocalizationKey.RC_TROPHY_WINS_NEEDED_LABEL, "{0}/{1}" },
                { LocalizationKey.RC_UNLOCK_LEVEL_LABLE, "Unlocks at level {0}"},
                { LocalizationKey.RC_MYSTERY_CARD_LABLE, "More countries ahead"},

                { LocalizationKey.RC_PRIZE_DESCRIPTION_LABEL, "Double your coins" },
                { LocalizationKey.RC_2X, "2x" },
                { LocalizationKey.RC_MINIMUM_ENTRY_FEE_TITLE_LABEL, "Minimum Fee:" },

                { LocalizationKey.RC_REWARDED_GOALS_LABEL, "REWARDED GOALS" },
                { LocalizationKey.RC_PROGRESS_LABEL, "COMPLETED {0} of {1}" },
                { LocalizationKey.RC_REMAINING_DAYS_HOURS_LABEL, "{0} days {1} hours left" },
                { LocalizationKey.RC_REMAINING_HOURS_MINUTES_LABEL, "{0} hours {1} minutes left" },
                { LocalizationKey.RC_REMAINING_MINUTES_SECONDS_LABEL, "{0} minutes {1} seconds left" },
                { LocalizationKey.RC_REMAINING_SECONDS_LABEL, "{0} seconds left" },
                { LocalizationKey.RC_TIME_UP_LABEL, "00:00" },

                { LocalizationKey.RC_NEXT_ROOM_LABEL, "Get ready for the next game mode in" },

                #endregion

                #region RoomInfoCard

                { LocalizationKey.RIC_RULES_LABEL, "For every {0} {0:PLRL|match|matches} won you win 1 trophy. The more you win the higher room titles you earn and the more respect you get from opponents." },
                { LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_1, "x{0}" },
                { LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_2, "x{0}" },
                { LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_3, "x{0}" },

                #endregion

                #region Matchmaking

                { LocalizationKey.MM_ROOM_DURATION, "{0} {0:PLRL|Minute|Minutes}" },
                { LocalizationKey.MM_PLAYER_LEVEL, "Level {0}" },
                { LocalizationKey.MM_OPPONENT_LEVEL, "Level {0}" },
                { LocalizationKey.MM_PRIZE_TITLE_LABEL, "Prize" },

                #endregion

                #region EndGame

                { LocalizationKey.EG_ROOM_DURATION_LABEL, "{0} {0:PLRL|Minute|Minutes}" },

                { LocalizationKey.EG_RESULTS_PLAYER_WON_LABEL, "Winner" },
                { LocalizationKey.EG_RESULTS_OPPONENT_WON_LABEL, "Winner" },
                { LocalizationKey.EG_RESULTS_DRAWN_LABEL, "Draw" },

                { LocalizationKey.EG_PLAYER_LEVEL_LABEL, "Level {0}" },
                { LocalizationKey.EG_OPPONENT_LEVEL_LABEL, "Level {0}" },
                { LocalizationKey.EG_PRIZE_TITLE_LABEL, "Prize" },
                
                { LocalizationKey.EG_NEW_MATCH_BUTTON_LABEL, "New Match" },

                #endregion

                #region Promotions

                { LocalizationKey.LVLP_PROMOTION_TITLE_LABEL, "Level Up!" },
                { LocalizationKey.LVLP_REWARD_TITLE_LABEL, "Reward" },
                { LocalizationKey.LVLP_NEXT_PROMOTION_MESSAGE_LABEL, "{0} {0:PLRL|level|levels} to {1}" },
                { LocalizationKey.LVLP_CONTINUE_BUTTON_LABEL, "Continue" },

                { LocalizationKey.LGP_PROMOTION_TITLE_LABEL, "League Promoted!" },
                { LocalizationKey.LGP_NEXT_PROMOTION_MESSAGE_LABEL, "Next league at level {0}" },
                { LocalizationKey.LGP_CONTINUE_BUTTON_LABEL, "Continue" },

                { LocalizationKey.TP_ROOM_DURATION_LABEL, "{0}m" },
                { LocalizationKey.TP_PROMOTION_TITLE_LABEL, "Trophy Won!" },
                { LocalizationKey.TP_TROPHY_COUNT_TITLE_LABEL, "Country Trophies" },
                { LocalizationKey.TP_CONTINUE_BUTTON_LABEL, "Continue" },

                { LocalizationKey.RTP_ROOM_DURATION_LABEL, "{0}m" },
                { LocalizationKey.RTP_PROMOTION_TITLE_LABEL, "Country Title Achived!" },
                { LocalizationKey.RTP_REWARD_TITLE_LABEL, "Reward" },
                { LocalizationKey.RTP_CONTINUE_BUTTON_LABEL, "Continue" },

                #endregion

                #region IapModal

                { LocalizationKey.IAP_ATTEMPT_LABEL, "Purchasing... Please wait." },
                { LocalizationKey.IAP_FAILURE_LABEL, "Purchase failed." },

                #endregion

                #region Game

                { LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN, "VICTORY" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE, "DEFEAT" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW, "DRAW" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE, "By Checkmate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE, "By Stalemate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL, "By Insufficient Material" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE, "By Fifty Move Rule" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE, "By Threefold Repeat Rule" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED, "By Time" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED, "Your Opponent Left" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION, "By Resignation" },
                { LocalizationKey.GM_DRAW_DIALOG_HEADING, "CLAIM DRAW" },
                { LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE, "Claim draw by Fifty Move rule?" },
                { LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE, "Claim draw by Threefold Repeat rule?" },
                { LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON, "YES" },
                { LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON, "NO" },
                { LocalizationKey.GM_ROOM_PRIZE, "Win\n{0}" },
                { LocalizationKey.GM_ROOM_DURATION, "{0}m" },
                { LocalizationKey.GM_PLAYER_LEVEL, "Level {0}" },
                { LocalizationKey.GM_WAITING_FOR_OPPONENT, "Waiting for opponent..." },

                #endregion

                //
                // Client-server key-values
                //

                #region RoomIds

                { LocalizationKey.BULLET, "Bullet" },
                { LocalizationKey.BLITZ, "Blitz" },
                { LocalizationKey.CLASSIC, "Classic" },

                { LocalizationKey.ALLINONE, "All In One" },

                { LocalizationKey.CUBA, "Cuba" },
                { LocalizationKey.ENGLAND, "England" },
                { LocalizationKey.INDIA, "India" },
                { LocalizationKey.GERMANY, "Germany" },
                { LocalizationKey.SPAIN, "Spain" },
                { LocalizationKey.FRANCE, "France" },
                { LocalizationKey.HUNGARY, "Hungary" },
                { LocalizationKey.UKRAINE, "Ukraine" },
                { LocalizationKey.CHINA, "China" },
                { LocalizationKey.USA, "USA" },
                { LocalizationKey.RUSSIA, "Russia" },

                #endregion

                #region RoomTitles

                // TODO: Create localization keys for room titles.
                { LocalizationKey.ROOM_TITLE_NONE, "Rookie" },
                { LocalizationKey.ROOM_TITLE_1, "Club Champion" },
                { LocalizationKey.ROOM_TITLE_2, "Pro Champion" },
                { LocalizationKey.ROOM_TITLE_3, "National Champion" },

                #endregion

                #region LeagueIds

                // TODO(mubeeniqbal): We need to fix this because at some places
                // we might need this all in upper case, lower case at other
                // times and so on.
                { LocalizationKey.LEAGUE_1, "Iron League" },
                { LocalizationKey.LEAGUE_2, "Bronze League" },
                { LocalizationKey.LEAGUE_3, "Silver League" },
                { LocalizationKey.LEAGUE_4, "Gold League" },
                { LocalizationKey.LEAGUE_5, "Crystal League" },
                { LocalizationKey.LEAGUE_6, "Onyx League" },
                { LocalizationKey.LEAGUE_7, "Emerald League" },
                { LocalizationKey.LEAGUE_8, "Champion League" },
                { LocalizationKey.LEAGUE_9, "Royal League" },
                { LocalizationKey.LEAGUE_10, "Legend League" },

                #endregion

                #region Currency1ShopItemIds

                { LocalizationKey.CURRENCY_1_1, "Pico Coins" },
                { LocalizationKey.CURRENCY_1_2, "Nano Coins" },
                { LocalizationKey.CURRENCY_1_3, "Micro Coins" },
                { LocalizationKey.CURRENCY_1_4, "Kilo Coins" },
                { LocalizationKey.CURRENCY_1_5, "Mega Coins" },
                { LocalizationKey.CURRENCY_1_6, "Giga Coins" },

                #endregion

                #region Currency2ShopItemIds

                { LocalizationKey.CURRENCY_2_1, "Pico Bucks" },
                { LocalizationKey.CURRENCY_2_2, "Nano Bucks" },
                { LocalizationKey.CURRENCY_2_3, "Micro Bucks" },
                { LocalizationKey.CURRENCY_2_4, "Kilo Bucks" },
                { LocalizationKey.CURRENCY_2_5, "Mega Bucks" },
                { LocalizationKey.CURRENCY_2_6, "Giga Bucks" },

                #endregion

                #region CoachShopItemIds

                { LocalizationKey.COACH_1, "Club Coach" },
                { LocalizationKey.COACH_2, "National Coach" },
                { LocalizationKey.COACH_3, "International Coach" },
                { LocalizationKey.COACH_4, "Grandmaster Coach" },

                #endregion

                #region PromotionIds

                { LocalizationKey.TWO_X, "2X" },
                { LocalizationKey.POPULAR, "Popular" },
                { LocalizationKey.BEST_DEAL, "Best Deal" },

                #endregion

                #region CPUMenu

                { LocalizationKey.CPU_MENU_HEADING, "Play vs CPU" },
                { LocalizationKey.CPU_MENU_STRENGTH, "CPU Strength" },
                { LocalizationKey.CPU_MENU_DURATION, "Time Limit" },
                { LocalizationKey.CPU_MENU_DURATION_NONE, "None" },
                { LocalizationKey.CPU_MENU_PLAYER_COLOR, "Play As" },
                { LocalizationKey.CPU_MENU_PLAY, "PLAY" },
                { LocalizationKey.CPU_MENU_STATS, "STATS" },
                { LocalizationKey.CPU_GAME_CPU_NAME, "CPU" },
                { LocalizationKey.CPU_GAME_CPU_STRENGTH, "Strength {0}" },

                { LocalizationKey.CPU_GAME_RESIGN_BUTTON, "RESIGN" },
                { LocalizationKey.CPU_GAME_UNDO_BUTTON, "UNDO" },
                { LocalizationKey.CPU_GAME_HINT_BUTTON, "GET HINT" },
                { LocalizationKey.CPU_GAME_EXIT_BUTTON, "BACK TO LOBBY*" },
                { LocalizationKey.CPU_GAME_CONTINUE_BUTTON, "CONTINUE" },
                { LocalizationKey.CPU_GAME_EXIT_EXPLAINATION, "*Your progress is saved." },
                { LocalizationKey.CPU_RESULTS_STATS_BUTTON, "VIEW STATS" },
                { LocalizationKey.CPU_RESULTS_EXIT_BUTTON, "BACK TO MENU" },

                #endregion
            };
        }
    }
}
