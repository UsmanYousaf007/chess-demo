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
        public const string CHECK_INTERNET_CONNECTION = "checkInternetConnection";

        #endregion

        #region Lobby

        public const string ELO_SCORE = "eloScore";
        public const string FACEBOOK_LOGIN = "facebookLogin";
        public const string REMOVE_ADS = "removeAds";
        public const string FREE_NO_ADS_PERIOD = "freeNoAdsPeriod";
        public const string FREE_NO_ADS_MINUTES = "freeNoAdsMinutes";
        public const string FREE_NO_ADS_HOURS = "freeNoAdsHours";
        public const string FREE_NO_ADS_DAYS = "freeNoAdsDays";

        #endregion

        #region Game

        public const string GM_RESULT_DIALOG_HEADING_WIN = "gmResultDialogHeadingWin";
        public const string GM_RESULT_DIALOG_HEADING_LOSE = "gmResultDialogHeadingLose";
        public const string GM_RESULT_DIALOG_HEADING_DRAW = "gmResultDialogHeadingDraw";
        public const string GM_RESULT_DIALOG_HEADING_DECLINED = "gmResultDialogHeadingDeclined";
        public const string GM_RESULT_DIALOG_REASON_CHECKMATE = "gmResultDialogReasonCheckmate";
        public const string GM_RESULT_DIALOG_REASON_STALEMATE = "gmResultDialogReasonStalemate";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL = "gmResultDialogReasonDrawByInsufficientMaterial";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE = "gmResultDialogReasonDrawByFiftyMoveRule";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE = "gmResultDialogReasonDrawByThreefoldRepeatRule";
        public const string GM_RESULT_DIALOG_REASON_TIMER_EXPIRED = "gmResultDialogReasonTimerExpired";
        public const string GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED = "gmPlayerDisconnected";
        public const string GM_RESULT_DIALOG_REASON_PLAYER_DECLINED = "gmPlayerDeclined";
        public const string GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER = "gmResultDialogReasonResignationPlayer";
        public const string GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT = "gmResultDialogReasonResignationOpponent";
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
        public const string GM_DISCONNECTED = "gmDisconnected";
        public const string GM_ADVANTAGE = "gmAdvantage";
        public const string GM_EXIT_BUTTON_LOBBY = "gsmExitButtonLobby";
        public const string GM_EXIT_BUTTON_COLLECT_REWARD = "gsmExitButtonCollectReward";

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
        public const string CPU_MENU_PLAY_FRIENDS = "cpuMenuPlayFriends";
        public const string CPU_MENU_PLAY_CPU = "cpuMenuPlayCPU";
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
        public const string CPU_GAME_CONTINUE_BUTTON = "cpuGameContinueButton";
        public const string CPU_GAME_EXIT_EXPLAINATION = "cpuGameExitExplanation";
        public const string CPU_RESULTS_CLOSE_BUTTON = "cpuResultsCloseButton";
        public const string CPU_RESULTS_STATS_BUTTON = "cpuResultsStatsButton";
        public const string CPU_RESULTS_EXIT_BUTTON = "cpuResultsExitButton";

        #endregion 

        #region GameResults

        public const string RESULTS_CLOSE_BUTTON = "resultsCloseButton";
        public const string RESULTS_COLLECT_REWARD_BUTTON = "resultsCollectRewardButton";
        public const string RESULTS_SKIP_REWARD_BUTTON = "resultsSkipRewardButton";
        public const string RESULTS_EARNED = "resultsEarned";
        public const string RESULTS_REWARD = "resultsReward";

        #endregion

        #region Multiplayer

        public const string MULTIPLAYER_SEARCHING = "multiplayerSearching";
        public const string MULTIPLAYER_FOUND = "multiplayerFound";

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

		#endregion

        #region Friends

		public const string FRIENDS_SECTION_NEW_MATCHES = "friendsNewMatches";
        public const string FRIENDS_SECTION_ACTIVE_MATCHES = "friendsActiveMatches";
        public const string FRIENDS_SECTION_ACTIVE_MATCHES_EMPTY = "friendsActiveMatchesEmpty";
        public const string FRIENDS_SECTION_PLAY_A_FRIEND = "friendsPlayAFriend";
        public const string FRIENDS_SECTION_PLAY_SOMEONE_NEW = "friendsPlaySomeoneNew";

        public const string FRIENDS_NO_FRIENDS_TEXT = "friendsInviteLabel";
		public const string FRIENDS_INVITE_TEXT = "friendsInviteText";
		public const string FRIENDS_REFRESH_TEXT = "friendsRefreshText";
		public const string FRIENDS_CONFIRM_LABEL = "friendsConfirmLabel";
		public const string FRIENDS_YES_LABEL = "friendsYesLabel";
		public const string FRIENDS_NO_LABEL = "friendsNoLabel";
		public const string FRIENDS_VS_LABEL = "friendsVsLabel";
		public const string FRIENDS_WINS_LABEL = "friendsWinsLabel";
		public const string FRIENDS_DRAWS_LABEL = "friendsDrawsLabel";
		public const string FRIENDS_TOTAL_GAMES_LABEL = "friendsTotalGamesLabel";
		public const string FRIENDS_BLOCK_LABEL = "friendsBlockLabel";
        public const string FRIENDS_FACEBOOK_CONNECT_TEXT = "friendsFacebookConnectText";
        public const string FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT = "friendsFacebookLoginButtonText";
        public const string FRIENDS_WAITING_FOR_PLAYERS = "friendsWaitingForPlayers";

        #endregion

        #region Share

        public const string SHARE_STANDARD = "shareStandard";

        #endregion


        #region Bottom Nav

        public const string NAV_HOME = "navHome";
        public const string NAV_PROFILE = "navProfile";
        public const string NAV_SHOP = "navShop";
        public const string NAV_FRIENDS = "navFriends";

        #endregion

        #region Long Play

        public const string IN_GAME_BACK = "longPlayBackToFriends";
        public const string BOT_NAV_NEXT = "botNavNext";
        public const string BOT_NAV_COMPANY = "botNavCompany";
        public const string LONG_PLAY_NOT_NOW = "longPlayNotNow";
        public const string LONG_PLAY_ACCEPT = "longPlayAccept";
        public const string LONG_PLAY_CANCEL = "longPlayCancel";
        public const string LONG_PLAY_NEW_MATCH_GREETING = "longPlayNewMatchGreeting";
        public const string LONG_PLAY_OK = "longPlayOk";
        public const string LONG_PLAY_WAITING = "longPlayWaiting";
        public const string LONG_PLAY_MINUTES = "longPlayMinutes";
        public const string LONG_PLAY_HOURS = "longPlayHours";
        public const string LONG_PLAY_DAYS = "longPlayDays";
        public const string LONG_PLAY_CHALLENGED_YOU = "longPlayChallengedYou";
        public const string LONG_PLAY_YOUR_TURN = "longPlayYourTurn";
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
        public const string REMOVE_COMMUNITY_FRIEND_YES = "removeCommunityFriendYes";
        public const string REMOVE_COMMUNITY_FRIEND_NO = "removeCommunityFriendNo";
        public const string REMOVE_COMMUNITY_FRIEND_TITLE = "removeCommunityFriendTitle";
        public const string NEW_GAME_CONFIRM_RANKED = "newGameConfirmRanked";
        public const string NEW_GAME_CONFIRM_FRIENDLY = "newGameConfirmnFriendly";
        public const string NEW_GAME_CONFIRM_TITLE = "newGameConfirmTitle";
        public const string FRIENDLY_GAME_CAPTION = "friendlyGameCaption";
        public const string LONG_PLAY_VIEW = "longPlayView";
       


        #endregion


        #region RateApp

        public const string RATE_APP_TITLE = "rateAppTitle";
        public const string RATE_APP_SUB_TITLE = "rateAppSubTitle";
        public const string RATE_APP_RATE = "rate";
        public const string RATE_APP_NOT_NOW = "notNow";

        #endregion
    }
}
