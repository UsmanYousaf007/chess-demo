/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-05 12:50:47 UTC+05:00

using strange.extensions.mediation.impl;
using TurboLabz.Common;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class InventoryLootDismantleModalMediator : Mediator
    {
        // View injection
        [Inject] public InventoryLootDismantleModalView view { get; set; }
        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }
        [Inject] public SellForgeCardsSignal sellForgeCardsSignal { get; set; }

        public Signal dismantleResultSignal = new Signal();

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.dismantleButtonClickedSignal.AddListener(OnDismantleButtonClicked);

            dismantleResultSignal.AddListener(OnDismantleSuccess);
            view.Init();
        }

        public override void OnRemove()
        {
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
            view.dismantleButtonClickedSignal.RemoveListener(OnDismantleButtonClicked);

            dismantleResultSignal.RemoveListener(OnDismantleSuccess);
        }

        [ListensTo(typeof(UpdateInventoryLootDismantleModalViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(ShowModalViewSignal))]
        public void OnShowView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.InventoryLootDismantle)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideModalViewSignal))]
        public void OnHideView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.InventoryLootDismantle)
            {
                view.Hide();
            }
        }

        private void OnCloseButtonClicked()
        {
            closeModalViewSignal.Dispatch();
        }

        private void OnDismantleButtonClicked(ForgeCardVO vo)
        {
            sellForgeCardsSignal.Dispatch(vo , dismantleResultSignal);
        }

        private void OnDismantleSuccess()
        {
            view.OnDismantleSuccess();
            LogUtil.Log("Its a dismantle success", "yellow");
        }
    }
}
