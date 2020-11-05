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

namespace TurboLabz.Multiplayer
{
    public class CCSOpponentTurn : CCS
    {
        private ChessboardCommand chessboardCommand;
        private long adStartTime = 0;

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

                if (cmd.activeMatchInfo.isLongPlay && cmd.activeMatchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW)
                {
                    cmd.waitingForOpponentAcceptSignal.Dispatch();
                }
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
                     (cmd.activeChessboard.playerFromSquare == null) &&
                     (cmd.activeChessboard.playerToSquare == null))
            {
                cmd.hidePlayerFromIndicatorSignal.Dispatch();
                cmd.hidePlayerToIndicatorSignal.Dispatch();
            }
            else if (CameFromState(cmd, typeof(CCSSafeMoveDialog)))
            {
                cmd.enableOpponentTurnInteraction.Dispatch();
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.SQUARE_CLICKED)
            {
                ChessSquare clickedSquare = cmd.activeChessboard.clickedSquare;

                // See if a player piece was clicked
                if (IsPlayerPiece(cmd, clickedSquare.piece))
                {
                    chessboard.playerFromSquare = clickedSquare;
                    return new CCSOpponentTurnPieceSelected();
                }
                // If not, then we go back to the non selected opponent turn state
                else
                {
                    chessboard.playerFromSquare = null;
                    chessboard.playerToSquare = null;
                    return null;
                }
            }
            // The player has completed the move (including backend)
            else if (evt == ChessboardEvent.PLAYER_MOVE_COMPLETE)
            {
                PlayerMoveCompleted(cmd);

                // Show in game ad for 30min match mode here.
                MatchInfo matchInfo = cmd.activeMatchInfo;
                if (matchInfo.gameTimeMode == GameTimeMode.ThirtyMin &&
                    cmd.adsSettingsModel.showInGame30Min &&
                    chessboard.backendPlayerTimer.Seconds < cmd.adsSettingsModel.secondsElapsedDisable30MinInGame)
                {
                    ShowInGameAd(cmd.matchInfoModel.activeChallengeId, matchInfo, cmd, AnalyticsContext.interstitial_in_game_30_min, AdPlacements.interstitial_in_game_30_min, OnShowAdComplete);
                }
                else if (matchInfo.isLongPlay &&
                    cmd.adsSettingsModel.showInGameClassic &&
                    chessboard.backendPlayerTimer.Seconds < cmd.adsSettingsModel.secondsElapsedDisable30MinInGame)
                {
                    ShowInGameAd(cmd.matchInfoModel.activeChallengeId, matchInfo, cmd, AnalyticsContext.interstitial_in_game_classic, AdPlacements.Interstitial_in_game_classic);
                }

                return null;
            }
            // We received an opponent moved event from the backend service
            else if (evt == ChessboardEvent.OPPONENT_MOVE_COMPLETE)
            {
                HandleOpponentBackendMoved(cmd);

                if (chessboard.gameEndReason != GameEndReason.NONE)
                {
                    HandleGameEnded(cmd);
                    return new CCSOpponentTurnCompletedGameEnded();
                }
                else
                {
                    cmd.receiveTurnSwapTimeControlSignal.Dispatch();

                    if (chessboard.fiftyMoveDrawAvailable)
                    {
                        return new CCSFiftyMoveDrawOnOpponentTurnAvailable();
                    }
                    else if (chessboard.threefoldRepeatDrawAvailable)
                    {
                        return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
                    }

                    return new CCSPlayerTurn();
                }
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }

        private void ShowInGameAd(string challengeId, MatchInfo matchInfo, ChessboardCommand cmd, AnalyticsContext adContext, AdPlacements placementId, Action OnAdCompleteCallback = null)
        {
            long secondsSinceLastAdShown = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - matchInfo.lastAdShownUTC) / 1000;
            if (secondsSinceLastAdShown > cmd.adsSettingsModel.secondsBetweenIngameAds)
            {
                chessboardCommand = cmd;

                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.rewardType = GSBackendKeys.ClaimReward.NONE;
                vo.challengeId = challengeId;
                vo.placementId = placementId;
                vo.OnAdCompleteCallback = OnAdCompleteCallback;
                cmd.playerModel.adContext = adContext;

                adStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                cmd.showAdSignal.Dispatch(vo, false);
            }
        }

        private void OnShowAdComplete()
        {
            long adTimeSpent = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (adStartTime / 1000);
            if (adTimeSpent > 2 && chessboardCommand != null)
            {
                chessboardCommand.stopTimersSignal.Dispatch();

                IChessService chessService = chessboardCommand.chessService;
                Chessboard chessboard = chessboardCommand.activeChessboard;

                bool isPlayerTurn = (chessboard.playerColor == chessService.GetNextMoveColor());
                RenderNewGame(chessboardCommand, isPlayerTurn);

                if (isPlayerTurn)
                {
                    chessboardCommand.enablePlayerTurnInteraction.Dispatch();
                }
                else
                {
                    chessboardCommand.enableOpponentTurnInteraction.Dispatch();
                }

                adStartTime = 0;
            }
        }
    }
}
