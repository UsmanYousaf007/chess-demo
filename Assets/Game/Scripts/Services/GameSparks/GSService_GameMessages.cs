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
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }

        private void AddGameMessageListeners()
        {
            ChallengeTurnTakenMessage.Listener += OnChallengeTurnTakenMessage;
        }

        private void OnChallengeTurnTakenMessage(ChallengeTurnTakenMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            InitChallengeMessage(challengeId, message.ScriptData);

            bool isActiveChallenge = (matchInfoModel.activeChallengeId == challengeId) ? true : false;

            Chessboard chessboard = chessboardModel.chessboards[challengeId];
            chessboard.isPlayerTurn = (message.Challenge.NextPlayer == playerModel.id) ? true : false;

            // The player's own move data / models are updated locally on the fly.
            // If the current turn player id is the local player, that means that
            // this message is related to the opponents move
            if (chessboard.isPlayerTurn)
            {
                GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

                if (message.Challenge.ShortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE)
                {
                    UpdateTimerData(chessboard, gameData);
                }

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
            InitChallengeMessage(challengeId, message.ScriptData);

            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            chessboard.isPlayerTurn = (message.Challenge.NextPlayer == playerModel.id) ? true : false;
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

            bool isQuickMatch = (message.Challenge.ShortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE) ?
                true : false;

            AnnounceResults(challengeId, chessboard, gameData, playerModel.id, isQuickMatch);
        }

        private void OnGameChallengeLostMessage(ChallengeLostMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            InitChallengeMessage(challengeId, message.ScriptData);

            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            chessboard.isPlayerTurn = (message.Challenge.NextPlayer == playerModel.id) ? true : false;
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

            bool isQuickMatch = (message.Challenge.ShortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE) ?
                true : false;
            
            AnnounceResults(challengeId, chessboard, gameData, matchInfoModel.activeMatch.opponentPublicProfile.playerId, isQuickMatch);
        }

        private void OnGameChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            string challengeId = message.Challenge.ChallengeId;
            InitChallengeMessage(challengeId, message.ScriptData);

            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            chessboard.isPlayerTurn = (message.Challenge.NextPlayer == playerModel.id) ? true : false;
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

            bool isQuickMatch = (message.Challenge.ShortCode == GSBackendKeys.Match.QUICK_MATCH_SHORT_CODE) ?
                true : false;
            
            AnnounceResults(challengeId, chessboard, gameData, null, isQuickMatch);
        }
    }
}
