/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System;
using GameSparks.Core;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        // Services
        [Inject] public IChessService chessService { get; set; }

        private void SetupGame(string challengeId, GSData gameData)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create new chessboard
            Chessboard chessboard = new Chessboard();
            chessboardModel.chessboards[challengeId] = chessboard;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize fixed game data
            MatchInfo matchInfo = matchInfoModel.matches[challengeId];
            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfo.opponentPublicProfile.playerId);
            chessboard.isAiGame = matchInfo.isBotMatch;
            chessboard.playerColor = GSBackendKeys.PLAYER_COLOR_MAP[playerData.GetString(GSBackendKeys.COLOR)];
            chessboard.opponentColor = GSBackendKeys.PLAYER_COLOR_MAP[opponentData.GetString(GSBackendKeys.COLOR)];
            chessboard.lastMoveTime = TimeUtil.ToDateTime(matchInfo.gameStartTimeMilliseconds);
            //if (!matchInfo.isLongPlay)
            //{
                long gameDuration = gameData.GetLong(GSBackendKeys.GAME_DURATION).Value;
                chessboard.gameDuration = TimeSpan.FromMilliseconds(gameDuration);
            //}

            ///////////////////////////////////////////////////////////////////////////////////////
            // Handle test panel configuration
            if (Debug.isDebugBuild)
            {
                GSData testChessConfig = gameData.GetGSData(GSBackendKeys.TEST_CHESS_CONFIG);

                if (testChessConfig != null)
                {
                    chessboard.overrideFen = testChessConfig.GetString(GSBackendKeys.FEN);
                    chessboard.overrideAiStrength = (AiOverrideStrength)testChessConfig.GetInt(GSBackendKeys.AI_DIFFICULTY);
                    chessboard.overrideAiResignBehaviour = (AiOverrideResignBehaviour)testChessConfig.GetInt(GSBackendKeys.AI_RESIGN_BEHAVIOUR);
                }
            }
        }

        private void UpdateGame(string challengeId, GSData gameData)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // Update dynamic game data
            MatchInfo matchInfo = matchInfoModel.matches[challengeId];
            Chessboard chessboard = chessboardModel.chessboards[challengeId];
            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfo.opponentPublicProfile.playerId);


            GSData offerDraw = gameData.GetGSData(GSBackendKeys.OFFER_DRAW);
            if (offerDraw != null)
            {
                matchInfo.drawOfferStatus = offerDraw.GetString(GSBackendKeys.OFFER_DRAW_STATUS);
                matchInfo.drawOfferedBy = offerDraw.GetString(GSBackendKeys.OFFER_DRAW_OFFERED_BY);
            }

            if (playerData.ContainsKey(GSBackendKeys.POWER_UP_USED_COUNT))
            {
                matchInfo.playerPowerupUsedCount = playerData.GetInt(GSBackendKeys.POWER_UP_USED_COUNT).Value;
            }

            if (opponentData.ContainsKey(GSBackendKeys.POWER_UP_USED_COUNT))
            {
                matchInfo.opponentPowerupUsedCount = opponentData.GetInt(GSBackendKeys.POWER_UP_USED_COUNT).Value;
            }

            chessboard.isPlayerTurn = (gameData.GetString(GSBackendKeys.CURRENT_TURN_PLAYER_ID) == playerModel.id) ? true : false;
            GSData lastMove = gameData.GetGSData(GSBackendKeys.LAST_MOVE);

            if (!chessboard.isPlayerTurn)
            {
                chessboard.previousPlayerTurnFen = playerData.ContainsKey(GSBackendKeys.FEN) ? playerData.GetString(GSBackendKeys.FEN) : null;
            }
            chessboard.fen = gameData.GetString(GSBackendKeys.FEN);


            if (lastMove != null)
            {
                long lastMoveTimestamp = lastMove.GetLong(GSBackendKeys.TIME_STAMP).Value;
                chessboard.lastMoveTime = TimeUtil.ToDateTime(lastMoveTimestamp);
            }

            long playerTimerMs = playerData.GetLong(GSBackendKeys.TIMER).Value;
            long opponentTimerMs = opponentData.GetLong(GSBackendKeys.TIMER).Value;
            chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerTimerMs);
            chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentTimerMs);

            // Load move history
            IList<GSData> backendMoveList = gameData.GetGSDataList(GSBackendKeys.MOVE_LIST);
            chessboard.moveList = new List<ChessMove>();

            foreach (GSData data in backendMoveList)
            {
                ChessMove move = new ChessMove();
                string fromSquareStr = data.GetString(GSBackendKeys.FROM_SQUARE);
                string toSquareStr = data.GetString(GSBackendKeys.TO_SQUARE);

                move.from = chessService.GetFileRankLocation(fromSquareStr[0], fromSquareStr[1]);
                move.to = chessService.GetFileRankLocation(toSquareStr[0], toSquareStr[1]);
                move.promo = data.GetString(GSBackendKeys.PROMOTION);
                chessboard.moveList.Add(move);
            }

            // Store the game end reason and reason (if any)
            string gameEndReasonKey = gameData.GetString(GSBackendKeys.GAME_END_REASON);

            if (gameEndReasonKey != null)
            {
                GameEndReason gameEndReason = GSBackendKeys.GAME_END_REASON_MAP[gameEndReasonKey];
                chessboard.gameEndReason = gameEndReason;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Handle player turn
            //
            // The player's own move data / models are updated locally on the fly.
            // If the current turn player id is the local player, that means that
            // this message is related to the opponents move/
            //
            // Also handle the case where the game ended but the turns were not swapped since it 
            // was game over.
            bool isPlayerTurn = chessboard.isPlayerTurn;

            if (chessboard.isPlayerTurn || GameEndHasMove(chessboard))
            {
                UpdateMoveData(chessboard, gameData);

                chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerData.GetLong(GSBackendKeys.TIMER).Value);
                chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentData.GetLong(GSBackendKeys.TIMER).Value);
            }
        }

        private void UpdateMoveData(Chessboard chessboard, GSData gameData)
        {
            GSData lastMove = gameData.GetGSData(GSBackendKeys.LAST_MOVE);

            // Leave if last move is null
            if (lastMove == null)
                return;

            string fromSquare = lastMove.GetString(GSBackendKeys.FROM_SQUARE);
            string toSquare = lastMove.GetString(GSBackendKeys.TO_SQUARE);

            // Leave if no piece has moved yet (the last move might have a timestamp so we get to this point)
            // Not the cleanest code on the server.
            if (fromSquare == null)
                return;

            FileRank fromFileRank;
            fromFileRank.file = Array.IndexOf(GSFileRank.GSFiles, fromSquare[0].ToString());
            fromFileRank.rank = Array.IndexOf(GSFileRank.GSRanks, fromSquare[1].ToString());

            FileRank toFileRank;
            toFileRank.file = Array.IndexOf(GSFileRank.GSFiles, toSquare[0].ToString());
            toFileRank.rank = Array.IndexOf(GSFileRank.GSRanks, toSquare[1].ToString());

            chessboard.opponentFromSquare = chessboard.squares[fromFileRank.file, fromFileRank.rank];
            chessboard.opponentToSquare = chessboard.squares[toFileRank.file, toFileRank.rank];
            chessboard.opponentMoveFlag = GSBackendKeys.MOVE_FLAG_MAP[lastMove.GetString(GSBackendKeys.MOVE_FLAG)];
            chessboard.fiftyMoveDrawAvailable = gameData.GetBoolean(GSBackendKeys.IS_FIFTY_MOVE_RULE_ACTIVE).Value;
            chessboard.threefoldRepeatDrawAvailable = gameData.GetBoolean(GSBackendKeys.IS_THREEFOLD_REPEAT_RULE_ACTIVE).Value;
        }

        private void HandleActiveMove(string challengeId)
        {
            if (challengeId != matchInfoModel.activeChallengeId)
                return;

            Chessboard chessboard = chessboardModel.chessboards[challengeId];
            if (chessboard.isPlayerTurn)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);
            }
            else
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
            }
        }

        private void HandleActiveGameEnd(string challengeId)
        {
            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            #region analytics
            preferencesModel.gameFinishedCount++;
            appsFlyerService.TrackLimitedEvent(AnalyticsEventId.game_finished, preferencesModel.gameFinishedCount);

            if (!preferencesModel.isInstallDayOver && preferencesModel.gameFinishedCount <= 10)
            {
                appsFlyerService.TrackRichEvent("install_day_game_finished_" + preferencesModel.gameFinishedCount);
            }

            if (matchInfoModel.matches.ContainsKey(challengeId))
            {
                var matchInfo = matchInfoModel.matches[challengeId];
                var context = matchInfo.isLongPlay ? AnalyticsContext.long_match : AnalyticsContext.quick_match;
                hAnalyticsService.LogMultiplayerGameEvent(AnalyticsEventId.game_finished.ToString(), "gameplay", context.ToString(), challengeId);

                if (!preferencesModel.isInstallDayOver && preferencesModel.gameFinishedCount <= 10)
                {
                    hAnalyticsService.LogMultiplayerGameEvent("install_day_game_finished_" + preferencesModel.gameFinishedCount, "gameplay", context.ToString(), challengeId);
                }

                if (matchInfo.isRanked)
                {
                    preferencesModel.rankedMatchesFinishedCount++;

                    if (preferencesModel.rankedMatchesFinishedCount >= 15 && !preferencesModel.isFirstRankedGameOfTheDayFinished)
                    {
                        preferencesModel.isFirstRankedGameOfTheDayFinished = true;
                        analyticsService.Event(AnalyticsEventId.elo, AnalyticsParameter.elo, playerModel.eloScore);
                    }
                }

                gameModesAnalyticsService.ProcessGameCount(matchInfo);
            }            
            #endregion

            if (challengeId != matchInfoModel.activeChallengeId)
                return;

            // Add cases where the game ending does not have a move to the checks below
            if (GameEndHasMove(chessboard))
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);
            }

            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ENDED);
        }

        private bool GameEndHasMove(Chessboard chessboard)
        {
            GameEndReason gameEndReason = chessboard.gameEndReason;

            if ((!chessboard.isPlayerTurn) &&
                (gameEndReason != GameEndReason.PLAYER_DISCONNECTED) &&
                (gameEndReason != GameEndReason.RESIGNATION) &&
                (gameEndReason != GameEndReason.TIMER_EXPIRED) &&
                (gameEndReason != GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE) &&
                (gameEndReason != GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE) &&
                (gameEndReason != GameEndReason.DRAW_BY_DRAW_OFFERED) &&
                (gameEndReason != GameEndReason.DECLINED) &&
                (gameEndReason != GameEndReason.NONE))
            {
                return true;
            }

            return false;
        }
    }
}
