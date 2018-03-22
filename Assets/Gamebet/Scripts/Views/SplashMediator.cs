/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-11-20 04:14:13 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class SplashMediator : Mediator
    {
        [Inject] public BootSignal bootSignal { get; set; }

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
            bootSignal.Dispatch();
        }
    }
}
