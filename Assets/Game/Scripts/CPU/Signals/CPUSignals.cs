/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 18:33:07 UTC+05:00
/// 
/// @description
/// [add_description_here]
using strange.extensions.signal.impl;
using System;
using TurboLabz.Chess;
using System.Collections.Generic;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess
{
    // CPU LOBBY
    public class SaveGameSignal : Signal {}
    public class UpdateMenuViewSignal : Signal<CPULobbyVO> {}
    public class AdjustStrengthSignal : Signal<bool> {}
    public class AdjustDurationSignal : Signal<bool> {}
    public class AdjustPlayerColorSignal : Signal<bool> {}
	public class AdjustThemeSignal : Signal<bool> {}
    public class UpdateStrengthSignal : Signal<CPULobbyVO> {}
    public class UpdateDurationSignal : Signal<CPULobbyVO> {}
    public class UpdatePlayerColorSignal : Signal<CPULobbyVO> {}
	public class UpdateThemeSignal : Signal<CPULobbyVO> {}
    public class LoadCPUGameSignal : Signal {}
    public class ShareAppSignal : Signal {}
    public class ShowAdSignal : Signal {}
    public class UpdateAdsSignal : Signal<AdsVO>{}

	// PLAYER
	public class SavePlayerSignal : Signal {}

    // CPU STATS
    public class LoadStatsSignal : Signal {}
    public class UpdateStatsDurationSignal : Signal<string> {}
    public class SaveStatsSignal : Signal<int> {}
    public class UpdateStatsSignal : Signal<CPUStatsVO> {};
    public class EnterPlaybackSignal : Signal {};

	// CPU STORE
	public class LoadStoreSignal : Signal {}
	public class UpdateStoreSignal : Signal<CPUStoreVO> {}
	public class PurchaseStoreItemSignal : Signal<string, bool> {}
	public class UpdateStoreBuyDlgSignal : Signal<StoreItem> {}
	public class UpdateStoreNotEnoughBucksDlgSignal : Signal<StoreItem> {}
	public class UpdateStoreBuckPacksDlgSignal : Signal<CPUStoreVO> {}
	public class LoadBuckPacksSignal : Signal {}

    // CPU GAME
    public class StartNewGameSignal : Signal {}
    public class DevFenValueChangedSignal : Signal<string> {}
    public class ChessboardEventSignal : Signal<ChessboardEvent> {}
    public class SetupChessboardSignal : Signal<SetupChessboardVO> {}
    public class UpdateChessboardSignal : Signal<ChessSquare[,]> {}
    public class AiTurnSignal : Signal {}
    public class ShowPlayerFromIndicatorSignal : Signal<ChessSquare> {}
    public class ShowPlayerToIndicatorSignal : Signal<ChessSquare> {}
    public class HidePlayerFromIndicatorSignal : Signal {}
    public class HidePlayerToIndicatorSignal : Signal {}
    public class ShowPossibleMovesSignal : Signal<FileRank, List<ChessSquare>> {}
    public class HidePossibleMovesSignal : Signal {}
    public class UpdateOpponentMoveSignal : Signal<MoveVO> {}
    public class UpdatePlayerMoveSignal : Signal<MoveVO> {}
    public class UpdatePlayerPrePromoMoveSignal : Signal<MoveVO> {}
    public class ShowFiftyMoveDrawDialogSignal : Signal {}
    public class ShowThreefoldRepeatDrawDialogSignal : Signal {}
    public class HideDrawDialogSignal : Signal {}
    public class UpdateResultDialogSignal : Signal<GameEndReason, bool> {}
    public class UpdatePromoDialogSignal : Signal<ChessColor> {}
    public class UpdatePromoSignal : Signal<MoveVO> {}
    public class RunTimeControlSignal : Signal {}
    public class TakeTurnSwapTimeControlSignal : Signal {}
    public class ReceiveTurnSwapTimeControlSignal : Signal {}
    public class StopTimersSignal : Signal {}
    public class PauseTimersSignal : Signal {}
    public class ResumeTimersSignal : Signal {}
    public class EnablePlayerTurnInteractionSignal : Signal {}
    public class EnableOpponentTurnInteractionSignal : Signal {}
    public class UpdateMoveForResumeSignal : Signal<MoveVO, bool> {}
    public class UpdateUndoButtonSignal : Signal<bool, int> {}
    public class DisableUndoButtonSignal : Signal {}
    public class DisableMenuButtonSignal : Signal {}
    public class DisableHintButtonSignal : Signal {}
    public class SquareClickedSignal : Signal<FileRank> {}
    public class PromoSelectedSignal : Signal<string> {}
    public class ClaimDrawSignal : Signal {}
    public class RejectDrawSignal : Signal {}
    public class ResignSignal : Signal {}
    public class InitTimersSignal : Signal<InitTimerVO> {}
    public class InitInfiniteTimersSignal : Signal<bool> {}
    public class UpdatePlayerTimerSignal : Signal<TimeSpan> {}
    public class UpdateOpponentTimerSignal : Signal<TimeSpan> {}
    public class PlayerTimerExpiredSignal : Signal {}
    public class OpponentTimerExpiredSignal : Signal {}
    public class ShowOpponentFromIndicatorSignal : Signal<ChessSquare> {}
    public class ShowOpponentToIndicatorSignal : Signal<ChessSquare> {}
    public class HideOpponentFromIndicatorSignal : Signal {}
    public class HideOpponentToIndicatorSignal : Signal {}
    public class UndoMoveSignal : Signal {}
    public class RenderHintSignal : Signal<HintVO> {}
    public class GetHintSignal : Signal {}
    public class UpdateHintCountSignal : Signal<int> {}
    public class TurnSwapSignal : Signal<bool> {}
    public class UpdateGameInfoSignal : Signal<GameInfoVO> {}
    public class EnableResultsDialogButtonSignal : Signal {}
    public class LoadLobbySignal : Signal {}

    // SKINS
    public class ApplySkinSignal : Signal<string> {}
    public class UpdateSkinSignal : Signal {}

}
