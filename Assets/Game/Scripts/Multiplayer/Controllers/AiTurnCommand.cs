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

        public override void Execute()
        {
            Retain();
            startTime = Time.time;

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
            else
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_5M;
            }



            // TODO: In the future, if we add 1 minute games, use the IsOneMinuteGame flag in the vo
            // to make the bots more aggressive so people can't spam the time control.

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
            else if (vo.aiMoveDelay == AiMoveDelay.ONLINE_5M)
            {
                int index = Mathf.Min(vo.aiMoveNumber, AiMoveTimes.M_5.Length - 1);
                float[] delayRange = AiMoveTimes.M_5[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
            }

            float timeElapsed = Time.time - startTime;
            delay -= timeElapsed;
            delay = Mathf.Max(0, delay);
            //yield return new WaitForSecondsRealtime(delay);

            if (chessboard.aiWillResign &&
                chessService.GetScore(chessboard.playerColor) > BotSettings.AI_RESIGN_SCORE_THRESHOLD)
            {
                backendService.AiResign().Then(OnResign);
            }
            else
            {
                backendService.AiTurn(from, to, promo, (long)delay).Then(OnTurnTaken); 
            }
        }

        private void OnTurnTaken(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

        private void OnResign(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

            
    }


}
