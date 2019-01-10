/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-15 20:49:16 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class CCS
    {
        public virtual void RenderDisplayOnEnter(ChessboardCommand cmd)
        {   
        }

        public virtual CCS HandleEvent(ChessboardCommand cmd)
        {
            return null;
        }

        protected bool IsPlayerPiece(ChessboardCommand cmd, ChessPiece piece)
        {
            return ((piece != null) && (piece.color == cmd.activeChessboard.playerColor));
        }

        protected bool CameFromState(ChessboardCommand cmd, Type state)
        {
            return (cmd.activeChessboard.previousState.GetType() == state);
        }

        protected void RenderNewGame(ChessboardCommand cmd, bool isPlayerTurn, bool isUndo = false)
        {
            // Load the game view
            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);

            // Detect if opponent clock should be paused
            if (!isUndo)
            {
                RunTimeControlVO runTimeControlVO;

                runTimeControlVO.pauseAfterSwap =
                    cmd.activeMatchInfo.isLongPlay &&
                    cmd.activeMatchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW &&
                    !isPlayerTurn;


                if (cmd.activeMatchInfo.isLongPlay)
                {
                    runTimeControlVO.waitingForOpponentToAccept = cmd.activeMatchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW;
                }
                else
                {
                    runTimeControlVO.waitingForOpponentToAccept = false;
                }


                // Initialize and launch our time control
                cmd.runTimeControlSignal.Dispatch(runTimeControlVO);
            }

            IPlayerModel playerModel = cmd.playerModel;
            Chessboard activeChessboard = cmd.activeChessboard;

            // Setup the initial rotation and skin
            bool isPlayerWhite = (activeChessboard.playerColor == ChessColor.WHITE);
            SetupChessboardVO setupVO;
            setupVO.isPlayerWhite = isPlayerWhite;
            setupVO.isLongPlay = cmd.activeMatchInfo.isLongPlay;
            cmd.setupChessboardSignal.Dispatch(setupVO);

            // Place the pieces
            cmd.updateChessboardSignal.Dispatch(cmd.activeChessboard.squares);

            // Reset opponent move render value
            activeChessboard.opponentMoveRenderComplete = true;

            // If we are resuming, update the view to account for the moves
            // that were made
            if (activeChessboard.moveVOCache.Count > 0 && activeChessboard.overrideFen == null)
            {
                bool wasPlayerTurn = (activeChessboard.playerColor == ChessColor.WHITE);

                foreach (MoveVO moveVO in activeChessboard.moveVOCache)
                {
                    cmd.updateMoveForResumeSignal.Dispatch(moveVO, wasPlayerTurn);
                    wasPlayerTurn = !wasPlayerTurn;
                }
            }

            // If we are resuming, check if the game had ended.
            if (!isUndo)
            {
                if (activeChessboard.gameEndReason != GameEndReason.NONE)
                {
                    cmd.chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ENDED);
                }
            }

            // Initialize the hint button
            cmd.updateHintCountSignal.Dispatch(playerModel.PowerUpHintCount);
            cmd.turnSwapSignal.Dispatch(isPlayerTurn);
        }

        protected void RenderOpponentMove(ChessboardCommand cmd)
        {
            // Update the view with the opponent move
            cmd.activeChessboard.opponentMoveRenderComplete = false;
            cmd.updateOpponentMoveSignal.Dispatch(GetMoveVO(cmd.activeChessboard, false));
            cmd.hidePlayerFromIndicatorSignal.Dispatch();
            cmd.hidePlayerToIndicatorSignal.Dispatch();
        }

        protected void RenderPlayerMove(ChessboardCommand cmd)
        {
            // Update the view with the player move
            cmd.hidePossibleMovesSignal.Dispatch();
            cmd.updatePlayerMoveSignal.Dispatch(GetMoveVO(cmd.activeChessboard, true));
        }

        protected void RenderPromo(ChessboardCommand cmd)
        {
            cmd.updatePromoSignal.Dispatch(GetMoveVO(cmd.activeChessboard, true));
        }

        protected void HandleOpponentBackendMoved(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;

            ChessMoveResult moveResult = cmd.chessService.MakeMove(
                chessboard.opponentFromSquare.fileRank,
                chessboard.opponentToSquare.fileRank,
                GetPromoFromMove(chessboard.opponentMoveFlag),
                false, 
                chessboard.squares);

            // We just need a few values from the move result, the rest of
            // the values came in from the backend
            chessboard.isPlayerInCheck = moveResult.isPlayerInCheck;
            chessboard.isOpponentInCheck = moveResult.isOpponentInCheck;
            chessboard.playerScore = cmd.chessService.GetScore(chessboard.playerColor);
            chessboard.opponentScore = cmd.chessService.GetScore(chessboard.opponentColor);
            chessboard.notation.Add(moveResult.description);
            chessboard.capturedSquare = moveResult.capturedSquare;

            // Clear out the player from square if the opponent
            // has captured the piece he had selected
            if (chessboard.opponentToSquare.Equals(chessboard.playerFromSquare))
            {
                chessboard.playerFromSquare = null;
            }
        }

        protected CCS HandlePlayerMove(ChessboardCommand cmd, string promo)
        {
            Chessboard chessboard = cmd.activeChessboard;

            ChessMoveResult moveResult = cmd.chessService.MakeMove(
                chessboard.playerFromSquare.fileRank,
                chessboard.playerToSquare.fileRank,
                promo,
                true,
                chessboard.squares);

            // The from and to squares are already set in the model. We had to make the move
            // inside the chess core engine in order to extract the move flag and then set it
            // to the model.
            chessboard.playerMoveFlag = moveResult.moveFlag;
            chessboard.isPlayerInCheck = moveResult.isPlayerInCheck;
            chessboard.isOpponentInCheck = moveResult.isOpponentInCheck;
            chessboard.playerScore = cmd.chessService.GetScore(chessboard.playerColor);
            chessboard.opponentScore = cmd.chessService.GetScore(chessboard.opponentColor);
            chessboard.notation.Add(moveResult.description);
            chessboard.capturedSquare = moveResult.capturedSquare;
            chessboard.threefoldRepeatDrawAvailable = moveResult.isThreefoldRepeatRuleActive;
            chessboard.fiftyMoveDrawAvailable = moveResult.isFiftyMoveRuleActive;
            chessboard.gameEndReason = moveResult.gameEndReason;

            if (chessboard.inSafeMode)
            {
                return new CCSSafeMoveDialog();
            }

            return ConfirmPlayerMove(cmd, chessboard);
        }

        protected CCS ConfirmPlayerMove(ChessboardCommand cmd, Chessboard chessboard)
        {
            if (chessboard.gameEndReason != GameEndReason.NONE)
            {
                SendPlayerTurn(cmd, cmd.activeChessboard.playerMoveFlag, false, false, false, true);
                return new CCSPlayerTurnCompletedGameEnded();
            }
            else if (chessboard.threefoldRepeatDrawAvailable)
            {
                return new CCSThreefoldRepeatDrawOnPlayerTurnAvailable();
            }
            else if (chessboard.fiftyMoveDrawAvailable)
            {
                return new CCSFiftyMoveDrawOnPlayerTurnAvailable();
            }

            SendPlayerTurn(cmd, chessboard.playerMoveFlag, false, false, false, false);
            cmd.turnSwapSignal.Dispatch(false);
            return new CCSOpponentTurn();
        }

        protected void SendPlayerTurn(ChessboardCommand cmd,
                                      ChessMoveFlag moveFlag,
                                      bool claimFiftyMoveDraw,
                                      bool claimThreefoldRepeatDraw,
                                      bool rejectThreefoldRepeatDraw,
                                      bool gameEndedByMove)
        {
            Chessboard chessboard = cmd.activeChessboard;

            // Stop the timers if the game has ended after the player move.
            if (claimFiftyMoveDraw ||
                claimThreefoldRepeatDraw ||
                gameEndedByMove)
            {
                cmd.stopTimersSignal.Dispatch();
                chessboard.timersStopped = true;
            }
            // The game has not ended, so swap the timers
            else
            {
                bool pauseAfterSwap = cmd.activeMatchInfo.isLongPlay && cmd.activeMatchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW;
                cmd.takeTurnSwapTimeControlSignal.Dispatch(pauseAfterSwap);
            }
                
            PlayerTurnVO vo;
            vo.fromSquare = chessboard.playerFromSquare;
            vo.toSquare = chessboard.playerToSquare;
            vo.promo = GetPromoFromMove(moveFlag);
            vo.claimFiftyMoveDraw = claimFiftyMoveDraw;
            vo.claimThreefoldRepeatDraw = claimThreefoldRepeatDraw;
            vo.rejectThreefoldRepeatDraw = rejectThreefoldRepeatDraw;

            cmd.backendPlayerTurnSignal.Dispatch(vo);
        }

        protected void HandleGameEnded(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;

            if (!chessboard.timersStopped)
            {
                cmd.stopTimersSignal.Dispatch();
                chessboard.timersStopped = true;
            }
        }

        protected string GetPromoFromMove(ChessMoveFlag moveFlag)
        {
            string promo = null;

            if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_QUEEN)
            {
                promo = ChessPieceName.BLACK_QUEEN;
            }
            else if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_ROOK)
            {
                promo = ChessPieceName.BLACK_ROOK;
            }
            else if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_BISHOP)
            {
                promo = ChessPieceName.BLACK_BISHOP;
            }
            else if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_KNIGHT)
            {
                promo = ChessPieceName.BLACK_KNIGHT;
            }

            return promo;
        }

        protected MoveVO GetMoveVO(Chessboard chessboard, bool isPlayerTurn)
        {
            MoveVO moveVO;

            moveVO.fromSquare = isPlayerTurn ? chessboard.playerFromSquare : chessboard.opponentFromSquare;
            moveVO.toSquare = isPlayerTurn ? chessboard.playerToSquare : chessboard.opponentToSquare;
            moveVO.moveFlag = isPlayerTurn ? chessboard.playerMoveFlag : chessboard.opponentMoveFlag;
            moveVO.pieceColor = isPlayerTurn? chessboard.playerColor : chessboard.opponentColor;

            moveVO.isPlayerInCheck = chessboard.isPlayerInCheck;
            moveVO.isOpponentInCheck = chessboard.isOpponentInCheck;
            moveVO.playerScore = chessboard.playerScore;
            moveVO.opponentScore = chessboard.opponentScore;
            moveVO.notation = chessboard.notation;

            moveVO.capturedSquare = chessboard.capturedSquare;

            // TODO: track total number of moves if required.
            // This is 'disabled' and set to 0 in multiplayer since it is not used.
            moveVO.totalMoveCount = 0;
           
            return moveVO;
        }

        /*
         * Loop through all the moves and update the following stateful entities:
         * 1) The chess service 
         * 2) The chessboard model
         * 3) The moveVOs that are sent to the view
         */
        protected void ProcessResume(ChessboardCommand cmd)
        {
            IChessService chessService = cmd.chessService;
            IChessAiService chessAiService = cmd.chessAiService;
            Chessboard chessboard = cmd.activeChessboard;

            chessboard.moveVOCache = new List<MoveVO>();
            chessboard.aiMoveNumber = 0;

            bool isPlayerTurn = (chessboard.playerColor == chessService.GetNextMoveColor());

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

        protected void OpponentMoveRenderCompleted(ChessboardCommand cmd)
        {
            cmd.activeChessboard.opponentMoveRenderComplete = true;
            cmd.turnSwapSignal.Dispatch(true);
        }
    }
}
