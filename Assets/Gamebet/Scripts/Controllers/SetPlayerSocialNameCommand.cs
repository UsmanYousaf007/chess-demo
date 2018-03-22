/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-07 22:19:02 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class SetPlayerSocialNameCommand : Command
    {
        // Signal parameters
        [Inject] public string name { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            backendService.SetPlayerSocialName(name).Then(OnSetPlayerSocialName);
        }

        private void OnSetPlayerSocialName(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                loadLobbySignal.Dispatch();
            }
            else
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
