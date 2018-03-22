/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-16 19:21:39 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet 
{
    public class AuthMediator : Mediator
    {
        // Dispatch signals
        [Inject] public AuthGuestSignal authGuestSignal { get; set; }
        [Inject] public AuthFacebookSignal authFacebookSignal { get; set; }

        // View injection
        [Inject] public AuthView view { get; set; }

        public override void OnRegister()
        {
            view.authGuestButtonClickedSignal.AddListener(OnAuthGuestButtonClicked);
            view.authFacebookButtonClickedSignal.AddListener(OnAuthFacebookButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.authGuestButtonClickedSignal.RemoveListener(OnAuthGuestButtonClicked);
            view.authFacebookButtonClickedSignal.RemoveListener(OnAuthFacebookButtonClicked);
        }

        private void OnAuthGuestButtonClicked()
        {
            // Todo: view.showSpinner()
            authGuestSignal.Dispatch();
        }

        private void OnAuthFacebookButtonClicked()
        {
            // Todo: view.showSpinner()
            authFacebookSignal.Dispatch();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.AUTHENTICATION) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.AUTHENTICATION)
            {
                view.Hide();
            }
        }
    }
}
