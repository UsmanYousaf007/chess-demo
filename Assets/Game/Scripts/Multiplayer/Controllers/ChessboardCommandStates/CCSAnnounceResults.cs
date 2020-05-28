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


            if (chessboard.gameEndReason == GameEndReason.DECLINED)
            {
                var analyticsVO = new MatchAnalyticsVO();
                analyticsVO.context = AnalyticsContext.rejected;
                analyticsVO.matchType = "classic";
                analyticsVO.eventID = AnalyticsEventId.match_find;
                var opponentId = cmd.matchInfoModel.activeMatch.opponentPublicProfile.playerId;

                if (cmd.playerModel.friends.ContainsKey(opponentId))
                {
                    var friendType = cmd.playerModel.friends[opponentId].friendType;
                    if (friendType.Equals(GSBackendKeys.Friend.TYPE_SOCIAL))
                    {
                        analyticsVO.friendType = "friends_facebook";

                    }
                    else if (friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE) ||
                             friendType.Equals(GSBackendKeys.Friend.TYPE_COMMUNITY))
                    {
                        analyticsVO.friendType = "friends_community";
                    }
                }
                else
                {
                    analyticsVO.friendType = "community";
                }

                cmd.matchAnalyticsSignal.Dispatch(analyticsVO);
            }

            var matchAnalyticsVO = new MatchAnalyticsVO();
            matchAnalyticsVO.eventID = AnalyticsEventId.match_end;
            matchAnalyticsVO.friendType = string.Empty;
            matchAnalyticsVO.context = GetGameEndContext(chessboard.gameEndReason, playerWins, cmd.matchInfoModel.activeMatch.isBotMatch);

            if (cmd.matchInfoModel.activeMatch.isLongPlay)
            {
                cmd.unregisterSignal.Dispatch(cmd.matchInfoModel.activeChallengeId);
                matchAnalyticsVO.matchType = "classic";
            }
            else
            {
                if (cmd.matchInfoModel.activeMatch.isOneMinGame)
                {
                    matchAnalyticsVO.matchType = "1m";
                }
                else if (cmd.matchInfoModel.activeMatch.isTenMinGame)
                {
                    matchAnalyticsVO.matchType = "10m";
                }
                else
                {
                    matchAnalyticsVO.matchType = "5m";
                }

                if (cmd.matchInfoModel.activeMatch.isBotMatch)
                {
                    matchAnalyticsVO.friendType = "bot";
                }
                else
                {
                    matchAnalyticsVO.friendType = "player";
                }

                cmd.matchInfoModel.matches.Remove(cmd.matchInfoModel.activeChallengeId);
                cmd.chessboardModel.chessboards.Remove(cmd.matchInfoModel.activeChallengeId);
                cmd.matchInfoModel.activeChallengeId = null;
                cmd.matchInfoModel.activeLongMatchOpponentId = null;
            }

            cmd.hintAvailableSignal.Dispatch(false);
            cmd.hindsightAvailableSignal.Dispatch(false);
            cmd.disableUndoBtnSignal.Dispatch(false);
            cmd.matchAnalyticsSignal.Dispatch(matchAnalyticsVO);
        }

        private AnalyticsContext GetGameEndContext(GameEndReason reason, bool playerWins, bool isBot)
        {
            var context = AnalyticsContext.clock;

            switch (reason)
            {
                case GameEndReason.TIMER_EXPIRED:
                    if (isBot)
                    {
                        context = playerWins ? AnalyticsContext.clock_player_win : AnalyticsContext.clock_bot_win;
                    }
                    else
                    {
                        context = AnalyticsContext.clock;
                    }
                    break;

                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE:
                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE:
                case GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL:
                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE:
                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE:
                    context = AnalyticsContext.draw;
                    break;

                case GameEndReason.DRAW_BY_DRAW_OFFERED:
                    context = AnalyticsContext.draw_agreement;
                    break;

                case GameEndReason.PLAYER_DISCONNECTED:
                    context = AnalyticsContext.disconect;
                    break;

                case GameEndReason.DECLINED:
                    context = AnalyticsContext.declined;
                    break;

                case GameEndReason.CHECKMATE:
                    if (isBot)
                    {
                        context = playerWins ? AnalyticsContext.check_mate_player_win : AnalyticsContext.check_mate_bot_win;
                    }
                    else
                    {
                        context = AnalyticsContext.check_mate;
                    }
                    break;

                case GameEndReason.RESIGNATION:
                    if (isBot)
                    {
                        context = playerWins ? AnalyticsContext.resign_player_win : AnalyticsContext.resign_bot_win;
                    }
                    else
                    {
                        context = AnalyticsContext.resign;
                    }
                    break;

            }

            return context;
        }
    }
}
