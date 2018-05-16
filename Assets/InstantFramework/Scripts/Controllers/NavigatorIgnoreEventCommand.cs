/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-15 20:38:14 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using System.Text;

namespace TurboLabz.InstantFramework
{
    public class NavigatorIgnoreEventCommand : Command
    {
        // Signal parameters
        [Inject] public NavigatorEvent navigatorEvent { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }

        public override void Execute()
        {
            navigatorModel.ignoreEvent = navigatorEvent;
        }
    }
}
