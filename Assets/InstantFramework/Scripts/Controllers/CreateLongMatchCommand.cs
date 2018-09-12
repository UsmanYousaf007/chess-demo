﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;


namespace TurboLabz.InstantFramework
{
    public class CreateLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public FriendBarBusySignal friendBarBusySignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            Retain();

            friendBarBusySignal.Dispatch(opponentId, true);
            matchInfoModel.createLongMatchAborted = false;
            backendService.CreateLongMatch(opponentId).Then(OnCreateLongMatch);
        }

        private void OnCreateLongMatch(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                if (matchInfoModel.createLongMatchAborted)
                {
                    friendBarBusySignal.Dispatch(opponentId, false);    
                }
            }
            else if (result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
