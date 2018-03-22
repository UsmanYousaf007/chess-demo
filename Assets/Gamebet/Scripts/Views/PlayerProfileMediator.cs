/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-23 18:17:30 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class PlayerProfileMediator : Mediator
    {
        // Dispatch signals
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // View injection
        [Inject] public PlayerProfileView view { get; set; }
        
        public override void OnRegister()
        {
            view.backButtonClickedSignal.AddListener(OnBackButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.backButtonClickedSignal.RemoveListener(OnBackButtonClicked);
        }

        [ListensTo(typeof(UpdatePlayerProfileViewSignal))]
        public void OnUpdateView(PlayerProfileVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdatePlayerProfilePictureSignal))]
        public void OnUpdateProfilePicture(Sprite sprite)
        {
            view.UpdateProfilePicture(sprite);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PLAYER_PROFILE) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PLAYER_PROFILE)
            {
                view.Hide();
            }
        }

        private void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }
    }
}
