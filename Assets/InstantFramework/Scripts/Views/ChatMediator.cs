/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class ChatMediator : Mediator
    {
        // View injection
        [Inject] public ChatView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
       
        public override void OnRegister()
        {
            view.Init();

            view.backToGameButtonClickedSignal.AddListener(OnBackToGameButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAT) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAT)
            {
                view.Hide();
            }
        }

        private void OnBackToGameButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }
    }
}
