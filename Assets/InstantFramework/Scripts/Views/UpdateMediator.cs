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

using System.Collections;
using strange.extensions.mediation.impl;
using UnityEngine;
using UpdateManager;

namespace TurboLabz.InstantFramework
{
    public class UpdateMediator : Mediator
    {
        // View injection
        [Inject] public UpdateView view { get; set; }
        [Inject] public IAppUpdateService appUpdateService { get; set; }

        
        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.UPDATE) 
            {
                appUpdateService.CheckForUpdate();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.UPDATE)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(AppUpdateSignal))]
        public void UpdateAvailable(bool isUpdateAvailable)
        {
            view.SetAppUpdateFlag(isUpdateAvailable);
        }

        [ListensTo(typeof(SetUpdateURLSignal))]
        public void OnSetUpdateURL(string url)
        {
            view.SetUpdateURL(url);
        }
    }
}
