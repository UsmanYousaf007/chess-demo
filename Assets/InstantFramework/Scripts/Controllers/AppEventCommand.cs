/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-01 11:39:01 UTC+05:00
/// 
/// @description
/// [add_description_here]

using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class AppEventCommand : Command
    {
        // Signal parameters
        [Inject] public AppEvent appEvent { get; set; }

        // Dispatch signals
        [Inject] public GameAppEventSignal gameAppEventSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SoftReconnectingSignal softReconnectingSignal { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        //bool softReconnecting = false;

        public override void Execute()
        {
            gameAppEventSignal.Dispatch(appEvent);


            if (appEvent == AppEvent.ESCAPED)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            }
            /*
            else if (appEvent == AppEvent.PAUSED &&
                     navigatorModel.currentViewId != NavigatorViewId.RECONNECTING &&
                     navigatorModel.currentViewId != NavigatorViewId.SPLASH &&
                     navigatorModel.currentViewId != NavigatorViewId.HARD_STOP &&
                     navigatorModel.currentViewId != NavigatorViewId.UPDATE &&
                     navigatorModel.currentViewId != NavigatorViewId.MULTIPLAYER_FIND_DLG &&
                     !(navigatorModel.currentViewId == NavigatorViewId.MULTIPLAYER && !matchInfoModel.activeMatch.isLongPlay)
                    )
            {
                softReconnecting = true;
                backendService.MonitorConnectivity(false);
                GS.Disconnect();
                GS.GameSparksAvailable -= GameSparksAvailable;
                GS.GameSparksAvailable += GameSparksAvailable;
                softReconnectingSignal.Dispatch(true);
                Retain();

            }
            else if (appEvent == AppEvent.RESUMED)
            {
                if (softReconnecting)
                {
                    GS.Reconnect();
                }
            }
            */
        }

        /*
        void GameSparksAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                backendService.MonitorConnectivity(true);
                softReconnectingSignal.Dispatch(false);
                GS.GameSparksAvailable -= GameSparksAvailable;
                softReconnecting = false;
                Release();
            }

            // Do not release here, this is deliberate.
        }
        */
    }
}
