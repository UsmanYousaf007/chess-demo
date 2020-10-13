using System;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;

namespace HUF.NotificationsUnity.Runtime.Implementation.Dummy
{
    public class DummyNotificationsService : ILocalNotificationsService
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(DummyNotificationsService) );

        public event Action<ConsentStatus> OnAskForPermissionComplete;

        public void Dispose()
        {
            HLog.Log( logPrefix, "Dispose called" );
        }

        public string ScheduleNotification( NotificationData notificationData )
        {
            HLog.Log( logPrefix, "Schedule notification called" );
            return string.Empty;
        }

        public void ClearScheduledNotification( string notificationId )
        {
            HLog.Log( logPrefix, $"Clear scheduled notification with id {notificationId} called" );
        }

        public void ClearAllNotifications()
        {
            HLog.Log( logPrefix, "Clear all notifications called" );
        }

        public string GetLastIntentData()
        {
            HLog.Log( logPrefix, "Get last intent data called" );
            return string.Empty;
        }

        public ConsentStatus GetConsentStatus()
        {
            HLog.Log( logPrefix, "Has consent called" );
            return ConsentStatus.Undefined;
        }

        public void AskForPermission( bool registerForRemoteNotifications )
        {
            HLog.Log( logPrefix, $"Asked for notifications permission" );
            OnAskForPermissionComplete.Dispatch( ConsentStatus.Granted );
        }
    }
}