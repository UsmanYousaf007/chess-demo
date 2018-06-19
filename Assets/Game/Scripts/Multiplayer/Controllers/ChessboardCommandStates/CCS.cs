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
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

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
            return ((piece != null) && (piece.color == cmd.chessboardModel.playerColor));
        }

        protected bool CameFromState(ChessboardCommand cmd, Type state)
        {
            return (cmd.chessboardModel.previousState.GetType() == state);
        }

        protected void RenderNewGame(ChessboardCommand cmd, bool isPlayerTurn)
        {
            // Load the game view
            //cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MP_PLAY);

            // Initialize and launch our time control
            cmd.runTimeControlSignal.Dispatch();

            IPlayerModel playerModel = cmd.playerModel;
            IMatchInfoModel matchInfoModel = cmd.matchInfoModel;
            IChessboardModel chessboardModel = cmd.chessboardModel;
            PublicProfile playerPublicProfile = playerModel.publicProfile;
            PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;

            /*
            string roomId = matchInfoModel.roomId;
            RoomSetting roomInfo = cmd.roomSettingsModel.settings[roomId];

            // Update the game info areas
            MatchInfoVO vo;
            vo.playerName = playerPublicProfile.name;
            vo.playerRoomTitleId = playerPublicProfile.roomRecords[roomId].roomTitleId;
            vo.playerLevel = playerPublicProfile.level;
            vo.playerCountryId = playerPublicProfile.countryId;
            vo.opponentName = opponentPublicProfile.name;
            vo.opponentRoomTitleId = opponentPublicProfile.roomRecords[roomId].roomTitleId;
            vo.opponentLevel = opponentPublicProfile.level;
            vo.opponentCountryId = opponentPublicProfile.countryId;
            vo.roomId = roomId;
            vo.durationMinutes = (int)(roomInfo.gameDuration / 60000f);
            vo.prize = roomInfo.prize;
            vo.isPlayerTurn = isPlayerTurn;
            vo.opponentProfilePictureSprite = opponentPublicProfile.profilePicture;

            cmd.updateMatchInfoSignal.Dispatch(vo);
            */

            // Setup the initial rotation and skin
            bool isPlayerWhite = (chessboardModel.playerColor == ChessColor.WHITE);
            SetupChessboardVO setupVO;
            setupVO.isPlayerWhite = isPlayerWhite;
            cmd.setupChessboardSignal.Dispatch(setupVO);

            // Place the pieces
            cmd.updateChessboardSignal.Dispatch(cmd.chessboardModel.squares);

            // Reset opponent move render value
            cmd.chessboardModel.opponentMoveRenderComplete = true;

            // If we are resuming, update the view to account for the moves
            // that were made
            if (matchInfoModel.isResuming && chessboardModel.overrideFen == null)
            {
                bool wasPlayerTurn = (chessboardModel.playerColor == ChessColor.WHITE);

                foreach (MoveVO moveVO in chessboardModel.resumeMoves)
                {
                    cmd.updateMoveForResumeSignal.Dispatch(moveVO, wasPlayerTurn);
                    wasPlayerTurn = !wasPlayerTurn;
                }
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
        }

        protected void RenderPromo(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;
            cmd.updatePromoSignal.Dispatch(GetMoveVO(model, true));
        }

        protected void HandleOpponentBackendMoved(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;

            ChessMoveResult moveResult = cmd.chessService.MakeMove(
                model.opponentFromSquare.fileRank,
                model.opponentToSquare.fileRank,
                GetPromoFromMove(model.opponentMoveFlag),
                false, 
                model.squares);

            // We just need a few values from the move result, the rest of
            // the values came in from the backend
            model.isPlayerInCheck = moveResult.isPlayerInCheck;
            model.isOpponentInCheck = moveResult.isOpponentInCheck;
            model.playerScore = cmd.chessService.GetScore(model.playerColor);
            model.opponentScore = cmd.chessService.GetScore(model.opponentColor);
            model.notation.Add(moveResult.description);
            model.capturedSquare = moveResult.capturedSquare;

            // Clear out the player from square if the opponent
            // has captured the piece he had selected
            if (model.opponentToSquare.Equals(model.playerFromSquare))
            {
                model.playerFromSquare = null;
            }
        }

        protected CCS HandlePlayerMove(ChessboardCommand cmd, string promo)
        {
            IChessboardModel model = cmd.chessboardModel;

            ChessMoveResult moveResult = cmd.chessService.MakeMove(
                model.playerFromSquare.fileRank,
                model.playerToSquare.fileRank,
                promo,
                true,
                model.squares);

            // The from and to squares are already set in the model. We had to make the move
            // inside the chess core engine in order to extract the move flag and then set it
            // to the model.
            model.playerMoveFlag = moveResult.moveFlag;
            model.isPlayerInCheck = moveResult.isPlayerInCheck;
            model.isOpponentInCheck = moveResult.isOpponentInCheck;
            model.playerScore = cmd.chessService.GetScore(model.playerColor);
            model.opponentScore = cmd.chessService.GetScore(model.opponentColor);
            model.notation.Add(moveResult.description);
            model.capturedSquare = moveResult.capturedSquare;
            model.threefoldRepeatDrawAvailable = moveResult.isThreefoldRepeatRuleActive;
            model.fiftyMoveDrawAvailable = moveResult.isFiftyMoveRuleActive;

            if (moveResult.gameEndReason != GameEndReason.NONE)
            {
                SendPlayerTurn(cmd, cmd.chessboardModel.playerMoveFlag, false, false, false, true);
                return new CCSPlayerTurnCompletedGameEnded();
            }
            else if (moveResult.isThreefoldRepeatRuleActive)
            {
                return new CCSThreefoldRepeatDrawOnPlayerTurnAvailable();
            }
            else if (moveResult.isFiftyMoveRuleActive)
            {
                return new CCSFiftyMoveDrawOnPlayerTurnAvailable();
            }

            SendPlayerTurn(cmd, model.playerMoveFlag, false, false, false, false);
            return new CCSOpponentTurn();
        }

        protected void SendPlayerTurn(ChessboardCommand cmd,
                                      ChessMoveFlag moveFlag,
                                      bool claimFiftyMoveDraw,
                                      bool claimThreefoldRepeatDraw,
                                      bool rejectThreefoldRepeatDraw,
                                      bool gameEndedByMove)
        {
            IChessboardModel model = cmd.chessboardModel;

            // Stop the timers if the game has ended after the player move.
            if (claimFiftyMoveDraw ||
                claimThreefoldRepeatDraw ||
                gameEndedByMove)
            {
                cmd.stopTimersSignal.Dispatch();
                model.timersStopped = true;
            }
            // The game has not ended, so swap the timers
            else
            {
                cmd.takeTurnSwapTimeControlSignal.Dispatch();
            }
                
            PlayerTurnVO vo;
            vo.fromSquare = model.playerFromSquare;
            vo.toSquare = model.playerToSquare;
            vo.promo = GetPromoFromMove(moveFlag);
            vo.claimFiftyMoveDraw = claimFiftyMoveDraw;
            vo.claimThreefoldRepeatDraw = claimThreefoldRepeatDraw;
            vo.rejectThreefoldRepeatDraw = rejectThreefoldRepeatDraw;

            cmd.backendPlayerTurnSignal.Dispatch(vo);
        }

        protected void HandleGameEnded(ChessboardCommand cmd)
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

            // TODO: track total number of moves if required.
            // This is 'disabled' and set to 0 in multiplayer since it is not used.
            moveVO.totalMoveCount = 0;
           
            return moveVO;
        }
    }
}
