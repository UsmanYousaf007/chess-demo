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
        [Inject] public RunTimeControlSignal runTimeControlSignal { get; set; }
        [Inject] public ChallengeMessageProcessedSignal challengeMessageProcessedSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        


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
                LogUtil.Log("OnChallengeTurnTakenMessage() OUT OF ORDER MESSAGE!!!", "red");
                return;
            }

            //[Note] : As game reconnects, We sync data from server to update the local game state,
            // This message also updates the local game state So to AVOID doublication / Ai crash :  we simply returns 
            if (appInfoModel.syncInProgress)
            { 
                LogUtil.Log("OnChallengeTurnTakenMessage() SYNC DATA IS IN PROCESS So RETURN PLZ !!!", "red");
                return;
            }

            ParseChallengeData(message.Challenge.ChallengeId, challengeData);
            HandleActiveMove(message.Challenge.ChallengeId);

            challengeMessageProcessedSignal.Dispatch(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeWonMessage(ChallengeWonMessage message)
        {
            if (appInfoModel.syncInProgress)
            {
                LogUtil.Log("OnGameChallengeWonMessage() SYNC DATA IS IN PROCESS So RETURN PLZ !!!", "red");
                return;
            }

            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData, true);
            HandleActiveGameEnd(message.Challenge.ChallengeId);

            challengeMessageProcessedSignal.Dispatch(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeLostMessage(ChallengeLostMessage message)
        {
            if (appInfoModel.syncInProgress)
            {
                LogUtil.Log("OnGameChallengeLostMessage() SYNC DATA IS IN PROCESS So RETURN PLZ !!!", "red");
                return;
            }

            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            ParseChallengeData(message.Challenge.ChallengeId, challengeData, true);
            HandleActiveGameEnd(message.Challenge.ChallengeId);

            challengeMessageProcessedSignal.Dispatch(message.Challenge.ChallengeId);
        }

        private void OnGameChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            if (appInfoModel.syncInProgress)
            {
                LogUtil.Log("OnGameChallengeDrawnMessage() SYNC DATA IS IN PROCESS So RETURN PLZ !!!", "red");
                return;
            }

            GSData challengeData = message.ScriptData.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

            // Store the game end reason and reason (if any)
            string gameEndReasonKey = gameData.GetString(GSBackendKeys.GAME_END_REASON);

            if (gameEndReasonKey != null)
            {
                GameEndReason gameEndReason = GSBackendKeys.GAME_END_REASON_MAP[gameEndReasonKey];
                //chessboard.gameEndReason = gameEndReason;

                if (gameEndReason.Equals(GameEndReason.ABANDON))
                {
                    Debug.Log("MATCH ABANDON  > > > > > > > > > > " + message.Challenge.ChallengeId);

                    matchInfoModel.matches.Remove(message.Challenge.ChallengeId);
                    chessboardModel.chessboards.Remove(message.Challenge.ChallengeId);
                    matchInfoModel.activeChallengeId = null;
                    matchInfoModel.activeLongMatchOpponentId = null;
                    loadLobbySignal.Dispatch();
                    return;
                }
            }

            ParseChallengeData(message.Challenge.ChallengeId, challengeData, true);
            HandleActiveGameEnd(message.Challenge.ChallengeId, true);

            challengeMessageProcessedSignal.Dispatch(message.Challenge.ChallengeId);
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

                challengeMessageProcessedSignal.Dispatch(challengeId);
            }else if (message.ExtCode == GSBackendKeys.CHALLENGE_OFFER_DRAW_MESSAGE)
            {
                GSData challengeData = message.Data.GetGSData(GSBackendKeys.ChallengeData.CHALLENGE_DATA_KEY);
                string challengeId = message.Data.GetString(GSBackendKeys.ChallengeData.CHALLENGE_ID);

                // If it is not the active challenge, we are done updating the challenge state
                //if (challengeId != matchInfoModel.activeChallengeId)
                //{
                  //  return;
                //}

                ParseChallengeDataOfferDraw(challengeId, challengeData, false);
            }
        }

        private bool GameSparksOutOfOrderPatchFailed(string challengeId, GSData challengeData)
        {
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);
            IList<GSData> backendMoveList = gameData.GetGSDataList(GSBackendKeys.MOVE_LIST);

            if (chessboardModel.chessboards.ContainsKey(challengeId) == false ||
                    chessboardModel.chessboards[challengeId].moveList.Count >= backendMoveList.Count)
            {
                return true;
            }

            return false;
        }
    }
}
