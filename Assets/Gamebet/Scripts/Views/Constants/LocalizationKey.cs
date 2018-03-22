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

namespace TurboLabz.Gamebet
{
    public static class LocalizationKey
    {
        // TODO(mubeeniqbal): All the client only keys must be enums instead of
        // strings. We're using string type only for the client-server keys
        // because keys that come in from the server are of string type thus we
        // have to conform to that.

        // TODO: Move out game keys to the game folder as a partial class

        // Client only keys
        //
        // To change the value on one of these you don't have to make changes
        // on the server side.

        #region Loading

        public const string LD_MESSAGE_LABEL = "ldMessageLabel";

        #endregion

        #region RetryConnection

        public const string RC_ERROR_CODE = "rCErrorLabel";

        #endregion

        #region Auth

        public const string AUTH_SELECT_LOGIN_LABEL = "authSelectLoginLabel";
        public const string AUTH_AUTH_GUEST_BUTTON_LABEL = "authAuthGuestButtonLabel";
        public const string AUTH_AUTH_FACEBOOK_BUTTON_LABEL = "authAuthFacebookButtonLabel";

        #endregion

        #region SetPlayerSocialName

        public const string SPSN_HEADING_LABEL = "spsnHeadingLabel";

        #endregion

        #region Lobby

        public const string LOBBY_PLAY_BUTTON_LABEL = "lobbyPlayButtonLabel";
        public const string CPU_BUTTON_LABEL = "cpuButtonLabel";
        public const string LEARN_BUTTON_LABEL = "learnButtonLabel";
        public const string LOBBY_FREE_CURRENCY_1_BUTTON_LABEL = "lobbyFreeCurrency1ButtonLabel";
        public const string LOBBY_FEEDBACK_BUTTON_LABEL = "lobbyFeedbackButtonLabel";
        public const string LOBBY_LEVEL = "lobbyLevel";

        #endregion

        #region PlayerProfile

        public const string PP_LEVEL_LABEL = "ppLevelLabel";
        public const string PP_XP_MAX_LEVEL_LABEL = "ppXpMaxLevelLabel";
        public const string PP_XP_LABEL = "ppXpLabel";
        public const string PP_CURRENCY_1_TITLE_LABEL = "ppCurrency1TitleLabel";
        public const string PP_CURRENCY_2_TITLE_LABEL = "ppCurrency2TitleLabel";
        public const string PP_CURRENCY_1_WINNINGS_TITLE_LABEL = "ppCurrency1WinningsTitleLabel";
        public const string PP_WIN_RATE_TITLE_LABEL = "ppWinRateTitleLabel";
        public const string PP_WIN_RATE_LABEL = "ppWinRateLabel";
        public const string PP_GAMES_WON_TITLE_LABEL = "ppGamesWonTitleLabel";
        public const string PP_GAMES_WON_LABEL = "ppGamesWonLabel";
        public const string PP_GAMES_LOST_TITLE_LABEL = "gamesLostTitleLabel";
        public const string PP_GAMES_LOST_LABEL = "ppGamesLostLabel";
        public const string PP_GAMES_DRAWN_TITLE_LABEL = "ppGamesDrawnTitleLabel";
        public const string PP_GAMES_DRAWN_LABEL = "ppGamesDrawnLabel";
        public const string PP_ROOM_TITLES_HEADER_LABEL = "ppRoomTitlesHeaderLabel";
        public const string PP_NO_ROOM_TITLES_LABEL = "ppNoRoomTitlesLabel";
        public const string PP_ROOM_DURATION_LABEL = "ppRoomDurationLabel";
        public const string PP_TROPHIES_WON_LABEL = "trophiesWonLabel";

        #endregion

        #region Shop

        public const string S_HEADING_LABEL = "sHeadingLabel";
        public const string S_COLLECTION_LABEL = "sCollectionLabel";
        public const string S_LOOT_LABEL = "sLootLabel";
        public const string S_LOOT_BOXES_LABEL = "sLootBoxesLabel";
        public const string S_AVATARS_LABEL = "sAvatarsLabel";
        public const string S_BORDER_LABEL = "sBorderLabel";
        public const string S_CHESS_SKINS_LABEL = "sChessSkinsLabel";
        public const string S_CHAT_LABEL = "sChatLabel";
        public const string S_CURRENCY_LABEL = "sCurrencyLabel";
        public const string S_FREE_CURRENCY_1_LABEL = "sFreeCurrency1Label";
        public const string S_AVATARS_BORDERS_LABEL = "sAvatarBorders";
        public const string S_BUCKS_COINS_LABEL = "sBucksCoins";

        public const string S_COMMON_LABEL = "sCommonLabel";
        public const string S_RARE_LABEL = "sRareLabel";
        public const string S_EPIC_LABEL = "sEpicLabel";
        public const string S_LEGENDARY_LABEL = "sLegendaryLabel";

        public const string S_BUCKS_LABEL = "sBucksLabel";
        public const string S_COINS_LABEL = "sCoinsLabel";

        public const string S_OWNED_LABEL = "sOwnedLabel";

        public const string S_BONUS_LABEL = "sBonusLabel";

        #endregion

        #region Inventory

        public const string I_INFO_LABEL = "iInfoLabel";
        public const string I_EQUIP_LABEL = "iEquipLabel";
        public const string I_EQUIPPED_LABEL = "iEquippedLabel";

        public const string I_DISMANTLE_DIALOGUE_LABEL = "iDismantleDIALOGUELabel";
        public const string I_BUILD_BUTTON_LABEL = "iBuildButtonLabel";
        public const string I_DISMANTLE_BUTTON_LABEL = "iDismantleButtonLabel";
        public const string I_COLLECT_BUTTON_LABEL = "iCollectButtonLabel";

        public const string I_DIVIDER_LABEL = "iDividerLabel";
        public const string I_X_LABEL = "iXLabel";
        public const string I_NOT_ENOUGH_CARDS_LABEL = "iNotEnoughCardsLabel";
        public const string I_OWNED_Label = "iOwnedLabel";
        public const string I_EMPTY_LOOT_SCREEN_LABEL = "iEmptyLootScreenLabel";

        #endregion

        #region ShopModals
        public const string S_M_BUY_FOR_LABEL = "sMBuyForLabel";
        public const string S_M_NOT_ENOUGH_BUCKS_LABEL = "sMNotEnoughBucksLabel";
        public const string S_M_YES_LABEL = "sMYes";
        public const string S_M_NO_LABEL = "sMNo";
        public const string S_M_OWNED_LABEL = "sMOwnedLabel";
        public const string S_M_ADD_TO_COLLECTION_LABEL = "sMAddToCollectionLabel";
        public const string S_M_VIEW_COLLECTION_LABEL = "sMViewCollectionLabel";
        public const string S_M_CANCEL_LABEL = "sMCancelLabel";

        public const string S_M_PURCHASE_CHESS_SKIN_FOR_LABEL = "sMPurchaseChessSkinForLabel";
        public const string S_M_BOUGHT_CHESS_SKIN_LABEL = "sMBoughtChessSkinLabel";

        public const string S_M_PURCHASE_AVATARS_FOR_LABEL = "sMPurchaseAvatarsForLabel";
        public const string S_M_BOUGHT_AVATARS_LABEL = "sMBoughtAvatarsLabel";

        public const string S_M_PURCHASE_AVATARS_BORDER_FOR_LABEL = "sMPurchaseAvatarsBorderForLabel";
        public const string S_M_BOUGHT_AVATARS_BORDER_LABEL = "sMBoughtAvatarsBorderLabel";

        public const string S_M_PURCHASE_CURRENCY_FOR_LABEL = "sMPurchaseCurrencyForLabel";

        public const string S_M_ADD_TO_LOOT_LABEL = "sMAddToLootLabel";

        public const string S_M_BOUGHT_LOOT_BOX_LABEL = "sMBoughtLootBoxLabel";

        public const string S_M_BONUS_LABEL = "sMBonusLabel";
        #endregion

        #region ShopIds

        public const string COMMON = "Common";
        public const string RARE = "Rare";
        public const string EPIC = "Epic";
        public const string LEGENDARY = "Legendary";

        public const string BUCKS = "bucks";
        public const string COINS = "coins";
        #endregion

        #region Rooms

        public const string ROOMS_FREE_CURRENCY_1_BUTTON_LABEL = "roomsFreeCurrency1ButtonLabel";
        public const string ROOMS_GAME_DURATION_1_TOGGLE_LABEL = "roomsGameDuration1ToggleLabel";
        public const string ROOMS_GAME_DURATION_2_TOGGLE_LABEL = "roomsGameDuration2ToggleLabel";
        public const string ROOMS_GAME_DURATION_3_TOGGLE_LABEL = "roomsGameDuration3ToggleLabel";

        #endregion

        #region RoomCard

        public const string RC_ROOM_DURATION_LABEL = "rcRoomDurationLabel";
        public const string RC_PRIZE_TITLE_LABEL = "rcPrizeTitleLabel";
        public const string RC_ENTRY_FEE_TITLE_LABEL = "rcEntryFeeTitleLabel";
        public const string RC_ZERO_TROPHY_COUNT_LABEL = "rcZeroTrophyCountLabel";
        public const string RC_TROPHY_COUNT_LABEL = "rcTrophyCountLabel";
        public const string RC_WINS_NEEDED_TITLE_LABEL = "rcWinsNeededTitleLabel";
        public const string RC_TROPHY_WINS_NEEDED_LABEL = "rcTrophyWinsNeededLabel";
        public const string RC_UNLOCK_LEVEL_LABLE = "rcUnlockLevelLabel";
        public const string RC_MYSTERY_CARD_LABLE = "rcMysteryCardLabel";

        public const string RC_PRIZE_DESCRIPTION_LABEL = "rcPrizeDescriptionLabel";
        public const string RC_2X = "rc2x";
        public const string RC_MINIMUM_ENTRY_FEE_TITLE_LABEL = "rcMinimumEntryFeeTitleLabel";

        public const string RC_REWARDED_GOALS_LABEL = "rcRewardedGoalsLabel";
        public const string RC_PROGRESS_LABEL = "rcProgressLabel";
        public const string RC_REMAINING_DAYS_HOURS_LABEL = "rcRemainingDaysHoursLabel";
        public const string RC_REMAINING_HOURS_MINUTES_LABEL = "rcRemainingHoursMinutesLabel";
        public const string RC_REMAINING_MINUTES_SECONDS_LABEL = "rcRemainingMinutesSecondsLabel";
        public const string RC_REMAINING_SECONDS_LABEL = "rcRemainingSecondsLabel";
        public const string RC_TIME_UP_LABEL = "rcTimeUpLabel";

        public const string RC_NEXT_ROOM_LABEL = "rcNextRoomLabel";

        #endregion

        #region RoomInfoCard

        public const string RIC_RULES_LABEL = "ricRulesLabel";
        public const string RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_1 = "ricTrophiesNeededForRoomTitle1";
        public const string RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_2 = "ricTrophiesNeededForRoomTitle2";
        public const string RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_3 = "ricTrophiesNeededForRoomTitle3";

        #endregion

        #region Matchmaking

        public const string MM_ROOM_DURATION = "mmRoomDuration";
        public const string MM_PLAYER_LEVEL = "mmPlayerLevel";
        public const string MM_OPPONENT_LEVEL = "mmOpponentLevel";
        public const string MM_PRIZE_TITLE_LABEL = "mmPrizeTitleLabel";

        #endregion

        #region EndGame

        public const string EG_ROOM_DURATION_LABEL = "egRoomNameSubtitle";

        public const string EG_RESULTS_PLAYER_WON_LABEL = "egPlayerResultsLabelWon";
        public const string EG_PLAYER_RESULTS_LABEL_LOST = "egPlayerResultsLabelLost";
        public const string EG_RESULTS_DRAWN_LABEL = "egPlayerResultsLabelDrawn";

        public const string EG_RESULTS_OPPONENT_WON_LABEL = "egOpponentResultsLabelWon";
        public const string EG_OPPONENT_RESULTS_LABEL_LOST = "egOpponentResultsLabelLost";
        public const string EG_OPPONENT_RESULTS_LABEL_DRAWN = "egOpponentResultsLabelDrawn";

        public const string EG_PLAYER_LEVEL_LABEL = "egPlayerLevel";
        public const string EG_OPPONENT_LEVEL_LABEL = "egOpponentLevel";
        public const string EG_PRIZE_TITLE_LABEL = "egPrizeTitleLabel";

        public const string EG_REMATCH_BUTTON_LABEL = "egRematchButtonLabel";
        public const string EG_NEW_MATCH_BUTTON_LABEL = "egNewMatchButtonLabel";

        #endregion

        #region Promotions

        public const string LVLP_PROMOTION_TITLE_LABEL = "lvlpPromotionTitleLabel";
        public const string LVLP_REWARD_TITLE_LABEL = "lvlpRewardTitleLabel";
        public const string LVLP_NEXT_PROMOTION_MESSAGE_LABEL = "lvlpNextPromotionMessageLabel";
        public const string LVLP_CONTINUE_BUTTON_LABEL = "lvlpContinueButtonLabel";

        public const string LGP_PROMOTION_TITLE_LABEL = "lgpPromotionTitleLabel";
        public const string LGP_NEXT_PROMOTION_MESSAGE_LABEL = "lgpNextPromotionMessageLabel";
        public const string LGP_CONTINUE_BUTTON_LABEL = "lgpContinueButtonLabel";

        public const string TP_ROOM_DURATION_LABEL = "tpRoomDurationLabel";
        public const string TP_PROMOTION_TITLE_LABEL = "tpPromotionTitleLabel";
        public const string TP_TROPHY_COUNT_TITLE_LABEL = "tpTrophyCountTitleLabel";
        public const string TP_CONTINUE_BUTTON_LABEL = "tpContinueButtonLabel";

        public const string RTP_ROOM_DURATION_LABEL = "rtpRoomDurationLabel";
        public const string RTP_PROMOTION_TITLE_LABEL = "rtpPromotionTitleLabel";
        public const string RTP_REWARD_TITLE_LABEL = "rtpRewardTitleLabel";
        public const string RTP_CONTINUE_BUTTON_LABEL = "rtpContinueButtonLabel";

        #endregion
        
        #region IapModal

        public const string IAP_ATTEMPT_LABEL = "iapAttemptLabel";
        public const string IAP_FAILURE_LABEL = "iapFailureLabel";

        #endregion

        // TODO: Separate out game specific stuff into a separate file.
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
        public const string GM_RESULT_DIALOG_REASON_RESIGNATION = "gmResultDialogReasonResignation";
        public const string GM_DRAW_DIALOG_HEADING = "gmDrawDialogHeading";
        public const string GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE = "gmDrawDialogClaimByFiftyMoveRule";
        public const string GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE = "gmDrawDialogClaimByThreefoldRepeatRule";
        public const string GM_DRAW_DIALOG_YES_BUTTON = "gmDrawDialogYesButton";
        public const string GM_DRAW_DIALOG_NO_BUTTON = "gmDrawDialogNoButton";
        public const string GM_ROOM_PRIZE = "gmRoomPrize";
        public const string GM_ROOM_DURATION = "gmRoomDuration";
        public const string GM_PLAYER_LEVEL = "gmPlayerLevel";
        public const string GM_WAITING_FOR_OPPONENT = "gmWaitingForOpponent";

        #endregion

        // Client-server keys
        //
        // To change the value on one of these you also have to make changes
        // on the server side. The keys of the server and client must match.

        #region SharedKeys

        public const string NONE = "none";

        #endregion

        #region LeaguesIds

        public const string LEAGUE_1 = "league1";
        public const string LEAGUE_2 = "league2";
        public const string LEAGUE_3 = "league3";
        public const string LEAGUE_4 = "league4";
        public const string LEAGUE_5 = "league5";
        public const string LEAGUE_6 = "league6";
        public const string LEAGUE_7 = "league7";
        public const string LEAGUE_8 = "league8";
        public const string LEAGUE_9 = "league9";
        public const string LEAGUE_10 = "league10";

        #endregion

        #region RoomTitles

        public const string ROOM_TITLE_NONE = "roomTitleNone";
        public const string ROOM_TITLE_1 = RoomTitleId.ROOM_TITLE_1;
        public const string ROOM_TITLE_2 = RoomTitleId.ROOM_TITLE_2;
        public const string ROOM_TITLE_3 = RoomTitleId.ROOM_TITLE_3;

        #endregion

        #region RoomIds

        public const string BULLET = "bullet";
        public const string BLITZ = "blitz";
        public const string CLASSIC = "classic";
        public const string ALLINONE = "allInOne";

        public const string CUBA = "cuba";
        public const string ENGLAND = "england";
        public const string INDIA = "india";
        public const string GERMANY = "germany";
        public const string SPAIN = "spain";
        public const string FRANCE = "france";
        public const string HUNGARY = "hungary";
        public const string UKRAINE = "ukraine";
        public const string CHINA = "china";
        public const string USA = "usa";
        public const string RUSSIA = "russia";

        #endregion

        #region Currency1ShopItemIds

        public const string CURRENCY_1_1 = "coins1";
        public const string CURRENCY_1_2 = "coins2";
        public const string CURRENCY_1_3 = "coins3";
        public const string CURRENCY_1_4 = "coins4";
        public const string CURRENCY_1_5 = "coins5";
        public const string CURRENCY_1_6 = "coins6";

        #endregion

        #region Currency2ShopItemIds

        public const string CURRENCY_2_1 = "bucks1";
        public const string CURRENCY_2_2 = "bucks2";
        public const string CURRENCY_2_3 = "bucks3";
        public const string CURRENCY_2_4 = "bucks4";
        public const string CURRENCY_2_5 = "bucks5";
        public const string CURRENCY_2_6 = "bucks6";

        #endregion

        #region CoachShopItemIds

        public const string COACH_1 = "coach1";
        public const string COACH_2 = "coach2";
        public const string COACH_3 = "coach3";
        public const string COACH_4 = "coach4";

        #endregion

        #region PromotionIds

        public const string TWO_X = "2x";
        public const string POPULAR = "popular";
        public const string BEST_DEAL = "bestDeal";

        #endregion

        #region CPUMenu

        public const string CPU_MENU_HEADING = "cpuMenuHeading";
        public const string CPU_MENU_STRENGTH = "cpuMenuStrength";
        public const string CPU_MENU_DURATION = "cpuMenuDuration";
        public const string CPU_MENU_DURATION_NONE = "cpuMenuDurationNone";
        public const string CPU_MENU_PLAYER_COLOR = "cpuMenuPlayerColor";
        public const string CPU_MENU_PLAY = "cpuMenuPlay";
        public const string CPU_MENU_STATS = "cpuMenuStats";
        public const string CPU_GAME_CPU_NAME = "cpuGameCpuName";
        public const string CPU_GAME_CPU_STRENGTH = "cpuGameCpuStrength";
        public const string CPU_GAME_RESIGN_BUTTON = "cpuGameResignButton";
        public const string CPU_GAME_UNDO_BUTTON = "cpuGameUndoButton";
        public const string CPU_GAME_HINT_BUTTON = "cpuGameHintButton";
        public const string CPU_GAME_EXIT_BUTTON = "cpuGameExitButton";
        public const string CPU_GAME_CONTINUE_BUTTON = "cpuGameContinueButton";
        public const string CPU_GAME_EXIT_EXPLAINATION = "cpuGameExitExplanation";
        public const string CPU_RESULTS_STATS_BUTTON = "cpuResultsStatsButton";
        public const string CPU_RESULTS_EXIT_BUTTON = "cpuResultsExitButton";

        #endregion
    }
}
