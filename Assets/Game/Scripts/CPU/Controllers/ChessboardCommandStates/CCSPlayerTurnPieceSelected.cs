/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 21:44:17 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using TurboLabz.Chess;

namespace TurboLabz.CPU
{
    public class CCSPlayerTurnPieceSelected : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;

            // If we came here after an opponent has moved
            if (CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                RenderOpponentMove(cmd);

                cmd.showPlayerToIndicatorSignal.Dispatch(model.playerFromSquare);
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                ShowPossibleMoves(cmd);
            }
            // If we came here after a valid piece was clicked
            else if (CameFromState(cmd, typeof(CCSPlayerTurn)) ||
                     CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                cmd.showPlayerToIndicatorSignal.Dispatch(model.playerFromSquare);
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                ShowPossibleMoves(cmd);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;

            // Handle square clicked events
            if (evt == ChessboardEvent.SQUARE_CLICKED)
            {
                IChessboardModel model = cmd.chessboardModel;
                ChessSquare clickedSquare = model.clickedSquare;

                // See if a new player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece) &&
                    !clickedSquare.Equals(model.playerFromSquare))
                {
                    model.playerFromSquare = clickedSquare;
                    return this;
                }
                else if (IsValidMove(cmd, model.playerFromSquare, clickedSquare)) // Else see if we have made a valid move
                {
                    if (!model.opponentMoveRenderComplete)
                    {
                        return null;
                    }

                    model.playerToSquare = clickedSquare;

                    if (HasPawnReachedEnd(model))
                    {
                        // We set a temp move flag here because we render a temporary
                        // move of the pawn before we know the final promo selection
                        model.playerMoveFlag = ChessMoveFlag.STANDARD;
                        return new CCSPromoDialog();
                    }

                    return DoPlayerMove(cmd, null);
                }
                else // Invalid target or same target
                {
                    cmd.chessboardModel.playerFromSquare = null;
                    return new CCSPlayerTurn();
                }
            }
            else if (evt == ChessboardEvent.OPPONENT_MOVE_RENDER_COMPLETED)
            {
                OpponentMoveRenderCompleted(cmd);
                return null;
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                ProcessGameEndTimers(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }

        private void ShowPossibleMoves(ChessboardCommand cmd)
        {
            FileRank fromSquare = cmd.chessboardModel.playerFromSquare.fileRank;
            List<ChessSquare> possibleMoves = cmd.chessService.GetPossibleMoves(fromSquare, cmd.chessboardModel.squares);
            cmd.showPossibleMovesSignal.Dispatch(fromSquare, possibleMoves);
        }

        private bool IsValidMove(ChessboardCommand cmd, ChessSquare fromSquare, ChessSquare toSquare)
        {
            List<ChessSquare> possibleMoves = cmd.chessService.GetPossibleMoves(fromSquare.fileRank, cmd.chessboardModel.squares);

            foreach (ChessSquare square in possibleMoves)
            {
                if (toSquare.fileRank.file == square.fileRank.file && toSquare.fileRank.rank == square.fileRank.rank)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasPawnReachedEnd(IChessboardModel model)
        {
            ChessSquare fromSquare = model.playerFromSquare;
            ChessSquare toSquare = model.playerToSquare;

            string pieceName = fromSquare.piece.name;

            if ((pieceName.ToLower() == ChessPieceName.BLACK_PAWN) &&
                (toSquare.fileRank.rank == 0 || toSquare.fileRank.rank == 7))
            {
                return true;
            }

            return false;
        }
    }
}
