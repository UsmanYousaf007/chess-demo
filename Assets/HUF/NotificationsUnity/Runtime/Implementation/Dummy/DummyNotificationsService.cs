using System;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.NotificationsUnity.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;

namespace HUF.NotificationsUnity.Runtime.Implementation.Dummy
{
    public class DummyNotificationsService : ILocalNotificationsService
    {
        public const string EDITOR_NOTIFICATIONS_VALUES_KEY = nameof(EDITOR_NOTIFICATIONS_VALUES_KEY);
        public const string EDITOR_NOTIFICATION_KEY = nameof(EDITOR_NOTIFICATION_KEY);
        public const string EDITOR_NOTIFICATION_CAN_CLEAN_KEY = nameof(EDITOR_NOTIFICATION_CAN_CLEAN_KEY);

        static readonly HLogPrefix logPrefix = new HLogPrefix( HNotificationsUnity.logPrefix, nameof(DummyNotificationsService) );

        public event Action<ConsentStatus> OnAskForPermissionComplete;

        public void Dispose()
        {
            HLog.Log( logPrefix, "Dispose called" );
        }

        public string ScheduleNotification( NotificationData notificationData )
        {
            HLog.Log( logPrefix, "Schedule notification called" );
            SaveNotification( notificationData );
            return string.Empty;
        }

        void SaveNotification( NotificationData notificationData )
        {
            HPlayerPrefs.SetString(
                $"{EDITOR_NOTIFICATION_KEY}{HPlayerPrefs.GetInt( EDITOR_NOTIFICATIONS_VALUES_KEY )}",
                JsonUtility.ToJson( notificationData ) );

            HPlayerPrefs.SetInt( EDITOR_NOTIFICATIONS_VALUES_KEY,
                HPlayerPrefs.GetInt( EDITOR_NOTIFICATIONS_VALUES_KEY ) + 1 );
        }

        public void ClearScheduledNotification( string notificationId )
        {
            HLog.Log( logPrefix, $"Clear scheduled notification with id {notificationId} called" );
        }

        public void ClearAllNotifications()
        {
            HLog.Log( logPrefix, "Clear all notifications called" );

            if ( !HPlayerPrefs.GetBool( EDITOR_NOTIFICATION_CAN_CLEAN_KEY ) )
                return;

            HPlayerPrefs.SetInt( EDITOR_NOTIFICATIONS_VALUES_KEY, 0 );
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