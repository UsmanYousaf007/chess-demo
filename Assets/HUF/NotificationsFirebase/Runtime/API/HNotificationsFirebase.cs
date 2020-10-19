using System;
using Firebase.Messaging;
using HUF.Notifications.Runtime.API;
using HUF.NotificationsFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.NotificationsFirebase.Runtime.API
{
    public static class HNotificationsFirebase
    {
        static FirebasePushNotificationsService firebaseNotifications;

        /// <summary>
        /// Raw Firebase message receiver.
        /// </summary>
        [PublicAPI]
        public static event EventHandler<MessageReceivedEventArgs> OnMessageReceived
        {
            add => FirebaseMessaging.MessageReceived += value;
            remove => FirebaseMessaging.MessageReceived -= value;
        }

        /// <summary>
        /// Returns whether Firebase Notifications is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; }

        /// <summary>
        /// Initializes Firebase Cloud Messaging (Firebase Notifications).
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( IsInitialized )
                return;

            firebaseNotifications = new FirebasePushNotificationsService();
            HNotifications.Push.RegisterService( firebaseNotifications );
            firebaseNotifications.InitializeNotifications();
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes Firebase Cloud Messaging (Firebase Notifications).
        /// </summary>
        /// <param name="callback">Callback invoked after initialization is finished regardless of the outcome</param>
        [PublicAPI]
        public static void Init( Action callback )
        {
            Init();

            if ( callback == null )
                return;

            void HandleInitComplete()
            {
                firebaseNotifications.OnInitialized -= HandleInitComplete;
                callback();
            }

            if ( firebaseNotifications.IsInitialized )
                callback();
            else
            {
                firebaseNotifications.OnInitialized += HandleInitComplete;
            }
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( HConfigs.HasConfig<FirebaseNotificationsConfig>() &&
                 HConfigs.GetConfig<FirebaseNotificationsConfig>().AutoInit )
            {
                Init();
            }
        }
    }
}