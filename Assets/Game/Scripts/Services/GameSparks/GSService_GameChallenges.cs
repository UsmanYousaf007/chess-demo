/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System;

using GameSparks.Api.Messages;
using GameSparks.Core;

using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;
using GameSparks.Api.Responses;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        // Services
        [Inject] public IChessService chessService { get; set; }

        private void LoadGameData(GSData gameData, string challengeId)
        {
            Chessboard chessboard = new Chessboard();
            chessboardModel.chessboards[challengeId] = chessboard;
            MatchInfo matchInfo = matchInfoModel.matches[challengeId];
            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfo.opponentPublicProfile.playerId);

            chessboard.isPlayerTurn = (gameData.GetString(GSBackendKeys.CURRENT_TURN_PLAYER_ID) == playerModel.id) ? true : false;
            chessboard.isAiGame = matchInfo.isBotMatch;
            chessboard.playerColor = GSBackendKeys.PLAYER_COLOR_MAP[playerData.GetString(GSBackendKeys.COLOR)];
            chessboard.opponentColor = GSBackendKeys.PLAYER_COLOR_MAP[opponentData.GetString(GSBackendKeys.COLOR)];
            chessboard.fen = gameData.GetString(GSBackendKeys.FEN);

            long gameDuration = gameData.GetLong(GSBackendKeys.GAME_DURATION).Value;
            if (gameDuration > 0)
            {
                chessboard.gameDuration = TimeSpan.FromMilliseconds(gameDuration);
                long playerTimerMs = playerData.GetLong(GSBackendKeys.TIMER).Value;
                long opponentTimerMs = opponentData.GetLong(GSBackendKeys.TIMER).Value;
                chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerTimerMs);
                chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentTimerMs);
            }
            else
            {
                LoadResumeData(gameData, challengeId);
            }

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

        private void LoadResumeData(GSData gameData, string challengeId)
        {
            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            // Load up the backend moves
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
        }

        private void AnnounceResults(string challengeId, Chessboard chessboard, GSData gameData, string winnerId, bool isQuickMatch)
        {
            if (isQuickMatch)
            {
                UpdateTimerData(chessboard, gameData);
            }

            GameEndReason gameEndReason = GSBackendKeys.GAME_END_REASON_MAP[gameData.GetString(GSBackendKeys.GAME_END_REASON)];
            chessboard.gameEndReason = gameEndReason;
            chessboard.winnerId = winnerId;

            // Add cases where the game ending does not have a move to the checks below
            bool gameEndHasMove = ((!chessboard.isPlayerTurn) &&
                (gameEndReason != GameEndReason.PLAYER_DISCONNECTED) &&
                (gameEndReason != GameEndReason.RESIGNATION) &&
                (gameEndReason != GameEndReason.TIMER_EXPIRED) &&
                (gameEndReason != GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE) &&
                (gameEndReason != GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE));

            // We update move data only if there was a move made that ended this game and
            // it was the opponents turn.
            bool isActiveChallenge = (matchInfoModel.activeChallengeId == challengeId) ? true : false;

            if (gameEndHasMove)
            {
                UpdateMoveData(chessboard, gameData);

                if (isActiveChallenge)
                {
                    chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);
                }
            }
            else
            {
                if (isActiveChallenge)
                {
                    chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ENDED);
                }
            }
        }

        private void UpdateMoveData(Chessboard chessboard, GSData gameData)
        {
            GSData lastMove = gameData.GetGSData(GSBackendKeys.LAST_MOVE);
            string fromSquare = lastMove.GetString(GSBackendKeys.FROM_SQUARE);
            string toSquare = lastMove.GetString(GSBackendKeys.TO_SQUARE);

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

        private void UpdateTimerData(Chessboard chessboard, GSData gameData)
        {
            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfoModel.activeMatch.opponentPublicProfile.playerId);

            chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerData.GetLong(GSBackendKeys.TIMER).Value);
            chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentData.GetLong(GSBackendKeys.TIMER).Value);
        }


    }
}
