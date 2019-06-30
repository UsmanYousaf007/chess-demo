/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using TurboLabz.Multiplayer;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class ResumeMatchCommand : Command
    {
        // Params
        [Inject] public NavigatorViewId prevViewId { get; set; }
        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ReceptionSignal receptionSignal { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }


        public override void Execute()
        {
            Retain();


            getInitDataCompleteSignal.AddListener(OnGetInitDataComplete);
            receptionSignal.Dispatch(true);
        }

        private void OnGetInitDataComplete()
        {
            if (prevViewId == NavigatorViewId.CPU)
            {
                startCPUGameSignal.Dispatch();
            }
            else if (matchInfoModel.activeChallengeId != null)
            {
                startGameSignal.Dispatch();
            }
            else
            {
                loadLobbySignal.Dispatch();
            }

            getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
            Release();
        }
    }
}
