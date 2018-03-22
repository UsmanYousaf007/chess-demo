/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 13:32:44 UTC+05:00

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class ShopCurrency1ModalMediator : Mediator
    {
        // View injection
        [Inject] public ShopCurrency1ModalView view { get; set; }
        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }
        [Inject] public PurchaseShopItemSignal purchaseShopItemSignal { get; set; }
        [Inject] public LoadShopCurrencySignal loadShopCurrencySignal { get; set; }

        public Signal purchaseAvatarsResultSignal = new Signal();

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.confirmPurchaseButtonClickedSignal.AddListener(OnConfirmPurchaseButtonClicked);

            purchaseAvatarsResultSignal.AddListener(OnPurchaseResult);

            view.Init();
        }

        public override void OnRemove()
        {
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
            view.confirmPurchaseButtonClickedSignal.RemoveListener(OnConfirmPurchaseButtonClicked);

            purchaseAvatarsResultSignal.AddListener(OnPurchaseResult);
        }

        [ListensTo(typeof(UpdateShopCurrency1ModalViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP_CURRENCY_1_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP_CURRENCY_1_DLG)
            {
                view.Hide();
            }
        }

        public void OnPurchaseResult()
        {
            view.OnPurchaseResult();
        }

        private void OnCloseButtonClicked()
        {
            loadShopCurrencySignal.Dispatch();
        }

        private void OnConfirmPurchaseButtonClicked(string activeShopItemId)
        {
            purchaseShopItemSignal.Dispatch(activeShopItemId, purchaseAvatarsResultSignal);
        }

    }
}