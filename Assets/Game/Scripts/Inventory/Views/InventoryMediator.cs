﻿using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class InventoryMediator : Mediator
    {
        //View Injection
        [Inject] public InventoryView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        //Dispatch Signals
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.applyThemeSignal.AddListener(OnApplyTheme);
            view.unlockAllThemesSignal.AddListener(OnUnlockAllThemes);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INVENTORY)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INVENTORY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnApplyTheme()
        {
            if (view.HasSkinChanged())
            {
                savePlayerInventorySignal.Dispatch("");
                view.originalSkinId = view.playerModel.activeSkinId;
                hAnalyticsService.LogEvent("selection", "menu", "", "theme_change");
            }
        }

        private void OnUnlockAllThemes()
        {
            analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.unlock_all_themes);
            purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.ALL_THEMES_PACK, true);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            if (view.playerModel.OwnsAllThemes())
            {
                view.ShowThemeBanner(false);
            }
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnProductPurchased(StoreItem item)
        {
            if (item.key.Equals(GSBackendKeys.ShopItem.ALL_THEMES_PACK))
            {
                view.ShowThemeBanner(false);

                if (view.isActiveAndEnabled)
                {
                    analyticsService.Event(AnalyticsEventId.banner_purchased, AnalyticsContext.unlock_all_themes);
                }
            }
        }
    }
}