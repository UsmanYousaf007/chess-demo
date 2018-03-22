/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-12 15:40:39 UTC+05:00

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadFreeCurrency1ModalCommand : Command
    {
        // Dispatch signals.
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateFreeCurrency1ModalViewSignal updateFreeCurrency1ModalViewSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FREE_CURRENCY_1_DLG);

            // Send true for isAdAvailable here regardless of whether it is or
            // not. This is because we want to show the play ad button first to
            // the user and only upon hitting the play ad button screen we play
            // an ad or let the user know if ads are not available.
            updateFreeCurrency1ModalViewSignal.Dispatch(true);
        }
    }
}
