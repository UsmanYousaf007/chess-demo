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
            if (!matchInfo.isLongPlay)
            {
                long gameDuration = gameData.GetLong(GSBackendKeys.GAME_DURATION).Value;
                chessboard.gameDuration = TimeSpan.FromMilliseconds(gameDuration);
            }

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

            chessboard.isPlayerTurn = (gameData.GetString(GSBackendKeys.CURRENT_TURN_PLAYER_ID) == playerModel.id) ? true : false;
            chessboard.fen = gameData.GetString(GSBackendKeys.FEN);
            GSData lastMove = gameData.GetGSData(GSBackendKeys.LAST_MOVE);

            if (lastMove != null)
            {
                long lastMoveTimestamp = lastMove.GetLong(GSBackendKeys.TIME_STAMP).Value;
                chessboard.lastMoveTime = TimeUtil.ToDateTime(lastMoveTimestamp);
            }

            if (!matchInfo.isLongPlay)
            {    
                long playerTimerMs = playerData.GetLong(GSBackendKeys.TIMER).Value;
                long opponentTimerMs = opponentData.GetLong(GSBackendKeys.TIMER).Value;
                chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerTimerMs);
                chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentTimerMs);
            }
            else
            {
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
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Handle player turn
            //
            // The player's own move data / models are updated locally on the fly.
            // If the current turn player id is the local player, that means that
            // this message is related to the opponents move
            if (chessboard.isPlayerTurn)
            {
                UpdateMoveData(chessboard, gameData);

                if (!matchInfo.isLongPlay)
                {
                    // Update quick match timers
                    chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerData.GetLong(GSBackendKeys.TIMER).Value);
                    chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentData.GetLong(GSBackendKeys.TIMER).Value);
                }
            }

            // Store the game end reason and reason (if any)
            string gameEndReasonKey = gameData.GetString(GSBackendKeys.GAME_END_REASON);
            if (gameEndReasonKey != null)
            {
                GameEndReason gameEndReason = GSBackendKeys.GAME_END_REASON_MAP[gameEndReasonKey];
                chessboard.gameEndReason = gameEndReason;
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
            if (challengeId != matchInfoModel.activeChallengeId)
                return;

            Chessboard chessboard = chessboardModel.chessboards[challengeId];
            GameEndReason gameEndReason = chessboard.gameEndReason;

            // Add cases where the game ending does not have a move to the checks below
            if ((!chessboard.isPlayerTurn) &&
                (gameEndReason != GameEndReason.PLAYER_DISCONNECTED) &&
                (gameEndReason != GameEndReason.RESIGNATION) &&
                (gameEndReason != GameEndReason.TIMER_EXPIRED) &&
                (gameEndReason != GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE) &&
                (gameEndReason != GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE) &&
                (gameEndReason != GameEndReason.DECLINED))
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);
            }

            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ENDED);
        }
    }
}
