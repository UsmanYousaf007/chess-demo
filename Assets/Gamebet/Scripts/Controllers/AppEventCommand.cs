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

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class AppEventCommand : Command
    {
        // Signal parameters
        [Inject] public AppEvent appEvent { get; set; }

        // Dispatch signals
        [Inject] public ConnectBackendSignal connectBackendSignal { get; set; }
        [Inject] public DisconnectBackendSignal disconnectBackendSignal { get; set; }
        [Inject] public GameAppEventSignal gameAppEventSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IAppEventModel appEventModel { get; set; }

        public override void Execute()
        {
            if (appEvent == AppEvent.PAUSED)
            {
                OnAppPaused();
            }
            else if (appEvent == AppEvent.RESUMED)
            {
                OnAppResumed();
            }
            
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            gameAppEventSignal.Dispatch(appEvent);
        }

        private void OnAppPaused()
        {
            // Handle gamesparks connectivity
            if (appEventModel.reconnectOnPause)
            {
                disconnectBackendSignal.Dispatch();
            }
        }

        private void OnAppResumed()
        {
            // Handle gamesparks connectivity
            if (appEventModel.reconnectOnPause)
            {
                connectBackendSignal.Dispatch();
            }
        }
    }
}
