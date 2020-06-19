using System;
using HUF.Notifications.Runtime.Data.Structs;

namespace HUF.Notifications.Runtime.API
{
    public interface ILocalNotificationsService : IDisposable
    {
        string ScheduleNotification(NotificationData notificationData);
        void ClearScheduledNotification(string notificationId);
        void ClearAllNotifications();
        string GetLastIntentData();
        ConsentStatus GetConsentStatus();
    }
}