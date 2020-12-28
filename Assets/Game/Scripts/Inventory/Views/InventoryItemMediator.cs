using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
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

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

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
                preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_GEMS][item.key] += 1;

                if (item.key.Equals(GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY))
                {
                    analyticsService.Event(AnalyticsEventId.key_obtained_gem, AnalyticsContext.inventory);
                }

                if (item.key.Equals(GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT))
                {
                    preferencesModel.freeDailyHint = FreePowerUpStatus.BOUGHT;
                }
                else if(item.key.Equals(GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER))
                {
                    preferencesModel.freeDailyRatingBooster = FreePowerUpStatus.BOUGHT;
                }
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

                        if (key.Equals(GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT))
                        {
                            preferencesModel.freeDailyHint = FreePowerUpStatus.BOUGHT;
                        }
                        else if (key.Equals(GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER))
                        {
                            preferencesModel.freeDailyRatingBooster = FreePowerUpStatus.BOUGHT;
                        }

                        if (view.gameObject.activeInHierarchy)
                        {
                            analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(key).ToString(), 1, "inventory", "rewarded_video");
                            preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_VIDEOS][key] += 1;
                            view.OnItemUnclocked();

                            if (key.Equals(GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY))
                            {
                                analyticsService.Event(AnalyticsEventId.key_obtained_rv, AnalyticsContext.inventory);
                            }
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
