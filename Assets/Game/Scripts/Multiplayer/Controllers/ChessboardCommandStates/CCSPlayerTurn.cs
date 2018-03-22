/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 15:48:15 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;

namespace TurboLabz.MPChess
{
    public class CCSPlayerTurn : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            // If we're starting a new game
            if (CameFromState(cmd, typeof(CCSDefault)))
            {
                RenderNewGame(cmd, true);
                cmd.enablePlayerTurnInteraction.Dispatch();
            }
            // If we came here after an opponent has moved
            else if (CameFromState(cmd, typeof(CCSOpponentTurn)) ||  
                     CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                RenderOpponentMove(cmd);
                cmd.enablePlayerTurnInteraction.Dispatch();
            }
            // If we came here after an invalid target square was clicked
            else if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                cmd.hidePlayerToIndicatorSignal.Dispatch();
                cmd.hidePossibleMovesSignal.Dispatch();
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;
            ChessboardEvent evt = cmd.chessboardEvent;

            // Handle square clicked events
            if (evt == ChessboardEvent.SQUARE_CLICKED)
            {
                ChessSquare clickedSquare = model.clickedSquare;

                // See if a player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece))
                {
                    model.playerFromSquare = clickedSquare;
                    return new CCSPlayerTurnPieceSelected();
                }
            }
            else if (evt == ChessboardEvent.OPPONENT_MOVE_RENDER_COMPLETED)
            {
                cmd.chessboardModel.opponentMoveRenderComplete = true;
                return null;
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
