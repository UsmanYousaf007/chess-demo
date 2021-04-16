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
        [Inject] public ShowRewardedAdSignal rewardedAdSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        //Service
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPreGameAdsService preGameAdsService { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        public override void OnRegister()
        {
            view.Init();
            view.serverClock = backendService.serverClock;
            view.powerModeButtonClickedSignal.AddListener(OnPowerModeButtonClicked);
            view.conutinueButtonSignal.AddListener(OnContinueButtonClicked);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGemsSignal);
            view.closeButtonSignal.AddListener(OnCloseSignal);

            view.showRewardedAdSignal.AddListener(OnPlayRewardedVideoClicked);
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
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
            preGameAdsService.ShowPreGameAd().Then(() => startCPUGameSignal.Dispatch(powerMode));
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

        [ListensTo(typeof(PowerPlayRewardClaimedSignal))]
        public void OnPowerPlayRewardClaimed()
        {
            if (view.isActiveAndEnabled)
            {
                view.OnEnablePowerMode();
                view.StartTimer(playerModel.rvUnlockTimestamp);
                analyticsService.Event(AnalyticsEventId.rv_used, AnalyticsContext.power_mode);
            }
        }

        private void OnPlayRewardedVideoClicked(AdPlacements adPlacements)
        {
            rewardedAdSignal.Dispatch(adPlacements);
        }

        //private void OnSetCoolDownTimer(long coolDownTimeUTC)
        //{
        //    preferencesModel.rvCoolDownTimeUTC = coolDownTimeUTC;

        //}

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
    }
}
