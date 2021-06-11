/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-16 13:27:47 UTC+05:00
///
/// @description
/// [add_description_here]

using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.promise.api;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public class AiTurnCommand : Command
    {
        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectDataSignal { get; set; }
        [Inject] public AiTurnRequestedSignal aiTurnRequestedSignal { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }

        Chessboard chessboard;
        float startTime;
        AiMoveInputVO vo;
        private string challengeId = null;

        public override void Execute()
        {
            Retain();
            startTime = Time.time;
            challengeId = matchInfoModel.activeChallengeId;

            chessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            if (chessboard == null)
            {
                throw new System.Exception("chessboard is null");
            }

            ++chessboard.aiMoveNumber;

            vo = new AiMoveInputVO();
            vo.aiColor = chessboard.opponentColor;
            vo.playerColor = chessboard.playerColor;
            vo.lastPlayerMove = chessboard.lastPlayerMove;
            vo.squares = chessboard.squares;
            vo.opponentTimer = chessboard.backendOpponentTimer;
            vo.aiMoveNumber = chessboard.aiMoveNumber;
            vo.fen = chessService.GetFen();
            vo.isBot = true;

            // Strength
            vo.cpuStrengthPct = matchInfoModel.activeMatch.botDifficulty;

            if (chessboard.overrideAiStrength == AiOverrideStrength.SMART)
            {
                vo.cpuStrengthPct = 1f;
            }
            else if (chessboard.overrideAiStrength == AiOverrideStrength.STUPID)
            {
                vo.cpuStrengthPct = 0f;
            }

            // Speed
            if (vo.opponentTimer.TotalSeconds < 10)
            {
                vo.aiMoveDelay = AiMoveDelay.NONE;
            }
            else if (vo.opponentTimer.TotalSeconds < 30)
            {
                vo.aiMoveDelay = AiMoveDelay.CPU;
            }
            else if (matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.OneMin)
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_1M;
            }
            else if (matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.ThreeMin)
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_3M;
            }
            else if (matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.TenMin)
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_10M;
            }
            else if (matchInfoModel.activeMatch.gameTimeMode == GameTimeMode.ThirtyMin)
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_30M;
            }
            else
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_5M;
            }

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo);

            if (promise == null)
            {
                throw new System.Exception("promise is null");
            }

            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            //routineRunner.StartCoroutine(SimulateDelay(from, to, promo));
            SimulateDelay(from, to, promo);
        }

        void SimulateDelay(FileRank from, FileRank to, string promo)
        {
            float delay = 0f;

            if (vo.aiMoveDelay == AiMoveDelay.CPU)
            {
                delay = AiMoveTimes.M_CPU;
            }
            else if (vo.aiMoveDelay == AiMoveDelay.ONLINE_1M)
            {
                const float M1_DELAY_FACTOR = 1.5f;

                int index = Mathf.Min(vo.aiMoveNumber, AiMoveTimes.M_1.Length - 1);
                float[] delayRange = AiMoveTimes.M_1[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
                delay = delay * M1_DELAY_FACTOR;
            }
            else if (vo.aiMoveDelay == AiMoveDelay.ONLINE_3M)
            {
                const float M3_DELAY_FACTOR = 1.5f;

                int index = Mathf.Min(vo.aiMoveNumber, AiMoveTimes.M_3.Length - 1);
                float[] delayRange = AiMoveTimes.M_3[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
                delay = delay * M3_DELAY_FACTOR;
            }
            else if (vo.aiMoveDelay == AiMoveDelay.ONLINE_5M)
            {
                int index = Mathf.Min(vo.aiMoveNumber, AiMoveTimes.M_5.Length - 1);
                float[] delayRange = AiMoveTimes.M_5[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
            }
            else if (vo.aiMoveDelay == AiMoveDelay.ONLINE_10M)
            {
                int index = Mathf.Min(vo.aiMoveNumber, AiMoveTimes.M_10.Length - 1);
                float[] delayRange = AiMoveTimes.M_10[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
            }
            else if (vo.aiMoveDelay == AiMoveDelay.ONLINE_30M)
            {
                int index = Mathf.Min(vo.aiMoveNumber, AiMoveTimes.M_30.Length - 1);
                float[] delayRange = AiMoveTimes.M_30[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
            }

            float timeElapsed = Time.time - startTime;
            delay -= timeElapsed;
            delay = Mathf.Max(0, delay);
            //yield return new WaitForSecondsRealtime(delay);

            if (chessboard.aiWillResign &&
                chessService.GetScore(chessboard.playerColor) > BotSettings.AI_RESIGN_SCORE_THRESHOLD &&
                chessboard.backendPlayerTimer.TotalMinutes > 1 &&
                !chessAiService.IsReactionaryCaptureAvailable())
            {
                backendService.AiResign().Then(OnResign);
            }
            else
            {
                backendService.AiTurn(from, to, promo, (long)delay).Then(OnTurnTaken);
                aiTurnRequestedSignal.Dispatch(delay);

            }
        }

        private void OnTurnTaken(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                syncReconnectDataSignal.Dispatch(challengeId);
            }

            Release();
        }

        private void OnResign(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                syncReconnectDataSignal.Dispatch(challengeId);
            }

            Release();
        }

            
    }


}
