/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class CCSSafeMoveDialog : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                RenderPlayerMove(cmd);
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_SAFE_MOVE_DLG);
            }
            else if (CameFromState(cmd, typeof(CCSPromoDialog)))
            {
                RenderPromo(cmd);
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_SAFE_MOVE_DLG);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;
            Chessboard chessboard = cmd.activeChessboard;

            if (evt == ChessboardEvent.MOVE_CONFIRMED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return ConfirmPlayerMove(cmd, chessboard);
            }
            else if (evt == ChessboardEvent.MOVE_UNDO)
            {
                cmd.resetCapturedPiecesSignal.Dispatch();
                cmd.chessService.NewGame(chessboard.squares);
                ProcessResume(cmd);
                RenderNewGame(cmd, true, true);
                cmd.enablePlayerTurnInteraction.Dispatch();
                return new CCSPlayerTurn();
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }
    }
}
