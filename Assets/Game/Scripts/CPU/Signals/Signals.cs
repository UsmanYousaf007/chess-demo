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

namespace TurboLabz.CPU
{
    // CPU GAME
    public class SaveGameSignal : Signal {}
    public class LoadGameSignal : Signal {}
    public class StartCPUGameSignal : Signal<bool> {}
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
    public class UpdateResultDialogSignal : Signal<GameEndReason, bool, int, bool> {}
    public class UpdatePromoDialogSignal : Signal<ChessColor> {}
    public class UpdatePromoSignal : Signal<MoveVO> {}
    public class AutoQueenPromoSignal : Signal<ChessColor> { }
    public class RunTimeControlSignal : Signal {}
    public class TakeTurnSwapTimeControlSignal : Signal {}
    public class ReceiveTurnSwapTimeControlSignal : Signal {}
    public class StopTimersSignal : Signal {}
    public class PauseTimersSignal : Signal {}
    public class ResumeTimersSignal : Signal {}
    public class EnablePlayerTurnInteractionSignal : Signal {}
    public class EnableOpponentTurnInteractionSignal : Signal {}
    public class UpdateMoveForResumeSignal : Signal<MoveVO, bool> {}
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
    public class RenderHintSignal : Signal<HintVO> {}
    public class CancelHintSingal : Signal { }
    public class GetHintSignal : Signal<bool> {}
    public class ToggleSafeModeSignal : Signal { }
    public class SafeMoveSignal : Signal<bool> { }
    public class UpdateHintCountSignal : Signal<int> {}
    public class UpdateHindsightCountSignal : Signal<int> { }
    public class UpdateSafeMoveCountSignal : Signal<int> { }
    public class UpdateSafeMoveStateSignal : Signal<bool> { }
    public class TurnSwapSignal : Signal<bool> {}
    public class UpdateGameInfoSignal : Signal<GameInfoVO> {}
    public class EnableResultsDialogButtonSignal : Signal {}
    public class HindsightAvailableSignal : Signal<bool> { }
    public class HintAvailableSignal : Signal<bool> { }
    public class DisableUndoBtnSignal : Signal<bool> { }
    public class StepSignal : Signal<bool> { }
    public class ToggleStepForwardSignal : Signal<bool> { }
    public class ToggleStepBackwardSignal : Signal<bool> { }
    public class OnboardingTooltipSignal : Signal<MoveVO> { }
    public class ShowStrengthOnboardingTooltipSignal : Signal<bool> { }
    public class ShowCoachOnboardingTooltipSignal : Signal<bool> { }
    public class SetupSpecialHintSignal : Signal<SpecialHintVO> { }
    public class SpecialHintAvailableSignal : Signal<bool> { }
}
