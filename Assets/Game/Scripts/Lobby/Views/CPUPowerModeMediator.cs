using strange.extensions.mediation.impl;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class CPUPowerModeMediator : Mediator
    {
        // View injection
        [Inject] public CPUPowerModeView view { get; set; }

        //Dispatch signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }

        //Service
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.powerModeButtonClickedSignal.AddListener(OnPowerModeButtonClicked);
            view.conutinueButtonSignal.AddListener(OnContinueButtonClicked);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGemsSignal);
            view.closeButtonSignal.AddListener(OnCloseSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_POWER_MODE)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_POWER_MODE)
            {
                view.Hide();
            }
        }

        private void OnPowerModeButtonClicked()
        {
            purchaseStoreItemSignal.Dispatch(view.shortCode, true);
        }

        private void OnContinueButtonClicked(bool powerMode)
        {
            startCPUGameSignal.Dispatch(powerMode);
        }

        private void OnNotEnoughGemsSignal()
        {
            SpotPurchaseMediator.analyticsContext = "cpu_pre_game_power_mode";
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        private void OnCloseSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.shortCode) && view.isActiveAndEnabled)
            {
                view.OnEnablePowerMode();
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.cpu_pre_game_power_mode);
                analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "booster_used", AnalyticsContext.cpu_pre_game_power_mode.ToString());
            }
        }
    }
}
