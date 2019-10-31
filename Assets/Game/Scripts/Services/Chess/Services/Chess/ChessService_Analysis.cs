/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 09:35:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using ChessboardLib;

namespace TurboLabz.Chess
{
    public partial class ChessService
    {
        public bool IsSquareDefended(FileRank attackDestination, ChessColor defendingPlayerColor)
        {
            Player defender = (defendingPlayerColor == ChessColor.BLACK) ? Game.PlayerBlack : Game.PlayerWhite;
            Square square = Board.GetSquare(attackDestination.file, attackDestination.rank);

            // Now see if the defending player pieces are protecting this square
            Moves defendingMoves = new Moves();
            square.AttackersMoveList(defendingMoves, defender);

            if (defendingMoves.Count > 0)
            {
                return true;
            }
                
            return false;
        }

        public bool WillSquareBeDefendedAfterMove(FileRank attackOrigin,
                                                  FileRank attackDestination,
                                                  string promo, 
                                                  ChessColor defendingPlayerColor)
        {
            Player defender = (defendingPlayerColor == ChessColor.BLACK) ? Game.PlayerBlack : Game.PlayerWhite;

            // First make the move
            Move targetMove = GetTargetMove(attackOrigin, attackDestination, promo);
            Move moveUndo = targetMove.Piece.Move(targetMove.Name, targetMove.To);
           
            // Now see if the defending player pieces are protecting this square
            Moves defendingMoves = new Moves();
            targetMove.To.AttackersMoveList(defendingMoves, defender);

            if (defendingMoves.Count > 0)
            {
                Move.Undo(moveUndo);
                return true;
            }

            Move.Undo(moveUndo);
            return false;
        }

        public ChessMove GetCheapestAttackingMoveToSquareAfterMove(FileRank from,
                                                                   FileRank to,
                                                                   string promo)
        {

            // First make the move
            Move targetMove = GetTargetMove(from, to, promo);
            Move moveUndo = targetMove.Piece.Move(targetMove.Name, targetMove.To);

            // Now get the cheapest attacking move to the square
            ChessMove cheapestAttackingMove = GetCheapestAttackingMoveToSquare(to);
            Move.Undo(moveUndo);
            return cheapestAttackingMove;
        }

        public ChessMove GetCheapestAttackingMoveToSquare(FileRank location)
        {
            Square square = Board.GetSquare(location.file, location.rank);
            Moves moves = new Moves();
            square.AttackersMoveList(moves, square.Piece.Player.OpposingPlayer);

            if (moves.Count == 0)
            {
                return null;
            }

            Moves legalMoves = new Moves();

            foreach (Move potentialMove in moves)
            {
                if (!CausesCheck(potentialMove))
                {
                    legalMoves.Add(potentialMove);
                }
            }

            if (legalMoves.Count == 0)
            {
                return null;
            }

            moves = null; // This is important because we only want to use legal moves from hereon.

            int cheapestIndex = -1;
            int cheapestValue = int.MaxValue;

            for (int i = 0; i < legalMoves.Count; ++i)
            {
                int thisValue = legalMoves[i].Piece.Value;

                if (thisValue < cheapestValue)
                {
                    cheapestIndex = i;
                    cheapestValue = thisValue;
                }
            }

            Move targetMove = legalMoves[cheapestIndex];
            ChessMove chessMove = CreateChessMoveFromMove(targetMove);

            return chessMove;
        }

        public List<ChessMove> GetCaptureMoves(FileRank attackOrigin)
        {
            Moves moves = GetMoves(attackOrigin);
            List<ChessMove> captureMoves = new List<ChessMove>();

            foreach(Move move in moves)
            {
                if (move.PieceCaptured != null)
                {
                    captureMoves.Add(CreateChessMoveFromMove(move));
                }
            }

            return captureMoves;
        }

        public ChessPiece GetPieceAtLocation(FileRank location)
        {
            Square square = Board.GetSquare(location.file, location.rank);

            if (square == null)
            {
                return null;
            }

            if (square.Piece == null)
            {
                return null;
            }

            ChessPiece piece = new ChessPiece();
            piece.name = GetChessPieceName(square.Piece);
            piece.color = (square.Piece.Player.Colour == Player.PlayerColourNames.White) ? ChessColor.WHITE : ChessColor.BLACK;


            return piece;
        }

        public bool WillMoveCauseWeakExchangeOrFeed(FileRank from, FileRank to, string promo)
        {
            bool weakExchangeOrFeed = false;

            // Before anything, see if we are capturing a more valuable or equal piece
            Piece toPiece = Board.GetPiece(to.file, to.rank);

            if (toPiece != null)
            {
                Piece fromPiece = Board.GetPiece(from.file, from.rank);

                if (fromPiece.Value < toPiece.Value)
                {
                    return false;
                }
            }

            // Make the move
            Move targetMove = GetTargetMove(from, to, promo);
            Move moveUndo = targetMove.Piece.Move(targetMove.Name, targetMove.To);

            Player player = targetMove.Piece.Player;

            // Store local copy of player pieces since this list can change
            // when testing out moves and undoing them
            Pieces playerPieces = new Pieces();

            foreach(Piece piece in player.Pieces)
            {
                playerPieces.Add(piece);
            }
           
            // Now go through the player pieces and analyse the ones under attack
            foreach (Piece playerPiece in playerPieces)
            {
                // Get the cheapest attacking move to this square
                FileRank pieceLocation;
                pieceLocation.file = playerPiece.Square.File;
                pieceLocation.rank = playerPiece.Square.Rank;

                ChessMove cheapestAttackMove = GetCheapestAttackingMoveToSquare(pieceLocation);

                // No one is attacking this piece, move to the next piece
                if (cheapestAttackMove == null)
                {
                    continue;
                }

                // We are under attack, get all the pieces defending this piece
                Moves defendingMoves = new Moves();
                playerPiece.Square.AttackersMoveList(defendingMoves, player);

                // No one is defending this piece, this is a feed
                if (defendingMoves.Count == 0)
                {
                    weakExchangeOrFeed = true;
                    break;
                }

                // Something is defending this piece so an exchange is possible.
                // Check if this exchange is worth it.
                Piece cheapestAttackingPiece = Board.GetPiece(cheapestAttackMove.from.file, cheapestAttackMove.from.rank);

                if (cheapestAttackingPiece.Value < playerPiece.Value)
                {
                    weakExchangeOrFeed = true;
                    break;
                }
            }

            Move.Undo(moveUndo);
            return weakExchangeOrFeed;
        }

        public List<FileRank> GetProfitableCapturesAvailable(ChessColor attackerColor)
        {
            Player attacker = (attackerColor == ChessColor.BLACK) ? Game.PlayerBlack : Game.PlayerWhite;

            // Store local copy of player pieces since this list can change
            // when testing out moves and undoing them
            Pieces defenderPieces = new Pieces();

            foreach(Piece piece in attacker.OpposingPlayer.Pieces)
            {
                defenderPieces.Add(piece);
            }

            List<FileRank> profitableCaptures = new List<FileRank>();

            foreach (Piece defenderPiece in defenderPieces)
            {
                bool profitable = false;

                // Get the cheapest attacking move to this square
                FileRank pieceLocation;
                pieceLocation.file = defenderPiece.Square.File;
                pieceLocation.rank = defenderPiece.Square.Rank;

                ChessMove cheapestAttackMove = GetCheapestAttackingMoveToSquare(pieceLocation);

                // If I'm attacking the defender piece...
                if (cheapestAttackMove != null)
                {
                    // If no one is defending this piece, then it qualifies as profitable
                    Moves movesDefendingPiece = new Moves();
                    defenderPiece.Square.AttackersMoveList(movesDefendingPiece, attacker.OpposingPlayer);

                    if (movesDefendingPiece.Count == 0)
                    {
                        profitable = true;
                    }
                    // This is being defended, so lets compare the piece values and see
                    // if the exchange is worth it
                    else
                    {
                        Piece cheapestAttackingPiece = Board.GetPiece(cheapestAttackMove.from.file, cheapestAttackMove.from.rank);  

                        if (cheapestAttackingPiece.Value < defenderPiece.Value)
                        {
                            profitable = true;
                        }
                    }

                    if (profitable)
                    {
                        FileRank fileRank;
                        fileRank.file = defenderPiece.Square.File;
                        fileRank.rank = defenderPiece.Square.Rank;
                        profitableCaptures.Add(fileRank);
                    }
                }
            }
                
            return profitableCaptures;
        }

        public int GetPieceCount(ChessColor color)
        {
            if (color == ChessColor.BLACK)
            {
                return Game.PlayerBlack.Pieces.Count;
            }
            else
            {
                return Game.PlayerWhite.Pieces.Count;
            }
        }

        private ChessMove CreateChessMoveFromMove(Move targetMove)
        {
            bool isWhitePiece = (targetMove.Piece.Player.Colour == Player.PlayerColourNames.White) ? true : false;
            ChessMove chessMove = new ChessMove();
            FileRank from, to;
            from.file = targetMove.From.File;
            from.rank = targetMove.From.Rank;
            to.file = targetMove.To.File;
            to.rank = targetMove.To.Rank;
            chessMove.from = from;
            chessMove.to = to;
            ChessPiece piece = new ChessPiece();
            piece.name = GetChessPieceName(targetMove.Piece);
            piece.color = isWhitePiece ? ChessColor.WHITE : ChessColor.BLACK;
            chessMove.piece = piece;
            chessMove.promo = null;

            if (targetMove.IsPromotion())
            { 
                if (targetMove.Name == Move.MoveNames.PawnPromotionQueen)
                {
                    chessMove.promo = isWhitePiece ? ChessPieceName.WHITE_QUEEN : ChessPieceName.BLACK_QUEEN;
                }
                else if (targetMove.Name == Move.MoveNames.PawnPromotionRook)
                {
                    chessMove.promo = isWhitePiece ? ChessPieceName.WHITE_ROOK : ChessPieceName.BLACK_ROOK;
                }
                else if (targetMove.Name == Move.MoveNames.PawnPromotionBishop)
                {
                    chessMove.promo = isWhitePiece ? ChessPieceName.WHITE_BISHOP : ChessPieceName.BLACK_BISHOP;
                }
                else if (targetMove.Name == Move.MoveNames.PawnPromotionKnight)
                {
                    chessMove.promo = isWhitePiece ? ChessPieceName.WHITE_KNIGHT : ChessPieceName.BLACK_KNIGHT;
                }
            }

            return chessMove;
        }

        private Piece GetPlayerPieceByName(Player player, Piece.PieceNames name)
        {
            Pieces pieces = player.Pieces;

            foreach (Piece piece in pieces)
            {
                if (piece.Name == name)
                {
                    return piece;
                }
            }

            return null;
        }

        private bool CausesCheck(Move move)
        {
            Move moveUndo = move.Piece.Move(move.Name, move.To);
            bool causesCheck = move.Piece.Player.IsInCheck;
            Move.Undo(moveUndo);
            return causesCheck;
        }
    }
}
