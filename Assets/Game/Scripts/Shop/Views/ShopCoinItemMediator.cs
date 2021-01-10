using strange.extensions.mediation.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ShopCoinItemMediator : Mediator
    {
        //View injection
        [Inject] public ShopCoinItemView view { get; set; }

        //Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }

        //Listeners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.buyButtonSignal.AddListener(OnPurchaseSignal);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGemsSignal);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnPurchaseSignal(VirtualGoodsTransactionVO vo)
        {
            virtualGoodsTransactionResultSignal.AddOnce((res) => view.OnPurchaseCompleted(res));
            virtualGoodsTransactionSignal.Dispatch(vo);
        }

        private void OnNotEnoughGemsSignal(long coins)
        {
            SpotPurchaseMediator.analyticsContext = $"{coins}_coins_pack_state_2";
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO playerInventory)
        {
            view.OnInventoryUpdated();
        }
    }
}
