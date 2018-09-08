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
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class Chessboard
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
        public TimeSpan backendPlayerTimer { get; set; }
        public TimeSpan backendOpponentTimer { get; set; }
        public TimeSpan gameDuration { get; set; }
        public GameEndReason gameEndReason { get; set; }
        public bool isAiGame { get; set; }
        public int aiMoveNumber { get; set; }
        public ChessMove lastPlayerMove { get; set; }
        public ChessColor playerColor { get; set; }
        public ChessColor opponentColor { get; set; }
        public bool aiWillResign { get; set; }
        public int opponentLevel { get; set; }
        public bool opponentMoveRenderComplete { get; set; }
        public string fen { get; set; }
        public bool timersStopped { get; set; }
        public List<string> notation { get; set; }
        public List<ChessMove> moveList { get; set; }
        public List<MoveVO> moveVOCache { get; set; }
        public bool inPlaybackMode { get; set; }
        public DateTime lastMoveTime { get; set; }

        // Overrides
        public string overrideFen { get; set; }
        public AiOverrideStrength overrideAiStrength { get; set; }
        public AiOverrideResignBehaviour overrideAiResignBehaviour { get; set; }

        public Chessboard()
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
            backendPlayerTimer = TimeSpan.Zero;
            backendOpponentTimer = TimeSpan.Zero;
            gameDuration = TimeSpan.Zero;
            gameEndReason = GameEndReason.NONE;
            isAiGame = false;
            aiMoveNumber = 0;
            lastPlayerMove = null;
            playerColor = ChessColor.NONE;
            opponentColor = ChessColor.NONE;
            aiWillResign = false;
            opponentLevel = 0;
            opponentMoveRenderComplete = false;
            fen = null;
            timersStopped = false;
            notation = new List<string>();
            moveList = new List<ChessMove>();
            moveVOCache = new List<MoveVO>();
            inPlaybackMode = false;
            lastMoveTime = DateTime.UtcNow;

            overrideFen = null;
            overrideAiStrength = AiOverrideStrength.NONE;
            overrideAiResignBehaviour = AiOverrideResignBehaviour.NONE;
        }
    }

    public class ChessboardModel : IChessboardModel
    {
        public Dictionary<string, Chessboard> chessboards { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            chessboards = new Dictionary<string, Chessboard>();
        }

        public Chessboard AddChessboard(string challengeId)
        {
            Chessboard chessboard = new Chessboard();
            chessboards.Add(challengeId, chessboard);
            return chessboard;
        }
    }
}
