#if UNITY_IOS
using System;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.Utils.Runtime.Extensions;
using Unity.Notifications.iOS;

namespace HUF.NotificationsUnity.Runtime.Implementation.iOS
{
    public static class NotificationDataiOSUtils
    {
        public static iOSNotification CreateIOsNotification( this NotificationData notificationData )
        {
            return new iOSNotification
            {
                Identifier = notificationData.identifier,
                Title = notificationData.title,
                Subtitle = notificationData.text,
                ShowInForeground = true,
                CategoryIdentifier = notificationData.categoryIdentifier,
                Trigger = new iOSNotificationTimeIntervalTrigger
                {
                    TimeInterval = new TimeSpan( 0, 0, notificationData.delayInSeconds )
                },
                ForegroundPresentationOption = ( PresentationOption.Alert |
                                                 PresentationOption.Sound |
                                                 PresentationOption.Badge ),
                Data = notificationData.intentData.IsNullOrEmpty() ? string.Empty : notificationData.intentData
            };
        }
    }
}
#endif