/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using HUF.Notifications.Runtime.API;

namespace TurboLabz.InstantFramework
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private string pushToken = null;

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Listen to signals
        [Inject] public AppEventSignal appEventSignal { get; set; }

        // Signals
        [Inject] public NotificationRecievedSignal notificationRecievedSignal { get; set; }

        bool isNotificationOpened;

        public void Init() 
        {
            HNotifications.Push.OnFirebaseNotificationsReceived += OnMessageReceived;
            ProcessToken(HNotifications.Push.CachedToken);
        }

        private void ProcessToken(string token) 
        {
            if (token == null) return;
            backendService.PushNotificationRegistration(token);
            pushToken = token;
            TLUtils.LogUtil.Log("Firebase deviceToken: " + pushToken, "red");
        }

        public string GetToken()
        {
            return pushToken;
        }

        private void OnAppEvent(AppEvent evt)
        {

        }

        public virtual void OnMessageReceived(Firebase.Messaging.FirebaseMessage message)
        {
            var notification = message.Notification;
            isNotificationOpened = message.NotificationOpened;

            // Bail if push notification was not clicked.
            // Socket messaging handles notifications when game is running.
            if (!isNotificationOpened)
            {
                return;
            }

            NotificationVO notificationVO;
            notificationVO.title = "unassigned";
            notificationVO.body = "unassigned";

            if (notification != null)
            {
                notificationVO.title = notification.Title;
                notificationVO.body = notification.Body;
            }
            notificationVO.isOpened = isNotificationOpened;
            notificationVO.senderPlayerId = message.Data.ContainsKey("senderPlayerId") == true ? message.Data["senderPlayerId"] : "undefined";
            notificationVO.challengeId = message.Data.ContainsKey("challengeId") == true ? message.Data["challengeId"] : "undefined";
            notificationVO.matchGroup = message.Data.ContainsKey("matchGroup") == true ? message.Data["matchGroup"] : "undefined";
            notificationVO.avatarId = message.Data.ContainsKey("avatarId") == true ? message.Data["avatarId"] : "undefined";
            notificationVO.avaterBgColorId = message.Data.ContainsKey("avatarBgColorId") == true ? message.Data["avatarBgColorId"] : "undefined";
            notificationVO.profilePicURL = message.Data.ContainsKey("profilePicURL") == true ? message.Data["profilePicURL"] : "undefined";
            notificationVO.isPremium = message.Data.ContainsKey("isSubscriber") == true ? bool.Parse(message.Data["isSubscriber"]) : false;
            notificationVO.timeSent = message.Data.ContainsKey("creationTimestamp") == true ? long.Parse(message.Data["creationTimestamp"]) : 0;
            notificationVO.actionCode = message.Data.ContainsKey("actionCode") == true ? message.Data["actionCode"] : "undefined";

        notificationRecievedSignal.Dispatch(notificationVO);
        }

        public bool IsNotificationOpened()
        {
            return isNotificationOpened;
        }

        public void ClearNotifications()
        {
#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#elif UNITY_ANDROID
            Unity.Notifications.Android.AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif
        }
    }
}