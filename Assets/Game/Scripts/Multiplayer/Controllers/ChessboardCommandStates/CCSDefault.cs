/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 14:30:36 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class CCSDefault : CCS
    {
        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;
            IChessService chessService = cmd.chessService;
            IChessAiService chessAiService = cmd.chessAiService;

            if (cmd.chessboardEvent == ChessboardEvent.GAME_STARTED)
            {
                if (chessboard.overrideFen != null)
                {
                    chessService.NewGame(chessboard.fen, chessboard.squares);
                }
                else
                {
                    chessService.NewGame(chessboard.squares);
                    ProcessResume(cmd);
                }

                // Initialize the Ai service
                if (chessboard.isAiGame)
                {
                    if (chessboard.overrideAiResignBehaviour == AiOverrideResignBehaviour.ALWAYS)
                    {
                        chessboard.aiWillResign = true;    
                    }
                    else if (chessboard.overrideAiResignBehaviour == AiOverrideResignBehaviour.NEVER)
                    {
                        chessboard.aiWillResign = false;
                    }
                    else 
                    {
                        chessboard.aiWillResign = (UnityEngine.Random.Range(0, 100) < BotSettings.AI_RESIGN_CHANCE);
                    }

                    chessAiService.NewGame();
                }

                // Wait for player turn or execute Ai turn
                if (chessboard.isPlayerTurn)
                {
                    if (chessboard.threefoldRepeatDrawAvailable)
                    {
                        return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
                    }
                    else if (chessboard.fiftyMoveDrawAvailable)
                    {
                        return new CCSFiftyMoveDrawOnOpponentTurnAvailable();
                    }

                    return new CCSPlayerTurn();
                }
                else
                {
                    if (chessboard.isAiGame)
                    {
                        cmd.aiTurnSignal.Dispatch();
                    }

                    return new CCSOpponentTurn();
                }
            }

            return null;
        }

        /*
         * Loop through all the moves and update the following stateful entities:
         * 1) The chess service 
         * 2) The chessboard model
         * 3) The moveVOs that are sent to the view
         */
        private void ProcessResume(ChessboardCommand cmd)
        {
            IChessService chessService = cmd.chessService;
            IChessAiService chessAiService = cmd.chessAiService;
            Chessboard chessboard = cmd.activeChessboard;

            chessboard.moveVOCache = new List<MoveVO>();
            chessboard.aiMoveNumber = 0;

            bool isPlayerTurn = (chessboard.playerColor == chessService.GetNextMoveColor());

            LogUtil.Log("PROCESSING RESUME. MOVE LIST COUNT = " + chessboard.moveList.Count, "white");

            foreach (ChessMove move in chessboard.moveList)
            {
                ChessMoveResult moveResult = chessService.MakeMove(move.from,
                                                 move.to, 
                                                 move.promo, 
                                                 isPlayerTurn, 
                                                 chessboard.squares);

                if (isPlayerTurn)
                {
                    chessboard.playerMoveFlag = moveResult.moveFlag;
                    chessboard.playerFromSquare = chessboard.squares[move.from.file, move.from.rank];
                    chessboard.playerToSquare = chessboard.squares[move.to.file, move.to.rank];

                    ChessMove lastPlayerMove = new ChessMove();
                    lastPlayerMove.from = chessboard.playerFromSquare.fileRank;
                    lastPlayerMove.to = chessboard.playerToSquare.fileRank;
                    lastPlayerMove.piece = chessboard.playerFromSquare.piece;
                    lastPlayerMove.promo = GetPromoFromMove(chessboard.playerMoveFlag);
                    chessboard.lastPlayerMove = lastPlayerMove;
                }
                else
                {
                    chessboard.opponentMoveFlag = moveResult.moveFlag;
                    chessboard.opponentFromSquare = chessboard.squares[move.from.file, move.from.rank];
                    chessboard.opponentToSquare = chessboard.squares[move.to.file, move.to.rank];
                    ++chessboard.aiMoveNumber;
                }

                chessboard.isPlayerInCheck = moveResult.isPlayerInCheck;
                chessboard.isOpponentInCheck = moveResult.isOpponentInCheck;
                chessboard.playerScore = cmd.chessService.GetScore(chessboard.playerColor);
                chessboard.opponentScore = cmd.chessService.GetScore(chessboard.opponentColor);
                chessboard.notation.Add(moveResult.description);
                chessboard.capturedSquare = moveResult.capturedSquare;
                chessboard.threefoldRepeatDrawAvailable = moveResult.isThreefoldRepeatRuleActive;
                chessboard.fiftyMoveDrawAvailable = moveResult.isFiftyMoveRuleActive;

                MoveVO vo = GetMoveVO(chessboard, isPlayerTurn);
                chessboard.moveVOCache.Add(vo);

                isPlayerTurn = !isPlayerTurn;
            }
        }
    }
}
