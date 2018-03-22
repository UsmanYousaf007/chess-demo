/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 12:42:32 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class LoadModalViewCommand : Command
    {
        // Command parameters
        [Inject] public ModalViewId modalViewId { get; set; }

        // Dispatch signals
        [Inject] public ShowModalViewSignal showModalViewSignal { get; set; }
        [Inject] public HideModalViewSignal hideModalViewSignal { get; set; } 

        // Models
        [Inject] public IModalViewStateModel modalViewStateModel { get; set; }

        public override void Execute()
        {
            ModalViewId currentView = modalViewStateModel.currentView;

            modalViewStateModel.previousView = currentView;
            hideModalViewSignal.Dispatch(currentView);
            showModalViewSignal.Dispatch(modalViewId);
            modalViewStateModel.currentView = modalViewId;
        }
    }
}
