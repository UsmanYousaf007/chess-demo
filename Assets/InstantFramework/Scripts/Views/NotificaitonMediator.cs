/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class NotificationMediator : Mediator
    {
        // Dispatch signals

        // View injection
        [Inject] public NotificationView view { get; set; }

        // Signals

        // Services

        public override void OnRegister()
        {
            view.Init();
        }

        public void OnShowView()
        {
            view.Show();
        }

        public void OnHideView()
        {
            view.Hide();
        }

        [ListensTo(typeof(NotificationRecievedSignal))]
        public void OnNotificationRecieved(NotificationVO notificationVO)
        {
            if (notificationVO.isOpened == false)
            {
                view.AddNotification(notificationVO);
            }
            else
            {
                view.ProcessOpenedNotification(notificationVO);
            }
        }

        [ListensTo(typeof(PauseNotificationsSignal))]
        public void OnPauseNotifications(bool enable)
        {
            view.Pause(enable);
        }

        [ListensTo(typeof(ShowFadeBlockerSignal))]
        public void OnFadeBlocker()
        {
            view.FadeBlocker();
        }
    }
}