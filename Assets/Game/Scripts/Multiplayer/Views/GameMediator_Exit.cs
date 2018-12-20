/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public ResignSignal resignSignal { get; set; }

        public void OnRegisterMenu()
        {
            view.InitMenu();

            view.menuButtonClickedSignal.AddListener(OnMenuButtonClicked);
            view.resignButtonClickedSignal.AddListener(OnResignClicked);
            view.continueButtonClickedSignal.AddListener(OnContinueButtonClicked);
        }

        public void OnRemoveMenu()
        {
            view.CleanupMenu();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowMenuView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_EXIT_DLG) 
            {
                view.ShowMenu();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideMenuView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_EXIT_DLG)
            {
                view.HideMenu();
            }
        }

        private void OnMenuButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_EXIT_DLG);
        }

        private void OnResignClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
            resignSignal.Dispatch(null);
        }

        private void OnContinueButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }
    }
}
