/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantChess;

namespace TurboLabz.InstantFramework
{
    public class SplashMediator : Mediator
    {
        // Dispatch signals
        //[Inject] public LoadCPUGameSignal loadGameSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SplashAnimCompleteSignal splashAnimCompleteSignal { get; set; }

        // View injection
        [Inject] public SplashView view { get; set; }
        
        public override void OnRegister()
        {
            view.splashAnimationCompletedSignal.AddListener(OnSplashAnimationCompleted);
            view.Init();
        }

        public override void OnRemove()
        {
            view.splashAnimationCompletedSignal.RemoveListener(OnSplashAnimationCompleted);
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

        private void OnSplashAnimationCompleted()
        {
            splashAnimCompleteSignal.Dispatch();
        }
    }
}

