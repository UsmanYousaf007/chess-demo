/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public class ResumeMatchCommand : Command
    {
        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.ResumeMatchData().Then(OnResumeMatchData);
        }

        public void OnResumeMatchData(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                startGameSignal.Dispatch();
            }
            else 
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
