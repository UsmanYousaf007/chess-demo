using System;
using HUF.Notifications.Runtime.Data.Structs;

namespace HUF.Notifications.Runtime.API
{
    public interface ILocalNotificationsService : IDisposable
    {
        event Action<ConsentStatus> OnAskForPermissionComplete;

        string ScheduleNotification( NotificationData notificationData );
        void ClearScheduledNotification( string notificationId );
        void ClearAllNotifications();
        string GetLastIntentData();
        ConsentStatus GetConsentStatus();
        void AskForPermission( bool registerForRemoteNotifications );
    }
}