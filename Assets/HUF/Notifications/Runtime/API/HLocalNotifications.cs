using System;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;

namespace HUF.Notifications.Runtime.API
{
    public class HLocalNotifications
    {
        const string NOTIFICATIONS_NOT_INITIALIZED_WARNING =
            "Local Notifications not initialized yet. Initialize notifications before use.";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HLocalNotifications) );

        ILocalNotificationsService notifications;

        public event Action<ConsentStatus> OnAskForPermissionComplete
        {
            add
            {
                if ( notifications == null )
                {
                    HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                    return;
                }

                notifications.OnAskForPermissionComplete += value;
            }
            remove
            {
                if ( notifications == null )
                {
                    HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                    return;
                }

                notifications.OnAskForPermissionComplete -= value;
            }
        }

        /// <summary>
        /// Registers local notifications service.
        /// </summary>
        /// <param name="service">The service to be registered.</param>
        [PublicAPI]
        public void RegisterService( ILocalNotificationsService service )
        {
            notifications?.Dispose();
            notifications = service;
            HLog.Log( logPrefix, $"Service {service.GetType()} initialized" );
        }

        /// <summary>
        /// Schedules notification.
        /// </summary>
        /// <param name="notificationData">Data for scheduled notifications.</param>
        /// <returns>An ID string for scheduled notification if success,
        /// <see cref="NotificationData.INVALID_NOTIFICATION_ID"/> otherwise.</returns>
        [PublicAPI]
        public string ScheduleNotification( NotificationData notificationData )
        {
            if ( notifications == null )
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                return NotificationData.INVALID_NOTIFICATION_ID;
            }

            return notifications.ScheduleNotification( notificationData );
        }

        /// <summary>
        /// Clears scheduled notification.
        /// </summary>
        /// <param name="notificationId">A notification id that needs to be cleared.</param>
        [PublicAPI]
        public void ClearScheduledNotification( string notificationId )
        {
            if ( notifications == null )
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                return;
            }

            notifications.ClearScheduledNotification( notificationId );
        }

        /// <summary>
        /// Clears all scheduled notifications.
        /// </summary>
        [PublicAPI]
        public void ClearAllNotifications()
        {
            if ( notifications == null )
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                return;
            }

            notifications.ClearAllNotifications();
        }

        /// <summary>
        /// Returns last intent data after opening the app from a notification.
        /// </summary>
        /// <returns>String value if intent data is present, empty string otherwise. If null is returned
        /// no notification service is present.</returns>
        [PublicAPI]
        public string GetLastIntentData()
        {
            if ( notifications == null )
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                return string.Empty;
            }

            return notifications.GetLastIntentData();
        }

        /// <summary>
        /// Reads current user consent status from device settings.
        /// Currently only iOS is supported.
        /// </summary>
        /// <returns>Consent status that reflects actual state.</returns>
        [PublicAPI]
        public ConsentStatus GetConsentStatus()
        {
            if ( notifications == null )
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                return ConsentStatus.Undefined;
            }

            return notifications.GetConsentStatus();
        }

        /// <summary>
        /// Asks for permission to receive notifications.
        /// Currently only iOS is supported.
        /// </summary>
        /// <param name="registerForRemoteNotifications">It should also ask for remote notifications permission.</param>
        [PublicAPI]
        public void AskForPermission( bool registerForRemoteNotifications )
        {
            if ( notifications == null )
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING );
                return;
            }

            notifications.AskForPermission( registerForRemoteNotifications );
        }
    }
}
