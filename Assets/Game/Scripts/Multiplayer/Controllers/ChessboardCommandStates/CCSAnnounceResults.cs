/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class CCSAnnounceResults : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            Chessboard chessboard = cmd.activeChessboard;
            bool playerWins = (cmd.matchInfoModel.activeMatch.winnerId == cmd.playerModel.id) ? true : false;

            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG);

            ResultsVO vo = new ResultsVO();
            vo.reason = chessboard.gameEndReason;
            vo.playerWins = playerWins;
            vo.currentEloScore = cmd.playerModel.eloScore;
            vo.eloScoreDelta = cmd.matchInfoModel.activeMatch.playerEloScoreDelta;
            vo.isRanked = cmd.activeMatchInfo.isRanked;
            vo.powerupUsedCount = cmd.matchInfoModel.activeMatch.playerPowerupUsedCount; //playerWins ? cmd.metaDataModel.rewardsSettings.matchWinReward : cmd.metaDataModel.rewardsSettings.matchRunnerUpReward;
            vo.removeAds = cmd.playerModel.HasRemoveAds(cmd.metaDataModel.adsSettings);
            vo.playerName = cmd.playerModel.name;
            vo.opponentName = cmd.activeMatchInfo.opponentPublicProfile.name;
            vo.challengeId = cmd.matchInfoModel.activeChallengeId;

            cmd.updateResultsDialogSignal.Dispatch(vo);
            cmd.matchInfoModel.lastCompletedMatch = cmd.matchInfoModel.activeMatch;

            if (cmd.matchInfoModel.activeMatch.isLongPlay)
            {
                cmd.unregisterSignal.Dispatch(cmd.matchInfoModel.activeChallengeId);

                // Analytics
                long startMs = cmd.activeMatchInfo.gameStartTimeMilliseconds;
                long currentTime = cmd.backendService.serverClock.currentTimestamp;
                TimeSpan elapsed = TimeSpan.FromMilliseconds(currentTime - startMs);
                cmd.analyticsService.Event(AnalyticsEventId.long_match_complete_duration, AnalyticsParameter.duration, elapsed.TotalHours);
            }
            else
            {
                // Analytics
                if (cmd.matchInfoModel.activeMatch.isBotMatch)
                {
                    if (playerWins)
                    {
                        cmd.analyticsService.Event(AnalyticsEventId.bot_quick_match_won, 
                            AnalyticsParameter.bot_difficulty, 
                            cmd.activeMatchInfo.botDifficulty);
                    }
                    else
                    {
                        cmd.analyticsService.Event(AnalyticsEventId.bot_quick_match_lost,
                            AnalyticsParameter.bot_difficulty,
                            cmd.activeMatchInfo.botDifficulty);
                    }
                }

                cmd.matchInfoModel.matches.Remove(cmd.matchInfoModel.activeChallengeId);
                cmd.chessboardModel.chessboards.Remove(cmd.matchInfoModel.activeChallengeId);
                cmd.matchInfoModel.activeChallengeId = null;
                cmd.matchInfoModel.activeLongMatchOpponentId = null;
            }

            cmd.hintAvailableSignal.Dispatch(false);
            cmd.hindsightAvailableSignal.Dispatch(false);
            cmd.disableUndoBtnSignal.Dispatch(false);
            
        }
    }
}
