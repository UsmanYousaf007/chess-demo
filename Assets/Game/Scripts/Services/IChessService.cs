/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:27:56 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.Chess
{
    public interface IChessService
    {
        void NewGame(string fen, ChessSquare[,] squares);
        void NewGame(ChessSquare[,] squares);
        List<ChessSquare> GetPossibleMoves(FileRank fromSquare, ChessSquare[,] squares);
        ChessMoveResult MakeMove(FileRank fromSquare, FileRank toSquare, string promo, bool isPlayerTurn, ChessSquare[,] squares);
        string GetFen();
        string GetAlgebraicLocation(FileRank fileRank);
        FileRank GetFileRankLocation(char file, char rank);
        ChessColor GetNextMoveColor();

        // Analysis API
        bool WillSquareBeDefendedAfterMove(FileRank attackOrigin, FileRank attackDestination, string promo, ChessColor defendingPlayerColor);
        ChessMove GetCheapestAttackingMoveToSquareAfterMove(FileRank from, FileRank to, string promo);
        bool IsSquareDefended(FileRank attackDestination, ChessColor defendingPlayerColor);
        ChessMove GetCheapestAttackingMoveToSquare(FileRank location);
        List<ChessMove> GetCaptureMoves(FileRank attackOrigin);
        ChessPiece GetPieceAtLocation(FileRank location);
        int GetPieceCount(ChessColor color);
        List<FileRank> GetProfitableCapturesAvailable(ChessColor attackerColor);
        bool WillMoveCauseWeakExchangeOrFeed(FileRank from, FileRank to, string promo);
        int GetScore(ChessColor color);
    }
}
