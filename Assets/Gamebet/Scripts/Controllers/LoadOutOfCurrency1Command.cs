/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author #AUTHOR# <#AUTHOR_EMAIL#>
/// @company #COMPANY# <#COMPANY_URL#>
/// @date #DATE#

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadOutOfCurrency1Command : Command
    {
        // Dispatch signals.
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateOutOfCurrency1ModalViewSignal updateOutOfCurrency1ModalViewSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_OUT_OF_CURRENCY_1_DLG);

            // Send true for isAdAvailable here regardless of whether it is or
            // not. This is because we want to show the play ad button first to
            // the user and only upon hitting the play ad button screen we play
            // an ad or let the user know if ads are not available.
            updateOutOfCurrency1ModalViewSignal.Dispatch(true);
        }
    }
}
