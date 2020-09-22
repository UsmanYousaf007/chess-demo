using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;

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

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

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
            view.UpdateSubscriptionStatus();
        }
        
        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.shortCode) && view.gameObject.activeInHierarchy)
            {
                view.PlayAnimation();
                var itemId = item.displayName.Replace(' ', '_').ToLower();
                analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(item.key).ToString(), 1, "inventory", "gems");
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, "gems", item.currency3Cost, "inventory", itemId);
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
                        if (view.gameObject.activeInHierarchy)
                        {
                            view.ShowTooltip();
                        }
                        break;

                    case InventoryVideoResult.SUCCESS:
                        if (view.gameObject.activeInHierarchy)
                        {
                            view.OnRewardedPointAdded();
                        }
                        else
                        {
                            view.SetupRewardBar();
                        }
                        break;

                    case InventoryVideoResult.ITEM_UNLOCKED:
                        if (view.gameObject.activeInHierarchy)
                        {
                            analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(key).ToString(), 1, "inventory", "rewarded_video");
                            view.OnItemUnclocked();
                        }
                        else
                        {
                            view.SetupRewardBar();
                        }
                        break;
                }
            }
        }
    }
}
