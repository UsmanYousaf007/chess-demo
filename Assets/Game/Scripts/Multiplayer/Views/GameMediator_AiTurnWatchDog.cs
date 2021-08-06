/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using GameSparks.Core;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using System.Collections;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectDataSignal { get; set; }

        private Coroutine aiTurnWatchdogCR = null;

        [ListensTo(typeof(AiTurnRequestedSignal))]
        public void OnAiTurnRequested(float delay)
        {
            LogUtil.Log("AiTurnWatchdog: Start AiTurnWatchdog", "cyan");

            aiTurnWatchdogCR = routineRunner.StartCoroutine(AiTurnWatchdogCR(-2.0f + delay));
        }

        [ListensTo(typeof(ChallengeMessageProcessedSignal))]
        public void OnAiTurnChallengeMessageProcessed(string challengeId)
        {
            if (challengeId != null && challengeId == matchInfoModel.activeChallengeId)
            {
                if (aiTurnWatchdogCR != null)
                {
                    LogUtil.Log("AiTurnWatchdog: No need to sync data", "cyan");

                    routineRunner.StopCoroutine(aiTurnWatchdogCR);
                    aiTurnWatchdogCR = null;
                }
            }
        }

        private IEnumerator AiTurnWatchdogCR(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            if (chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                LogUtil.Log("AiTurnWatchdog: SyncReconnectData", "cyan");

                view.chessboardBlocker.SetActive(true);
                backendService.SyncReconnectData(matchInfoModel.activeChallengeId).Then(OnAiTurnSyncReconnectData);
            }
        }

        private void OnAiTurnSyncReconnectData(BackendResult backendResult)
        {
            if (chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                view.chessboardBlocker.SetActive(false);
            }

            if (backendResult == BackendResult.SUCCESS)
            {
                // Apply the match state if it has not ended
                if (chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
                {
                    LogUtil.Log("AiTurnWatchdog: OnSyncReconnectData", "cyan");

                    Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
                    MatchInfo activeMatchInfo = matchInfoModel.activeMatch;
                    activeChessboard.currentState = null;

                    stopTimersSignal.Dispatch();
                    startGameSignal.Dispatch();
                }
            }
        }
    }
}
