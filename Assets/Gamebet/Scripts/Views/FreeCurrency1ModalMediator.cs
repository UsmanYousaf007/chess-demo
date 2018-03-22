/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 12:52:48 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class FreeCurrency1ModalMediator : Mediator
    {
        // View injection
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public FreeCurrency1ModalView view { get; set; }

        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal  { get; set; }
        [Inject] public PlayAdSignal playAdSignal { get; set; }

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.playAdButtonClickedSignal.AddListener(OnPlayAdButtonClicked);
            view.okButtonClickedSignal.AddListener(OnOkButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.okButtonClickedSignal.RemoveListener(OnOkButtonClicked);
            view.playAdButtonClickedSignal.RemoveListener(OnPlayAdButtonClicked);
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
        }

        [ListensTo(typeof(UpdateFreeCurrency1ModalViewSignal))]
        public void OnUpdateView(bool isAdAvailable)
        {
            view.UpdateView(isAdAvailable);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FREE_CURRENCY_1_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.FREE_CURRENCY_1_DLG)
            {
                view.Hide();
            }
        }

        private void OnCloseButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.CLOSE_DLG);
        }

        private void OnPlayAdButtonClicked()
        {
            playAdSignal.Dispatch();
        }

        private void OnOkButtonClicked()
        {

        }
    }
}
