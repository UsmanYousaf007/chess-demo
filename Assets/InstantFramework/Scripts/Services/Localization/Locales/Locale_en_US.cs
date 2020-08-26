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
                { LocalizationKey.MIN1_GAME_TEXT, "1 Min" },
                { LocalizationKey.MIN5_GAME_TEXT, "5 Min" },
                { LocalizationKey.MIN10_GAME_TEXT, "10 Min" },
                { LocalizationKey.MIN30_GAME_TEXT, "30 Min" },
                { LocalizationKey.ON_TEXT, "On" },
                { LocalizationKey.OFF_TEXT, "Off" },
                { LocalizationKey.ACCEPT_TEXT, "Accept" },
                { LocalizationKey.DECLINE_TEXT, "Decline" },
                { LocalizationKey.START_TEXT, "Start" },

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
                { LocalizationKey.SIGN_IN, "Sign in with Apple" },
                { LocalizationKey.PLAYING_LEVEL, "Resume lvl "},

                #endregion

                #region Game

                { LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN, "VICTORY" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE, "DEFEAT" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW, "DRAW" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED, "DECLINED" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE, "By Checkmate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE, "By Stalemate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_OFFERED_DRAW, "By Agreement" },
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
                { LocalizationKey.GM_SPECIAL_HINT_NOT_AVAILABLE, "hints allowed per game"},

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
                { LocalizationKey.CPU_MENU_PLAY_ONLINE, "Fast" },
                { LocalizationKey.CPU_MENU_PLAY_ONLINE_CLASSIC, "Classic" },
                { LocalizationKey.CPU_MENU_PLAY_ONLINE_DESCRIPTION, "Random Player" },
                { LocalizationKey.CPU_MENU_PLAY_ONLINE_DESCRIPTION_CLASSIC30, "Random Player 30 Min" },
                { LocalizationKey.CPU_MENU_PLAY_FRIENDS, "Friends & World" },
                { LocalizationKey.CPU_MENU_PLAY_CPU, "Computer" },
                { LocalizationKey.CPU_MENU_SINGLE_PLAYER_GAME, "Single Player" },
                { LocalizationKey.EASY, "Easy" },
                { LocalizationKey.HARD, "Hard" },
                { LocalizationKey.CPU_MENU_THEMES, "THEMES" },
                { LocalizationKey.CPU_GAME_CPU_NAME, "Computer" },
                { LocalizationKey.CPU_GAME_CPU_STRENGTH, "Level" },
                { LocalizationKey.CPU_GAME_PLAYER_NAME, "Player" },
                { LocalizationKey.CLASSIC_MODE_TIME, "Long Play" },

                { LocalizationKey.CPU_GAME_RESIGN_BUTTON, "RESIGN GAME" },
                { LocalizationKey.CPU_GAME_UNDO_BUTTON, "UNDO" },
                { LocalizationKey.CPU_GAME_HINT_BUTTON, "HINT" },
                { LocalizationKey.CPU_GAME_TURN_PLAYER, "YOUR MOVE" },
                { LocalizationKey.CPU_GAME_TURN_OPPONENT, "THEIR MOVE" },
                { LocalizationKey.CPU_GAME_EXIT_DLG_TITLE, "Menu" },
                { LocalizationKey.CPU_GAME_SAVE_AND_EXIT, "Save & Exit" },
                { LocalizationKey.CPU_GAME_SAVE_AND_EXIT_CAP, "SAVE & EXIT" },
                { LocalizationKey.CPU_GAME_CONTINUE_BUTTON, "RESUME" },
                { LocalizationKey.CPU_GAME_OFFER_DRAW_BUTTON, "OFFER DRAW" },
                { LocalizationKey.CPU_RESULTS_CLOSE_BUTTON, "Back to Game" },
                { LocalizationKey.CPU_RESULTS_STATS_BUTTON, "PROGRESS" },
                { LocalizationKey.CPU_RESULTS_EXIT_BUTTON, "GAMES" },

                #endregion

                #region Lessons

                { LocalizationKey.LESSONS_TITLE, "Lessons" },
                { LocalizationKey.LESSONS_DESCRIPTION, "Learn to Play" },
                { LocalizationKey.LESSONS_START, "Start Lesson"},
                { LocalizationKey.LESSONS_COMPLETED_TITLE, "Great Work!"},
                { LocalizationKey.LESSONS_COMPLETED_DESCRIPTION, "You have completed all of the available lessons.\nStay tuned for more advanced lessons coming soon."},

                #endregion

                #region GameResults

                { LocalizationKey.RESULTS_CLOSE_BUTTON, "View Board" },
                { LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON, "Collect" },
                { LocalizationKey.RESULTS_RECOVER_RATING_BUTTON, "Recover Rating" },
                { LocalizationKey.RESULTS_BOOST_RATING_BUTTON, "Boost Rating" },
                { LocalizationKey.RESULTS_SKIP_REWARD_BUTTON, "Back to Lobby" },
                { LocalizationKey.RESULTS_EARNED, "Earn Rewards" },
                { LocalizationKey.RESULTS_REWARD, "You Earned" },
                { LocalizationKey.RESULTS_BOOSTED, "Boosted!"},
                { LocalizationKey.RESULTS_BOOST_DRAW, "Rating boosts are available for wins and losses only"},
                { LocalizationKey.RESULTS_BOOST_FRIENDLY, "Rating boosts are available for ranked games only"},

                #endregion

                #region Multiplayer

                { LocalizationKey.MULTIPLAYER_WAITING_FOR_OPPONENT, "Waiting for opponent..." },
                { LocalizationKey.MULTIPLAYER_SEARCHING, "Searching..." },
                { LocalizationKey.MULTIPLAYER_FOUND, "Get Ready..." },
                { LocalizationKey.QUICK_MATCH_FAILED, "Try Later" },
                { LocalizationKey.QUICK_MATCH_FAILED_REASON, "Player is already in a game" },
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
                { LocalizationKey.STATS_TAKE_PHOTO, "Take Photo" },
                { LocalizationKey.STATS_CHOOSE_PHOTO, "Choose Existing" },
                { LocalizationKey.STATS_PHOTO_TITLE, "Change Photo" },

                { LocalizationKey.STATS_OPEN_SETTINGS_TITLE, "Permissions" },
                { LocalizationKey.STATS_OPEN_SETTINGS_SUBTITLE, "This option requires photo library access to set your profile picture" },
                { LocalizationKey.STATS_OPEN_SETTINGS, "Settings" },
                #endregion

                #region Store

                { LocalizationKey.STORE_TITLE_BUNDLES, "SPECIALS" },
                { LocalizationKey.STORE_TAB_POWERUPS, "POWERUPS" },
                { LocalizationKey.STORE_TAB_THEMES, "THEMES" },
                { LocalizationKey.STORE_TAB_COINS, "COINS" },

                { LocalizationKey.STORE_POWERUP_TITLE_SAFEMOVE, "UNDO" },
                { LocalizationKey.STORE_POWERUP_TITLE_HINT, "MOVE STRENGTH" },
                { LocalizationKey.STORE_POWERUP_TITLE_HINDSIGHT, "COACH" },

                { LocalizationKey.STORE_BUNDLE_FIELD_OWNED, "Owned" },
                { LocalizationKey.STORE_BUNDLE_FIELD_REMAINING, "Remaining" },
                { LocalizationKey.STORE_BUNDLE_FIELD_DAYS, "Days" },

                { LocalizationKey.STORE_CONFIRM_DLG_TITLE_BUY, "Buy" },
                { LocalizationKey.STORE_NOT_AVAILABLE, "Try Later" },
                { LocalizationKey.STORE_GETTING_PACKAGE, "Getting Package" },


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

                { LocalizationKey.SUBSCRIPTION_DLG_TITLE, "Upgrade To Premium!"},
                { LocalizationKey.SUBSCRIPTION_DLG_DISCLAIMER, "Cancel subscription any time. Subscription automatically renews unless auto-renew is turned off at least 24-hours before the end of the current period by going to your Account Settings after purchase. Payment will be charged to iTunes or Google Play Account at confirmation of purchase. Any unused portion of free trial period, if offered, will be forfeited when you purchase a subscription."},
                { LocalizationKey.SUBSCRIPTION_DLG_FREE_TRIAL, "Start 3 Days Free Trial."},
                { LocalizationKey.SUBSCRIPTION_DLG_PRICE, "Then (price) per month"},
                { LocalizationKey.SUBSCRIPTION_DLG_PURCHASE_BUTTON, "Continue"},
                { LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE, "Restore Purchase"},
                { LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY, "Privacy Policy"},
                { LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE, "Terms of Use"},
                
                { LocalizationKey.PROMOTON_DLG_TITLE, "Premium Subscription"},
                { LocalizationKey.PROMOTION_DLG_PURCHASE, "Start 3 Days Free Trial."},
                { LocalizationKey.PROMOTION_DLG_PRICE, "Then (price) per month"},
                { LocalizationKey.PROMOTION_DLG_PURCHASE_BUTTON, "Subscribe Now"},

                { LocalizationKey.SHOP_SPECIAL_PACKS, "SPECIAL PACKS"},
                { LocalizationKey.SHOP_GEM_PACKS, "GEM PACKS"},
                { LocalizationKey.SHOP_SUBSCRIPTION_STRIP, "All Packs\nPremium Upgrade!"},
                { LocalizationKey.SHOP_PURCHASED_DLG_GAINED, "You Gained:"},
                { LocalizationKey.SHOP_PURHCASED_DLG_OK, "Got It"},

                { LocalizationKey.INVENTORY_SPECIAL_ITEMS, "SPECIAL ITEMS"},
                { LocalizationKey.INVENTORY_ITEM_UNLOCK, "Unlock"},
                { LocalizationKey.INVENTORY_WATCH_AD, "Watch Ad"},
                { LocalizationKey.INVENTORY_TOOL_TIP, "Video will be available soon"},
                { LocalizationKey.INVENTORY_SUBSCIRPTION_ENABLE, "Subscription Enabled"},
                { LocalizationKey.INVENTORY_OR, "Or"},
                { LocalizationKey.INVENTORY_UNLIMITED, "Unlimited"},

                { LocalizationKey.SPOT_PURHCASE_TITLE, "Low On Gems?"},
                { LocalizationKey.SPOT_PURCHASE_SUB_TITLE, "Purchase a Gem Pack to Continue!"},

                #endregion

                #region Friends

				{ LocalizationKey.FRIENDS_SECTION_NEW_MATCHES, "NEW MATCHES" },
                { LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES, "ACTIVE MATCHES" },
                { LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES_EMPTY, "There are no active matches." },
                { LocalizationKey.FRIENDS_SECTION_RECENTLY_COMPLETED_MATCHES, "RECENTLY PLAYED" },

                { LocalizationKey.FRIENDS_SECTION_PLAY_A_FRIEND, "PLAY A FRIEND" },
                { LocalizationKey.FRIENDS_SECTION_PLAY_SOMEONE_NEW, "PLAY SOMEONE NEW" },
                { LocalizationKey.FRIENDS_SECTION_SEARCH_RESULTS, "SEARCH RESULTS" },
                { LocalizationKey.FRIENDS_NO_FRIENDS_TEXT, "Invite Friends" },
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
                { LocalizationKey.FRIENDS_MANAGE_BLOCKED, "Manage Blocked Players"},
                { LocalizationKey.FRIENDS_BLOCK_SEARCH, "Search blocked players by name.."},
                { LocalizationKey.FRIENDS_UNBLOCK, "Unblock"},
                { LocalizationKey.FRIENDS_BLOCKED, "BLOCKED PLAYERS"},
                { LocalizationKey.FRIENDS_BLOCKED_EMPTY_LIST, "You haven't blocked anyone"},
                { LocalizationKey.FRIENDS_UNBLOCK_FAILED_TITLE, "Unblock Failed"},
                { LocalizationKey.FRIENDS_UNBLOCK_FAILED_DESC, "Unblocking a player will add them as a friend but your friends limit is reached, please remove unused friends strip to unblock"},
                #endregion

                #region Share

                { LocalizationKey.SHARE_STANDARD, "Hey, let's play Chess!" },

                #endregion

                #region Bottom Nav

                { LocalizationKey.NAV_HOME, "Games" },
                { LocalizationKey.NAV_PROFILE, "Profile" },
                { LocalizationKey.NAV_SHOP, "Shop" },
                { LocalizationKey.NAV_FRIENDS, "Friends" },
                { LocalizationKey.NAV_INVENTORY, "Inventory" },

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
                { LocalizationKey.REMATCH, "Rematch" },
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
                { LocalizationKey.NEW_GAME_CONFIRM_FRIENDLY, "5 Min"},
                { LocalizationKey.NEW_GAME_CONFIRM_FRIENDLY_10_MIN, "10 Min"},
                { LocalizationKey.NEW_GAME_CONFIRM_RANKED, "Classic 48h"},
                { LocalizationKey.NEW_GAME_CONFIRM_TITLE, "START A CLASSIC GAME"},
                { LocalizationKey.NEW_GAME_CONFIRM_START_GAME, "Start Game"},
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
                { LocalizationKey.RATE_APP_RATE, "RATE US" },
                { LocalizationKey.RATE_APP_TELL, "TELL US" },
                { LocalizationKey.RATE_APP_NOT_NOW, "May be Later" },
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
                {LocalizationKey.SETTINGS_CHAT_ON_DISCORD, "Chat with us on Discord" },
                { LocalizationKey.SETTINGS_ACCOUNT_PERSONALISED_ADS, "Personalised Ads" },
                { LocalizationKey.SETTINGS_ACCOUNT_MANAGE_SUBSCRIPTION, "Manage Subscription" },
                { LocalizationKey.SETTINGS_ACCOUNT_INFO, "Try for free" },
                { LocalizationKey.SETTINGS_ACCOUNT_RENEW, "Renews on (date)" },
                { LocalizationKey.SETTINGS_ON, "On" },
                { LocalizationKey.SETTINGS_OFF, "Off" },
                { LocalizationKey.SETTINGS_FAQ, "FAQ" },

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


                #region LeagueProfileStrip

                { LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_ENDS_IN, "League Ends in"},
                { LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TAP, "TAP"},
                { LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TROPHIES, "Trophies"},
                { LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_RANK, "Rank"},

                #endregion

                #region TournamentItem

                { LocalizationKey.TOURNAMENT_LIVE_ITEM_HEADING, "Grand Prize"},
                { LocalizationKey.TOURNAMENT_LIVE_ITEM_SUB_HEADING, "Score high, Win Big!"},
                { LocalizationKey.TOURNAMENT_LIVE_ITEM_ENDS_IN, "Ends in"},

                { LocalizationKey.TOURNAMENT_LEADERBOARD_RULES, "Rules"},
                { LocalizationKey.TOURNAMENT_LEADERBOARD_TOTAL_SCORE, "Total Score"},

                { LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_RANK, "Rank"},
                { LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_TOTAL_PLAYER_SCORE, "Player Score"},
                { LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_REWARDS, "Rewards"},

                { LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_YOU_HAVE, "You have"},
                { LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_FREE_PLAY, "Free Play"},
                { LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_TICKET_PLAY, "Play"},

                #endregion

                #region InBox

                { LocalizationKey.INBOX_HEADING, "InBox"},
                { LocalizationKey.INBOX_SECTION_HEADER_REWARDS, "Rewards"},

                #endregion
    };
        }
    }
}
