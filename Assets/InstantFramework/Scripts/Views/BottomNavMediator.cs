/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class BottomNavMediator : Mediator
    {
        // View injection
        [Inject] public BottomNavView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public LoadStoreSignal loadStoreSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }


        public override void OnRegister()
        {
            view.Init();

            view.homeButtonClickedSignal.AddListener(OnHomeButtonClicked);
            view.shopButtonClickedSignal.AddListener(OnShopButtonClicked);
            view.friendsButtonClickedSignal.AddListener(OnFriendsButtonClicked);
        }

        public override void OnRemove()
        {
            view.homeButtonClickedSignal.RemoveAllListeners();
            view.shopButtonClickedSignal.RemoveAllListeners();
            view.friendsButtonClickedSignal.RemoveAllListeners();
        }

        void OnHomeButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }

        void OnShopButtonClicked()
        {
            loadStoreSignal.Dispatch();
        }

        void OnFriendsButtonClicked()
        {
            loadFriendsSignal.Dispatch();
        }
    }
}
