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
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);

            if (GameSparksOutOfOrderPatchFailed(message.Challenge.ChallengeId, challengeData))
            {
                LogUtil.Log("OUT OF ORDER MESSAGE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return;
            }

            ParseChallengeData(message.Challenge.ChallengeId, challengeData);
            HandleActiveMove(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeWonMessage(ChallengeWonMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData);
            HandleActiveGameEnd(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeLostMessage(ChallengeLostMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData);
            HandleActiveGameEnd(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData);
            HandleActiveGameEnd(message.Challenge.ChallengeId);
        }

        private bool GameSparksOutOfOrderPatchFailed(string challengeId, GSData challengeData)
        {
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);
            IList<GSData> backendMoveList = gameData.GetGSDataList(GSBackendKeys.MOVE_LIST);

            if (chessboardModel.chessboards[challengeId].moveList.Count >= backendMoveList.Count)
            {
                return true;
            }

            return false;
        }
    }
}
