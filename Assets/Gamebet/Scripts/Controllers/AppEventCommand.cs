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
        [Inject] public GameAppEventSignal gameAppEventSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public override void Execute()
        {
            gameAppEventSignal.Dispatch(appEvent);

            if (appEvent == AppEvent.ESCAPED)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            }
        }
    }
}
