/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-08 12:31:05 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class SetPlayerSocialNameMediator : Mediator
    {
        // Dispatch signals
        [Inject] public SetPlayerSocialNameSignal setPlayerSocialNameSignal { get; set; }

        // View injection
        [Inject] public SetPlayerSocialNameView view { get; set; }

        public override void OnRegister()
        {
            view.nameOptionButtonClickedSignal.AddListener(OnNameOptionButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.nameOptionButtonClickedSignal.RemoveListener(OnNameOptionButtonClicked);
        }

        [ListensTo(typeof(UpdateSetPlayerSocialNameViewSignal))]
        public void OnUpdateLobbyView(SetPlayerSocialNameVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(ShowViewSignal))]
        public void OnShowView(ViewId viewId)
        {
            if (viewId == ViewId.SET_PLAYER_SOCIAL_NAME) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideViewSignal))]
        public void OnHideView(ViewId viewId)
        {
            if (viewId == ViewId.SET_PLAYER_SOCIAL_NAME)
            {
                view.Hide();
            }
        }

        private void OnNameOptionButtonClicked(string name)
        {
            setPlayerSocialNameSignal.Dispatch(name);
        }
    }
}
