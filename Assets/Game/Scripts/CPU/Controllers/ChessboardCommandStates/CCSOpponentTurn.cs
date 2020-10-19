/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 15:48:40 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public class CCSOpponentTurn : CCS
    {
        private ChessboardCommand chessboardCommand;

        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            // If we're starting a new game
            if (CameFromState(cmd, typeof(CCSDefault)))
            {
                RenderNewGame(cmd, false);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
            // If a player has completed his move
            else if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                RenderPlayerMove(cmd);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
            else if (CameFromState(cmd, typeof(CCSPromoDialog)))
            {
                RenderPromo(cmd);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
            // If a player has clicked a non-player piece or empty square after
            // selecting a piece previously
            else if (CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                cmd.hidePlayerToIndicatorSignal.Dispatch();
            }
            // If a player has clicked a non-player piece or empty square after
            // this state has loaded
            else if (CameFromState(cmd, typeof(CCSOpponentTurn)) &&
                     (cmd.chessboardModel.playerFromSquare == null) &&
                     (cmd.chessboardModel.playerToSquare == null))
            {
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                cmd.hidePlayerToIndicatorSignal.Dispatch();
            }
            else if (CameFromState(cmd, typeof(CCSSafeMoveDialog)))
            {
                cmd.chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.SQUARE_CLICKED)
            {
                ChessSquare clickedSquare = cmd.chessboardModel.clickedSquare;

                // See if a player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece))
                {
                    model.playerFromSquare = clickedSquare;
                    return new CCSOpponentTurnPieceSelected();
                }
                // If not, then we go back to the non selected opponent turn state
                else
                {
                    model.playerFromSquare = null;
                    model.playerToSquare = null;
                    return null;
                }
            }
            // The player has completed the move
            else if (evt == ChessboardEvent.PLAYER_MOVE_COMPLETE)
            {
                chessboardCommand = cmd;

                // Show computer game ad here, wait for ad to finish before making ai move.
                long utcNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (cmd.adsSettingsModel.showInGameCPU)
                {
                    cmd.cpuGameModel.lastAdShownUTC = utcNow;

                    ResultAdsVO vo = new ResultAdsVO();
                    vo.adsType = AdType.Interstitial;
                    vo.rewardType = GSBackendKeys.ClaimReward.NONE;
                    vo.challengeId = "";
                    vo.placementId = AdPlacements.interstitial_in_game_cpu;
                    vo.OnAdCompleteCallback = OnInGameAdComplete;
                    cmd.playerModel.adContext = AnalyticsContext.interstitial_in_game_cpu;

                    if (!cmd.playerModel.HasSubscription())
                    {
                        cmd.analyticsService.Event(AnalyticsEventId.ad_user_requested, cmd.playerModel.adContext);
                    }

                    cmd.showAdSignal.Dispatch(vo, false);
                }
                else
                {
                    OnInGameAdComplete();
                }

                return null;
            }
            // We received an opponent moved event from the backend service
            else if (evt == ChessboardEvent.OPPONENT_MOVE_COMPLETE)
            {
                if (model.gameEndReason != GameEndReason.NONE)
                {
                    ProcessGameEndTimers(cmd);
                    return new CCSAnnounceResults();
                }
                else
                {
                    return new CCSPlayerTurn();
                }
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                ProcessGameEndTimers(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }

        private void OnInGameAdComplete()
        {
            DoAiMove(chessboardCommand);

            // You can no longer go forward in history after making a move
            chessboardCommand.chessboardModel.trimmedMoveList.Clear();
            chessboardCommand.toggleStepForwardSignal.Dispatch(false);
        }
    }
}
