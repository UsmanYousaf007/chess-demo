/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-27 14:27:50 UTC+05:00

using strange.extensions.mediation.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class InventoryChessSkinsInfoModalMediator : Mediator
    {
        [Inject] public ApplyPlayerInventorySignal applyPlayerInventorySignal { get; set; }
        [Inject] public LoadInventoryChessSkinsSignal loadInventoryChessSkinsSignal { get; set; }
        
        // View injection
        [Inject] public InventoryChessSkinsInfoModalView view { get; set; }
        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.equipButtonClickedSignal.AddListener(OnEquipButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
            view.equipButtonClickedSignal.RemoveListener(OnEquipButtonClicked);
        }

        [ListensTo(typeof(UpdateInventoryChessSkinsInfoModalViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(ShowModalViewSignal))]
        public void OnShowView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.InventoryChessSkins)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideModalViewSignal))]
        public void OnHideView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.InventoryChessSkins)
            {
                view.Hide();
            }
        }

        private void OnCloseButtonClicked()
        {
            closeModalViewSignal.Dispatch();
        }

        private void OnEquipButtonClicked(string activeShopItemId)
        {
            loadInventoryChessSkinsSignal.Dispatch();
            LogUtil.Log("The equip button has been clicked" ,"red");
        }
    }
}
