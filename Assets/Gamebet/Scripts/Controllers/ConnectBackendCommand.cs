/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 16:24:31 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class ConnectBackendCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOADING);
            backendService.Connect().Then(OnConnect);
        }

        private void OnConnect(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                if (!backendService.isAuthenticated)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_AUTHENTICATION);
                }
            }
            else
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
