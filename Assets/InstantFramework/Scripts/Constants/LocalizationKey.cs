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

        #endregion

        #region Lobby

        public const string ELO_SCORE = "eloScore";
        public const string FACEBOOK_LOGIN = "facebookLogin";

        #endregion

        #region Game

        public const string GM_RESULT_DIALOG_HEADING_WIN = "gmResultDialogHeadingWin";
        public const string GM_RESULT_DIALOG_HEADING_LOSE = "gmResultDialogHeadingLose";
        public const string GM_RESULT_DIALOG_HEADING_DRAW = "gmResultDialogHeadingDraw";
        public const string GM_RESULT_DIALOG_REASON_CHECKMATE = "gmResultDialogReasonCheckmate";
        public const string GM_RESULT_DIALOG_REASON_STALEMATE = "gmResultDialogReasonStalemate";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL = "gmResultDialogReasonDrawByInsufficientMaterial";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE = "gmResultDialogReasonDrawByFiftyMoveRule";
        public const string GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE = "gmResultDialogReasonDrawByThreefoldRepeatRule";
        public const string GM_RESULT_DIALOG_REASON_TIMER_EXPIRED = "gmResultDialogReasonTimerExpired";
        public const string GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED = "gmPlayerDisconnected";
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
        public const string CPU_FREE_BUCKS_REWARD_OK = "cpuFreeBucksRewardOk";
        public const string CPU_FREE_BUCKS_REWARD_TITLE = "cpuFreeBucksRewardTitle";
        public const string CPU_FREE_BUCKS_BUTTON_GET = "cpuFreeBucksButtonGet";
        public const string CPU_FREE_BUCKS_BONUS = "cpuFreeBucksBonus";
        public const string CPU_FREE_BUCKS_BUTTON_NOT_AVAILABLE = "cpuFreeBucksButtonNotAvailable";
        public const string CPU_FREE_BUCKS_BUTTON_AVAILABLE = "cpuFreeBucksButtonAvailable";

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

		public const string CPU_STORE_BUCK_PACKS_TITLE = "storeBuckPacksTitle";
		public const string CPU_STORE_BUCK_PACKS_SUB_HEADING = "storeBuckPacksSubHeading";
		public const string CPU_STORE_BUCK_PACKS_STORE_NOT_AVAILABLE = "storeBuckPacksStoreNotAvailable";

		#endregion

        #region Share

        public const string SHARE_STANDARD = "shareStandard";

        #endregion


        #region Bottom Nav

        public const string NAV_HOME = "navHome";
        public const string NAV_PROFILE = "navProfile";
        public const string NAV_SHOP = "navShop";
        public const string NAV_SETTINGS = "navSettings";

        #endregion
    }
}
