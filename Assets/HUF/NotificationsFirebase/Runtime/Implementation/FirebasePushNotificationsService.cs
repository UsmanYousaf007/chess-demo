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
        string cachedToken;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebasePushNotificationsService) );

        public bool IsInitialized => !cachedToken.IsNullOrEmpty();
        public string CachedToken => cachedToken;

        public event UnityAction<string> OnNotificationReceived;
        public event UnityAction<FirebaseMessage> OnFirebaseNotificationReceived;


        public void InitializeNotifications()
        {
            if (HInitFirebase.IsInitialized)
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

        void AttachCallbacks()
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            cachedToken = token.Token;
        }

        void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnNotificationReceived.Dispatch(e.Message.RawData);
            OnFirebaseNotificationReceived.Dispatch(e.Message);
        }

        void OnFirebaseInitSuccess()
        {
            HInitFirebase.OnInitializationSuccess -= OnFirebaseInitSuccess;
            AttachCallbacks();
        }

        public void Dispose()
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }
    }
}