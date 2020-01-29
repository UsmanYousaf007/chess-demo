/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-21 15:56:44 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public class CCSAnnounceResults : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (GameEndHasMove(cmd))
            {
                if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
                {
                    RenderPlayerMove(cmd);
                }
                else if (CameFromState(cmd, typeof(CCSPromoDialog)))
                {
                    RenderPromo(cmd);
                } 
                else if (CameFromState(cmd, typeof(CCSOpponentTurn)) ||  
                         CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
                {
                    RenderOpponentMove(cmd);
                }
            }

            IChessboardModel model = cmd.chessboardModel;
            IPlayerModel playerModel = cmd.playerModel;
            bool playerWins = (model.winnerId == cmd.playerModel.id) ? true : false;

            GameEndReason gameEndReason = model.gameEndReason;

            cmd.disableMenuButtonSignal.Dispatch();

            cmd.cpuGameModel.inProgress = false;
            cmd.cpuGameModel.totalGames++;
            cmd.saveGameSignal.Dispatch();

            // Update player stats
            bool isDraw = false;

            if (gameEndReason == GameEndReason.STALEMATE ||
                gameEndReason == GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL)
            {
                isDraw = true;
            }

            int statResult = StatResult.NONE;

            if (playerWins)
            {
                if (cmd.chessboardModel.usedHelp)
                {
                    statResult = StatResult.SILVER;
                    cmd.analyticsService.LevelFail(cmd.cpuGameModel.cpuStrength);
                }
                else
                {
                    statResult = StatResult.GOLD;
                    cmd.analyticsService.LevelComplete(cmd.cpuGameModel.cpuStrength);
                }
            }
            else if (isDraw)
            {
                statResult = StatResult.NONE;
                cmd.analyticsService.LevelFail(cmd.cpuGameModel.cpuStrength);
            }
            else
            {
                statResult = StatResult.NONE;
                cmd.analyticsService.LevelFail(cmd.cpuGameModel.cpuStrength);
            }

            int powerupUsedCount = playerModel.cpuPowerupUsedCount;//playerWins ? cmd.metaDataModel.rewardsSettings.matchWinReward : cmd.metaDataModel.rewardsSettings.matchRunnerUpReward;
            bool isRemoveAds = cmd.playerModel.HasRemoveAds(cmd.metaDataModel.adsSettings);

            cmd.saveStatsSignal.Dispatch(statResult);

			cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_RESULTS_DLG);
			cmd.updateResultsDialogSignal.Dispatch(gameEndReason, playerWins, powerupUsedCount, isRemoveAds);

            cmd.hintAvailableSignal.Dispatch(false);
            cmd.hindsightAvailableSignal.Dispatch(false);
            cmd.disableUndoBtnSignal.Dispatch(false);
            cmd.toggleStepBackwardSignal.Dispatch(false);
            cmd.toggleStepForwardSignal.Dispatch(false);

            cmd.preferencesModel.gameFinishedCount++;
            cmd.hAnalyticsService.LogEvent(AnalyticsEventId.game_finished.ToString(), "gameplay", "cpu_match");
            cmd.appsFlyerService.TrackLimitedEvent(AnalyticsEventId.game_finished, cmd.preferencesModel.gameFinishedCount);
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            return null;
        }

        private bool GameEndHasMove(ChessboardCommand cmd)
        {
            GameEndReason reason = cmd.chessboardModel.gameEndReason;

            return (reason == GameEndReason.CHECKMATE ||
                    reason == GameEndReason.STALEMATE ||
                    reason == GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL);
        }
    }
}
