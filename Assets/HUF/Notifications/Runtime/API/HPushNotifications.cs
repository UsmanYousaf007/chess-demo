using Firebase.Messaging;
using System;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;

namespace HUF.Notifications.Runtime.API
{
    public class HPushNotifications
    {
        IPushNotificationsService notifications;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HPushNotifications) );

        /// <summary>
        /// Raised when a notification is received. The Parameter is a raw notification data.
        /// </summary>
        [PublicAPI]
        public event Action<byte[]> OnNotificationsReceived;

        /// <summary>
        /// Registers push notifications service.
        /// </summary>
        /// <param name="service">The service to be registered.</param>
        [PublicAPI]
        public void RegisterService( IPushNotificationsService service )
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
            if ( notifications != null && !notifications.IsInitialized )
            {
                notifications.InitializeNotifications();
                notifications.OnNotificationReceived += NotificationReceived;
            }

            HLog.Log( logPrefix, $"Push notification service initialized" );
        }

        void NotificationReceived( byte[] notificationData )
        {
            OnNotificationsReceived.Dispatch( notificationData );
        }

        public string CachedToken => notifications?.CachedToken;
        public FirebaseMessage CachedMessage => notifications?.CachedMessage;

    }}
