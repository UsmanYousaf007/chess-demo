using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        //Dispatch signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public ShowRewardedAdSignal rewardedAdSignal { get; set; }

        //Models
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        //Services
        [Inject] public ISchedulerService schedulerService { get; set; }
        public void OnRegisterPowerplay()
        {
            view.InitPowerplay();
            view.powerModeButtonClickedSignal.AddListener(OnPowerModeButtonClicked);
            view.powerplayCloseButtonSignal.AddListener(OnPowerplayCloseSignal);
            view.showPowerplayDlgButonSignal.AddListener(OnShowPowerplayDlgSignal);

            view.showRewardedAdInGameSignal.AddListener(OnPlayRewardedVideoClicked);
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
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
                view.serverClock = backendService.serverClock;
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
                OnPowerModeBought();
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.cpu_in_game_power_mode);
                analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "booster_used", AnalyticsContext.cpu_in_game_power_mode.ToString());
            }
        }

        private void OnPowerModeBought()
        {
            chessboardModel.powerMode = true;
            chessboardModel.freeHints = settingsModel.powerModeFreeHints;
            view.OnEnablePowerMode();
            view.UpdateSpecialHintButton(preferencesModel.cpuPowerUpsUsedCount, false, chessboardModel.freeHints);
            view.SetupPowerplayImage(true);
            OnPowerplayCloseSignal();
            view.SetupStepButtons();
        }

        [ListensTo(typeof(PowerPlayRewardClaimedSignal))]
        public void OnPowerPlayRewardClaimed()
        {
            if (view.isActiveAndEnabled)
            {
                OnPowerModeBought();
            }
        }

        private void OnPlayRewardedVideoClicked(AdPlacements adPlacements)
        {
            rewardedAdSignal.Dispatch(adPlacements);
        }

        private void OnSchedulerSubscriptionToggle(bool subscribe)
        {
            if (subscribe)
            {
                schedulerService.Subscribe(view.SchedulerCallBack);
            }
            else
            {
                schedulerService.UnSubscribe(view.SchedulerCallBack);
            }
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_powerplay)
            {
                if ((result == AdsResult.FINISHED || result == AdsResult.SKIPPED))
                {
                    //this.rewardedVideoShown = true;
                    analyticsService.Event(AnalyticsEventId.rv_used, AnalyticsContext.power_mode);
                }

                else if (result == AdsResult.NOT_AVAILABLE)
                {
                    view.EnableVideoAvailabilityTooltip();
                    Invoke("view.DisableVideoAvailabilityTooltip", 5);
                }
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
