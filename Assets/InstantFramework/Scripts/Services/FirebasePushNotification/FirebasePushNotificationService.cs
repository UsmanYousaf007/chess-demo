/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using UnityEngine.Purchasing.Security;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using HUF.InitFirebase.Runtime.API;
using HUF.Notifications.Runtime.API;
using Firebase.Messaging;

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

            appEventSignal.AddListener(OnAppEvent);

#if !UNITY_EDITOR
            if (HInitFirebase.IsInitialized)
            {
                LogUtil.Log("FB Notif initialized");
                HandleFirebaseInitComplete();
            }
            else
            {
                LogUtil.Log("FB Notif handler added");
                HInitFirebase.OnInitializationSuccess += HandleFirebaseInitComplete;
            }
#endif

            //Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
            //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            //{
            //    if (task.Result == Firebase.DependencyStatus.Available)
            //    {
            //        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = false;
            //        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            //        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync();

            //        //if (string.IsNullOrEmpty(HInitFirebase.CachedToken))
            //        //{
            //        //    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            //        //}
            //        //else
            //        //{
            //        //    ProcessToken(HInitFirebase.CachedToken);
            //        //}

            //        TLUtils.LogUtil.Log("Firebase intialization success.");
            //    }
            //    else
            //    {
            //        TLUtils.LogUtil.Log("Firebase could not resolve all dependencies: " + dependencyStatus, "red");
            //    }
            //}
            //);
        }

        void HandleFirebaseInitComplete()
        {
            ProcessToken(HNotifications.Push.CachedToken);
            ProcessMessage(HNotifications.Push.CachedMessage);
        }

        public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            ProcessToken(token.Token);
        }

        public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            ProcessMessage(e.Message);
        }

        private void ProcessToken(string token)
        {
            backendService.PushNotificationRegistration(token);
            pushToken = token;
            TLUtils.LogUtil.Log("Firebase deviceToken: " + pushToken, "red");
        }

        private void ProcessMessage(FirebaseMessage message)
        {
            if (message == null)
            {
                LogUtil.Log("FB Notif message null");
                return;
            }

            var notification = message.Notification;
            isNotificationOpened = message.NotificationOpened;

            // Bail if push notification was not clicked.
            // Socket messaging handles notifications when game is running.
            if (!isNotificationOpened)
            {
                LogUtil.Log("FB Notif not opened");
                return;
            }

            LogUtil.Log("FB Notif opened");

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
    }
}