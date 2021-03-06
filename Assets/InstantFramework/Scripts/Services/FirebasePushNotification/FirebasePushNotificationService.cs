/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using HUF.Notifications.Runtime.API;
using Firebase.Messaging;
using System.Collections;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private string pushToken = null;

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }

        // Listen to signals
        [Inject] public AppEventSignal appEventSignal { get; set; }
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

        // Dispatch Signals
        [Inject] public NotificationRecievedSignal notificationRecievedSignal { get; set; }

        //Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        bool isNotificationOpened;

        public void Init()
        {
            appEventSignal.AddListener(OnAppEvent);
            RegisterNotification();
            ClearNotifications();
            FirebaseMessaging.MessageReceived += OnMessageReceived;

            if (appInfoModel.isAutoSubscriptionDlgShown)
            {
                subscriptionDlgClosedSignal.AddOnce(HandleFirebaseInitCompleteAsync);
            }
            else
            {
                HandleFirebaseInitComplete();
            }
        }

        private void HandleFirebaseInitCompleteAsync()
        {
            routineRunner.StartCoroutine(HandleFirebaseInitCompleteWithDelay());
        }

        private IEnumerator HandleFirebaseInitCompleteWithDelay()
        {
            yield return new WaitForEndOfFrame();
            HandleFirebaseInitComplete();
        }

        private void HandleFirebaseInitComplete()
        {
            ProcessToken(HNotifications.Push.CachedToken);
            ProcessMessage(HNotifications.Push.CachedMessage);
        }

        public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            ProcessMessage(e.Message);
        }

        private void ProcessToken(string token)
        {
            if (token == null)
            {
                return;
            }

            backendService.PushNotificationRegistration(token);
            pushToken = token;
            TLUtils.LogUtil.Log("Firebase deviceToken: " + pushToken, "red");
        }

        private void ProcessMessage(FirebaseMessage message)
        {
            if (message == null)
            {
                return;
            }

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

            if (!(message.Data.ContainsKey("league") && int.TryParse(message.Data["league"], out notificationVO.league)))
            {
                notificationVO.league = 0;
            }

            notificationRecievedSignal.Dispatch(notificationVO);
        }

        public string GetToken()
        {
            return pushToken;
        }

        private void OnAppEvent(AppEvent evt)
        {

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

        private void RegisterNotification()
        {
#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.None);
#endif
        }
    }
}