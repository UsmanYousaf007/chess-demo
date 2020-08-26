using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class InventoryItemMediator : Mediator
    {
        //View injection
        [Inject] public InventoryItemView view { get; set; }

        //Dispatch Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowInventoryRewardedVideoSignal showInventoryRewardedVideoSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.buyButtonSignal.AddListener(OnPurchaseSignal);
            view.notEnoughCurrencyToUnlockSignal.AddListener(OnNotEnoughCurrency);
            view.watchAdSignal.AddListener(OnWatchVideo);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnPurchaseSignal(string shortCode)
        {
            purchaseStoreItemSignal.Dispatch(shortCode, true);
        }

        private void OnWatchVideo(InventoryVideoVO vo)
        {
            showInventoryRewardedVideoSignal.Dispatch(vo);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            view.SetupPriceAndCount();
        }
        
        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.shortCode))
            {
                view.PlayAnimation();
            }
        }

        private void OnNotEnoughCurrency()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        [ListensTo(typeof(InventoryVideoResultSignal))]
        public void OnVideoResult(InventoryVideoResult result, string key)
        {
            if (key.Equals(view.shortCode))
            {
                switch (result)
                {
                    case InventoryVideoResult.NOT_AVAILABLE:
                        view.ShowTooltip();
                        break;

                    case InventoryVideoResult.SUCCESS:
                        view.OnRewardedPointAdded();
                        break;

                    case InventoryVideoResult.ITEM_UNLOCKED:
                        view.OnItemUnclocked();
                        break;
                }
            }
        }
    }
}
