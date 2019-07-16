﻿/// @license Propriety <http://license.url>
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
        [Inject] public RunTimeControlSignal runTimeControlSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }


        private void AddGameMessageListeners()
        {
            ChallengeTurnTakenMessage.Listener += OnChallengeTurnTakenMessage;
            ScriptMessage.Listener += OnGameScriptMessage;
        }

        private void RemoveGameMessageListeners()
        {
            ChallengeTurnTakenMessage.Listener -= OnChallengeTurnTakenMessage;
            ScriptMessage.Listener -= OnGameScriptMessage;
        }

        private void OnChallengeTurnTakenMessage(ChallengeTurnTakenMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);

            if (GameSparksOutOfOrderPatchFailed(message.Challenge.ChallengeId, challengeData))
            {
                LogUtil.Log("OUT OF ORDER MESSAGE!!!", "red");
                return;
            }

            ParseChallengeData(message.Challenge.ChallengeId, challengeData);
            HandleActiveMove(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeWonMessage(ChallengeWonMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData, true);
            HandleActiveGameEnd(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeLostMessage(ChallengeLostMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData, true);
            HandleActiveGameEnd(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData, true);
            HandleActiveGameEnd(message.Challenge.ChallengeId);
        }

        private void OnGameScriptMessage(ScriptMessage message)
        {
            if (message.ExtCode == GSBackendKeys.CHALLENGE_ACCEPT_MESSAGE)
            {
                GSData challengeData = message.Data.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                string challengeId = message.Data.GetString(GSBackendKeys.ChallengeData.CHALLENGE_ID);
                ParseChallengeData(challengeId, challengeData, false);

                // If it is not the active challenge, we are done updating the challenge state
                if (challengeId != matchInfoModel.activeChallengeId)
                {
                    return;
                }

                if (matchInfoModel.activeMatch != null && playerModel.id == matchInfoModel.activeMatch.challengedId)
                {
                    return;
                }

                // If I'm the challenger and viewing the board, and it is the opponents move,
                // then the opponents clock should start ticking.
                RunTimeControlVO vo;
                vo.pauseAfterSwap = false;
                vo.waitingForOpponentToAccept = false;
                vo.playerJustAcceptedOnPlayerTurn = false;
                runTimeControlSignal.Dispatch(vo);
            }
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
