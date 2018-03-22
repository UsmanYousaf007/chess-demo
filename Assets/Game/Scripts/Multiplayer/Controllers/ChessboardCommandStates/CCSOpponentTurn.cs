/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 15:48:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;

namespace TurboLabz.MPChess
{
    public class CCSOpponentTurn : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            // If we're starting a new game
            if (CameFromState(cmd, typeof(CCSDefault)))
            {
                RenderNewGame(cmd, false);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
            // If a player has completed his move
            else if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                RenderPlayerMove(cmd);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
            else if (CameFromState(cmd, typeof(CCSPromoDialog)))
            {
                RenderPromo(cmd);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
            // If a player has clicked a non-player piece or empty square after
            // selecting a piece previously
            else if (CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                cmd.hidePlayerToIndicatorSignal.Dispatch();
            }
            // If a player has clicked a non-player piece or empty square after
            // this state has loaded
            else if (CameFromState(cmd, typeof(CCSOpponentTurn)) &&
                     (cmd.chessboardModel.playerFromSquare == null) &&
                     (cmd.chessboardModel.playerToSquare == null))
            {
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                cmd.hidePlayerToIndicatorSignal.Dispatch();
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.SQUARE_CLICKED)
            {
                ChessSquare clickedSquare = cmd.chessboardModel.clickedSquare;

                // See if a player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece))
                {
                    model.playerFromSquare = clickedSquare;
                    return new CCSOpponentTurnPieceSelected();
                }
                // If not, then we go back to the non selected opponent turn state
                else
                {
                    model.playerFromSquare = null;
                    model.playerToSquare = null;
                    return null;
                }
            }
            // The player has completed the move (including backend)
            else if (evt == ChessboardEvent.PLAYER_MOVE_COMPLETE)
            {
                // Nows when Ai takes its turn
                if (model.isAiGame)
                {
                    cmd.aiTurnSignal.Dispatch();
                }
                    
                return null;
            }
            // We received an opponent moved event from the backend service
            else if (evt == ChessboardEvent.OPPONENT_MOVE_COMPLETE)
            {
                HandleOpponentBackendMoved(cmd);

                if (model.gameEndReason != GameEndReason.NONE)
                {
                    HandleGameEnded(cmd);
                    return new CCSOpponentTurnCompletedGameEnded();
                }
                else
                {
                    cmd.receiveTurnSwapTimeControlSignal.Dispatch();

                    if (model.fiftyMoveDrawAvailable)
                    {
                        return new CCSFiftyMoveDrawOnOpponentTurnAvailable();
                    }
                    else if (model.threefoldRepeatDrawAvailable)
                    {
                        return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
                    }

                    return new CCSPlayerTurn();
                }
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
