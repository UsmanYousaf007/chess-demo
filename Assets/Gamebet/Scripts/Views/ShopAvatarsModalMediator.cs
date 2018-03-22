/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 11:58:18 UTC+05:00

using strange.extensions.mediation.impl;
using TurboLabz.Common;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class ShopAvatarsModalMediator : Mediator
    {
        // View injection
        [Inject] public ShopAvatarsModalView view { get; set; }
        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }
        [Inject] public PurchaseShopItemSignal purchaseShopItemSignal { get; set; }
        [Inject] public LoadShopAvatarsSignal loadShopAvatarsSignal { get; set; }
        [Inject] public LoadInventorySignal loadInventorySignal { get; set; }

        public Signal purchaseAvatarsResultSignal = new Signal();

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.confirmPurchaseButtonClickedSignal.AddListener(OnConfirmPurchaseButtonClicked);
            view.addToCollectionButtonClickedSignal.AddListener(OnAddToCollectionButtonClicked);
            view.viewCollectionButtonClickedSignal.AddListener(OnViewCollectionButtonClicked);

            purchaseAvatarsResultSignal.AddListener(OnPurchaseResult);

            view.Init();
        }

        public override void OnRemove()
        {
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
            view.confirmPurchaseButtonClickedSignal.RemoveListener(OnConfirmPurchaseButtonClicked);
            view.addToCollectionButtonClickedSignal.RemoveListener(OnAddToCollectionButtonClicked);

            purchaseAvatarsResultSignal.AddListener(OnPurchaseResult);
        }

        [ListensTo(typeof(UpdateShopAvatarsModalViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP_AVATARS_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP_AVATARS_DLG)
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
            loadShopAvatarsSignal.Dispatch();
        }

        private void OnConfirmPurchaseButtonClicked(string activeShopItemId)
        {
            purchaseShopItemSignal.Dispatch(activeShopItemId, purchaseAvatarsResultSignal);
        }

        public void OnAddToCollectionButtonClicked()
        {
            loadShopAvatarsSignal.Dispatch();
        }

        public void OnViewCollectionButtonClicked()
        {
            loadInventorySignal.Dispatch();
        }
    }
}
