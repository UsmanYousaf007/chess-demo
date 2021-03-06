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

namespace TurboLabz.CPU
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
            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);

            // Initialize and launch our time control
            cmd.initInfiniteTimersSignal.Dispatch(isPlayerTurn);
                
            ICPUGameModel cpuGameModel = cmd.cpuGameModel;
            cpuGameModel.lastAdShownUTC = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); ;

            IChessboardModel chessboardModel = cmd.chessboardModel;

            // Setup the initial rotation
            bool isPlayerWhite = (chessboardModel.playerColor == ChessColor.WHITE);
            SetupChessboardVO vo;
            vo.isPlayerWhite = isPlayerWhite;
            cmd.setupChessboardSignal.Dispatch(vo);

            // Place the pieces
            cmd.updateChessboardSignal.Dispatch(cmd.chessboardModel.squares);

            // Reset opponent move render value
            cmd.chessboardModel.opponentMoveRenderComplete = true;

            // Toggle the hint button based on player turn
            //cmd.turnSwapSignal.Dispatch(isPlayerTurn);

            // Update the game info
            GameInfoVO gameInfoVO = new GameInfoVO();
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
            }

            // Initialize the game powerups
            cmd.updateHintCountSignal.Dispatch(cmd.playerModel.PowerUpHintCount);
            if (cmd.chessboardModel.lastPlayerMove != null)
            {
                cmd.turnSwapSignal.Dispatch(isPlayerTurn);
            }
            // game crashes in case if first turn is of cpu
            // and player presses menu button while cpu is executing move
            // button will be enabled after cpu made its move
            else if (!isPlayerTurn) 
            {
                cmd.disableMenuButtonSignal.Dispatch();
            }
            

            cmd.updateHindsightCountSignal.Dispatch(cmd.playerModel.PowerUpHindsightCount);
            cmd.hindsightAvailableSignal.Dispatch(cmd.chessboardModel.previousPlayerTurnFen != null);
            cmd.hintAvailableSignal.Dispatch(cmd.chessboardModel.previousPlayerTurnFen != null);

            //chessboardModel.inSafeMode = cmd.playerModel.PowerUpSafeMoveCount > 0 ? cmd.preferencesModel.isSafeMoveOn : false;
            chessboardModel.inSafeMode = false;
            cmd.updateSafeMoveStateSignal.Dispatch(chessboardModel.inSafeMode);
            cmd.updateSafeMoveCountSignal.Dispatch(cmd.playerModel.PowerUpSafeMoveCount);

            // Initialize the step buttons
            if (cmd.chessboardModel.moveList.Count > 1) cmd.toggleStepBackwardSignal.Dispatch(true);
            if (cmd.chessboardModel.trimmedMoveList.Count > 1) cmd.toggleStepForwardSignal.Dispatch(true);

            var specialHintVO = new SpecialHintVO();
            specialHintVO.specialHintStoreItem = cmd.metaDataModel.store.items[GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT];
            specialHintVO.isAvailable = cmd.preferencesModel.cpuPowerUpsUsedCount < cmd.metaDataModel.settingsModel.hintsAllowedPerGame;
            specialHintVO.hintsAllowedPerGame = cmd.metaDataModel.settingsModel.hintsAllowedPerGame;
            specialHintVO.isPlayerTurn = isPlayerTurn;
            specialHintVO.hintCount = cmd.playerModel.GetInventoryItemCount(GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT);
            specialHintVO.powerModeHints = cmd.chessboardModel.freeHints;
            specialHintVO.advantageThreshold = cmd.metaDataModel.settingsModel.advantageThreshold;

            cmd.setupSpecialHintSignal.Dispatch(specialHintVO);
        }

        protected void RenderOpponentMove(ChessboardCommand cmd)
        {
            // Update the view with the opponent move
            IChessboardModel model = cmd.chessboardModel;
            model.opponentMoveRenderComplete = false;
            MoveVO moveVO = GetMoveVO(model, false);
            cmd.updateOpponentMoveSignal.Dispatch(moveVO);
            cmd.hidePlayerFromIndicatorSignal.Dispatch();
            cmd.hidePlayerToIndicatorSignal.Dispatch();
            cmd.onboardingTooltipSignal.Dispatch(moveVO);
        }

        protected void RenderPlayerMove(ChessboardCommand cmd)
        {
            // Update the view with the player move
            IChessboardModel model = cmd.chessboardModel;
            MoveVO moveVO = GetMoveVO(model, true);
            cmd.hidePossibleMovesSignal.Dispatch();
            cmd.updatePlayerMoveSignal.Dispatch(moveVO);
            cmd.chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
            cmd.onboardingTooltipSignal.Dispatch(moveVO);
        }

        protected void RenderPromo(ChessboardCommand cmd)
        {
            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
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
            chessboardModel.previousPlayerTurnFen = cmd.chessService.GetFen();

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

            if (chessboardModel.inSafeMode)
            {
                return new CCSSafeMoveDialog();
            }

            return ConfirmPlayerMove(cmd, chessboardModel);
        }

        protected CCS ConfirmPlayerMove(ChessboardCommand cmd, IChessboardModel chessboardModel)
        {
            if (chessboardModel.gameEndReason != GameEndReason.NONE)
            {
                if (chessboardModel.gameEndReason == GameEndReason.CHECKMATE)
                {
                    chessboardModel.winnerId = cmd.playerModel.id;
                }

                ProcessGameEndTimers(cmd);
                return new CCSAnnounceResults();
            }

            cmd.takeTurnSwapTimeControlSignal.Dispatch();
            cmd.hindsightAvailableSignal.Dispatch(true);
            cmd.turnSwapSignal.Dispatch(false);
            cmd.hintAvailableSignal.Dispatch(true);
            return new CCSOpponentTurn();
        }

        protected void OpponentMoveRenderCompleted(ChessboardCommand cmd)
        {
            cmd.receiveTurnSwapTimeControlSignal.Dispatch();
            cmd.chessboardModel.opponentMoveRenderComplete = true;
            cmd.turnSwapSignal.Dispatch(true);
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
