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
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public ResignSignal resignSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSignal { get; set; }

        public void OnRegisterMenu()
        {
            view.InitMenu();

            view.menuButtonClickedSignal.AddListener(OnMenuButtonClicked);
            view.resignButtonClickedSignal.AddListener(OnResignClicked);
            view.continueButtonClickedSignal.AddListener(OnContinueButtonClicked);
            view.saveAndExitButtonClickedSignal.AddListener(OnSaveAndExitButtonClicked);
            view.returnToLobbySignal.AddListener(OnReturnToLobby);
            view.showResultsDlgSignal.AddListener(OnShowResultsDlg);
        }

        public void OnRemoveMenu()
        {
            view.CleanupMenu();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowMenuView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_EXIT_DLG) 
            {
                pauseTimersSignal.Dispatch();
                view.ShowMenu();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideMenuView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_EXIT_DLG)
            {
                resumeTimersSignal.Dispatch();
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

        [ListensTo(typeof(GameDisconnectingSignal))]
        public void OnGameDisconnecting()
        {
            if (view.IsVisible())
            {
                saveGameSignal.Dispatch();
            }
        }

        private void OnMenuButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_EXIT_DLG);
        }

        private void OnResignClicked()
        {
            cancelHintSignal.Dispatch();
            resignSignal.Dispatch();
        }

        private void OnContinueButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
        }

        private void OnSaveAndExitButtonClicked()
        {
            saveGameSignal.Dispatch();
            //loadLobbySignal.Dispatch();
            //cancelHintSignal.Dispatch();
        }

        private void OnReturnToLobby()
        {
            loadLobbySignal.Dispatch();
            cancelHintSignal.Dispatch();
        }

        private void OnShowResultsDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_RESULTS_DLG);
        }
    }
}
