﻿/// @license Propriety <http://license.url>
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
    public class ReconnectingMediator : Mediator
    {
        // View injection
        [Inject] public ReconnectingView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RECONNECTING) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RECONNECTING) 
            {
                view.Hide();
            }
        }
    }
}
