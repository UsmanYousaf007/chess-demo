/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.Multiplayer;
using System.Collections.Generic;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
namespace TurboLabz.InstantFramework
{
    public class AppEventCommand : Command
    {
        // Signal parameters
        [Inject] public AppEvent appEvent { get; set; }

        // Dispatch signals
        [Inject] public GameAppEventSignal gameAppEventSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }
        [Inject] public ClosePromotionDlgSignal closePromotionDlgSignal { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IPushNotificationService firebasePushNotificationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //bool softReconnecting = false;

        public override void Execute()
        {
            gameAppEventSignal.Dispatch(appEvent);

            // Log analytics for quit while in reconnecting 
            if (appEvent == AppEvent.QUIT && appInfoModel.isReconnecting != DisconnectStates.FALSE)
            {
                analyticsService.Event(AnalyticsEventId.app_quit_during_disconnected);
            }

            if (appEvent == AppEvent.PAUSED || appEvent == AppEvent.QUIT)
            {
                appInfoModel.isResumeGS = false;

                navigatorEventSignal.Dispatch(NavigatorEvent.IGNORE);
                modelsSaveToDiskSignal.Dispatch();
                hAnalyticsService.LogEvent(AnalyticsEventId.focus_lost.ToString(), "focus");

                //only schedule local notificaitons once player model is filled with data
                if (playerModel.id != null)
                {
                    setLocalNotificationNumber();
                }
            }
            else if (appEvent == AppEvent.ESCAPED)
            {
                if (appInfoModel.isReconnecting != DisconnectStates.FALSE)
                    return;

                if (appInfoModel.internalAdType != InternalAdType.NONE)
                {
                    closePromotionDlgSignal.Dispatch();
                    return;
                }
                // if (!chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
                // return;

                if (appInfoModel.isVideoLoading)
                {
                    return;
                }

                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            }
            else if (appEvent == AppEvent.RESUMED)
            {
                appInfoModel.isResumeGS = true;
                backendService.ScheduleSwitchOffResumeGS();

                if (SplashLoader.launchCode != 1)
                {
                    var appsFlyerId = new KeyValuePair<string, object>("appsflyer_id", hAnalyticsService.GetAppsFlyerId());

                    if (firebasePushNotificationService.IsNotificationOpened())
                    {
                        hAnalyticsService.LogEvent("launch_opened", "launch", "notification", appsFlyerId);
                    }
                    else
                    {
                        hAnalyticsService.LogEvent("launch_opened", "launch", appsFlyerId);
                    }
                    SplashLoader.launchCode = 2;
                }

                firebasePushNotificationService.ClearNotifications();
                navigatorModel.currentState.RenderDisplayOnEnter();
            }
        }

        public void setLocalNotificationNumber()
        {
#if UNITY_IOS
            NotificationServices.ClearRemoteNotifications();
            NotificationServices.CancelAllLocalNotifications();
            NotificationServices.ClearLocalNotifications();

            if (playerModel.notificationCount > 0)
            {
                LocalNotification setNotificationCount1 = new LocalNotification();
                setNotificationCount1.fireDate = System.DateTime.Now.AddSeconds(1f);
                setNotificationCount1.applicationIconBadgeNumber = playerModel.notificationCount;
                setNotificationCount1.hasAction = false;
                NotificationServices.ScheduleLocalNotification(setNotificationCount1);
            }
#endif
        }
    }
}
