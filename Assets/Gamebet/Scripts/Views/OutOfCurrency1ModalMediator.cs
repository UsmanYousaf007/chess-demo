/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-09 14:25:35 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class OutOfCurrency1ModalMediator : Mediator
    {
        // View injection
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public OutOfCurrency1ModalView view { get; set; }

        [Inject] public LoadViewSignal loadViewSignal { get; set; }
        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }
        [Inject] public PlayAdSignal playAdSignal { get; set; }

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.buyCurrency1ButtonClickedSignal.AddListener(OnBuyCurrency1ButtonClicked);
            view.playAdButtonClickedSignal.AddListener(OnPlayAdButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.playAdButtonClickedSignal.RemoveListener(OnPlayAdButtonClicked);
            view.buyCurrency1ButtonClickedSignal.RemoveListener(OnBuyCurrency1ButtonClicked);
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
        }

        [ListensTo(typeof(UpdateOutOfCurrency1ModalViewSignal))]
        public void OnUpdateView(bool isAdAvailable)
        {
            view.UpdateView(isAdAvailable);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.OUT_OF_CURRENCY_1_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.OUT_OF_CURRENCY_1_DLG)
            {
                view.Hide();
            }
        }

        private void OnCloseButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.CLOSE_DLG);
        }

        private void OnBuyCurrency1ButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP);
        }

        private void OnPlayAdButtonClicked()
        {
            playAdSignal.Dispatch();
        }
    }
}
