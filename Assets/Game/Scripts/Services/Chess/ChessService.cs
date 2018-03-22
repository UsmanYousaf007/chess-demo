/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:26:18 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;

using ChessboardLib;

using TurboLabz.Common;

namespace TurboLabz.Chess
{
    public partial class ChessService : IChessService
    {
        private Dictionary<Move.MoveNames, ChessMoveFlag> moveNameMap = new Dictionary<Move.MoveNames, ChessMoveFlag>() {
            { Move.MoveNames.Standard, ChessMoveFlag.STANDARD },
            { Move.MoveNames.CastleQueenSide, ChessMoveFlag.CASTLE_QUEEN_SIDE },
            { Move.MoveNames.CastleKingSide, ChessMoveFlag.CASTLE_KING_SIDE },
            { Move.MoveNames.PawnPromotionQueen, ChessMoveFlag.PAWN_PROMOTION_QUEEN },
            { Move.MoveNames.PawnPromotionRook, ChessMoveFlag.PAWN_PROMOTION_ROOK },
            { Move.MoveNames.PawnPromotionBishop, ChessMoveFlag.PAWN_PROMOTION_BISHOP },
            { Move.MoveNames.PawnPromotionKnight, ChessMoveFlag.PAWN_PROMOTION_KNIGHT },
            { Move.MoveNames.EnPassent, ChessMoveFlag.EN_PASSANT }
        };

        private static char[] FILE_MAP = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};

        public void NewGame(string fen, ChessSquare[,] squares)
        {
            Game.New(fen);
            UpdateChessboard(squares);
        }

        public void NewGame(ChessSquare[,] squares)
        {
            Game.New();
            UpdateChessboard(squares);
        }

        public ChessColor GetNextMoveColor()
        {
            return (Game.PlayerToPlay.Colour == Player.PlayerColourNames.Black) ? ChessColor.BLACK : ChessColor.WHITE;
        }

        public List<ChessSquare> GetPossibleMoves(FileRank fromSquare, ChessSquare[,] squares)
        {
            Moves moves = GetMoves(fromSquare);

            List<ChessSquare> possibleMoves = new List<ChessSquare>();

            foreach(Move move in moves)
            {
                possibleMoves.Add(squares[move.To.File, move.To.Rank]);
            }

            return possibleMoves;
        }

        public ChessMoveResult MakeMove(FileRank fromSquare, FileRank toSquare, string promo, bool isPlayerTurn, ChessSquare[,] squares)
        {
            Move targetMove = GetTargetMove(fromSquare, toSquare, promo);

            Assertions.Assert(targetMove != null, "Invalid chess move.");

            ChessMoveResult moveResult = new ChessMoveResult();

            // We get the captured piece info before making the move because the piece
            // references cease to exist once its captured.
            if (targetMove.PieceCaptured != null)
            {
                Square square = targetMove.PieceCaptured.Square;

                ChessSquare capturedSquare = new ChessSquare();
                ChessSquare chessboardSquare = squares[square.File, square.Rank];
                capturedSquare.fileRank = chessboardSquare.fileRank;
                capturedSquare.piece = new ChessPiece();
                capturedSquare.piece.color = chessboardSquare.piece.color;
                capturedSquare.piece.name = chessboardSquare.piece.name;
                moveResult.capturedSquare =  capturedSquare;
            }
            else
            {
                moveResult.capturedSquare = null;
            }

            Game.MakeAMove(targetMove.Name, targetMove.From.Piece, targetMove.To);
            UpdateChessboard(squares);

            moveResult.moveFlag = moveNameMap[targetMove.Name];
            moveResult.isFiftyMoveRuleActive = targetMove.IsFiftyMoveDraw;

            if (!isPlayerTurn)
            {
                moveResult.isPlayerInCheck = targetMove.Piece.Player.OpposingPlayer.IsInCheck;
                moveResult.isOpponentInCheck = false;
            }
            else
            {
                moveResult.isOpponentInCheck = targetMove.Piece.Player.OpposingPlayer.IsInCheck;
                moveResult.isPlayerInCheck = false;
            }

            moveResult.isThreefoldRepeatRuleActive = targetMove.Piece.Player.TLCanClaimMoveRepetitionDraw();

            // Has the game ended due to the move?
            if (Game.PlayerToPlay.Status == Player.PlayerStatusNames.InCheckMate)
            {
                moveResult.gameEndReason = GameEndReason.CHECKMATE;
            }
            else if (Game.PlayerToPlay.Status == Player.PlayerStatusNames.InStalemate)
            {
                moveResult.gameEndReason = GameEndReason.STALEMATE;
            }
            else if (Game.PlayerBlack.CanClaimInsufficientMaterialDraw ||
                     Game.PlayerWhite.CanClaimInsufficientMaterialDraw)
            {
                moveResult.gameEndReason = GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL;
            }
            else
            {
                moveResult.gameEndReason = GameEndReason.NONE;
            }

            moveResult.description = targetMove.TLDescription;

            return moveResult;
        }

        public string GetFen()
        {
            return Fen.GetBoardPosition();
        }

        public int GetScore(ChessColor color)
        {
            if (color == ChessColor.BLACK)
            {
                return (Game.PlayerBlack.CapturedEnemyPiecesTotalBasicValue - Game.PlayerWhite.CapturedEnemyPiecesTotalBasicValue);
            }
            else
            {
                return (Game.PlayerWhite.CapturedEnemyPiecesTotalBasicValue - Game.PlayerBlack.CapturedEnemyPiecesTotalBasicValue);
            }
        }

        public FileRank GetFileRankLocation(char file, char rank)
        {
            FileRank fileRank;
            fileRank.file = Array.IndexOf(FILE_MAP, file);
            fileRank.rank = int.Parse(rank.ToString()) - 1;

            return fileRank;
        }

        public string GetAlgebraicLocation(FileRank fileRank)
        {
            return FILE_MAP[fileRank.file].ToString() + (fileRank.rank + 1).ToString();
        }

        private Move GetTargetMove(FileRank fromSquare, FileRank toSquare, string promo)
        {
            Moves moves = GetMoves(fromSquare);

            // Find the move in question
            Move targetMove = null;
            foreach(Move move in moves)
            {
                if (move.To.File == toSquare.file && move.To.Rank == toSquare.rank)
                {
                    if (promo != null)
                    {
                        if ((promo.ToLower() == ChessPieceName.BLACK_QUEEN && move.Name == Move.MoveNames.PawnPromotionQueen) ||
                            (promo.ToLower() == ChessPieceName.BLACK_ROOK && move.Name == Move.MoveNames.PawnPromotionRook) ||
                            (promo.ToLower() == ChessPieceName.BLACK_BISHOP && move.Name == Move.MoveNames.PawnPromotionBishop) ||
                            (promo.ToLower() == ChessPieceName.BLACK_KNIGHT && move.Name == Move.MoveNames.PawnPromotionKnight))
                        {
                            targetMove = move;
                            break;
                        }
                    }
                    else
                    {
                        targetMove = move;
                        break;
                    }
                }
            }

            return targetMove;
        }

        private Moves GetMoves(FileRank sourceSquare)
        {
            Square square = Board.GetSquare(sourceSquare.file, sourceSquare.rank);
            Piece piece = square.Piece;
            Assertions.Assert(piece != null, "No piece found on square:" + sourceSquare.file + "x" + sourceSquare.rank);
            Moves moves = new Moves();
            piece.GenerateLegalMoves(moves);
            return moves;
        }

        private void UpdateChessboard(ChessSquare[,] squares)
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    Piece piece = Board.GetSquare(file, rank).Piece;
                    string pieceName = null;

                    if (piece != null)
                    {   
                        pieceName = piece.Abbreviation.ToUpper();
                        if (piece.Player.Colour == Player.PlayerColourNames.Black)
                        {
                            pieceName = pieceName.ToLower();
                        }
                    }

                    ChessSquare square = new ChessSquare();
                    square.fileRank.file = file;
                    square.fileRank.rank = rank;

                    if (piece != null)
                    {
                        ChessPiece chessPiece = new ChessPiece();
                        chessPiece.name = pieceName;
                        chessPiece.color = (piece.Player.Colour == Player.PlayerColourNames.Black) ? ChessColor.BLACK : ChessColor.WHITE;
                        square.piece = chessPiece;
                    }
                    else
                    {
                        square.piece = null;
                    }

                    squares[file, rank] = square;
                }
            }
        }

        private string GetChessPieceName(Piece piece)
        {
            string name = null;
            bool isWhite = (piece.Player.Colour == Player.PlayerColourNames.White);

            if (piece.Name == Piece.PieceNames.Bishop)
            {
                name = isWhite ? ChessPieceName.WHITE_BISHOP : ChessPieceName.BLACK_BISHOP;
            }
            else if (piece.Name == Piece.PieceNames.King)
            {
                name = isWhite ? ChessPieceName.WHITE_KING : ChessPieceName.BLACK_KING;
            }
            else if (piece.Name == Piece.PieceNames.Knight)
            {
                name = isWhite ? ChessPieceName.WHITE_KNIGHT : ChessPieceName.BLACK_KNIGHT;
            }
            else if (piece.Name == Piece.PieceNames.Pawn)
            {
                name = isWhite ? ChessPieceName.WHITE_PAWN : ChessPieceName.BLACK_PAWN;
            }
            else if (piece.Name == Piece.PieceNames.Queen)
            {
                name = isWhite ? ChessPieceName.WHITE_QUEEN : ChessPieceName.BLACK_QUEEN;
            }
            else if (piece.Name == Piece.PieceNames.Rook)
            {
                name = isWhite ? ChessPieceName.WHITE_ROOK : ChessPieceName.BLACK_ROOK;
            }

            return name;
        }
    }
}
