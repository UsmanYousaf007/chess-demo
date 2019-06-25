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

namespace TurboLabz.InstantFramework
{
    public class Locale_en_US : ILocale
    {
        public IDictionary<string, string> data { get; private set; }

        public Locale_en_US()
        {
            data = new Dictionary<string, string>() {
                #region System

                { LocalizationKey.SPLASH_CONNECTING, "CONNECTING" },
                { LocalizationKey.HARD_STOP, "Oops! There was a problem.\n" +
                                             "Please restart the game." },
                { LocalizationKey.RECONNECTING, "Reconnecting..." },
                { LocalizationKey.UPDATE, "Update Required...\n\n" +
                    "Please download the latest version." },
                { LocalizationKey.UPDATE_BUTTON, "UPDATE" },
                { LocalizationKey.CHECK_INTERNET_CONNECTION, "Please check your internet connection." },


                #endregion

                #region TopNav

                { LocalizationKey.REMOVE_ADS, "Remove Ads" },
                { LocalizationKey.FREE_NO_ADS_PERIOD, "AD FREE for" },
                { LocalizationKey.FREE_NO_ADS_MINUTES, "minutes" },
                { LocalizationKey.FREE_NO_ADS_HOURS, "hours" },
                { LocalizationKey.FREE_NO_ADS_DAYS, "days" },

                #endregion

                #region Lobby

                { LocalizationKey.ELO_SCORE, "Rating" },
                { LocalizationKey.FACEBOOK_LOGIN, "Login" },
                { LocalizationKey.PLAYING_LEVEL, "Playing Level "},

                #endregion

                #region Game

                { LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN, "VICTORY" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE, "DEFEAT" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW, "DRAW" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED, "DECLINED" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE, "By Checkmate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE, "By Stalemate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL, "By Insufficient Material" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE, "By Fifty Move Rule" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE, "By Threefold Repeat Rule" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED, "By Time" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED, "By Disconnection" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_OPPONENT_LEFT, "Opponent Left" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED, "Player Busy" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER, "By Resignation" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT, "Opponent Resigned" },
                { LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE, "Claim Fifty Move Draw?" },
                { LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE, "Claim Threefold Repeat Draw?" },
                { LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON, "YES" },
                { LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON, "NO" },
                { LocalizationKey.GM_ROOM_DURATION, "{0} m" },
                { LocalizationKey.GM_DISCONNECTED, "Waiting for Opponent..." },
                { LocalizationKey.GM_ADVANTAGE, "Adv." },
                { LocalizationKey.GM_WIFI_WARNING, "Slow Internet" },
                { LocalizationKey.GM_EXIT_BUTTON_LOBBY, "LOBBY" },
                { LocalizationKey.GM_EXIT_BUTTON_COLLECT_REWARD, "COLLECT REWARD" },

                #endregion

                //
                // Client-server key-values
                //

                #region CPUMenu

                { LocalizationKey.CPU_MENU_STRENGTH, "Computer\nDifficulty" },
                { LocalizationKey.CPU_MENU_IN_PROGRESS, "Resume" },
                { LocalizationKey.CPU_MENU_DURATION, "Time Limit" },
                { LocalizationKey.CPU_MENU_DURATION_NONE, "None" },
                { LocalizationKey.CPU_MENU_PLAYER_COLOR, "Play As" },
				{ LocalizationKey.CPU_MENU_THEME, "Theme" },
                { LocalizationKey.CPU_MENU_PLAY_ONLINE, "Quick Match" },
                { LocalizationKey.CPU_MENU_PLAY_FRIENDS, "Friends & World" },
                { LocalizationKey.CPU_MENU_PLAY_CPU, "Play Computer" },
                { LocalizationKey.CPU_MENU_SINGLE_PLAYER_GAME, "Single Player Game" },
                { LocalizationKey.CPU_MENU_THEMES, "THEMES" },
                { LocalizationKey.CPU_GAME_CPU_NAME, "Computer" },
                { LocalizationKey.CPU_GAME_CPU_STRENGTH, "Difficulty" },
                { LocalizationKey.CPU_GAME_PLAYER_NAME, "Player" },

                { LocalizationKey.CPU_GAME_RESIGN_BUTTON, "RESIGN GAME" },
                { LocalizationKey.CPU_GAME_UNDO_BUTTON, "UNDO" },
                { LocalizationKey.CPU_GAME_HINT_BUTTON, "HINT" },
                { LocalizationKey.CPU_GAME_TURN_PLAYER, "YOUR MOVE" },
                { LocalizationKey.CPU_GAME_TURN_OPPONENT, "THEIR MOVE" },
                { LocalizationKey.CPU_GAME_EXIT_DLG_TITLE, "Menu" },
                { LocalizationKey.CPU_GAME_SAVE_AND_EXIT, "SAVE & EXIT" },
                { LocalizationKey.CPU_GAME_CONTINUE_BUTTON, "RESUME" },
                { LocalizationKey.CPU_RESULTS_CLOSE_BUTTON, "BACK TO GAME" },
				{ LocalizationKey.CPU_RESULTS_STATS_BUTTON, "PROGRESS" },
                { LocalizationKey.CPU_RESULTS_EXIT_BUTTON, "HOME" },

                #endregion

                #region GameResults

                { LocalizationKey.RESULTS_CLOSE_BUTTON, "View Board" },
                { LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON, "2X COINS" },
                { LocalizationKey.RESULTS_SKIP_REWARD_BUTTON, "COLLECT" },
                { LocalizationKey.RESULTS_EARNED, "You Earned" },
                { LocalizationKey.RESULTS_REWARD, "You Earned" },

                #endregion

                #region Multiplayer

                { LocalizationKey.MULTIPLAYER_SEARCHING, "Searching..." },
                { LocalizationKey.MULTIPLAYER_FOUND, "Get Ready..." },

                #endregion

                #region Stats

                { LocalizationKey.STATS_ONLINE_TITLE, "Online Performance" },
                { LocalizationKey.STATS_ONLINE_WIN_PCT, "Win %" },
                { LocalizationKey.STATS_ONLINE_WON, "Won" },
                { LocalizationKey.STATS_ONLINE_LOST, "Lost" },
                { LocalizationKey.STATS_ONLINE_DRAWN, "Drawn" },
                { LocalizationKey.STATS_ONLINE_TOTAL, "Total" },
                { LocalizationKey.STATS_COMPUTER_TITLE, "Computer Difficulty Beaten" },
                { LocalizationKey.STATS_LEGEND_GOLD, "Beat without PowerUps" },
                { LocalizationKey.STATS_LEGEND_SILVER, "Beat using PowerUps" },

                #endregion

                #region Store

                { LocalizationKey.STORE_TITLE_BUNDLES, "SPECIALS" },
                { LocalizationKey.STORE_TAB_POWERUPS, "POWERUPS" },
                { LocalizationKey.STORE_TAB_THEMES, "THEMES" },
                { LocalizationKey.STORE_TAB_COINS, "COINS" },

                { LocalizationKey.STORE_POWERUP_TITLE_SAFEMOVE, "UNDO" },
                { LocalizationKey.STORE_POWERUP_TITLE_HINT, "HINT" },
                { LocalizationKey.STORE_POWERUP_TITLE_HINDSIGHT, "HINDSIGHT" },

                { LocalizationKey.STORE_BUNDLE_FIELD_OWNED, "OWNED" },
                { LocalizationKey.STORE_BUNDLE_FIELD_REMAINING, "Remaining" },
                { LocalizationKey.STORE_BUNDLE_FIELD_DAYS, "Days" },

                { LocalizationKey.STORE_CONFIRM_DLG_TITLE_BUY, "Buy" },
                { LocalizationKey.STORE_NOT_AVAILABLE, "Try Later" },


                { LocalizationKey.CPU_STORE_HEADING, "THEMES" },
				{ LocalizationKey.CPU_STORE_OWNED, "OWNED" },
				{ LocalizationKey.CPU_STORE_BUCKS, "Coins" },

				{ LocalizationKey.CPU_STORE_BUY_THEME_TITLE, "Buy Theme" },
				{ LocalizationKey.CPU_STORE_BUY_BUY_BUTTON, "BUY" },

				{ LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_TITLE, "Not Enough Coins" },
				{ LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING, "Would you like to buy more coins?" },
				{ LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_BUY, "BUY" },
				{ LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_YES_BUTTON, "YES" },
                { LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_NO_BUTTON, "NO" },

				#endregion

                #region Friends

				{ LocalizationKey.FRIENDS_SECTION_NEW_MATCHES, "NEW MATCHES" },
                { LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES, "ACTIVE MATCHES" },
                { LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES_EMPTY, "There are no active matches." },
                { LocalizationKey.FRIENDS_SECTION_PLAY_A_FRIEND, "PLAY A FRIEND" },
                { LocalizationKey.FRIENDS_SECTION_PLAY_SOMEONE_NEW, "PLAY SOMEONE NEW" },
                { LocalizationKey.FRIENDS_SECTION_SEARCH_RESULTS, "SEARCH RESULTS" },
                { LocalizationKey.FRIENDS_NO_FRIENDS_TEXT, "Invite Facebook Friends" },
				{ LocalizationKey.FRIENDS_INVITE_TEXT, "Invite" },
				{ LocalizationKey.FRIENDS_REFRESH_TEXT, "Refresh" },
				{ LocalizationKey.FRIENDS_CONFIRM_LABEL, "Are You Sure?" },
				{ LocalizationKey.FRIENDS_YES_LABEL, "YES" },
				{ LocalizationKey.FRIENDS_NO_LABEL, "NO" },
				{ LocalizationKey.FRIENDS_VS_LABEL, "You VS Them" },
				{ LocalizationKey.FRIENDS_WINS_LABEL, "Wins" },
				{ LocalizationKey.FRIENDS_DRAWS_LABEL, "Draws" },
				{ LocalizationKey.FRIENDS_TOTAL_GAMES_LABEL, "Total Games: " },
				{ LocalizationKey.FRIENDS_BLOCK_LABEL, "Block User" },
                { LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT, "Play with your Friends" },
                { LocalizationKey.FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT, "Login" },
                { LocalizationKey.SAVE_YOUR_PROGRESS_TEXT, "Save your progress" },
                { LocalizationKey.FRIENDS_WAITING_FOR_PLAYERS, "Waiting for players ...\n" },

                #endregion

                #region Share

                { LocalizationKey.SHARE_STANDARD, "Hey, let's play Chess!" },

                #endregion

                #region Bottom Nav

                { LocalizationKey.NAV_HOME, "Home" },
                { LocalizationKey.NAV_PROFILE, "Profile" },
                { LocalizationKey.NAV_SHOP, "Shop" },
                { LocalizationKey.NAV_FRIENDS, "Friends" },

                #endregion

                #region Long Play

                { LocalizationKey.IN_GAME_BACK, "BACK" },
                { LocalizationKey.LONG_PLAY_BACK_TO_GAME, "BACK" },
                { LocalizationKey.LONG_PLAY_RESULTS_BACK, "BACK TO FRIENDS" },
                { LocalizationKey.BOT_NAV_NEXT, "Next" },
                { LocalizationKey.BOT_NAV_COMPANY, "Chess by Turbo Labz" },
                { LocalizationKey.LONG_PLAY_NOT_NOW, "Not Now" },
                { LocalizationKey.LONG_PLAY_ACCEPT, "Accept" },
                { LocalizationKey.LONG_PLAY_CANCEL, "Cancel" },
                { LocalizationKey.LONG_PLAY_NEW_MATCH_GREETING, "Hey, let's play :)" },
                { LocalizationKey.LONG_PLAY_DECLINE_APOLOGY, "Sorry, not now" },
                { LocalizationKey.LONG_PLAY_OK, "OK" },
                { LocalizationKey.LONG_PLAY_WAITING, "Waiting" },
                { LocalizationKey.LONG_PLAY_MINUTES, "{0}m" },
                { LocalizationKey.LONG_PLAY_HOURS, "{0}h" },
                { LocalizationKey.LONG_PLAY_DAYS, "{0}d" },
                { LocalizationKey.LONG_PLAY_CHALLENGED_YOU, "Challenged You" },
                { LocalizationKey.LONG_PLAY_YOUR_TURN, "Your Move" },
                { LocalizationKey.LONG_PLAY_THEIR_TURN, "Their Move" },
                { LocalizationKey.LONG_PLAY_WAITING_FOR_ACCEPT, "Waiting For Accept" },
                { LocalizationKey.LONG_PLAY_DECLINED, "Declined" },
                { LocalizationKey.LONG_PLAY_YOU_LOST, "You Lost" },
                { LocalizationKey.LONG_PLAY_YOU_WON, "You Won" },
                { LocalizationKey.LONG_PLAY_CANCELED, "Canceled" },
                { LocalizationKey.LONG_PLAY_DRAW, "Draw" },
                { LocalizationKey.LONG_PLAY_ACCEPT_TITLE, "Accept Challenge?" },
                { LocalizationKey.LONG_PLAY_ACCEPT_YES, "ACCEPT" },
                { LocalizationKey.LONG_PLAY_ACCEPT_NO, "DECLINE" },
                { LocalizationKey.CHAT_TODAY, "TODAY" },
                { LocalizationKey.CHAT_YESTERDAY, "YESTERDAY" },
                { LocalizationKey.CHAT_CLEAR, "CLEAR" },
                { LocalizationKey.CHAT_DEFAULT_DAY_LINE, "TODAY" },
                { LocalizationKey.CHAT_DEFAULT_SYSTEM_MESSAGE, "Start a new conversation. Say hello." },
                { LocalizationKey.NEW_GAME_CONFIRM_FRIENDLY, "PLAY FRIENDLY"},
                { LocalizationKey.NEW_GAME_CONFIRM_RANKED, "PLAY RANKED"},
                { LocalizationKey.NEW_GAME_CONFIRM_TITLE, "START A NEW GAME"},
                { LocalizationKey.FRIENDLY_GAME_CAPTION, "Friendly Game"},
                { LocalizationKey.REMOVE_COMMUNITY_FRIEND_NO, "NO"},
                { LocalizationKey.REMOVE_COMMUNITY_FRIEND_YES, "YES"},
                { LocalizationKey.REMOVE_COMMUNITY_FRIEND_TITLE, "Remove Player?"},
                { LocalizationKey.LONG_PLAY_VIEW, "View"},

                #endregion


                #region Rate App

                { LocalizationKey.RATE_APP_TITLE, "Enjoying" },
                { LocalizationKey.RATE_APP_SUB_TITLE_RATE, "Please take a few seconds to rate us on the store. It really helps." },
                { LocalizationKey.RATE_APP_SUB_TITLE_TELL, "Please tell us how we can improve the game for you." },
                { LocalizationKey.RATE_APP_RATE, "RATE NOW" },
                { LocalizationKey.RATE_APP_TELL, "TELL US" },
                { LocalizationKey.RATE_APP_NOT_NOW, "May be Later" },
                { LocalizationKey.RATE_APP_IMPROVE, "IMPROVE IT" },
                { LocalizationKey.RATE_APP_LIKE, "I LIKE IT" },
                { LocalizationKey.RATE_APP_LOVE, "I LOVE IT" },
                { LocalizationKey.RATE_APP_LOVE_FROM_TEAM, "Love from Chess Star Team" },

                #endregion
            };
        }
    }
}
