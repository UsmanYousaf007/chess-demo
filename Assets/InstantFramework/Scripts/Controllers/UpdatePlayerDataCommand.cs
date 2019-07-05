/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class UpdatePlayerDataCommand : Command
    {
     
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
     

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }


        public override void Execute()
        {
            Retain();
            backendService.UpdatePlayerData(playerModel.notificationCount).Then(OnResponse);

        }

        private void OnResponse(BackendResult result)
        {
            if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
            {
                //backendErrorSignal.Dispatch(result);
            }

            if (result == BackendResult.SUCCESS)
            {
               // Debug.Log("ALL SET TO GOOOOOOO");
            }

            Release();
        }

    }
}
