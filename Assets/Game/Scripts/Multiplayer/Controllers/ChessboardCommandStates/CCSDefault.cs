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
            IChessboardModel chessboardModel = cmd.chessboardModel;
            IMatchInfoModel matchInfoModel = cmd.matchInfoModel;
            IChessService chessService = cmd.chessService;
            IChessAiService chessAiService = cmd.chessAiService;

            if (cmd.chessboardEvent == ChessboardEvent.GAME_STARTED)
            {
                if (!matchInfoModel.activeMatch.isResuming || chessboardModel.overrideFen != null)
                {
                    chessService.NewGame(chessboardModel.fen, chessboardModel.squares);
                }
                else
                {
                    LogUtil.Log("NORMAL GAME RESUME.", "red");

                    chessService.NewGame(chessboardModel.squares);
                    bool isPlayerTurn = (chessboardModel.playerColor == chessService.GetNextMoveColor());
                    chessboardModel.resumeMoves = new List<MoveVO>();
                    chessboardModel.aiMoveNumber = 0;

                    foreach (ChessMove move in chessboardModel.backendMoveList)
                    {
                        ChessMoveResult moveResult = chessService.MakeMove(move.from, move.to, move.promo, isPlayerTurn, chessboardModel.squares);

                        if (isPlayerTurn)
                        {
                            chessboardModel.playerMoveFlag = moveResult.moveFlag;
                            chessboardModel.playerFromSquare = chessboardModel.squares[move.from.file, move.from.rank];
                            chessboardModel.playerToSquare = chessboardModel.squares[move.to.file, move.to.rank];

                            ChessMove lastPlayerMove = new ChessMove();
                            lastPlayerMove.from = chessboardModel.playerFromSquare.fileRank;
                            lastPlayerMove.to = chessboardModel.playerToSquare.fileRank;
                            lastPlayerMove.piece = chessboardModel.playerFromSquare.piece;
                            lastPlayerMove.promo = GetPromoFromMove(chessboardModel.playerMoveFlag);
                            chessboardModel.lastPlayerMove = lastPlayerMove;
                        }
                        else
                        {
                            chessboardModel.opponentMoveFlag = moveResult.moveFlag;
                            chessboardModel.opponentFromSquare = chessboardModel.squares[move.from.file, move.from.rank];
                            chessboardModel.opponentToSquare = chessboardModel.squares[move.to.file, move.to.rank];

                            if (chessboardModel.isAiGame)
                            {
                                ++chessboardModel.aiMoveNumber;
                            }
                        }

                        chessboardModel.isPlayerInCheck = moveResult.isPlayerInCheck;
                        chessboardModel.isOpponentInCheck = moveResult.isOpponentInCheck;
                        chessboardModel.playerScore = cmd.chessService.GetScore(chessboardModel.playerColor);
                        chessboardModel.opponentScore = cmd.chessService.GetScore(chessboardModel.opponentColor);
                        chessboardModel.notation.Add(moveResult.description);
                        chessboardModel.capturedSquare = moveResult.capturedSquare;
                        chessboardModel.threefoldRepeatDrawAvailable = moveResult.isThreefoldRepeatRuleActive;
                        chessboardModel.fiftyMoveDrawAvailable = moveResult.isFiftyMoveRuleActive;

                        MoveVO vo = GetMoveVO(chessboardModel, isPlayerTurn);
                        chessboardModel.resumeMoves.Add(vo);

                        isPlayerTurn = !isPlayerTurn;
                    }
                }

                // Initialize the Ai service
                if (chessboardModel.isAiGame)
                {
                    if (chessboardModel.overrideAiResignBehaviour == AiOverrideResignBehaviour.ALWAYS)
                    {
                        chessboardModel.aiWillResign = true;    
                    }
                    else if (chessboardModel.overrideAiResignBehaviour == AiOverrideResignBehaviour.NEVER)
                    {
                        chessboardModel.aiWillResign = false;
                    }
                    else 
                    {
                        chessboardModel.aiWillResign = (UnityEngine.Random.Range(0, 100) < BotSettings.AI_RESIGN_CHANCE);
                    }

                    chessAiService.NewGame();
                }

                // Wait for player turn or execute Ai turn
                if (chessboardModel.currentTurnPlayerId == cmd.playerModel.id)
                {
                    if (chessboardModel.threefoldRepeatDrawAvailable)
                    {
                        return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
                    }
                    else if (chessboardModel.fiftyMoveDrawAvailable)
                    {
                        return new CCSFiftyMoveDrawOnOpponentTurnAvailable();
                    }

                    return new CCSPlayerTurn();
                }
                else
                {
                    if (chessboardModel.isAiGame)
                    {
                        cmd.aiTurnSignal.Dispatch();
                    }

                    return new CCSOpponentTurn();
                }
            }
            else if (cmd.chessboardEvent == ChessboardEvent.OPPONENT_MOVE_COMPLETE)
            {
                HandleOpponentBackendMoved(cmd);

                MoveVO vo = GetMoveVO(chessboardModel, false);
                chessboardModel.resumeMoves.Add(vo);

                if (chessboardModel.gameEndReason != GameEndReason.NONE)
                {
                    HandleGameEnded(cmd);
                    return new CCSOpponentTurnCompletedGameEnded();
                }
                else
                {
                    cmd.receiveTurnSwapTimeControlSignal.Dispatch();

                    if (chessboardModel.fiftyMoveDrawAvailable)
                    {
                        return new CCSFiftyMoveDrawOnOpponentTurnAvailable();
                    }
                    else if (chessboardModel.threefoldRepeatDrawAvailable)
                    {
                        return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
                    }

                    return new CCSPlayerTurn();
                }
            }
            
            return null;
        }
    }
}
