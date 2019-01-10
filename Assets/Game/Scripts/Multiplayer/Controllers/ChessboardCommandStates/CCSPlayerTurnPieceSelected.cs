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

namespace TurboLabz.Multiplayer
{
    public class CCSPlayerTurnPieceSelected : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;

            // If we came here after an opponent has moved
            if (CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                RenderOpponentMove(cmd);

                cmd.showPlayerToIndicatorSignal.Dispatch(chessboard.playerFromSquare);
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                ShowPossibleMoves(cmd);
            }
            // If we came here after a valid piece was clicked
            else if (CameFromState(cmd, typeof(CCSPlayerTurn)) ||
                     CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                cmd.showPlayerToIndicatorSignal.Dispatch(chessboard.playerFromSquare);
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                ShowPossibleMoves(cmd);
            }
            // We came from one of the opponent move draw states which means that
            // the opponent move was already rendered
            else if (CameFromState(cmd, typeof(CCSFiftyMoveDrawOnOpponentTurnAvailablePieceSelected)) ||
                     CameFromState(cmd, typeof(CCSThreefoldRepeatDrawOnOpponentTurnAvailablePieceSelected)))
            {
                ShowPossibleMoves(cmd);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;

            // Handle square clicked events
            if (evt == ChessboardEvent.SQUARE_CLICKED)
            {
                Chessboard chessboard = cmd.activeChessboard;
                ChessSquare clickedSquare = chessboard.clickedSquare;

                // See if a new player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece) &&
                    !clickedSquare.Equals(chessboard.playerFromSquare))
                {
                    chessboard.playerFromSquare = clickedSquare;
                    return this;
                }
                else if (IsValidMove(cmd, chessboard.playerFromSquare, clickedSquare)) // Else see if we have made a valid move
                {
                    if (!chessboard.opponentMoveRenderComplete)
                    {
                        return null;
                    }

                    chessboard.playerToSquare = clickedSquare;

                    if (HasPawnReachedEnd(chessboard))
                    {
                        // We set a temp move flag here because we render a temporary
                        // move of the pawn before we know the final promo selection
                        chessboard.playerMoveFlag = ChessMoveFlag.STANDARD;
                        return new CCSPromoDialog();
                    }

                    return HandlePlayerMove(cmd, null);
                }
                else // Invalid target or same target
                {
                    cmd.activeChessboard.playerFromSquare = null;
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
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }

        private void ShowPossibleMoves(ChessboardCommand cmd)
        {
            FileRank fromSquare = cmd.activeChessboard.playerFromSquare.fileRank;
            List<ChessSquare> possibleMoves = cmd.chessService.GetPossibleMoves(fromSquare, cmd.activeChessboard.squares);
            cmd.showPossibleMovesSignal.Dispatch(fromSquare, possibleMoves);
        }

        private bool IsValidMove(ChessboardCommand cmd, ChessSquare fromSquare, ChessSquare toSquare)
        {
            List<ChessSquare> possibleMoves = cmd.chessService.GetPossibleMoves(fromSquare.fileRank, cmd.activeChessboard.squares);

            foreach (ChessSquare square in possibleMoves)
            {
                if (toSquare.fileRank.file == square.fileRank.file && toSquare.fileRank.rank == square.fileRank.rank)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasPawnReachedEnd(Chessboard chessboard)
        {
            ChessSquare fromSquare = chessboard.playerFromSquare;
            ChessSquare toSquare = chessboard.playerToSquare;

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
