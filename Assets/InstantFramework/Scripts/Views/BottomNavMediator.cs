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
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public LoadArenaSignal loadArenaSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateBottomNavSignal updateBottomNavSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.homeButtonClickedSignal.AddListener(OnHomeButtonClicked);
            view.friendsButtonClickedSignal.AddListener(OnFriendsButtonClicked);
            view.inventoryButtonClickedSignal.AddListener(OnInventoryButtonClicked);
            view.shopButtonClickedSignal.AddListener(OnShopButtonClicked);
            view.arenaButtonClickedSignal.AddListener(OnArenaButtonClicked);
        }

        public override void OnRemove()
        {
            view.homeButtonClickedSignal.RemoveAllListeners();
            view.friendsButtonClickedSignal.RemoveAllListeners();
            view.inventoryButtonClickedSignal.RemoveAllListeners();
            view.shopButtonClickedSignal.RemoveAllListeners();
        }

        void OnHomeButtonClicked()
        {
            loadLobbySignal.Dispatch();
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Home);
        }

        void OnFriendsButtonClicked()
        {
            loadFriendsSignal.Dispatch();
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Friends);
        }

        void OnArenaButtonClicked()
        {
            loadArenaSignal.Dispatch();
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Arena);
        }

        void OnInventoryButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INVENTORY);
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Inventory);
        }

        void OnShopButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP);
            updateBottomNavSignal.Dispatch(BottomNavView.ButtonId.Shop);
        }

        [ListensTo(typeof(UpdateBottomNavSignal))]
        public void OnUpdateView(BottomNavView.ButtonId buttonID)
        {
            view.UpdateAlerts();
            view.UpdateButtons(buttonID);
        }

        [ListensTo(typeof(ShowBottomNavSignal))]
        public void OnShowView(bool value)
        {
            view.Show(value);
        }
    }
}
