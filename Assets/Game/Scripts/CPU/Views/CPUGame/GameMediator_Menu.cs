/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]


using TurboLabz.Chess;
using TurboLabz.Common;
using TurboLabz.Gamebet;

namespace TurboLabz.CPUChess 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public ResignSignal resignSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        public void OnRegisterMenu()
        {
            view.InitMenu();

            view.menuButtonClickedSignal.AddListener(OnMenuButtonClicked);
            view.resignButtonClickedSignal.AddListener(OnResignClicked);
            view.exitButtonClickedSignal.AddListener(OnExitButtonClicked);
            view.continueButtonClickedSignal.AddListener(OnContinueButtonClicked);
        }

        public void OnRemoveMenu()
        {
            view.CleanupMenu();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowMenuView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_PLAY_MENU) 
            {
                view.ShowMenu();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideMenuView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_PLAY_MENU)
            {
                view.HideMenu();
            }
        }

        [ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleMenuButton(bool isPlayerTurn)
        {
            view.ToggleMenuButton(isPlayerTurn);
        }

        [ListensTo(typeof(DisableMenuButtonSignal))]
        public void OnDisableMenuButton()
        {
            view.DisableMenuButton();
        }

        private void OnMenuButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_PLAY_EXIT_DLG);
        }

        private void OnResignClicked()
        {
            resignSignal.Dispatch();
        }

        private void OnExitButtonClicked()
        {
            loadLobbySignal.Dispatch();
            // TODO: remove this hack, its used only for the lobby
            // which is using the old view manager
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
        }

        private void OnContinueButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_PLAY);
        }
    }
}
