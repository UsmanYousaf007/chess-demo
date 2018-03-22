/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-21 14:04:13 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class DisconnectBackendCommand : Command
    {
        // Services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            // We need to check here if the backend is already disconnected or
            // not because this command can be called at a time when the backend
            // is already disconnected e.g. an error occured in the app which
            // disconnects the backend and then we suspend the app at which
            // point this command will be called.
            if (backendService.connectionState != ConnectionState.DISCONNECTED)
            {
                backendService.Disconnect();
            }
        }
    }
}
