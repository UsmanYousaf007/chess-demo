/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-11-23 14:38:37 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LoadPreviousViewCommand : Command
    {
        // Dispatch signals
        [Inject] public ShowViewSignal showViewSignal { get; set; }
        [Inject] public HideViewSignal hideViewSignal { get; set; } 

        // Models
        [Inject] public IViewStateModel viewStateModel { get; set; }

        public override void Execute()
        {
            ViewId previousView = viewStateModel.previousView;

            Assertions.Assert(previousView != ViewId.NONE, "Previous view must not be ViewId.NONE!");

            hideViewSignal.Dispatch(viewStateModel.currentView);
            showViewSignal.Dispatch(previousView);
            viewStateModel.currentView = previousView;
        }
    }
}
