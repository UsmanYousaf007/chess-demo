/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using TurboLabz.Chess;

namespace TurboLabz.InstantChess
{
    public class ChessboardModel : IChessboardModel
    {
        public CCS currentState { get; set; }
        public CCS previousState { get; set; }
        public ChessSquare[,] squares { get; set; }
        public ChessSquare playerFromSquare { get; set; }
        public ChessSquare playerToSquare { get; set; }
        public ChessMoveFlag playerMoveFlag { get; set; }
        public ChessSquare opponentFromSquare { get; set; }
        public ChessSquare opponentToSquare { get; set; }
        public ChessMoveFlag opponentMoveFlag { get; set; }
        public string promoString { get; set; }
        public ChessSquare capturedSquare { get; set; }
        public int playerScore { get; set; }
        public int opponentScore { get; set; }
        public bool isPlayerInCheck { get; set; }
        public bool isOpponentInCheck { get; set; }
        public bool fiftyMoveDrawAvailable { get; set; }
        public bool threefoldRepeatDrawAvailable { get; set; }
        public ChessSquare clickedSquare { get; set; }
        public string selectedPromo { get; set; }
        public bool isPlayerTurn { get; set; }
        public TimeSpan playerTimer { get; set; }
        public TimeSpan opponentTimer { get; set; }
        public TimeSpan gameDuration { get; set; }
        public GameEndReason gameEndReason { get; set; }
        public string winnerId { get; set; }
        public int aiMoveNumber { get; set; }
        public ChessMove lastPlayerMove { get; set; }
        public AiMoveDelay aiMoveDelay { get; set; }
        public ChessColor playerColor { get; set; }
        public ChessColor opponentColor { get; set; }
        public bool aiWillResign { get; set; }
        public bool opponentMoveRenderComplete { get; set; }
        public bool timersStopped { get; set; }
        public List<string> notation { get; set; }
        public List<ChessMove> moveList { get; set; }
        public List<MoveVO> moveVOCache { get; set; }
        public float defaultMoveDelay { get; set; }
        public int availableHints { get; set; }
        public bool usedHelp { get; set; }
        public bool isUndo { get; set; }

        public void Reset()
        {
            currentState = null;
            previousState = null;
            squares = new ChessSquare[8, 8];
            playerFromSquare = null;
            playerToSquare = null;
            playerMoveFlag = ChessMoveFlag.NONE;
            opponentFromSquare = null;
            opponentToSquare = null;
            opponentMoveFlag = ChessMoveFlag.NONE;
            capturedSquare = null;
            playerScore = 0;
            opponentScore = 0;
            isPlayerInCheck = false;
            isOpponentInCheck = false;
            fiftyMoveDrawAvailable = false; 
            threefoldRepeatDrawAvailable = false;
            clickedSquare = null;
            selectedPromo = null;
            isPlayerTurn = false;
            playerTimer = TimeSpan.Zero;
            opponentTimer = TimeSpan.Zero;
            gameDuration = TimeSpan.Zero;
            gameEndReason = GameEndReason.NONE;
            winnerId = null;
            aiMoveNumber = 0;
            lastPlayerMove = null;
            aiMoveDelay = AiMoveDelay.CPU;
            playerColor = ChessColor.NONE;
            opponentColor = ChessColor.NONE;
            aiWillResign = false;
            opponentMoveRenderComplete = false;
            timersStopped = false;
            notation = new List<string>();
            moveList = new List<ChessMove>();
            moveVOCache = new List<MoveVO>();
            availableHints = CPUSettings.DEFAULT_HINT_COUNT;
            usedHelp = false;
            isUndo = false;
        }
    }
}
