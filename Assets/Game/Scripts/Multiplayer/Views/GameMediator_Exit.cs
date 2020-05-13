/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public ResignSignal resignSignal { get; set; }
        [Inject] public OfferDrawSignal offerDrawSignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSingal { get; set; }

        public void OnRegisterMenu()
        {
            view.InitMenu();

            view.menuButtonClickedSignal.AddListener(OnMenuButtonClicked);
            view.resignButtonClickedSignal.AddListener(OnResignClicked);
            view.continueButtonClickedSignal.AddListener(OnContinueButtonClicked);
            view.offerDrawButtonClickedSignal.AddListener(OnOfferDrawClicked);
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
            resignSignal.Dispatch("");
            cancelHintSingal.Dispatch();
        }

        private void OnOfferDrawClicked()
        {
            offerDrawSignal.Dispatch("offered");
            view.OnContinueButtonClicked();
            //cancelHintSingal.Dispatch();
        }

        private void OnContinueButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }

        [ListensTo(typeof(UpdateOfferDrawSignal))]
        public void OfferDrawStatusUpdate(OfferDrawVO offerDrawVO)
        {
            view.OfferDraw(offerDrawVO.status, offerDrawVO.offeredBy);
        }
    }
}
