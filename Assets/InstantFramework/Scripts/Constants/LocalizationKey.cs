/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-09 14:34:53 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.InstantFramework
{
    public static class LocalizationKey
    {
        #region System

        public const string SPLASH_CONNECTING = "splashConnecting";
        public const string HARD_STOP = "hardStop";
        public const string RECONNECTING = "reconnecting";
        public const string UPDATE = "update";
        public const string UPDATE_BUTTON = "updateButton";
        public const string UPDATE_WAIT = "updateAvailable";
        public const string CHECK_INTERNET_CONNECTION = "checkInternetConnection";
        public const string SESSION_TERMINATED = "SESSION_TERMINATED";

        #endregion

        #region Common

        public const string OKAY_TEXT = "okayText";
        public const string BACK_TEXT = "backText";
        public const string UPGRADE_TEXT = "upgradeText";
        public const string MIN1_GAME_TEXT = "min1GameText";
        public const string MIN3_GAME_TEXT = "min3GameText";
        public const string MIN5_GAME_TEXT = "min5GameText";
        public const string MIN10_GAME_TEXT = "min10GameText";
        public const string MIN30_GAME_TEXT = "min30GameText";
        public const string ON_TEXT = "onText";
        public const string OFF_TEXT = "offText";
        public const string ACCEPT_TEXT = "acceptText";
        public const string DECLINE_TEXT = "declineText";
        public const string START_TEXT = "startText";
        public const string LIVE_TEXT = "liveText";
        public const string WIN_TEXT = "winText";
        public const string LOSS_TEXT = "lossText";

        #endregion

        #region Lobby

        public const string ELO_SCORE = "eloScore";
        public const string FACEBOOK_LOGIN = "facebookLogin";
        public const string SIGN_IN = "signIn";
        public const string PLAY_TOURNAMENT = "playTournament";
        public const string REMOVE_ADS = "removeAds";
        public const string FREE_NO_ADS_PERIOD = "freeNoAdsPeriod";
        public const string FREE_NO_ADS_MINUTES = "freeNoAdsMinutes";
        public const string FREE_NO_ADS_HOURS = "freeNoAdsHours";
        public const string FREE_NO_ADS_DAYS = "freeNoAdsDays";
        public const string PLAYING_LEVEL = "playingLevel";
        public const string DONE = "Done";
        public const string SELECT_THEME = "selectTheme";
        public const string CHOOSE_THEME = "chooseTheme";
        public const string REWARD_UNLOCKED_TITLE = "rewardUnlockedTitle";
        public const string REWARD_THEME = "rewardTheme";
        public const string REWARD_UNLOCKED_SUBTITLE = "rewardUnlockedSubTitle";
        public const string REWARD_UNLOCKED_CLAIM = "rewardUnlockedClaim";
        public const string AD_SKIPPED_TITLE = "adSkippedTitle";
        public const string AD_SKIPPED_INFO_TEXT = "adSkippedInfoText";
        #endregion

        #region Game

        public const string GM_RESULT_DIALOG_HEADING_WIN = "gmResultDialogHeadingWin";
        public const string GM_RESULT_DIALOG_HEADING_LOSE = "gmResultDialogHeadingLose";
        public const string GM_RESULT_DIALOG_HEADING_DRAW = "gmResultDialogHeadingDraw";
        public const string GM_RESULT_DIALOG_HEADING_DECLINED = "gmResultDialogHeadingDeclined";
        public const string GM_RESULT_DIALOG_REASON_CHECKMATE = "gmResultDialogReasonCheckmate";
        public const string GM_RESULT_DIALOG_REASON_STALEMATE = "gmResultDialogReasonStalemate";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_OFFERED_DRAW = "gmResultDialogReasonDrawByOfferedDraw";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL = "gmResultDialogReasonDrawByInsufficientMaterial";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE = "gmResultDialogReasonDrawByFiftyMoveRule";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE = "gmResultDialogReasonDrawByThreefoldRepeatRule";
        public const string GM_RESULT_DIALOG_REASON_TIMER_EXPIRED = "gmResultDialogReasonTimerExpired";
        public const string GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED = "gmPlayerDisconnected";
        public const string GM_RESULT_DIALOG_REASON_PLAYER_OPPONENT_LEFT = "gmOpponentLeft";
        public const string GM_RESULT_DIALOG_REASON_PLAYER_DECLINED = "gmPlayerDeclined";
        public const string GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER = "gmResultDialogReasonResignationPlayer";
        public const string GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT = "gmResultDialogReasonResignationOpponent";
        public const string GM_RESULT_DIALOG_HEADING_TOURNAMENT_ROUND_SCORE = "gmResultDialogHeadingTournamentRoundScore";
        public const string GM_RESULT_DIALOG_BONUS_TOURNAMENT_ROUND_SCORE = "gmResultDialogBonusTournamentRoundScore";
        public const string GM_RESULT_DIALOG_LABEL_TICKETS_LEFT = "gmResultDialogLabelTicketsLeft";
        public const string GM_DRAW_DIALOG_HEADING = "gmDrawDialogHeading";
        public const string GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE = "gmDrawDialogClaimByFiftyMoveRule";
        public const string GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE = "gmDrawDialogClaimByThreefoldRepeatRule";
        public const string GM_DRAW_DIALOG_YES_BUTTON = "gmDrawDialogYesButton";
        public const string GM_DRAW_DIALOG_NO_BUTTON = "gmDrawDialogNoButton";
        public const string GM_ROOM_PRIZE = "gmRoomPrize";
        public const string GM_ROOM_DURATION = "gmRoomDuration";
        public const string GM_PLAYER_LEVEL = "gmPlayerLevel";
        public const string GM_WAITING_FOR_OPPONENT = "gmWaitingForOpponent";
        public const string GM_WIFI_WARNING = "gmWifiWarning";
        public const string GM_WIFI_RECONNECTING = "gsReconnecting";
        public const string GM_DISCONNECTED = "gmDisconnected";
        public const string GM_ADVANTAGE = "gmAdvantage";
        public const string GM_EXIT_BUTTON_LOBBY = "gsmExitButtonLobby";
        public const string GM_EXIT_BUTTON_COLLECT_REWARD = "gsmExitButtonCollectReward";
        public const string GM_SPECIAL_HINT_NOT_AVAILABLE = "gmSpecialHintNotAvailable";

        #endregion

        // Client-server keys
        //
        // To change the value on one of these you also have to make changes
        // on the server side. The keys of the server and client must match.

        #region SharedKeys

        public const string NONE = "none";

        #endregion

        #region CPUMenu

        public const string CPU_MENU_HEADING = "cpuMenuHeading";
        public const string CPU_MENU_STRENGTH = "cpuMenuStrength";
        public const string CPU_MENU_IN_PROGRESS = "cpuMenuInProgress";
        public const string CPU_MENU_DURATION = "cpuMenuDuration";
        public const string CPU_MENU_DURATION_NONE = "cpuMenuDurationNone";
        public const string CPU_MENU_PLAYER_COLOR = "cpuMenuPlayerColor";
        public const string CPU_MENU_THEME = "cpuMenuTheme";
        public const string CPU_MENU_PLAY_ONLINE = "cpuMenuPlayOnline";
        public const string CPU_MENU_PLAY_ONLINE_CLASSIC = "cpuMenuPlayOnlineClassic";
        public const string CPU_MENU_PLAY_ONLINE_DESCRIPTION = "cpuMenuPlayOnlineDescription";
        public const string CPU_MENU_PLAY_ONLINE_DESCRIPTION_CLASSIC30 = "cpuMenuPlayOnlineDescriptionClassic30";
        public const string CPU_MENU_PLAY_FRIENDS = "cpuMenuPlayFriends";
        public const string CPU_MENU_PLAY_CPU = "cpuMenuPlayCPU";
        public const string CPU_MENU_SINGLE_PLAYER_GAME = "cpuMenuSinglePlayerGame";
        public const string EASY = "easy";
        public const string HARD = "hard";
        public const string CPU_MENU_THEMES = "cpuMenuThemes";
        public const string CPU_GAME_CPU_NAME = "cpuGameCpuName";
        public const string CPU_GAME_CPU_STRENGTH = "cpuGameCpuStrength";
        public const string CPU_GAME_PLAYER_NAME = "cpuGamePlayerName";
        public const string CPU_GAME_RESIGN_BUTTON = "cpuGameResignButton";
        public const string CPU_GAME_UNDO_BUTTON = "cpuGameUndoButton";
        public const string CPU_GAME_HINT_BUTTON = "cpuGameHintButton";
        public const string CPU_GAME_TURN_PLAYER = "cpuGameTurnPlayer";
        public const string CPU_GAME_TURN_OPPONENT = "cpuGameTurnOpponent";
        public const string CPU_GAME_EXIT_DLG_TITLE = "cpuGameExitDlgTitle";
        public const string CPU_GAME_SAVE_AND_EXIT = "cpuGameSaveAndExit";
        public const string CPU_GAME_SAVE_AND_EXIT_CAP = "cpuGameSaveAndExitCap";
        public const string CPU_GAME_CONTINUE_BUTTON = "cpuGameContinueButton";
        public const string CPU_GAME_OFFER_DRAW_BUTTON = "cpuGameOfferDrawButton";
        public const string CPU_GAME_EXIT_EXPLAINATION = "cpuGameExitExplanation";
        public const string CPU_RESULTS_CLOSE_BUTTON = "cpuResultsCloseButton";
        public const string CPU_RESULTS_STATS_BUTTON = "cpuResultsStatsButton";
        public const string CPU_RESULTS_EXIT_BUTTON = "cpuResultsExitButton";
        public const string CLASSIC_MODE_TIME = "classicModeTime";

        #endregion

        #region Lessons

        public const string LESSONS_TITLE = "lessonsTitle";
        public const string LESSONS_DESCRIPTION = "lessonsDescription";
        public const string LESSONS_START = "lessonsStart";
        public const string LESSONS_COMPLETED_TITLE = "lessonCompletedTitle";
        public const string LESSONS_COMPLETED_DESCRIPTION = "lessonsCompletedDescription";

        #endregion

        #region GameResults

        public const string RESULTS_CLOSE_BUTTON = "resultsCloseButton";
        public const string RESULTS_TOURNAMENT_CLOSE_BUTTON = "resultsTournamentCloseButton";
        public const string RESULTS_COLLECT_REWARD_BUTTON = "resultsCollectRewardButton";
        public const string RESULTS_BOOST_RATING_BUTTON = "resultsBoostRatingButton";
        public const string RESULTS_RECOVER_RATING_BUTTON = "resultsRecoverRatingButton";
        public const string RESULTS_SKIP_REWARD_BUTTON = "resultsSkipRewardButton";
        public const string RESULTS_EARNED = "resultsEarned";
        public const string RESULTS_REWARD = "resultsReward";
        public const string RESULTS_BOOST_DRAW = "resultsBoostDraw";
        public const string RESULTS_BOOST_FRIENDLY = "resultsBoostFriendly";
        public const string RESULTS_BOOSTED = "resultsBoosted";

        #endregion

        #region Multiplayer

        public const string MULTIPLAYER_WAITING_FOR_OPPONENT = "multiplayerWaitingForOpponent";
        public const string MULTIPLAYER_SEARCHING = "multiplayerSearching";
        public const string MULTIPLAYER_FOUND = "multiplayerFound";
        public const string QUICK_MATCH_FAILED = "quickMatchFailed";
        public const string QUICK_MATCH_FAILED_REASON = "quickMatchFailedReason";

        public const string QUICK_MATCH_EXPIRED = "quickMatchExpired";
        public const string QUICK_MATCH_EXPIRED_REASON = "quickMatchExpiredReason";

        #endregion

        #region Stats

        public const string STATS_ONLINE_TITLE = "statsOnlineTitle";
        public const string STATS_ONLINE_WIN_PCT = "statsOnlineWinPct";
        public const string STATS_ONLINE_WON = "statsOnlineWon";
        public const string STATS_ONLINE_LOST = "statsOnlineLost";
        public const string STATS_ONLINE_DRAWN = "statsOnlineDrawn";
        public const string STATS_ONLINE_TOTAL = "statsOnlineTotal";
        public const string STATS_COMPUTER_TITLE = "statsComputerTitle";
        public const string STATS_LEGEND_GOLD = "statsLegendGold";
        public const string STATS_LEGEND_SILVER = "statsLegendSilver";
        public const string STATS_TAG = "statsTag";
        public const string STATS_TAKE_PHOTO = "statsTakePhoto";
        public const string STATS_CHOOSE_PHOTO = "statsSelectPhoto";
        public const string STATS_PHOTO_TITLE = "statsPhotoTitle";

        public const string STATS_OPEN_SETTINGS_TITLE = "statsOpenSettingsTitle";
        public const string STATS_OPEN_SETTINGS_SUBTITLE = "statsOpenSettingsSubTitle";
        public const string STATS_OPEN_SETTINGS = "statsOpenSettings";
        #endregion

        #region Store

        public const string STORE_TITLE_BUNDLES = "storeTitleBundles";
        public const string STORE_TAB_POWERUPS = "storeTabPowerUps";
        public const string STORE_TAB_THEMES = "storeTabThemes";
        public const string STORE_TAB_COINS = "storeTabCoins";

        public const string STORE_POWERUP_TITLE_SAFEMOVE = "storePowerUpTitleSafeMove";
        public const string STORE_POWERUP_TITLE_HINT = "storePowerUpTitleHint";
        public const string STORE_POWERUP_TITLE_HINDSIGHT = "storePowerUpTitleHindsight";

        public const string STORE_BUNDLE_FIELD_OWNED = "storeBundleFieldOwned";
        public const string STORE_BUNDLE_FIELD_REMAINING = "storeBundleFieldRemaining";
        public const string STORE_BUNDLE_FIELD_DAYS = "storeBundleFieldDays";

        public const string STORE_CONFIRM_DLG_TITLE_BUY = "storeConfirmDlgTitleBuy";
        public const string STORE_NOT_AVAILABLE = "storeNotAvailable";
        public const string STORE_GETTING_PACKAGE = "storeGetttingPackage";

        public const string CPU_STORE_HEADING = "storeHeading";
        public const string CPU_STORE_OWNED = "storeItemOwned";
        public const string CPU_STORE_BUCKS = "bucks";

        public const string CPU_STORE_BUY_THEME_TITLE = "storeBuyThemeTitle";
        public const string CPU_STORE_BUY_BUY_BUTTON = "storeBuyBuy";

        public const string CPU_STORE_NOT_ENOUGH_BUCKS_TITLE = "storeNotEnoughBucksTitle";
        public const string CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING = "storeNotEnoughBucksSubHeading";
        public const string CPU_STORE_NOT_ENOUGH_BUCKS_BUY = "storeNotEnoughBucksBuy";
        public const string CPU_STORE_NOT_ENOUGH_BUCKS_YES_BUTTON = "storeNotEnoughBucksYes";
        public const string CPU_STORE_NOT_ENOUGH_BUCKS_NO_BUTTON = "storeNotEnoughBucksNo";

        public const string STORE_PURCHASE_FAILED_VERIFICATION_TITLE = "storePurchaseFailedVerificationTitle";
        public const string STORE_PURCHASE_FAILED_VERIFICATION_DESC = "storePurchaseFailedVerificationDesc";
        public const string STORE_PURCHASE_FAILED_VERIFICATION_YES_BUTTON = "storePurchaseFailedVerificationYesButton";
        public const string STORE_PURCHASE_FAILED = "storePurchaseFailed";
        public const string STORE_PURCHASE_FAILED_REASON_CANCEL = "UserCancelled";
        public const string STORE_PURCHASE_FAILED_REASON_UNAVAILABLE = "PurchasingUnavailable";
        public const string STORE_PURCHASE_FAILED_REASON_PENDING = "ExistingPurchasePending";
        public const string STORE_PURCHASE_FAILED_REASON_PRODUCT_UNAVAILABLE = "ProductUnavailable";
        public const string STORE_PURCHASE_FAILED_REASON_INVALID = "SignatureInvalid";
        public const string STORE_PURCHASE_FAILED_REASON_DECLINED = "PaymentDeclined";
        public const string STORE_PURCHASE_FAILED_REASON_DUPLICATE = "DuplicateTransaction";
        public const string STORE_PURCHASE_FAILED_REASON_UNKNOWN = "Unknown";

        public const string SUBSCRIPTION_DLG_TITLE = "subscriptionDlgTitle";
        public const string SUBSCRIPTION_DLG_DISCLAIMER = "subscriptionDlgDisclaimer";
        public const string SUBSCRIPTION_DLG_RESTORE_PURCHASE = "subscriptionDlgRestorePurchase";
        public const string SUBSCRIPTION_DLG_PRIVACY_POLICY = "subscriptionDlgPrivacyPolicy";
        public const string SUBSCRIPTION_DLG_TERMS_OF_USE = "subscriptionDlgTermsOfUse";
        public const string SUBSCRIPTION_DLG_PURCHASE_BUTTON = "subscriptionDlgPurchaseButton";
        public const string SUBSCRIPTION_DLG_FREE_TRIAL = "subscriptionDlgFreeTrial";
        public const string SUBSCRIPTION_DLG_PRICE = "subscriptionDlgPrice";
        public const string SUBSCRIPTION_DLG_TERMS_AND_SERVICES = "subscriptionDlgTermsAndServices";

        public const string PROMOTON_DLG_TITLE = "promotionDlgTitle";
        public const string PROMOTION_DLG_PURCHASE_BUTTON = "promotionDlgPurchaseButton";
        public const string PROMOTION_DLG_PRICE = "promotionDlgPrice";
        public const string PROMOTION_DLG_PURCHASE = "promotionDlgPurchase";

        public const string SHOP_SAVER_BUNDLES = "shopSaverBundles";
        public const string SHOP_SPECIAL_PACKS = "shopSpecailPacks";
        public const string SHOP_GEM_PACKS = "shopGemPacks";
        public const string SHOP_SUBSCRIPTION_STRIP = "shopSubscriptionStrip";
        public const string SHOP_PURCHASED_DLG_GAINED = "shopPurchasedDlgGained";
        public const string SHOP_PURHCASED_DLG_OK = "shopPurchasedDlgOk";

        public const string INVENTORY_SPECIAL_ITEMS = "inventorySpecialItems";
        public const string INVENTORY_ITEM_UNLOCK = "inventoryItemUnlock";
        public const string INVENTORY_WATCH_AD = "inventoryWatchAd";
        public const string INVENTORY_TOOL_TIP = "inventoryToolTip";
        public const string INVENTORY_SUBSCIRPTION_ENABLE = "inventorySubscriptionEnabled";
        public const string INVENTORY_OR = "inventoryOr";
        public const string INVENTORY_UNLIMITED = "inventoryLimited";
        public const string INVENTORY_GET_MORE = "inventoryGetMore";
        public const string INVENTORY_YOU_OWN = "inventoryYouOwn";

        public const string SPOT_PURHCASE_TITLE = "spotPurchaseTitle";
        public const string SPOT_PURCHASE_SUB_TITLE = "spotPurchaseSubTitle";
        public const string SPOT_PURCHASE_FINE_PRINT = "spotPurchaseFinePrint";

        public const string SPOT_INVENTORY_TITLE = "spotInventoryTitle";
        public const string SPOT_INVENTORY_SUB_TITLE = "spotInventorySubtitle";

        #endregion

        #region Friends

        public const string FRIENDS_SECTION_NEW_MATCHES = "friendsNewMatches";
        public const string FRIENDS_SECTION_ACTIVE_MATCHES = "friendsActiveMatches";
        public const string FRIENDS_SECTION_ACTIVE_MATCHES_EMPTY = "friendsActiveMatchesEmpty";
        public const string FRIENDS_SECTION_RECENTLY_COMPLETED_MATCHES = "friendsRecentlyCompletedMatches";
        public const string FRIENDS_SECTION_PLAY_A_FRIEND = "friendsPlayAFriend";
        public const string FRIENDS_SECTION_PLAY_SOMEONE_NEW = "friendsPlaySomeoneNew";

        public const string FRIENDS_SECTION_SEARCH_RESULTS = "friendsSearchResults";

        public const string FRIENDS_NO_FRIENDS_TEXT = "friendsInviteLabel";
        public const string FRIENDS_INVITE_TEXT = "friendsInviteText";
        public const string FRIENDS_INVITE_BUTTON_TEXT = "friendsInviteButtonText";
        public const string FRIENDS_INVITE_TITLE_TEXT = "friendsInviteTitleText";

        public const string FRIENDS_REFRESH_TEXT = "friendsRefreshText";
        public const string FRIENDS_CONFIRM_LABEL = "friendsConfirmLabel";
        public const string FRIENDS_YES_LABEL = "friendsYesLabel";
        public const string FRIENDS_NO_LABEL = "friendsNoLabel";
        public const string FRIENDS_VS_LABEL = "friendsVsLabel";
        public const string FRIENDS_WINS_LABEL = "friendsWinsLabel";
        public const string FRIENDS_DRAWS_LABEL = "friendsDrawsLabel";
        public const string FRIENDS_TOTAL_GAMES_LABEL = "friendsTotalGamesLabel";
        public const string FRIENDS_BLOCK_LABEL = "friendsBlockLabel";
        public const string FRIENDS_CHAT_LABEL = "friendsChatLabel";
        public const string FRIENDS_BLOCK_TEXT = "friendsBlockText";
        public const string FRIENDS_FACEBOOK_CONNECT_TEXT = "friendsFacebookConnectText";
        public const string FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT = "friendsFacebookLoginButtonText";
        public const string FACEBBOK_LOGIN_REWARD_TEXT = "facebookLoginRewardText";
        public const string FRIENDS_WAITING_FOR_PLAYERS = "friendsWaitingForPlayers";
        public const string FRIENDS_MANAGE_BLOCKED = "friendsManageBlocked";

        public const string FRIENDS_FIND_FRIEND_TITLE = "friendsFindFriendTitle";
        public const string FRIENDS_FIND_FRIEND_LOGIN_INFO = "friendFindFriendLoginInfo";
        public const string FRIENDS_FIND_FRIEND_SEARCH_INFO = "friendFindFriendSearchInfo";
        public const string FRIENDS_FIND_FRIEND_INVITE_INFO = "friendFindFriendInviteInfo";

        public const string FRIENDS_ADD_TO_FRIENDS = "addToFriends";
        public const string FRIENDS_REMOVE_FROM_FRIENDS = "removeFromFriends";
        public const string FRIENDS_TEXT_FRIENDED = "FriendedText";

        public const string FRIENDS_BLOCK_SEARCH = "friendsSearch";
        public const string FRIENDS_UNBLOCK = "friendsUnblock";
        public const string FRIENDS_BLOCKED = "friendsBlocked";
        public const string FRIENDS_BLOCKED_EMPTY_LIST = "friendsBlockedEmptyList";
        public const string FRIENDS_UNBLOCK_FAILED_TITLE = "friendsUnblockedFailedTitle";
        public const string FRIENDS_UNBLOCK_FAILED_DESC = "friendsUnblockedFailedDesc";
        #endregion

        #region Share

        public const string SHARE_STANDARD = "shareStandard";

        #endregion


        #region Bottom Nav

        public const string NAV_HOME = "navHome";
        public const string NAV_PROFILE = "navProfile";
        public const string NAV_SHOP = "navShop";
        public const string NAV_FRIENDS = "navFriends";
        public const string NAV_INVENTORY = "navInventory";
        public const string NAV_ARENA = "navArena";
        public const string NAV_LESSON = "navLesson";

        #endregion

        #region Long Play

        public const string IN_GAME_BACK = "longPlayBackToFriends";
        public const string BOT_NAV_NEXT = "botNavNext";
        public const string BOT_NAV_COMPANY = "botNavCompany";
        public const string LONG_PLAY_NOT_NOW = "longPlayNotNow";
        public const string LONG_PLAY_FRIENDLY = "longPlayFriendly";
        public const string LONG_PLAY_RANKED = "longPlayRanked";
        public const string PLAY = "play";
        public const string VIEW = "view";
        public const string REMATCH = "rematch";
        public const string LONG_PLAY_ACCEPT = "longPlayAccept";
        public const string LONG_PLAY_CANCEL = "longPlayCancel";
        public const string LONG_PLAY_NEW_MATCH_GREETING = "longPlayNewMatchGreeting";
        public const string LONG_PLAY_DECLINE_APOLOGY = "longPlayDeclineApology";
        public const string LONG_PLAY_OK = "longPlayOk";
        public const string LONG_PLAY_WAITING = "longPlayWaiting";
        public const string LONG_PLAY_MINUTES = "longPlayMinutes";
        public const string LONG_PLAY_HOURS = "longPlayHours";
        public const string LONG_PLAY_DAYS = "longPlayDays";
        public const string LONG_PLAY_CHALLENGED_YOU = "longPlayChallengedYou";
        public const string LONG_PLAY_YOUR_TURN = "longPlayYourTurn";
        public const string LONG_PLAY_MATCH_PROGRESS = "matchInProgress";
        public const string LONG_PLAY_THEIR_TURN = "longPlayTheirTurn";
        public const string LONG_PLAY_WAITING_FOR_ACCEPT = "longPlayWaitingForAccept";
        public const string LONG_PLAY_DECLINED = "longPlayDeclined";
        public const string LONG_PLAY_YOU_LOST = "longPlayYouLost";
        public const string LONG_PLAY_YOU_WON = "longPlayYouWon";
        public const string LONG_PLAY_CANCELED = "longPlayCanceled";
        public const string LONG_PLAY_DRAW = "longPlayDraw";
        public const string LONG_PLAY_ACCEPT_TITLE = "longPlayAcceptTitle";
        public const string LONG_PLAY_ACCEPT_YES = "longPlayAcceptYes";
        public const string LONG_PLAY_ACCEPT_NO = "longPlayAcceptNo";
        public const string LONG_PLAY_BACK_TO_GAME = "longPlayBackToGame";
        public const string LONG_PLAY_RESULTS_BACK = "longPLayResultsBack";
        public const string CHAT_TODAY = "chatToday";
        public const string CHAT_YESTERDAY = "chatYesterday";
        public const string CHAT_CLEAR = "chatClear";
        public const string CHAT_DEFAULT_DAY_LINE = "chatDefaultDayLine";
        public const string CHAT_DEFAULT_SYSTEM_MESSAGE = "chatDefaultSystemMessage";
        public const string CHAT_DISABLED_SYSTEM_MESSAGE = "chatDisabledSystemMessage";
        public const string REMOVE_COMMUNITY_FRIEND_YES = "removeCommunityFriendYes";
        public const string REMOVE_COMMUNITY_FRIEND_NO = "removeCommunityFriendNo";
        public const string REMOVE_COMMUNITY_FRIEND_TITLE = "removeCommunityFriendTitle";
        public const string NEW_GAME_CONFIRM_RANKED = "newGameConfirmRanked";
        public const string NEW_GAME_CONFIRM_FRIENDLY = "newGameConfirmnFriendly";
        public const string NEW_GAME_CONFIRM_FRIENDLY_10_MIN = "newGameConfirmnFriendly10Min"; 
        public const string NEW_GAME_CONFIRM_TITLE = "newGameConfirmTitle";
        public const string NEW_GAME_CONFIRM_START_GAME = "newGameConfirmStartGame";
        public const string FRIENDLY_GAME_CAPTION = "friendlyGameCaption";
        public const string LONG_PLAY_VIEW = "longPlayView";
        public const string SHARE_GAME_SCREENSHOT = "shareGameShot";
        public const string SHARE = "share";

        #endregion


        #region RateApp

        public const string RATE_APP_TITLE = "rateAppTitle";
        public const string RATE_APP_SUB_TITLE_RATE = "rateAppSubTitleRate";
        public const string RATE_APP_SUB_TITLE_TELL = "rateAppSubTitleTell";
        public const string RATE_APP_RATE = "rate";
        public const string RATE_APP_TELL = "tell";
        public const string RATE_APP_NOT_NOW = "notNow";
        public const string RATE_APP_IMPROVE = "improve";
        public const string RATE_APP_LIKE = "like";
        public const string RATE_APP_LOVE = "love";
        public const string RATE_APP_LOVE_FROM_TEAM = "loveFromTeam";

        #endregion

        #region Settings

        public const string SETTINGS_TITLE = "settingsTitle";
        public const string SETTINGS_SOUND_TITLE = "settingsSoundTitle";
        public const string SETTINGS_SOUND_EFFECT = "settingsSoundEffect";
        public const string SETTINGS_ACCOUNT_TITLE = "settingsAccountTitle";
        public const string SETTINGS_ACCOUNT_UPGRADE_TO_PREMIUM = "settingsAccountUpgradeToPremium";
        public const string SETTINGS_CHAT_ON_DISCORD = "settingsChatOnDiscord";
        public const string SETTINGS_ACCOUNT_PERSONALISED_ADS = "settingsAccountPersonalisedAds";
        public const string SETTINGS_ACCOUNT_MANAGE_SUBSCRIPTION = "settingsAccountManageSubscription";
        public const string SETTINGS_ACCOUNT_INFO = "settingsAccountInfo";
        public const string SETTINGS_ACCOUNT_RENEW = "settingsAccountRenew";
        public const string SETTINGS_ON = "settingsOn";
        public const string SETTINGS_OFF = "settingsOff";
        public const string SETTINGS_FAQ = "settingsFAQ";

        #endregion

        #region EarnRewards

        public const string EARN_REWARDS_TITLE = "earnRewardsTitle";
        public const string EARN_REWARDS_INFO_TEXT = "earnRewardsInfoText";

        #endregion

        #region ManageSubscription

        public const string SUB_MANAGE = "subManage";
        public const string SUB_OPTIONS = "subOptions";
        public const string SUB_BENEFITS = "subBenefits";
        public const string SUB_POPULAR = "subPopular";
        public const string SUB_MONTHLY = "subMonthly";
        public const string SUB_ANNUAL = "subAnnual";
        public const string SUB_SWITCH_MONTHLY = "subSwitchMonthly";
        public const string SUB_SWITCH_ANNUAL = "subSwitchAnnual";
        public const string SUB_SWITCH_MONTHLY_BTN = "subSwitchMonthlyBtn";
        public const string SUB_SWITCH_ANNUAL_BTN = "subSwitchAnnualBtn";

        #endregion

        #region PlayerLeagueProfileStrip

        public const string PLAYER_LEAGUE_PROFILE_STRIP_ENDS_IN = "leagueEndsInLabel";
        public const string PLAYER_LEAGUE_PROFILE_STRIP_TAP = "tapLabel";
        public const string PLAYER_LEAGUE_PROFILE_STRIP_TROPHIES = "trophiesLabel";
        public const string PLAYER_LEAGUE_PROFILE_STRIP_RANK = "rankLabel";
        public const string PLAYER_LEAGUE_PROFILE_STRIP_YOUR_LEAGUE_TEXT = "playerLeagueProfileStripYourLeagueText";

        #endregion

        #region TournamentItem

        public const string TOURNAMENT_LIVE_ITEM_HEADING = "tournamentLiveItemHeading";
        public const string TOURNAMENT_LIVE_ITEM_SUB_HEADING = "tournamentLiveItemSubHeading";
        public const string TOURNAMENT_LIVE_ITEM_ENDS_IN = "tournamentLiveItemEndsIn";

        public const string TOURNAMENT_UPCOMING = "tournamentUpcoming";
        public const string TOURNAMENT_UPCOMING_STARTS_IN = "tournamentUpcomingStartsIn";
        public const string TOURNAMENT_UPCOMING_GET_NOTIFIED = "tournamentUpcomingGetNotified";
        public const string TOURNAMENT_UPCOMING_NOTICATION_ENABLED = "tournamentUpcomingNotificationEnabled";

        public const string TOURNAMENT_LEADERBOARD_RULES = "tournamentLeaderboardRules";
        public const string TOURNAMENT_LEADERBOARD_TOTAL_SCORE = "tournamentLeaderboardTotalScore";
        public const string TOURNAMENT_LEADERBOARD_YOUR_RANK = "tournamentLeaderboardYourRank";

        public const string TOURNAMENT_LEADERBOARD_COLUMN_HEADER_RANK = "tournamentLeaderboardColumnHeaderRank";
        public const string TOURNAMENT_LEADERBOARD_COLUMN_HEADER_TOTAL_PLAYER_SCORE = "tournamentLeaderboardColumnHeaderPlayerScore";
        public const string TOURNAMENT_LEADERBOARD_COLUMN_HEADER_REWARDS = "tournamentLeaderboardColumnHeaderRewards";

        public const string TOURNAMENT_LEADERBOARD_FOOTER_YOU_HAVE = "tournamentLeaderboardFooterYouHave";
        public const string TOURNAMENT_LEADERBOARD_FOOTER_FREE_PLAY = "tournamentLeaderboardFreePlay";
        public const string TOURNAMENT_LEADERBOARD_FOOTER_TICKET_PLAY = "tournamentLeaderboardFooterTicketPlay";
        public const string TOURNAMENT_LEADERBOARD_FOOTER_COLLECT_REWARDS = "tournamentLeaderboardFooterCollectRewards";

        public const string TOURNAMENT_CHEST_CONTENT_DIALOGUE_TITLE = "tournamentChestContentDialogueTitle";
        public const string TOURNAMENT_CHEST_CONTENT_DIALOGUE_DESCRIPTION = "tournamentChestContentDialogueDescriptiion";
        public const string TOURNAMENT_CHEST_CONTENT_DIALOGUE_OKAY_BUTTON_TEXT = "tournamentChestContentDialogueOkayButtonText";

        public const string TOURNAMENT_REWARD_DLG_TROPHIES_ADDED = "tournamentRewardDlgTrophiesAdded";

        #endregion

        #region InBox

        public const string INBOX_HEADING = "inBoxHeading";
        public const string INBOX_SECTION_HEADER_REWARDS = "inBoxSectionHeaderRewards";
        public const string INBOX_EMPTY_INBOX_LABEL = "inBoxEmptyInboxLabel";

        #endregion

        #region LeaguePerks

        public const string LEAGUE_PERKS_TITLE = "leaguePerksTitle";

        #endregion

        #region

        public const string NOTIFICATION_UPCOMING_TOURNAMENT_REMINDER_TITLE = "notificationUpcomingTournamentReminderTitle";
        public const string NOTIFICATION_UPCOMING_TOURNAMENT_STARTED_TITLE = "notifcationUpcomingTournamentStartedTitle";
        public const string NOTIFICATION_UPCOMING_TOURNAMENT_REMINDER_BODY = "notificationUpcomingTournamentReminderBody";
        public const string NOTIFICATION_UPCOMING_TOURNAMENT_STARTED_BODY = "notifcationUpcomingTournamentStartedBody";
        public const string NOTIFICATION_TOURNAMENT_END_TITLE = "notificationTournamentEndTitle";
        public const string NOTIFICATION_TOURNAMENT_END_BODY = "notificationTournamentEndBody";
        public const string NOTIFICATION_DAILY_REWARD_TITLE = "notificationDailyRewardTitle";
        public const string NOTIFICATION_SUBSCRIPTION_REWARD_TITLE = "notificationSubscriptionRewardTitle";
        public const string NOTIFICATION_DAILY_REWARD_BODY = "notificationDailyRewardBody";
        public const string NOTIFICATION_SUBSCRIPTION_REWARD_BODY = "notificationSubscriptionRewardBody";

        #endregion
    }
}
