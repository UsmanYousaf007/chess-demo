/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-11-20 08:35:32 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class BootCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ConnectBackendSignal connectBackendSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IAppEventModel appEventModel { get; set; }

        public override void Execute()
        {
            Retain();

            appEventModel.reconnectOnPause = true;

            // Initialize services.
            facebookService.Init().Then(OnFacebookServiceInit);

            connectBackendSignal.Dispatch();
        }

        private void OnFacebookServiceInit(FacebookResult result)
        {
            if (result != FacebookResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(BackendResult.FACEBOOK_INIT_FAILED);
            }

            Release();
        }
    }
}
