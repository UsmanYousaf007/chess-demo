/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-13 12:48:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework 
{
    public class GetGameStartTimeCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public GetGameStartTimeFailedSignal getGameStarTimeFailedSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.GetGameStartTime(matchInfoModel.activeChallengeId).Then(OnGetGameStartTime);
        }

        private void OnGetGameStartTime(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                long targetTime = matchInfoModel.activeMatch.gameStartTimeMilliseconds;
                long currentTime = backendService.serverClock.currentTimestamp;

                // If current time is past target time then don't wait at all
                float waitDuration = Mathf.Max((targetTime - currentTime) / 1000f, 0f);

                routineRunner.StartCoroutine(OnGetGameStartTimeCR(waitDuration));
            }
            else if (result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
                getGameStarTimeFailedSignal.Dispatch();
            }

            Release();
        }

        private IEnumerator OnGetGameStartTimeCR(float waitDuration)
        {
            yield return new WaitForSecondsRealtime(waitDuration);
            chessAiService.AiMoveRequestInit();
            startGameSignal.Dispatch();
        }
    }
}
