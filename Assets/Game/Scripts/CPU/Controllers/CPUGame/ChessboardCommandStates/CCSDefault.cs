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
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
    public class CCSDefault : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            // If we move back to default from the player turn or piece
            // selected turn, that means we are undoing the move. So we
            // just fire a game started event.
            if (CameFromState(cmd, typeof(CCSPlayerTurn)) ||
                CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                cmd.chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            if (cmd.chessboardEvent == ChessboardEvent.GAME_STARTED)
            {
                IChessboardModel chessboardModel = cmd.chessboardModel;
                ICPUGameModel cpuGameModel = cmd.cpuGameModel;
                IChessService chessService = cmd.chessService;
                IChessAiService chessAiService = cmd.chessAiService;

                // Initialize the core chessboard service
                if (UnityEngine.Debug.isDebugBuild && cpuGameModel.devFen != "")
                {
                    chessService.NewGame(cpuGameModel.devFen, chessboardModel.squares);
                }
                else
                {
                    chessService.NewGame(chessboardModel.squares);
                }

                // Initialize the Ai service
                chessAiService.NewGame();

                // Check who's turn it is
                chessboardModel.isPlayerTurn = (chessboardModel.playerColor == chessService.GetNextMoveColor());

                if (cpuGameModel.inProgress)
                {
                    ProcessResume(cmd);
                }
                else
                {
                    cpuGameModel.inProgress = true;
                }

                // Wait for player turn or execute Ai turn
                if (chessboardModel.isPlayerTurn)
                {
                    return new CCSPlayerTurn();
                }
                else
                {
                    cmd.aiTurnSignal.Dispatch();
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
            LogUtil.Log("Processing resume.. ", "yellow");

            IChessboardModel chessboardModel = cmd.chessboardModel;
            ICPUGameModel cpuGameModel = cmd.cpuGameModel;
            IChessService chessService = cmd.chessService;
            IChessAiService chessAiService = cmd.chessAiService;

            chessboardModel.moveVOCache = new List<MoveVO>();

            foreach (ChessMove move in chessboardModel.moveList)
            {
                ChessMoveResult moveResult = chessService.MakeMove(move.from,
                    move.to, 
                    move.promo, 
                    chessboardModel.isPlayerTurn, 
                    chessboardModel.squares);

                if (chessboardModel.isPlayerTurn)
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
                    ++chessboardModel.aiMoveNumber;
                }

                chessboardModel.isPlayerInCheck = moveResult.isPlayerInCheck;
                chessboardModel.isOpponentInCheck = moveResult.isOpponentInCheck;
                chessboardModel.playerScore = cmd.chessService.GetScore(chessboardModel.playerColor);
                chessboardModel.opponentScore = cmd.chessService.GetScore(chessboardModel.opponentColor);
                chessboardModel.notation.Add(moveResult.description);
                chessboardModel.capturedSquare = moveResult.capturedSquare;
                chessboardModel.threefoldRepeatDrawAvailable = moveResult.isThreefoldRepeatRuleActive;
                chessboardModel.fiftyMoveDrawAvailable = moveResult.isFiftyMoveRuleActive;

                MoveVO vo = GetMoveVO(chessboardModel, chessboardModel.isPlayerTurn);
                chessboardModel.moveVOCache.Add(vo);

                chessboardModel.isPlayerTurn = !chessboardModel.isPlayerTurn;
            }
        }
    }
}
