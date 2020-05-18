using Firebase.Messaging;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Notifications.Runtime.API
{
    public class HPushNotifications
    {
        IPushNotificationsService notifications;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HPushNotifications) );

        /// <summary>
        /// Dispatched when coming to app with notification. Parameter is raw data of notification.
        /// </summary>
        [PublicAPI]
        public event UnityAction<string> OnNotificationsReceived;

        /// <summary>
        /// Registers push notifications service.
        /// </summary>
        /// <param name="service">Service to be registered</param>
        [PublicAPI]
        public void RegisterService(IPushNotificationsService service)
        {
            notifications?.Dispose();
            notifications = service;
            HLog.Log( logPrefix, $"Service {service.GetType()} registered" );
        }

        /// <summary>
        /// Initializes push notifications.
        /// </summary>
        [PublicAPI]
        public void InitializePushNotifications()
        {
            if (notifications != null && !notifications.IsInitialized)
            {
                notifications.InitializeNotifications();
                notifications.OnNotificationReceived += NotificationReceived;
            }

            HLog.Log( logPrefix, $"Push notification service initialized" );
        }

        void NotificationReceived(string notificationData)
        {
            OnNotificationsReceived.Dispatch(notificationData);
        }

        public string CachedToken => notifications.CachedToken;
        public FirebaseMessage CachedMessage => notifications.CachedMessage;
    }
}