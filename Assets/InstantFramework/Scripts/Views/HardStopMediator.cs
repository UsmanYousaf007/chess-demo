/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class HardStopMediator : Mediator
    {
        // View injection
        [Inject] public HardStopView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.HARD_STOP) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(SetErrorAndHaltSignal))]
        public void OnSetErrorAndHalt(BackendResult error, string message)
        {
            //view.SetErrorAndHalt(error, message);
        }


    }
}
