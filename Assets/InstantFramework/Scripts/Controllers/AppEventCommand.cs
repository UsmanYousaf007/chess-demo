/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-01 11:39:01 UTC+05:00
/// 
/// @description
/// [add_description_here]

using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using UnityEngine.iOS;

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

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        //bool softReconnecting = false;

        public override void Execute()
        {
            gameAppEventSignal.Dispatch(appEvent);

            if (appEvent == AppEvent.PAUSED || appEvent == AppEvent.QUIT)
            {
                modelsSaveToDiskSignal.Dispatch();
                setLocalNotificationNumber();
            }
            else if (appEvent == AppEvent.ESCAPED)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            }
        }

        public void setLocalNotificationNumber()
        {
            NotificationServices.ClearRemoteNotifications();
            NotificationServices.CancelAllLocalNotifications();
            NotificationServices.ClearLocalNotifications();

            if (playerModel.notificationCount > 0)
            {
                LocalNotification setNotificationCount1 = new LocalNotification();
                setNotificationCount1.fireDate = System.DateTime.Now.AddSeconds(5f);
                setNotificationCount1.applicationIconBadgeNumber = playerModel.notificationCount;
                setNotificationCount1.hasAction = false;
                NotificationServices.ScheduleLocalNotification(setNotificationCount1);
            }
        }


    }
}
