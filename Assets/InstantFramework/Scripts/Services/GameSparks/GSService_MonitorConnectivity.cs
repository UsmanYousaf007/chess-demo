/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-22 12:55:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using UnityEngine;

using GameSparks.Api.Responses;

using TurboLabz.TLUtils;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public GameDisconnectingSignal gameDisconnectingSignal { get; set; }
        [Inject] public AppEventSignal appEventSignal { get; set;  }


        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }

        [Inject] public ResumeMatchSignal resumeMatchSignal { get; set; }

        //[Inject] public INavigatorModel navigatorModel { get; set; }

        NavigatorViewId prevViewId;

        public void MonitorConnectivity(bool enable)
        {
            GS.GameSparksAvailable -= GameSparksAvailable;

            if (enable)
            {
                GS.GameSparksAvailable += GameSparksAvailable;
            }
        }

        void GameSparksAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                //receptionSignal.Dispatch();
                LogUtil.Log("GS CONNECTED!", "red");

                resumeMatchSignal.Dispatch(prevViewId);
            }
            else
            {
                LogUtil.Log("GS DISCONNECTED!", "red");
                prevViewId = navigatorModel.currentViewId;
                GSFrameworkRequest.CancelRequestSession();

                // We are going to reset the game now so make sure that the models
                // save to disk as we would when going to the background
                gameDisconnectingSignal.Dispatch();
                modelsSaveToDiskSignal.Dispatch();
                modelsResetSignal.Dispatch();
                modelsLoadFromDiskSignal.Dispatch();

                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RECONNECTING);
            }
        }

    }
}
