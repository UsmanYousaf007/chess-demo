/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

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

            cmd.updateResultsDialogSignal.Dispatch(vo);

            if (cmd.matchInfoModel.activeMatch.isLongPlay)
            {
                cmd.analyticsService.LongMatchCompleted(chessboard.gameEndReason.ToString(), cmd.matchInfoModel.activeMatch.gameDurationMs);
                cmd.unregisterSignal.Dispatch(cmd.matchInfoModel.activeChallengeId);
            }
            else
            {
                // Analytics
                if (cmd.matchInfoModel.activeMatch.isBotMatch)
                {
                    cmd.analyticsService.QuickBotMatchCompleted(cmd.matchInfoModel.activeMatch.botDifficulty, chessboard.gameEndReason.ToString());
                }
                else
                {
                    cmd.analyticsService.QuickMatchCompleted(chessboard.gameEndReason.ToString());
                }

                cmd.matchInfoModel.matches.Remove(cmd.matchInfoModel.activeChallengeId);
                cmd.chessboardModel.chessboards.Remove(cmd.matchInfoModel.activeChallengeId);
                cmd.matchInfoModel.activeChallengeId = null;
                cmd.matchInfoModel.activeLongMatchOpponentId = null;
            }


        }
    }
}
