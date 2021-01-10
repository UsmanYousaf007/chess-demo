using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        //Dispatch signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

        //Models
        [Inject] public ISettingsModel settingsModel { get; set; }

        public void OnRegisterPowerplay()
        {
            view.InitPowerplay();
            view.powerModeButtonClickedSignal.AddListener(OnPowerModeButtonClicked);
            view.powerplayCloseButtonSignal.AddListener(OnPowerplayCloseSignal);
            view.showPowerplayDlgButonSignal.AddListener(OnShowPowerplayDlgSignal);
        }

        public void OnRemovePowerplay()
        {
            view.CleanupPowerplay();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowPowerplayView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_POWER_PLAY)
            {
                view.ShowPowerplay();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHidePowerplayView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_POWER_PLAY)
            {
                view.HidePowerplay();
            }
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.powerPlayShortCode) && view.IsVisible())
            {
                chessboardModel.powerMode = true;
                chessboardModel.freeHints = settingsModel.powerModeFreeHints;
                view.OnEnablePowerMode();
                view.UpdateSpecialHintButton(preferencesModel.cpuPowerUpsUsedCount, false, chessboardModel.freeHints);
                view.SetupPowerplayImage(true);
                OnPowerplayCloseSignal();
                view.SetupStepButtons();
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.cpu_in_game_power_mode);
                analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "booster_used", AnalyticsContext.cpu_in_game_power_mode.ToString());
            }
        }

        private void OnPowerModeButtonClicked()
        {
            purchaseStoreItemSignal.Dispatch(view.powerPlayShortCode, true);
        }

        private void OnPowerplayCloseSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnShowPowerplayDlgSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_POWER_PLAY);
        }
    }
}
