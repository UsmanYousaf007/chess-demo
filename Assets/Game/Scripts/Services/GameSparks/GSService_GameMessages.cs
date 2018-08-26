/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-16 06:14:00 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

using GameSparks.Api.Messages;
using GameSparks.Core;

using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;
using GameSparks.Api.Responses;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }

        private void AddGameMessageListeners()
        {
            ChallengeTurnTakenMessage.Listener += OnChallengeTurnTakenMessage;
        }

        private void OnChallengeTurnTakenMessage(ChallengeTurnTakenMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            bool isActiveChallenge = (matchInfoModel.activeChallengeId == challengeId) ? true : false;

            Chessboard chessboard = chessboardModel.chessboards[challengeId];
            chessboard.currentTurnPlayerId = message.Challenge.NextPlayer;

            // The player's own move data / models are updated locally on the fly.
            // If the current turn player id is the local player, that means that
            // this message is related to the opponents move
            if (chessboard.currentTurnPlayerId == playerModel.id)
            {
                GSData gameData = message.Challenge.ScriptData
                    .GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY)
                    .GetGSData(GSBackendKeys.GAME_DATA);

                UpdateTimerData(chessboard, gameData);
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
                    chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
                }
            }
        }

        private void OnGameChallengeWonMessage(ChallengeWonMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            chessboard.currentTurnPlayerId = message.Challenge.NextPlayer;
            GSData gameData = message.Challenge.ScriptData
                .GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY)
                .GetGSData(GSBackendKeys.GAME_DATA);
            AnnounceResults(challengeId, chessboard, gameData, playerModel.id);
        }

        private void OnGameChallengeLostMessage(ChallengeLostMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            chessboard.currentTurnPlayerId = message.Challenge.NextPlayer;
            GSData gameData = message.Challenge.ScriptData
                .GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY)
                .GetGSData(GSBackendKeys.GAME_DATA);
            AnnounceResults(challengeId, chessboard, gameData, matchInfoModel.activeMatch.opponentPublicProfile.playerId);
        }

        private void OnGameChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            chessboard.currentTurnPlayerId = message.Challenge.NextPlayer;
            GSData gameData = message.Challenge.ScriptData
                .GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY)
                .GetGSData(GSBackendKeys.GAME_DATA);
            AnnounceResults(challengeId, chessboard, gameData, null);
        }

        private void LoadChessboardModel(GSData gameData, string challengeId)
        {
            Chessboard chessboard = new Chessboard();
            chessboardModel.chessboards[challengeId] = chessboard;

            long gameDuration = gameData.GetLong(GSBackendKeys.GAME_DURATION).Value;
            chessboard.gameDuration = TimeSpan.FromMilliseconds(gameDuration);
            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfoModel.activeMatch.opponentPublicProfile.playerId);
            long playerTimerMs = playerData.GetLong(GSBackendKeys.TIMER).Value;
            long opponentTimerMs = opponentData.GetLong(GSBackendKeys.TIMER).Value;
            chessboard.backendPlayerTimer = TimeSpan.FromMilliseconds(playerTimerMs);
            chessboard.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentTimerMs);
            chessboard.currentTurnPlayerId = gameData.GetString(GSBackendKeys.CURRENT_TURN_PLAYER_ID);
            chessboard.isAiGame = matchInfoModel.activeMatch.isBotMatch;
            chessboard.playerColor = GSBackendKeys.PLAYER_COLOR_MAP[playerData.GetString(GSBackendKeys.COLOR)];
            chessboard.opponentColor = GSBackendKeys.PLAYER_COLOR_MAP[opponentData.GetString(GSBackendKeys.COLOR)];
            chessboard.fen = gameData.GetString(GSBackendKeys.FEN);

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

        private void AnnounceResults(string challengeId, Chessboard chessboard, GSData gameData, string winnerId)
        {
            UpdateTimerData(chessboard, gameData);

            GameEndReason gameEndReason = GSBackendKeys.GAME_END_REASON_MAP[gameData.GetString(GSBackendKeys.GAME_END_REASON)];
            chessboard.gameEndReason = gameEndReason;
            chessboard.winnerId = winnerId;

            // Add cases where the game ending does not have a move to the checks below
            bool gameEndHasMove = ((chessboard.currentTurnPlayerId == matchInfoModel.activeMatch.opponentPublicProfile.playerId) &&
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
