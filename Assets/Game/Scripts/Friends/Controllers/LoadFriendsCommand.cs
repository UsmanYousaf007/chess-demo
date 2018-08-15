/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class LoadFriendsCommand : Command
    {
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

		// Models
		[Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FRIENDS);



        }
    }
}
