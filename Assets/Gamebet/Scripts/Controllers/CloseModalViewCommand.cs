/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 15:38:35 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class CloseModalViewCommand : Command
    {
        // Dispatch signals
        [Inject] public HideModalViewSignal hideModalViewSignal { get; set; } 

        // Models
        [Inject] public IModalViewStateModel modalViewStateModel { get; set; }

        public override void Execute()
        {
            ModalViewId currentView = modalViewStateModel.currentView;
            hideModalViewSignal.Dispatch(currentView);
            modalViewStateModel.currentView = ModalViewId.NONE;
        }
    }
}
