/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class SettingsMediator : Mediator
    {
        // View injection
        [Inject] public SettingsView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.restorePurchaseButtonClickedSignal.AddListener(OnRestorePurchases);
            view.upgradeToPremiumButtonClickedSignal.AddListener(OnUpgradeToPremiumClicked);
            view.backButton.onClick.AddListener(OnBackButtonClicked);
            view.manageSubscriptionButtonClickedSignal.AddListener(OnManageSubscriptionClicked);
            view.applySettingsSignal.AddListener(OnApplySettings);
        }


        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SETTINGS)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.show_settings);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SETTINGS)
            {
                view.Hide();
            }
        }

        private void OnRestorePurchases()
        {
            restorePurchasesSignal.Dispatch();

#if UNITY_IOS
            hAnalyticsService.LogEvent("restore_ios_iap_clicked", "menu", "settings");
#endif
        }

        void OnBackButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                view.SetSubscriptionPrice();
            }
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            view.SetSubscriptionPrice();
        }

        void OnUpgradeToPremiumClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
            hAnalyticsService.LogEvent("upgrade_subscription_clicked", "menu", "settings");
        }

        private void OnManageSubscriptionClicked()
        {
            hAnalyticsService.LogEvent("manage_subscription_clicked", "menu", "settings");
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MANAGE_SUBSCRIPTION);
        }

        private void OnApplySettings()
        {
            if (view.HasSettingsChanged())
            {
                savePlayerInventorySignal.Dispatch();
            }

            //OnCloseDailogue();
        }
    }
}

