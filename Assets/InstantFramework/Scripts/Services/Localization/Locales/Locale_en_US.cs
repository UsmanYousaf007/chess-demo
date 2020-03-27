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
                { LocalizationKey.SESSION_TERMINATED, "You have been signed out of your Chess Stars account because your account is signed in on another device." },
                


                #endregion

                #region Lobby

                { LocalizationKey.OKAY_TEXT, "Ok" },
                { LocalizationKey.BACK_TEXT, "BACK" },
                 { LocalizationKey.UPGRADE_TEXT, "Upgrade" },

                #endregion

                #region TopNav

                { LocalizationKey.REMOVE_ADS, "Remove Ads" },
                { LocalizationKey.FREE_NO_ADS_PERIOD, "AD FREE for" },
                { LocalizationKey.FREE_NO_ADS_MINUTES, "minutes" },
                { LocalizationKey.FREE_NO_ADS_HOURS, "hours" },
                { LocalizationKey.FREE_NO_ADS_DAYS, "days" },
                { LocalizationKey.DONE, "Done"},
                { LocalizationKey.SELECT_THEME, "Change Theme"},
                { LocalizationKey.CHOOSE_THEME, "Select Theme"},
                { LocalizationKey.REWARD_UNLOCKED_TITLE, "Congratulations"},
                { LocalizationKey.REWARD_UNLOCKED_SUBTITLE, "Reward Unlocked!"},
                { LocalizationKey.REWARD_THEME, "Chess Theme"},
                { LocalizationKey.REWARD_UNLOCKED_CLAIM, "Claim"},
                { LocalizationKey.AD_SKIPPED_TITLE, "Ad Skipped"},
                { LocalizationKey.AD_SKIPPED_INFO_TEXT, "Please watch Ads without skipping to earn reward points"},

                #endregion

                #region Lobby

                { LocalizationKey.ELO_SCORE, "Rating" },
                { LocalizationKey.FACEBOOK_LOGIN, "Login" },
                { LocalizationKey.PLAYING_LEVEL, "Resume lvl "},

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
                { LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON, "Yes" },
                { LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON, "No" },
                { LocalizationKey.GM_ROOM_DURATION, "{0} m" },
                { LocalizationKey.GM_DISCONNECTED, "Waiting for Opponent..." },
                { LocalizationKey.GM_ADVANTAGE, "Adv" },
                { LocalizationKey.GM_WIFI_WARNING, "Slow Internet" },
                { LocalizationKey.GM_WIFI_RECONNECTING, "Reconnecting..." },
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
                { LocalizationKey.CPU_MENU_PLAY_ONLINE, "Speed Chess 5m" },
                { LocalizationKey.CPU_MENU_PLAY_ONLINE_CLASSIC, "Classic Chess" },
                { LocalizationKey.CPU_MENU_PLAY_FRIENDS, "Friends & World" },
                { LocalizationKey.CPU_MENU_PLAY_CPU, "Play Computer" },
                { LocalizationKey.CPU_MENU_SINGLE_PLAYER_GAME, "Single Player Game" },
                { LocalizationKey.EASY, "Easy" },
                { LocalizationKey.HARD, "Hard" },
                { LocalizationKey.CPU_MENU_THEMES, "THEMES" },
                { LocalizationKey.CPU_GAME_CPU_NAME, "Computer" },
                { LocalizationKey.CPU_GAME_CPU_STRENGTH, "Level" },
                { LocalizationKey.CPU_GAME_PLAYER_NAME, "Player" },
                { LocalizationKey.CLASSIC_MODE_TIME, "Long Play" },

                { LocalizationKey.CPU_GAME_RESIGN_BUTTON, "Resign Game" },
                { LocalizationKey.CPU_GAME_UNDO_BUTTON, "UNDO" },
                { LocalizationKey.CPU_GAME_HINT_BUTTON, "HINT" },
                { LocalizationKey.CPU_GAME_TURN_PLAYER, "YOUR MOVE" },
                { LocalizationKey.CPU_GAME_TURN_OPPONENT, "THEIR MOVE" },
                { LocalizationKey.CPU_GAME_EXIT_DLG_TITLE, "Menu" },
                { LocalizationKey.CPU_GAME_SAVE_AND_EXIT, "Save & Exit" },
                { LocalizationKey.CPU_GAME_CONTINUE_BUTTON, "Resume" },
                { LocalizationKey.CPU_RESULTS_CLOSE_BUTTON, "Back to Game" },
                { LocalizationKey.CPU_RESULTS_STATS_BUTTON, "PROGRESS" },
                { LocalizationKey.CPU_RESULTS_EXIT_BUTTON, "GAMES" },

                #endregion

                #region GameResults

                { LocalizationKey.RESULTS_CLOSE_BUTTON, "View Board" },
                { LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON, "Collect" },
                { LocalizationKey.RESULTS_SKIP_REWARD_BUTTON, "Back to Lobby" },
                { LocalizationKey.RESULTS_EARNED, "Earn Rewards" },
                { LocalizationKey.RESULTS_REWARD, "You Earned" },

                #endregion

                #region Multiplayer

                { LocalizationKey.MULTIPLAYER_WAITING_FOR_OPPONENT, "Waiting for opponent..." },
                { LocalizationKey.MULTIPLAYER_SEARCHING, "Searching..." },
                { LocalizationKey.MULTIPLAYER_FOUND, "Get Ready..." },
                { LocalizationKey.QUICK_MATCH_FAILED, "Try Later" },
                { LocalizationKey.QUICK_MATCH_FAILED_REASON, "Player is already in a 5 min game" },
                { LocalizationKey.QUICK_MATCH_EXPIRED, "Match Expired" },
                { LocalizationKey.QUICK_MATCH_EXPIRED_REASON, "You took too long to respond" },

                #endregion

                #region Stats

                { LocalizationKey.STATS_ONLINE_TITLE, "Online Performance" },
                { LocalizationKey.STATS_ONLINE_WIN_PCT, "Win %" },
                { LocalizationKey.STATS_ONLINE_WON, "Won" },
                { LocalizationKey.STATS_ONLINE_LOST, "Lost" },
                { LocalizationKey.STATS_ONLINE_DRAWN, "Drawn" },
                { LocalizationKey.STATS_ONLINE_TOTAL, "Total" },
                { LocalizationKey.STATS_COMPUTER_TITLE, "Computer Difficulty Beaten" },
                { LocalizationKey.STATS_LEGEND_GOLD, "• Gold star for beaten without move take-back •" },
                { LocalizationKey.STATS_LEGEND_SILVER, "Beat using PowerUps" },
                { LocalizationKey.STATS_TAG, "Tag" },

                #endregion

                #region Store

                { LocalizationKey.STORE_TITLE_BUNDLES, "SPECIALS" },
                { LocalizationKey.STORE_TAB_POWERUPS, "POWERUPS" },
                { LocalizationKey.STORE_TAB_THEMES, "THEMES" },
                { LocalizationKey.STORE_TAB_COINS, "COINS" },

                { LocalizationKey.STORE_POWERUP_TITLE_SAFEMOVE, "UNDO" },
                { LocalizationKey.STORE_POWERUP_TITLE_HINT, "MOVE STRENGTH" },
                { LocalizationKey.STORE_POWERUP_TITLE_HINDSIGHT, "COACH" },

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
                { LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_YES_BUTTON, "Yes" },
                { LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_NO_BUTTON, "No" },

                { LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_TITLE, "Verification Failed" },
                { LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_DESC, "If you had made the valid payment and haven't received the goods then please contact our support team" },
                { LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_YES_BUTTON, "Contact Us" },
                { LocalizationKey.STORE_PURCHASE_FAILED, "Purchase Failed" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_CANCEL, "Cancelled by user" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_DECLINED, "Payment declined" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_DUPLICATE, "Duplicate transaction for this item" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_INVALID, "Signature invalid" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_PENDING, "Existing purchase for this item is pending" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_PRODUCT_UNAVAILABLE, "Item is unavailable for purchase, please try again" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_UNAVAILABLE, "Purchasing is unavailable, check your payment settings" },
                { LocalizationKey.STORE_PURCHASE_FAILED_REASON_UNKNOWN, "Reason unknown" },

                { LocalizationKey.SUBSCRIPTION_DLG_TITLE, "Upgrade To Premium Subscription"},
                { LocalizationKey.SUBSCRIPTION_DLG_DISCLAIMER, "Cancel subscription any time. Subscription automatically renews unless auto-renew is turned off at least 24-hours before the end of the current period by going to your Account Settings after purchase. Payment will be charged to iTunes or Google Play Account at confirmation of purchase. Any unused portion of free trial period, if offered, will be forfeited when you purchase a subscription."},
                { LocalizationKey.SUBSCRIPTION_DLG_FREE_TRIAL, "Start 3 Days Free Trial."},
                { LocalizationKey.SUBSCRIPTION_DLG_PRICE, "Then (price) per month"},
                { LocalizationKey.SUBSCRIPTION_DLG_PURCHASE_BUTTON, "Subscribe"},
                { LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE, "Restore Purchase"},
                { LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY, "Privacy Policy"},
                { LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE, "Terms of Use"},
                

                { LocalizationKey.PROMOTON_DLG_TITLE, "Premium Subscription"},
                { LocalizationKey.PROMOTION_DLG_PURCHASE, "Start 3 Days Free Trial."},
                { LocalizationKey.PROMOTION_DLG_PRICE, "Then (price) per month"},
                { LocalizationKey.PROMOTION_DLG_PURCHASE_BUTTON, "Subscribe Now"},

                #endregion

                #region Friends

				{ LocalizationKey.FRIENDS_SECTION_NEW_MATCHES, "NEW MATCHES" },
                { LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES, "ACTIVE MATCHES" },
                { LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES_EMPTY, "There are no active matches." },
                { LocalizationKey.FRIENDS_SECTION_RECENTLY_COMPLETED_MATCHES, "RECENTLY PLAYED" },

                { LocalizationKey.FRIENDS_SECTION_PLAY_A_FRIEND, "PLAY A FRIEND" },
                { LocalizationKey.FRIENDS_SECTION_PLAY_SOMEONE_NEW, "PLAY SOMEONE NEW" },
                { LocalizationKey.FRIENDS_SECTION_SEARCH_RESULTS, "SEARCH RESULTS" },
                { LocalizationKey.FRIENDS_NO_FRIENDS_TEXT, "Invite Facebook Friends" },
                { LocalizationKey.FRIENDS_INVITE_TEXT, "Invite" },
                { LocalizationKey.FRIENDS_INVITE_BUTTON_TEXT, "Invite" },
                { LocalizationKey.FRIENDS_INVITE_TITLE_TEXT, "Invite a Friend" },
                { LocalizationKey.FRIENDS_REFRESH_TEXT, "Refresh" },
                { LocalizationKey.FRIENDS_CONFIRM_LABEL, "Are You Sure?" },
                { LocalizationKey.FRIENDS_YES_LABEL, "Yes" },
                { LocalizationKey.FRIENDS_NO_LABEL, "No" },
                { LocalizationKey.FRIENDS_VS_LABEL, "You VS Them" },
                { LocalizationKey.FRIENDS_WINS_LABEL, "Wins" },
                { LocalizationKey.FRIENDS_DRAWS_LABEL, "Draws" },
                { LocalizationKey.FRIENDS_TOTAL_GAMES_LABEL, "Total Games: " },
                { LocalizationKey.FRIENDS_BLOCK_LABEL, "Block User" },
                { LocalizationKey.FRIENDS_BLOCK_TEXT, "Block"},
                { LocalizationKey.FRIENDS_CHAT_LABEL, "Chat" },
                { LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT, "Connect with Facebook" },
                { LocalizationKey.FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT, "Login" },
                { LocalizationKey.FACEBBOK_LOGIN_REWARD_TEXT, "Get {0} Coins" },
                { LocalizationKey.FRIENDS_WAITING_FOR_PLAYERS, "Waiting for players ...\n" },
                { LocalizationKey.FRIENDS_FIND_FRIEND_TITLE, "FIND YOUR FRIENDS" },
                { LocalizationKey.FRIENDS_FIND_FRIEND_LOGIN_INFO, "Login to Facebook and friends who have the app will automactically appear" },
                { LocalizationKey.FRIENDS_FIND_FRIEND_SEARCH_INFO, "Use the search bar to find your friends by name" },
                { LocalizationKey.FRIENDS_FIND_FRIEND_INVITE_INFO, "Use 'Invite' to send an invitation to your friend on your favorite app (Whatsapp, Facebook, etc)" },
                { LocalizationKey.FRIENDS_ADD_TO_FRIENDS, "Add to Friends" },
                { LocalizationKey.FRIENDS_REMOVE_FROM_FRIENDS, "Remove from Friends" },
                { LocalizationKey.FRIENDS_TEXT_FRIENDED, "Friend" },
                #endregion

                #region Share

                { LocalizationKey.SHARE_STANDARD, "Hey, let's play Chess!" },

                #endregion

                #region Bottom Nav

                { LocalizationKey.NAV_HOME, "Games" },
                { LocalizationKey.NAV_PROFILE, "Profile" },
                { LocalizationKey.NAV_SHOP, "Shop" },
                { LocalizationKey.NAV_FRIENDS, "Friends" },

                #endregion

                #region Long Play

                { LocalizationKey.IN_GAME_BACK, "Games" },
                { LocalizationKey.LONG_PLAY_BACK_TO_GAME, "BACK" },
                { LocalizationKey.LONG_PLAY_RESULTS_BACK, "BACK TO GAMES" },
                { LocalizationKey.BOT_NAV_NEXT, "Next" },
                { LocalizationKey.BOT_NAV_COMPANY, "Chess by Turbo Labz" },
                { LocalizationKey.LONG_PLAY_NOT_NOW, "Not Now" },
                { LocalizationKey.LONG_PLAY_RANKED, "Ranked" },
                { LocalizationKey.LONG_PLAY_FRIENDLY, "Friendly" },
                { LocalizationKey.PLAY, "Play" },
                { LocalizationKey.VIEW, "View" },
                { LocalizationKey.LONG_PLAY_ACCEPT, "Accept" },
                { LocalizationKey.LONG_PLAY_CANCEL, "Cancel" },
                { LocalizationKey.LONG_PLAY_NEW_MATCH_GREETING, "Hey, let's play :)" },
                { LocalizationKey.LONG_PLAY_DECLINE_APOLOGY, "Sorry, not now" },
                { LocalizationKey.LONG_PLAY_OK, "Ok" },
                { LocalizationKey.LONG_PLAY_WAITING, "Waiting" },
                { LocalizationKey.LONG_PLAY_MINUTES, "{0}m" },
                { LocalizationKey.LONG_PLAY_HOURS, "{0}h" },
                { LocalizationKey.LONG_PLAY_DAYS, "{0}d" },
                { LocalizationKey.LONG_PLAY_CHALLENGED_YOU, "Challenged You" },
                { LocalizationKey.LONG_PLAY_MATCH_PROGRESS, "Match in Progress" },
                { LocalizationKey.LONG_PLAY_YOUR_TURN, "Your Move" },
                { LocalizationKey.LONG_PLAY_THEIR_TURN, "Their Move" },
                { LocalizationKey.LONG_PLAY_WAITING_FOR_ACCEPT, "Waiting For Accept" },
                { LocalizationKey.LONG_PLAY_DECLINED, "Declined" },
                { LocalizationKey.LONG_PLAY_YOU_LOST, "You Lost" },
                { LocalizationKey.LONG_PLAY_YOU_WON, "You Won" },
                { LocalizationKey.LONG_PLAY_CANCELED, "Canceled" },
                { LocalizationKey.LONG_PLAY_DRAW, "Draw" },
                { LocalizationKey.LONG_PLAY_ACCEPT_TITLE, "Accept Challenge?" },
                { LocalizationKey.LONG_PLAY_ACCEPT_YES, "Accept" },
                { LocalizationKey.LONG_PLAY_ACCEPT_NO, "Decline" },
                { LocalizationKey.CHAT_TODAY, "TODAY" },
                { LocalizationKey.CHAT_YESTERDAY, "YESTERDAY" },
                { LocalizationKey.CHAT_CLEAR, "Clear" },
                { LocalizationKey.CHAT_DEFAULT_DAY_LINE, "TODAY" },
                { LocalizationKey.CHAT_DEFAULT_SYSTEM_MESSAGE, "Start a new conversation. Say hello." },
                { LocalizationKey.CHAT_DISABLED_SYSTEM_MESSAGE, "To enable chat you must start at least one match with this player." },
                { LocalizationKey.NEW_GAME_CONFIRM_FRIENDLY, "5 Min Game"},
                { LocalizationKey.NEW_GAME_CONFIRM_RANKED, "Play Classic"},
                { LocalizationKey.NEW_GAME_CONFIRM_TITLE, "START A CLASSIC GAME"},
                { LocalizationKey.FRIENDLY_GAME_CAPTION, "Friendly"},
                { LocalizationKey.REMOVE_COMMUNITY_FRIEND_NO, "No"},
                { LocalizationKey.REMOVE_COMMUNITY_FRIEND_YES, "Yes"},
                { LocalizationKey.REMOVE_COMMUNITY_FRIEND_TITLE, "Remove player "},
                { LocalizationKey.LONG_PLAY_VIEW, "View"},
                { LocalizationKey.SHARE_GAME_SCREENSHOT, "Share Game Screenshot"},
                { LocalizationKey.SHARE, "Share"},

                #endregion


                #region Rate App

                { LocalizationKey.RATE_APP_TITLE, "Enjoying" },
                { LocalizationKey.RATE_APP_SUB_TITLE_RATE, "Please take a few seconds to rate us on the store. It really helps." },
                { LocalizationKey.RATE_APP_SUB_TITLE_TELL, "Please tell us how we can improve the game for you." },
                { LocalizationKey.RATE_APP_RATE, "RATE NOW" },
                { LocalizationKey.RATE_APP_TELL, "TELL US" },
                { LocalizationKey.RATE_APP_NOT_NOW, "Maybe Later" },
                { LocalizationKey.RATE_APP_IMPROVE, "No" },
                { LocalizationKey.RATE_APP_LIKE, "Yes" },
                { LocalizationKey.RATE_APP_LOVE, "Yes" },
                { LocalizationKey.RATE_APP_LOVE_FROM_TEAM, "Your feedback is valuable. Thank you." },

                #endregion

                #region Settings

                { LocalizationKey.SETTINGS_TITLE, "Settings" },
                { LocalizationKey.SETTINGS_SOUND_TITLE, "SOUND" },
                { LocalizationKey.SETTINGS_SOUND_EFFECT, "Sound Effects" },
                { LocalizationKey.SETTINGS_ACCOUNT_TITLE, "ACCOUNT" },
                { LocalizationKey.SETTINGS_ACCOUNT_UPGRADE_TO_PREMIUM, "Upgrade to Premium" },
                { LocalizationKey.SETTINGS_ACCOUNT_PERSONALISED_ADS, "Personalised Ads" },
                { LocalizationKey.SETTINGS_ACCOUNT_MANAGE_SUBSCRIPTION, "Manage Subscription" },
                { LocalizationKey.SETTINGS_ACCOUNT_INFO, "Try for free" },
                { LocalizationKey.SETTINGS_ACCOUNT_RENEW, "Renews on (date)" },
                { LocalizationKey.SETTINGS_ON, "On" },
                { LocalizationKey.SETTINGS_OFF, "Off" },

                #endregion

                #region Earn Rewards

                { LocalizationKey.EARN_REWARDS_TITLE, "Earn Rewards" },
                { LocalizationKey.EARN_REWARDS_INFO_TEXT, "Collect reward points on match result card" },

                #endregion

                #region ManageSubscription

                { LocalizationKey.SUB_MANAGE, "Manage Your Subscription"},
                { LocalizationKey.SUB_OPTIONS, "For other options, go to"},
                { LocalizationKey.SUB_POPULAR, "Our most popular plan"},
                { LocalizationKey.SUB_BENEFITS, "Same Benefits as above"},
                { LocalizationKey.SUB_MONTHLY, "You are subscribed to the\n<b>Monthly</b> Plan"},
                { LocalizationKey.SUB_ANNUAL, "You are subscribed to the\n<b>Yearly</b> Plan"},
                { LocalizationKey.SUB_SWITCH_MONTHLY, "Switch to the <b>Monthly</b> Plan"},
                { LocalizationKey.SUB_SWITCH_ANNUAL, "Switch to the <b>Yearly</b> Plan"},
                { LocalizationKey.SUB_SWITCH_MONTHLY_BTN, "Switch to Monthly"},
                { LocalizationKey.SUB_SWITCH_ANNUAL_BTN, "Switch to Yearly"},

                #endregion
            };
        }
    }
}
