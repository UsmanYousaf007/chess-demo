using System;
using Firebase.Extensions;
using Firebase.Messaging;
using HUF.Notifications.Runtime.API;
using HUF.NotificationsFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.NotificationsFirebase.Runtime.API
{
    public static class HNotificationsFirebase
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HNotificationsFirebase) );

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
        /// <param name="callback">A callback invoked after the initialization is finished regardless of the outcome.</param>
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

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        /// <summary>
        /// Prints a FCM token in the console and returns it in the callback.
        /// Can be used for sending test push messages to a single device.
        /// </summary>
        /// <param name="callback">An operation callback.</param>
        [PublicAPI]
        public static void FetchFCMToken( Action<string> callback )
        {
            FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread( task =>
            {
                if ( !task.IsCompleted || string.IsNullOrEmpty( task.Result ) )
                {
                    HLog.LogWarning( logPrefix, "Retrieving FCM token failed" );
                    callback.Dispatch( null );
                    return;
                }

                HLog.LogAlways( logPrefix, $"FCM Token:\n{task.Result}" );
                callback.Dispatch( task.Result );
            } );
        }
#endif

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
