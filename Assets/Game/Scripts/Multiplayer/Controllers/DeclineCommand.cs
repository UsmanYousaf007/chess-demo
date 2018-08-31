/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class DeclineCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.Decline().Then(OnDecline);
        }

        private void OnDecline(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }
            else
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FRIENDS);
            }

            Release();
        }
    }
}
