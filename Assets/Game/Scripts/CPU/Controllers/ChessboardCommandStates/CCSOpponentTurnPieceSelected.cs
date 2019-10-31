/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 22:29:29 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;

namespace TurboLabz.CPU
{
    public class CCSOpponentTurnPieceSelected : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSOpponentTurn)) ||
                CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                cmd.showPlayerToIndicatorSignal.Dispatch(cmd.chessboardModel.playerFromSquare);
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
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

                // See if a new player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece) &&
                    !clickedSquare.Equals(model.playerFromSquare))
                {
                    model.playerFromSquare = clickedSquare;
                    return this;
                }
                // If not, then we go back to the non selected player turn state
                else
                {
                    model.playerFromSquare = null;
                    return new CCSOpponentTurn();
                }
            }
            // If offboard was clicked
            else if (evt == ChessboardEvent.OFFBOARD_CLICKED)
            {
                model.playerFromSquare = null;
                return new CCSOpponentTurn();
            }
            // We got a confirmation of the player move being completed
            else if (evt == ChessboardEvent.PLAYER_MOVE_COMPLETE)
            {
                DoAiMove(cmd);
                return null;
            }
            // We received an opponent moved event from the backend service
            else if (evt == ChessboardEvent.OPPONENT_MOVE_COMPLETE)
            {
                if (model.gameEndReason != GameEndReason.NONE)
                {
                    ProcessGameEndTimers(cmd);
                    return new CCSAnnounceResults();
                }
                else
                {
                    // The opponent move could have deselected our piece due to a capture.
                    // In that case switch to the unselected state.
                    if (model.playerFromSquare == null)
                    {
                        return new CCSPlayerTurn();
                    }
                    else
                    {
                        return new CCSPlayerTurnPieceSelected();
                    }
                }
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                ProcessGameEndTimers(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }
    }
}
