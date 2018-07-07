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

        private void RemoveGameMessageListeners()
        {
            ChallengeTurnTakenMessage.Listener -= OnChallengeTurnTakenMessage;
        }

        private void OnChallengeTurnTakenMessage(ChallengeTurnTakenMessage message)
        {
            if (!IsCurrentChallenge(message.Challenge.ChallengeId))
            {
               return;
            }

            chessboardModel.currentTurnPlayerId = message.Challenge.NextPlayer;

            // The player's own move data / models are updated locally on the fly.
            // If the current turn player id is the local player, that means that
            // this message is related to the opponents move
            if (chessboardModel.currentTurnPlayerId == playerModel.id)
            {
                GSData gameData = message.ScriptData.GetGSData(GSBackendKeys.GAME_DATA);
                UpdateTimerData(gameData);
                UpdateMoveData(gameData);
                chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);
            }
            else
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.PLAYER_MOVE_COMPLETE);
            }
        }

        private void OnGameChallengeWonMessage(ChallengeWonMessage message)
        {
            chessboardModel.currentTurnPlayerId = message.Challenge.NextPlayer;
            GSData gameData = message.ScriptData.GetGSData(GSBackendKeys.GAME_DATA);
            AnnounceResults(gameData, playerModel.id);
        }

        private void OnGameChallengeLostMessage(ChallengeLostMessage message)
        {
            chessboardModel.currentTurnPlayerId = message.Challenge.NextPlayer;
            GSData gameData = message.ScriptData.GetGSData(GSBackendKeys.GAME_DATA);
            AnnounceResults(gameData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnGameChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            chessboardModel.currentTurnPlayerId = message.Challenge.NextPlayer;
            GSData gameData = message.ScriptData.GetGSData(GSBackendKeys.GAME_DATA);
            AnnounceResults(gameData, null);
        }

        private void LoadChessboardModel(GSData gameData)
        {
            chessboardModel.Reset();

            long gameDuration = gameData.GetLong(GSBackendKeys.GAME_DURATION).Value;
            chessboardModel.gameDuration = TimeSpan.FromMilliseconds(gameDuration);

            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfoModel.opponentPublicProfile.id);

            long playerTimerMs = playerData.GetLong(GSBackendKeys.TIMER).Value;
            long opponentTimerMs = opponentData.GetLong(GSBackendKeys.TIMER).Value;

            chessboardModel.backendPlayerTimer = TimeSpan.FromMilliseconds(playerTimerMs);
            chessboardModel.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentTimerMs);
            chessboardModel.currentTurnPlayerId = gameData.GetString(GSBackendKeys.CURRENT_TURN_PLAYER_ID);
            chessboardModel.isAiGame = matchInfoModel.isBotMatch;
            chessboardModel.playerColor = GSBackendKeys.PLAYER_COLOR_MAP[playerData.GetString(GSBackendKeys.COLOR)];
            chessboardModel.opponentColor = GSBackendKeys.PLAYER_COLOR_MAP[opponentData.GetString(GSBackendKeys.COLOR)];
            chessboardModel.fen = gameData.GetString(GSBackendKeys.FEN);

            if (Debug.isDebugBuild)
            {
                GSData testChessConfig = gameData.GetGSData(GSBackendKeys.TEST_CHESS_CONFIG);

                if (testChessConfig != null)
                {
                    chessboardModel.overrideFen = testChessConfig.GetString(GSBackendKeys.FEN);
                    chessboardModel.overrideAiStrength = (AiOverrideStrength)testChessConfig.GetInt(GSBackendKeys.AI_DIFFICULTY);
                    chessboardModel.overrideAiResignBehaviour = (AiOverrideResignBehaviour)testChessConfig.GetInt(GSBackendKeys.AI_RESIGN_BEHAVIOUR);
                    chessboardModel.overrideAiSpeed = (AiOverrideSpeed)testChessConfig.GetInt(GSBackendKeys.AI_SPEED);
                }
            }
        }

        private void AnnounceResults(GSData gameData, string winnerId)
        {
            UpdateTimerData(gameData);

            GameEndReason gameEndReason = GSBackendKeys.GAME_END_REASON_MAP[gameData.GetString(GSBackendKeys.GAME_END_REASON)];
            chessboardModel.gameEndReason = gameEndReason;
            chessboardModel.winnerId = winnerId;

            // Add cases where the game ending does not have a move to the checks below
            bool gameEndHasMove = ((chessboardModel.currentTurnPlayerId == matchInfoModel.opponentPublicProfile.id) &&
                                   (gameEndReason != GameEndReason.PLAYER_DISCONNECTED) &&
                                   (gameEndReason != GameEndReason.RESIGNATION) &&
                                   (gameEndReason != GameEndReason.TIMER_EXPIRED) &&
                                   (gameEndReason != GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE) &&
                                   (gameEndReason != GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE));

            // We update move data only if there was a move made that ended this game and
            // it was the opponents turn.
            if (gameEndHasMove)
            {
                UpdateMoveData(gameData);
                chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);
            }
            else
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ENDED);
            }

            // Finally update the player model
            playerModel.eloScore = 0;
            playerModel.totalGamesWon = 0;
            playerModel.totalGamesLost = 0;
            playerModel.totalGamesDrawn = 0;
        }

        private void UpdateMoveData(GSData gameData)
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

            chessboardModel.opponentFromSquare = chessboardModel.squares[fromFileRank.file, fromFileRank.rank];
            chessboardModel.opponentToSquare = chessboardModel.squares[toFileRank.file, toFileRank.rank];
            chessboardModel.opponentMoveFlag = GSBackendKeys.MOVE_FLAG_MAP[lastMove.GetString(GSBackendKeys.MOVE_FLAG)];
            chessboardModel.fiftyMoveDrawAvailable = gameData.GetBoolean(GSBackendKeys.IS_FIFTY_MOVE_RULE_ACTIVE).Value;
            chessboardModel.threefoldRepeatDrawAvailable = gameData.GetBoolean(GSBackendKeys.IS_THREEFOLD_REPEAT_RULE_ACTIVE).Value;
        }

        private void UpdateTimerData(GSData gameData)
        {
            GSData playerData = gameData.GetGSData(playerModel.id);
            GSData opponentData = gameData.GetGSData(matchInfoModel.opponentPublicProfile.id);

            chessboardModel.backendPlayerTimer = TimeSpan.FromMilliseconds(playerData.GetLong(GSBackendKeys.TIMER).Value);
            chessboardModel.backendOpponentTimer = TimeSpan.FromMilliseconds(opponentData.GetLong(GSBackendKeys.TIMER).Value);
        }
    }
}
