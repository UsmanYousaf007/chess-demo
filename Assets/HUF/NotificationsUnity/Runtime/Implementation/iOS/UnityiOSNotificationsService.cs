#if UNITY_IOS
using System.Linq;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;
using Unity.Notifications.iOS;

namespace HUF.NotificationsUnity.Runtime.Implementation.iOS
{
    public class UnityiOSNotificationsService : ILocalNotificationsService
    {
        public string ScheduleNotification(NotificationData notificationData)
        {
            if (!notificationData.IsNotificationValid())
                return NotificationData.INVALID_NOTIFICATION_ID;
            var notification = notificationData.CreateIOsNotification();
            iOSNotificationCenter.ScheduleNotification(notification);
            return notification.Identifier;
        }

        public void ClearScheduledNotification(string notificationId)
        {
            if (iOSNotificationCenter.GetDeliveredNotifications().Any(x => x.Identifier == notificationId))
                iOSNotificationCenter.RemoveDeliveredNotification(notificationId);
            else
                iOSNotificationCenter.RemoveScheduledNotification(notificationId);
        }

        public void ClearAllNotifications()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }

        public string GetLastIntentData()
        {
            return iOSNotificationCenter.GetLastRespondedNotification()?.Data;
        }

        public ConsentStatus GetConsentStatus()
        {
            var settings = iOSNotificationCenter.GetNotificationSettings();
            var consentStatus = ConvertConsentStatus(settings.AuthorizationStatus);
            return consentStatus;
        }

        public void Dispose()
        {
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }

        ConsentStatus ConvertConsentStatus(AuthorizationStatus status)
        {
            switch (status)
            {
                case AuthorizationStatus.Authorized:
                    return ConsentStatus.Granted;
                case AuthorizationStatus.Denied:
                    return ConsentStatus.Denied;
                default:
                    return ConsentStatus.Undefined;
            }
        }
    }
}
#endif