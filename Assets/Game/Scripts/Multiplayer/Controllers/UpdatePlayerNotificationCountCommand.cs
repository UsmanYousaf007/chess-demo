/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer
{
    public class UpdatePlayerNotificationCountCommand : Command
    {
        // Parameters
        [Inject] public int counter { get; set; }

        [Inject] public UpdatePlayerDataSignal updatePlayerDataSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            int previousValue = playerModel.notificationCount;

            playerModel.notificationCount = counter;

            if (previousValue != counter)
            {
                updatePlayerDataSignal.Dispatch();
            }
        }
    }
}
