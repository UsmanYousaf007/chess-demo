/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-11 11:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class ResignCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.Resign().Then(OnResign);
        }

        private void OnResign(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
