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
        [Inject] public UpdatePlayerDataSignal updatePlayerDataSignal  { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

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
            hAnalyticsService.LogEvent("clicked", "settings", "", "restore_ios_iap");
#endif
        }

        void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
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
            promotionsService.LoadSubscriptionPromotion();
        }

        private void OnManageSubscriptionClicked()
        {
            hAnalyticsService.LogEvent("clicked", "settings", "", "manage_subscription");
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MANAGE_SUBSCRIPTION);
        }

        private void OnApplySettings()
        {
            
        }
    }
}

