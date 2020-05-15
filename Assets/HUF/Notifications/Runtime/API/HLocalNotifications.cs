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

        /// <summary>
        /// Registers local notifications service.
        /// </summary>
        /// <param name="service">Service to be registered</param>
        [PublicAPI]
        public void RegisterService(ILocalNotificationsService service)
        {
            notifications?.Dispose();
            notifications = service;
            HLog.Log( logPrefix, $"Service {service.GetType()} initialized" );
        }

        /// <summary>
        /// Schedules notification.
        /// </summary>
        /// <param name="notificationData">Data for scheduled notifications</param>
        /// <returns>Id string for scheduled notification if success,
        /// <see cref="NotificationData.INVALID_NOTIFICATION_ID"/> if otherwise</returns>
        [PublicAPI]
        public string ScheduleNotification(NotificationData notificationData)
        {
            if (notifications == null)
            {
                HLog.LogWarning( logPrefix ,NOTIFICATIONS_NOT_INITIALIZED_WARNING);
                return NotificationData.INVALID_NOTIFICATION_ID;
            }

            return notifications.ScheduleNotification(notificationData);
        }

        /// <summary>
        /// Clears scheduled notification.
        /// </summary>
        /// <param name="notificationId">Notification id that needs to be cleared</param>
        [PublicAPI]
        public void ClearScheduledNotification(string notificationId)
        {
            if (notifications == null)
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING);
                return;
            }

            notifications.ClearScheduledNotification(notificationId);
        }

        /// <summary>
        /// Clears all schedules notifications.
        /// </summary>
        [PublicAPI]
        public void ClearAllNotifications()
        {
            if (notifications == null)
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING);
                return;
            }

            notifications.ClearAllNotifications();
        }

        /// <summary>
        /// Return last intent data after returning from notification.
        /// </summary>
        /// <returns>String value if intent data is present, empty string otherwise. If null is returned
        /// no notification service is present.</returns>
        [PublicAPI]
        public string GetLastIntentData()
        {
            if (notifications == null)
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING);
                return string.Empty;
            }

            return notifications.GetLastIntentData();
        }

        /// <summary>
        /// Reads current user consent status from device settings.
        /// Currently only iOS is supported.
        /// </summary>
        /// <returns>Consent status that reflects actual state</returns>
        [PublicAPI]
        public ConsentStatus GetConsentStatus()
        {
            if (notifications == null)
            {
                HLog.LogWarning( logPrefix, NOTIFICATIONS_NOT_INITIALIZED_WARNING);
                return ConsentStatus.Undefined;
            }

            return notifications.GetConsentStatus();
        }
    }
}