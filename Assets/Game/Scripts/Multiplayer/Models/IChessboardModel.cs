/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:36:26 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using TurboLabz.Chess;

namespace TurboLabz.Multiplayer
{
    public interface IChessboardModel
    {
        CCS currentState { get; set; }
        CCS previousState { get; set; }
        ChessSquare[,] squares { get; set; }
        ChessSquare playerFromSquare { get; set; }
        ChessSquare playerToSquare { get; set; }
        ChessMoveFlag playerMoveFlag { get; set; }
        ChessSquare opponentFromSquare { get; set; }
        ChessSquare opponentToSquare { get; set; }
        ChessMoveFlag opponentMoveFlag { get; set; }
        ChessSquare capturedSquare { get; set; }
        int playerScore { get; set; }
        int opponentScore { get; set; }
        bool isPlayerInCheck { get; set; }
        bool isOpponentInCheck { get; set; }
        bool fiftyMoveDrawAvailable { get; set; }
        bool threefoldRepeatDrawAvailable { get; set; }
        ChessSquare clickedSquare { get; set; }
        string selectedPromo { get; set; }
        string currentTurnPlayerId { get; set; }
        TimeSpan backendPlayerTimer { get; set; }
        TimeSpan backendOpponentTimer { get; set; }
        TimeSpan gameDuration { get; set; }
        GameEndReason gameEndReason { get; set; }
        string winnerId { get; set; }
        bool isAiGame { get; set; }
        int aiMoveNumber { get; set; }
        ChessMove lastPlayerMove { get; set; }
        ChessColor playerColor { get; set; }
        ChessColor opponentColor { get; set; }
        bool aiWillResign { get; set; }
        int opponentLevel { get; set; }
        bool opponentMoveRenderComplete { get; set; }
        string fen { get; set; }
        string overrideFen { get; set; }
        AiOverrideStrength overrideAiStrength { get; set; }
        AiOverrideResignBehaviour overrideAiResignBehaviour { get; set; }
        bool timersStopped { get; set; }
        List<string> notation { get; set; }
        List<ChessMove> backendMoveList { get; set; }
        List<MoveVO> resumeMoves { get; set; }
        bool inPlaybackMode { get; set; }

        void Reset();
    }
}
