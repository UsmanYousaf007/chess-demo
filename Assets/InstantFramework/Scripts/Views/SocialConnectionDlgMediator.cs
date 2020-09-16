/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class SocialConnectionDlgMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // View injection
        [Inject] public SocialConnectionDlgView view { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SOCIAL_CONNECTION_DLG)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SOCIAL_CONNECTION_DLG)
            {
                view.Hide();
            }
        }

        private void OnCloseButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
