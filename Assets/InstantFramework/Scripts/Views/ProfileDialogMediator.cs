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
using TurboLabz.TLUtils;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class ProfileDialogMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // View injection
        [Inject] public ProfileDialogView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.closeBtn.onClick.AddListener(OnClose);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PROFILE_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PROFILE_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateProfileDialogSignal))]
        public void OnUpdateProfileDialog(ProfileDialogVO vo)
        {
            view.UpdateProfileDialog(vo);
        }

        private void OnClose()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
