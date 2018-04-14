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

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.Chess;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
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
            return ((piece != null) && (piece.color == cmd.chessboardModel.playerColor));
        }

        protected bool CameFromState(ChessboardCommand cmd, Type state)
        {
            return (cmd.chessboardModel.previousState.GetType() == state);
        }

        protected void RenderNewGame(ChessboardCommand cmd, bool isPlayerTurn)
        {
            // Load the game view
            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PLAY);

            // Initialize and launch our time control if not playing an
            // infinite time game.
            if (cmd.cpuGameModel.durationIndex == 0)
            {
                cmd.initInfiniteTimersSignal.Dispatch(isPlayerTurn);
            }
            else
            {
                // We kick off the time control signal here because the game
                // view needs to be visible for it to function correctly
                cmd.runTimeControlSignal.Dispatch();
            }

            ICPUGameModel cpuGameModel = cmd.cpuGameModel;
            IChessboardModel chessboardModel = cmd.chessboardModel;

            // Setup the initial rotation
            bool isPlayerWhite = (chessboardModel.playerColor == ChessColor.WHITE);
            cmd.setupChessboardSignal.Dispatch(isPlayerWhite);

            // Place the pieces
            cmd.updateChessboardSignal.Dispatch(cmd.chessboardModel.squares);

            // Reset opponent move render value
            cmd.chessboardModel.opponentMoveRenderComplete = true;

            // Set the available hints counter
            cmd.updateHintCountSignal.Dispatch(cmd.chessboardModel.availableHints);

            // Toggle the hint button based on player turn
            cmd.turnSwapSignal.Dispatch(isPlayerTurn);

            // Set the undo button's default state
            cmd.disableUndoButtonSignal.Dispatch();

            // Decide whether to show ads after this game is completed
            cmd.cpuGameModel.showAd = (cmd.cpuGameModel.totalGames % CPUSettings.AD_FREQUENCY == 0)
                && cmd.cpuGameModel.totalGames > 0
                && cmd.adsService.isAdAvailable;
            
            // Update the game info
            GameInfoVO gameInfoVO = new GameInfoVO();
            gameInfoVO.showAd = cmd.cpuGameModel.showAd;
            gameInfoVO.cpuStrength = cmd.cpuGameModel.cpuStrength;
            cmd.updateGameInfoSignal.Dispatch(gameInfoVO);

            // If we are resuming and there were some moves made, update the
            // view to account for the moves
            int resumeMoveCount = chessboardModel.moveVOCache.Count;

            if (resumeMoveCount > 0)
            {
                bool wasPlayerTurn = (chessboardModel.playerColor == ChessColor.WHITE);

                foreach (MoveVO moveVO in chessboardModel.moveVOCache)
                {
                    cmd.updateMoveForResumeSignal.Dispatch(moveVO, wasPlayerTurn);
                    wasPlayerTurn = !wasPlayerTurn;
                }

                cmd.updateUndoButtonSignal.Dispatch(wasPlayerTurn, chessboardModel.moveVOCache.Count);
            }
        }

        protected void RenderOpponentMove(ChessboardCommand cmd)
        {
            // Update the view with the opponent move
            IChessboardModel model = cmd.chessboardModel;
            model.opponentMoveRenderComplete = false;
            cmd.updateOpponentMoveSignal.Dispatch(GetMoveVO(model, false));
            cmd.hidePlayerFromIndicatorSignal.Dispatch();
            cmd.hidePlayerToIndicatorSignal.Dispatch();
        }

        protected void RenderPlayerMove(ChessboardCommand cmd)
        {
            // Update the view with the player move
            IChessboardModel model = cmd.chessboardModel;
            cmd.hidePossibleMovesSignal.Dispatch();
            cmd.updatePlayerMoveSignal.Dispatch(GetMoveVO(model, true));
            cmd.chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
        }

        protected void RenderPromo(ChessboardCommand cmd)
        {
            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PLAY);
            IChessboardModel model = cmd.chessboardModel;
            cmd.updatePromoSignal.Dispatch(GetMoveVO(model, true));
            cmd.chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
        }

        protected void DoAiMove(ChessboardCommand cmd)
        {
            // Nows when Ai takes its turn and fills out
            // the move info into the chessboard Model
            cmd.aiTurnSignal.Dispatch();
        }

        protected CCS DoPlayerMove(ChessboardCommand cmd, string promo)
        {
            IChessboardModel chessboardModel = cmd.chessboardModel;

            // We need to save player ChessMove's for the Ai engine to analyse + add it to the move list
            ChessMove move = new ChessMove();
            move.from = chessboardModel.playerFromSquare.fileRank;
            move.to = chessboardModel.playerToSquare.fileRank;
            move.piece = chessboardModel.playerFromSquare.piece;
            move.promo = promo;
            chessboardModel.lastPlayerMove = move;
            chessboardModel.moveList.Add(move);

            ChessMoveResult moveResult = cmd.chessService.MakeMove(
                chessboardModel.playerFromSquare.fileRank,
                chessboardModel.playerToSquare.fileRank,
                promo,
                true,
                chessboardModel.squares);

            // The from and to squares are already set in the model. We had to make the move
            // inside the chess core engine in order to extract the move flag and then set it
            // to the model.
            chessboardModel.playerMoveFlag = moveResult.moveFlag;
            chessboardModel.isPlayerInCheck = moveResult.isPlayerInCheck;
            chessboardModel.isOpponentInCheck = moveResult.isOpponentInCheck;
            chessboardModel.playerScore = cmd.chessService.GetScore(chessboardModel.playerColor);
            chessboardModel.opponentScore = cmd.chessService.GetScore(chessboardModel.opponentColor);
            chessboardModel.notation.Add(moveResult.description);
            chessboardModel.capturedSquare = moveResult.capturedSquare;
            chessboardModel.gameEndReason = moveResult.gameEndReason;

            if (moveResult.gameEndReason != GameEndReason.NONE)
            {
               if (moveResult.gameEndReason == GameEndReason.CHECKMATE)
                {
                    chessboardModel.winnerId = cmd.playerModel.id;
                }

                ProcessGameEndTimers(cmd);
                return new CCSAnnounceResults();
            }

            cmd.takeTurnSwapTimeControlSignal.Dispatch();
            cmd.turnSwapSignal.Dispatch(false);
            return new CCSOpponentTurn();
        }

        protected void OpponentMoveRenderCompleted(ChessboardCommand cmd)
        {
            cmd.receiveTurnSwapTimeControlSignal.Dispatch();
            cmd.chessboardModel.opponentMoveRenderComplete = true;
            cmd.turnSwapSignal.Dispatch(true);
            cmd.updateUndoButtonSignal.Dispatch(true, cmd.chessboardModel.moveList.Count);
        }

        protected void ProcessGameEndTimers(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;

            if (!model.timersStopped)
            {
                cmd.stopTimersSignal.Dispatch();
                model.timersStopped = true;
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

        protected MoveVO GetMoveVO(IChessboardModel model, bool isPlayerTurn)
        {
            MoveVO moveVO;

            moveVO.fromSquare = isPlayerTurn ? model.playerFromSquare : model.opponentFromSquare;
            moveVO.toSquare = isPlayerTurn ? model.playerToSquare : model.opponentToSquare;
            moveVO.moveFlag = isPlayerTurn ? model.playerMoveFlag : model.opponentMoveFlag;
            moveVO.pieceColor = isPlayerTurn? model.playerColor : model.opponentColor;

            moveVO.isPlayerInCheck = model.isPlayerInCheck;
            moveVO.isOpponentInCheck = model.isOpponentInCheck;
            moveVO.playerScore = model.playerScore;
            moveVO.opponentScore = model.opponentScore;
            moveVO.notation = model.notation;

            moveVO.capturedSquare = model.capturedSquare;
            moveVO.totalMoveCount = model.moveList.Count;
           
            return moveVO;
        }
    }
}
