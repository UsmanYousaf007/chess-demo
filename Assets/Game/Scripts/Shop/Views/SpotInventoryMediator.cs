﻿using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class SpotInventoryMediator : Mediator
    {
        //View Injection
        [Inject] public SpotInventoryView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Dispatch Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowInventoryRewardedVideoSignal showInventoryRewardedVideoSignal { get; set; }
        [Inject] public SpotInventoryPurchaseCompletedSignal spotInventoryPurchaseCompletedSignal { get; set; }

        private string itemToUnlockShortCode;

        public override void OnRegister()
        {
            view.Init();
            view.closeDlgSignal.AddListener(OnCloseDlgSignal);
            view.buyButtonSignal.AddListener(OnPurchaseSignal);
            view.notEnoughCurrencyToUnlockSignal.AddListener(OnNotEnoughCurrency);
            view.watchAdSignal.AddListener(OnWatchVideo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_INVENTORY_DLG)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.spot_inventory_dlg);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_INVENTORY_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateSpotInventoryViewSignal))]
        public void OnUpdateView(SpotInventoryVO vo)
        {
            itemToUnlockShortCode = vo.itemToUnlockShortCode;
            view.UpdateView(vo);
        }

        private void OnCloseDlgSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnPurchaseSignal(string shortCode)
        {
            purchaseStoreItemSignal.Dispatch(shortCode, true);
        }

        private void OnWatchVideo(InventoryVideoVO vo)
        {
            showInventoryRewardedVideoSignal.Dispatch(vo);
        }

        private void OnNotEnoughCurrency()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            view.SetupPrice(inventory.gemsCount);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (view.isActiveAndEnabled && result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.storeItem.key))
            {
                OnCloseDlgSignal();
                var itemId = item.displayName.Replace(' ', '_').ToLower();
                analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(item.key).ToString(), 1, "spot_inventory", "gems");
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, "gems", item.currency3Cost, "spot_inventory", itemId);
                spotInventoryPurchaseCompletedSignal.Dispatch(itemToUnlockShortCode);
            }
        }

        [ListensTo(typeof(InventoryVideoResultSignal))]
        public void OnVideoResult(InventoryVideoResult result, string key)
        {
            if (view.isActiveAndEnabled && key.Equals(view.storeItem.key))
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
                        OnCloseDlgSignal();
                        analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(key).ToString(), 1, "spot_inventory", "rewarded_video");
                        spotInventoryPurchaseCompletedSignal.Dispatch(itemToUnlockShortCode);
                        break;
                }
            }
        }
    }
}
