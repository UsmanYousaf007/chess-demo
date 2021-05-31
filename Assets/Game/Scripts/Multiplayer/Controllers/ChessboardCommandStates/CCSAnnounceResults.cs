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
            vo.ratingBoostStoreItem = cmd.metaDataModel.store.items[GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER];
            vo.tournamentMatch = cmd.matchInfoModel.activeMatch.isTournamentMatch;
            vo.tournamentMatchScore = cmd.matchInfoModel.activeMatch.tournamentMatchScore;
            vo.winTimeBonus = cmd.matchInfoModel.activeMatch.tournamentMatchWinTimeBonus;
            vo.betValue = cmd.activeMatchInfo.betValue;
            vo.coinsMultiplyer = cmd.metaDataModel.settingsModel.GetSafeCoinsMultiplyer(Settings.ABTest.COINS_TEST_GROUP);
            vo.powerMode = cmd.activeMatchInfo.powerMode;
            vo.rewardDoubleStoreItem = cmd.metaDataModel.store.items[GSBackendKeys.ShopItem.SPECIAL_ITEM_REWARD_DOUBLER];
            vo.earnedStars = cmd.playerModel.leaguePromoted ? cmd.leaguesModel.GetLeagueInfo(cmd.playerModel.league - 1).winTrophies : cmd.leaguesModel.GetCurrentLeagueInfo().winTrophies;
            vo.movesCount = cmd.activeChessboard.moveList.Count;
            vo.fullGameAnalysisStoreItem = cmd.metaDataModel.store.items[GSBackendKeys.ShopItem.FULL_GAME_ANALYSIS];
            vo.freeGameAnalysisAvailable = cmd.playerModel.GetInventoryItemCount(GSBackendKeys.ShopItem.FULL_GAME_ANALYSIS) < cmd.metaDataModel.rewardsSettings.freeFullGameAnalysis;

            vo.canSeeRewardedVideo = cmd.playerModel.gems < cmd.adsSettingsModel.minGemsRequiredforRV && cmd.playerModel.rvUnlockTimestamp > 0
                && !(cmd.adsSettingsModel.removeRVOnPurchase && cmd.playerModel.HasPurchased()) && cmd.adsSettingsModel.CanShowAdWithAdPlacement(AdPlacements.RV_rating_booster.ToString());
            vo.coolDownTimeUTC = cmd.playerModel.rvUnlockTimestamp;

            cmd.updateResultsDialogSignal.Dispatch(vo);

            if (vo.isRanked && (playerWins || vo.reason == GameEndReason.DRAW_BY_DRAW_OFFERED || vo.reason == GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE ||
            vo.reason == GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE || vo.reason == GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL ||
            vo.reason == GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE || vo.reason == GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE))
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_REWARDS_DLG);
            }
            else
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG);
            }

            cmd.matchInfoModel.lastCompletedMatch = cmd.matchInfoModel.activeMatch;
            cmd.matchInfoModel.lastCompletedMatch.challengeId = cmd.matchInfoModel.activeChallengeId;
            cmd.matchInfoModel.lastCompletedMatch.gameEndReason = chessboard.gameEndReason.ToString();

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
                if (cmd.matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.OneMin)
                {
                    matchAnalyticsVO.matchType = "1m";
                }
                else if (cmd.matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.ThreeMin)
                {
                    matchAnalyticsVO.matchType = "3m";
                }
                else if (cmd.matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.TenMin)
                {
                    matchAnalyticsVO.matchType = "10m";
                }
                else if (cmd.matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.ThirtyMin)
                {
                    matchAnalyticsVO.matchType = "30m";
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
            cmd.specialHintAvailableSignal.Dispatch(false);
            cmd.disableUndoBtnSignal.Dispatch(false);
            cmd.matchAnalyticsSignal.Dispatch(matchAnalyticsVO);

            if (vo.betValue > 0)
            {
                var winnerCoins = vo.betValue * vo.coinsMultiplyer;
                var isDraw = matchAnalyticsVO.context == AnalyticsContext.draw || matchAnalyticsVO.context == AnalyticsContext.draw_agreement;
                var earnedCoins = vo.playerWins ? winnerCoins : isDraw ? vo.betValue : 0;

                if (earnedCoins > 0)
                {
                    cmd.playerModel.coins += (long)earnedCoins;
                    cmd.updatePlayerInventorySignal.Dispatch(cmd.playerModel.GetPlayerInventory());
                    cmd.analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)earnedCoins, "championship_coins", earnedCoins == vo.betValue ? "bet_reversed" : "game_won");
                }
            }
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
