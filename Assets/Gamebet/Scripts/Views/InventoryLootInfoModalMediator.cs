/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-08 16:45:23 UTC+05:00
using strange.extensions.mediation.impl;
using TurboLabz.Common;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class InventoryLootInfoModalMediator : Mediator
    {
        // View injection
        [Inject] public InventoryLootInfoModalView view { get; set; }
        [Inject] public CloseModalViewSignal closeModalViewSignal { get; set; }
        [Inject] public BuildForgeCardsSignal buildForgeCardsSignal { get; set; }

        public Signal buildForgeCardResultSignal = new Signal();

        public override void OnRegister()
        {
            view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
            view.buildButtonClickedSignal.AddListener(OnBuildButtonClicked);

            buildForgeCardResultSignal.AddListener(OnBuildForgeCardSuccess);
            view.Init();
        }

        public override void OnRemove()
        {
            view.closeButtonClickedSignal.RemoveListener(OnCloseButtonClicked);
            view.buildButtonClickedSignal.RemoveListener(OnBuildButtonClicked);

            buildForgeCardResultSignal.RemoveListener(OnBuildForgeCardSuccess);
        }

        [ListensTo(typeof(UpdateInventoryLootInfoModalViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(ShowModalViewSignal))]
        public void OnShowView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.InventoryLootInfo)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(HideModalViewSignal))]
        public void OnHideView(ModalViewId modalViewId)
        {
            if (modalViewId == ModalViewId.InventoryLootInfo)
            {
                view.Hide();
            }
        }

        private void OnCloseButtonClicked()
        {
            closeModalViewSignal.Dispatch();
        }

        private void OnBuildButtonClicked(string forgeCardKey)
        {
            buildForgeCardsSignal.Dispatch(forgeCardKey, buildForgeCardResultSignal);
        }

        private void OnBuildForgeCardSuccess()
        {
            view.OnBuildForgeCardSuccess();
        }
    }
}