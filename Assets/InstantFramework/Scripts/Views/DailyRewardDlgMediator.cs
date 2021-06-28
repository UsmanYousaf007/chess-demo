using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public class DailyRewardDlgMediator : Mediator
    {
        //View Injection
        [Inject] public DailyRewardDlgView view { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        // Signals
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public LoadCareerCardSignal loadCareerCardSignal { get; set; }
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }

        private RewardDlgVO _dailyRewardVO;

        public override void OnRegister()
        {
            view.Init();

            view._collectBtnClickedSignal.AddListener(OnCollectButtonClicked);
            view._collect2xBtnClickedSignal.AddListener(OnCollect2xButtonClicked);
            view._simpleCollectAnimationCompletedSignal.AddListener(() => OnRewardCollected(false));
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.DAILY_REWARD_DLG)
            {
                view.Show(playerModel.coins, playerModel.gems);
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.DAILY_REWARD_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateRewardDlgViewSignal))]
        public void OnUpdate(RewardDlgVO vo)
        {
            _dailyRewardVO = vo;
            view.UpdateView(vo, adsService.IsPersonalisedAdDlgShown());
        }

        public void OnCollectButtonClicked()
        {
            backendService.InBoxOpCollect(_dailyRewardVO.msgId).Then(OnCollectButtonBackendResponse);
        }

        private void OnCollect2xButtonClicked()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_daily_reward);
        }

        private void OnCollectButtonBackendResponse(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                view.PlaySimpleCollectAnimation();
            }
            else
            {
                view.EnableButtons(true);
            }
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_daily_reward)
            {
                switch (result)
                {
                    case AdsResult.FINISHED:
                        if (inboxModel.items.ContainsKey(_dailyRewardVO.msgId))
                        {
                            inboxModel.items.Remove(_dailyRewardVO.msgId);
                        }

                        OnRewardCollected(true);
                        break;

                    case AdsResult.FAILED:
                        view.EnableButtons(true);
                        break;

                    case AdsResult.NOT_AVAILABLE:
                        audioService.Play(audioService.sounds.SFX_TOOL_TIP);
                        view.toolTip.SetActive(true);
                        break;
                }
            }
        }

        private void OnRewardCollected(bool videoWatched)
        {
            SetNextDayNotificationReminder();
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            _dailyRewardVO.onCloseSignal?.Dispatch();

            // Dispatch rewards sequence signal here
            if (videoWatched)
            {
                RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO(_dailyRewardVO, videoWatched);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
                updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
            }

            updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            loadCareerCardSignal.Dispatch();
        }

        [ListensTo(typeof(DailyRewardClaimFailedSignal))]
        public void OnClaimRewardFailed()
        {
            SetNextDayNotificationReminder();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            _dailyRewardVO.onCloseSignal?.Dispatch();
            updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            loadCareerCardSignal.Dispatch();
        }

        private void SetNextDayNotificationReminder()
        {
            notificationsModel.UnregisterNotifications("league");

            var notification = new Notification();
            notification.title = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_TITLE);
            notification.body = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_BODY);
            notification.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1));
            notification.sender = "league";
            notificationsModel.RegisterNotification(notification);

            var reminder = new Notification();
            reminder.title = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_TITLE);
            reminder.body = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_BODY);
            reminder.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(settingsModel.dailyNotificationDeadlineHour).ToUniversalTime());
            reminder.sender = "league";
            notificationsModel.RegisterNotification(reminder);
        }
    }
}
