/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-09 16:22:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{	
	public class LoadViewCommand : Command
    {
        // Command params
        [Inject] public ViewId viewId { get; set; }

        // Dispatch signals
        [Inject] public ShowViewSignal showViewSignal { get; set; }
        [Inject] public HideViewSignal hideViewSignal { get; set; } 

        // Models
		[Inject] public IViewStateModel viewStateModel { get; set; }

		public override void Execute()
        {
            ViewId currentView = viewStateModel.currentView;

            viewStateModel.previousView = currentView;
            hideViewSignal.Dispatch(currentView);
            showViewSignal.Dispatch(viewId);
            viewStateModel.currentView = viewId;
		}
	}
}
