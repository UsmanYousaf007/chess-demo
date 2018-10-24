/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class RateAppDialogMediator : Mediator
    {
        // Dispatch signals

        // View injection
        [Inject] public RateAppDialogView view { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IRateAppService rateAppService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.notNowButton.onClick.AddListener(OnNotNow);
            view.rateButton.onClick.AddListener(OnRate);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RATE_APP_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RATE_APP_DLG)
            {
                view.Hide();
            }
        }

        private void OnNotNow()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnRate()
        {
            rateAppService.RateApp();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
