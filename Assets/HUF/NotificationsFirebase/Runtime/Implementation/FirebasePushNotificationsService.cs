using Firebase.Messaging;
using HUF.InitFirebase.Runtime.API;
using HUF.InitFirebase.Runtime.Config;
using HUF.Notifications.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.NotificationsFirebase.Runtime.Implementation
{
    public class FirebasePushNotificationsService : IPushNotificationsService
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebasePushNotificationsService) );

        string cachedToken;
        FirebaseMessage cachedMessage;
        bool isInitializing = false;

        public event UnityAction OnInitialized;
        public event UnityAction<string> OnNotificationReceived;

        public bool IsInitialized => !cachedToken.IsNullOrEmpty();
        public string CachedToken => cachedToken;
        public FirebaseMessage CachedMessage => cachedMessage;

        public void InitializeNotifications()
        {
            if ( isInitializing || IsInitialized )
                return;

            isInitializing = true;

            if ( HInitFirebase.IsInitialized )
            {
                AttachCallbacks();
                HLog.Log( logPrefix, "Service initialized" );
            }
            else
            {
                HInitFirebase.OnInitializationSuccess += OnFirebaseInitSuccess;

                if ( !HConfigs.HasConfig<HFirebaseConfig>() || HConfigs.GetConfig<HFirebaseConfig>().AutoInit )
                {
                    HLog.Log( logPrefix, "Service depends on Firebase. Initializing Firebase first." );
                    HInitFirebase.Init();
                    return;
                }

                HLog.Log( logPrefix, "Service depends on Firebase which will be manually initialized" );
            }
        }

        public void Dispose()
        {
            DetachCallbacks();
        }

        void OnFirebaseInitSuccess()
        {
            HInitFirebase.OnInitializationSuccess -= OnFirebaseInitSuccess;
            AttachCallbacks();
        }

        void AttachCallbacks()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        void DetachCallbacks()
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        void OnTokenReceived( object sender, TokenReceivedEventArgs token )
        {
            cachedToken = token.Token;
            isInitializing = false;
            OnInitialized.Dispatch();
        }

        void OnMessageReceived( object sender, MessageReceivedEventArgs e )
        {
            cachedMessage = e.Message;
            OnNotificationReceived.Dispatch( e.Message.RawData );
        }
    }
}