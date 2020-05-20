using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class AutoSubscriptionDailogueService : IAutoSubscriptionDailogueService
    {
        //Models
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        //Services
        [Inject] public IBackendService backendService { get; set; }

        //Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

        public bool CanShow()
        {
            var daysBetweenLastLogin = (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - preferencesModel.lastLaunchTime).TotalDays;
            preferencesModel.autoSubscriptionDlgShownCount = daysBetweenLastLogin >= adsSettingsModel.daysPerAutoSubscriptionDlgThreshold ? 0 : preferencesModel.autoSubscriptionDlgShownCount;

            return !playerModel.HasSubscription() && preferencesModel.autoSubscriptionDlgShownCount < adsSettingsModel.autoSubscriptionDlgThreshold;
        }

        public void Show()
        {
            if (CanShow())
            {
                preferencesModel.autoSubscriptionDlgShownCount++;
                appInfoModel.isAutoSubscriptionDlgShown = true;
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
                subscriptionDlgClosedSignal.AddOnce(() => appInfoModel.isAutoSubscriptionDlgShown = false);
            }
        }
    }
}
