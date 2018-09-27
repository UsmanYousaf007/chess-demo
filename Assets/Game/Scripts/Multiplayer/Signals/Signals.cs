/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-16 06:14:00 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;

using strange.extensions.signal.impl;
using TurboLabz.Chess;

namespace TurboLabz.Multiplayer
{
    // Command signals
    public class ChessboardEventSignal : Signal<ChessboardEvent> {}
    public class SquareClickedSignal : Signal<FileRank> {}
    public class BackendPlayerTurnSignal : Signal<PlayerTurnVO> {}

    public class RunTimeControlSignal : Signal {}
    public class PromoSelectedSignal : Signal<string> {}
    public class ClaimDrawSignal : Signal {}
    public class RejectDrawSignal : Signal {}
    public class ClaimFiftyMoveDrawSignal : Signal {}
    public class ClaimThreefoldRepeatDrawSignal : Signal {}
    public class ShowFiftyMoveDrawDialogSignal : Signal {}
    public class ShowThreefoldRepeatDrawDialogSignal : Signal {}
    public class HideDrawDialogSignal : Signal {}

    public class ResignSignal : Signal {}
    public class AiTurnSignal : Signal {}

    // Command to command signals
    public class TakeTurnSwapTimeControlSignal : Signal {}
    public class ReceiveTurnSwapTimeControlSignal : Signal {}

    // View update signals
    public class SetupChessboardSignal : Signal<SetupChessboardVO> {}
    public class UpdateChessboardSignal : Signal<ChessSquare[,]> {}
    public class InitTimersSignal : Signal<InitTimerVO> {}
    public class UpdatePlayerTimerSignal : Signal<TimeSpan> {}
    public class UpdateOpponentTimerSignal : Signal<TimeSpan> {}
    public class StopTimersSignal : Signal {}
    public class PlayerTimerExpiredSignal : Signal {}
    public class OpponentTimerExpiredSignal : Signal {}
    public class UpdateResultDialogSignal : Signal<ResultsVO> {}
    public class ShowPossibleMovesSignal : Signal<FileRank, List<ChessSquare>> {}
    public class HidePossibleMovesSignal : Signal {}
    public class ShowPlayerFromIndicatorSignal : Signal<ChessSquare> {}
    public class ShowPlayerToIndicatorSignal : Signal<ChessSquare> {}
    public class HidePlayerFromIndicatorSignal : Signal {}
    public class HidePlayerToIndicatorSignal : Signal {}
    public class ShowOpponentFromIndicatorSignal : Signal<ChessSquare> {}
    public class ShowOpponentToIndicatorSignal : Signal<ChessSquare> {}
    public class HideOpponentFromIndicatorSignal : Signal {}
    public class HideOpponentToIndicatorSignal : Signal {}
    public class UpdateOpponentMoveSignal : Signal<MoveVO> {}
    public class UpdatePlayerMoveSignal : Signal<MoveVO> {}
    public class UpdatePlayerPrePromoMoveSignal : Signal<MoveVO> {}
    public class UpdatePromoDialogSignal : Signal<ChessColor> {}
    public class HidePromoDialogSignal : Signal {}
    public class UpdatePromoSignal : Signal<MoveVO> {}
    public class EnablePlayerTurnInteractionSignal : Signal {}
    public class EnableOpponentTurnInteractionSignal : Signal {}
    public class UpdateMoveForResumeSignal : Signal<MoveVO, bool> {}
    public class InitInfiniteTimersSignal : Signal<bool> {}
    public class ExitLongMatchSignal : Signal {}
    public class ResetActiveMatchSignal : Signal {}
    public class EnableGameChatSignal : Signal<bool> {}
}
