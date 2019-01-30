/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

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
            vo.rewardCoins = playerWins ? cmd.metaDataModel.rewardsSettings.matchWinAdReward : cmd.metaDataModel.rewardsSettings.matchRunnerUpAdReward;
            vo.adRewardType = playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN_AD : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN_AD;
            vo.removeAds = cmd.playerModel.HasRemoveAds(cmd.metaDataModel.adsSettings);

            cmd.updateResultsDialogSignal.Dispatch(vo);

            if (cmd.matchInfoModel.activeMatch.isLongPlay)
            {
                cmd.unregisterSignal.Dispatch(cmd.matchInfoModel.activeChallengeId);
            }
            else
            {
                // Analytics
                if (cmd.matchInfoModel.activeMatch.isBotMatch)
                {
                    if (playerWins)
                    {
                        cmd.analyticsService.Event(AnalyticsEvent.bot_quick_match_won, 
                            AnalyticsParameter.bot_difficulty, 
                            cmd.activeMatchInfo.botDifficulty);
                    }
                    else
                    {
                        cmd.analyticsService.Event(AnalyticsEvent.bot_quick_match_lost,
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
            cmd.safeMoveAvailableSignal.Dispatch(false);
        }
    }
}
