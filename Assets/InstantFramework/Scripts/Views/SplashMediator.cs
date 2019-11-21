/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class SplashMediator : Mediator
    {
        // Dispatch signals
        //[Inject] public LoadCPUGameSignal loadGameSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // View injection
        [Inject] public SplashView view { get; set; }
        
        public override void OnRegister()
        {
            view.Init();
        }

        public override void OnRemove()
        {
        }
        
        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPLASH) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPLASH)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(SplashWifiIsHealthySignal))]
        public void OnWifiHealthUpdate()
        {
            view.WifiHealthUpdate();
        }

        [ListensTo(typeof(ShowSplashContentSignal))]
        public void OnShowContent(bool show)
        {
            view.ShowContent(show);
        }
    }
}

