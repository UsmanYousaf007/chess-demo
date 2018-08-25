/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-21 16:11:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class CCSThreefoldRepeatDrawOnOpponentTurnAvailablePieceSelected : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                RenderOpponentMove(cmd);
                cmd.showPlayerToIndicatorSignal.Dispatch(cmd.activeChessboard.playerFromSquare);
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
            }
            else if (CameFromState(cmd, typeof(CCSThreefoldRepeatDrawOnOpponentTurnAvailablePieceSelected)))
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.DRAW_CLAIMED)
            {   
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                cmd.claimThreefoldRepeatDrawSignal.Dispatch();
                return new CCSDrawClaimedOnOpponentTurn();
            }
            else if (evt == ChessboardEvent.DRAW_REJECTED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new CCSPlayerTurnPieceSelected();
            }
            else if (evt == ChessboardEvent.OPPONENT_MOVE_RENDER_COMPLETED)
            {
                cmd.activeChessboard.opponentMoveRenderComplete = true;
                return new CCSThreefoldRepeatDrawOnOpponentTurnAvailablePieceSelected();
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }
    }
}
