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

            Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
                {
                    if (task.Result == Firebase.DependencyStatus.Available) 
                    {
                        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = false;
                        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync();
                        TLUtils.LogUtil.Log("Firebase intialization success.");
                    } 
                    else 
                    {
                        TLUtils.LogUtil.Log("Firebase could not resolve all dependencies: " + dependencyStatus, "red");
                    }
                }
            );
        }

        public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) 
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            backendService.PushNotificationRegistration(token.Token);
            pushToken = token.Token;
            TLUtils.LogUtil.Log("Firebase deviceToken: " + pushToken, "red");
        }

        public string GetToken()
        {
            return pushToken;
        }

        private void OnAppEvent(AppEvent evt)
        {

        }

        public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            var notification = e.Message.Notification;
            isNotificationOpened = e.Message.NotificationOpened;

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
            notificationVO.senderPlayerId = e.Message.Data.ContainsKey("senderPlayerId") == true ? e.Message.Data["senderPlayerId"] : "undefined";
            notificationVO.challengeId = e.Message.Data.ContainsKey("challengeId") == true ? e.Message.Data["challengeId"] : "undefined";
            notificationVO.matchGroup = e.Message.Data.ContainsKey("matchGroup") == true ? e.Message.Data["matchGroup"] : "undefined";
            notificationVO.avatarId = e.Message.Data.ContainsKey("avatarId") == true ? e.Message.Data["avatarId"] : "undefined";
            notificationVO.avaterBgColorId = e.Message.Data.ContainsKey("avatarBgColorId") == true ? e.Message.Data["avatarBgColorId"] : "undefined";
            notificationVO.profilePicURL = e.Message.Data.ContainsKey("profilePicURL") == true ? e.Message.Data["profilePicURL"] : "undefined";
            notificationVO.isPremium = e.Message.Data.ContainsKey("isSubscriber") == true ? bool.Parse(e.Message.Data["isSubscriber"]) : false;
            notificationVO.timeSent = e.Message.Data.ContainsKey("creationTimestamp") == true ? long.Parse(e.Message.Data["creationTimestamp"]) : 0;
            notificationVO.actionCode = e.Message.Data.ContainsKey("actionCode") == true ? e.Message.Data["actionCode"] : "undefined";

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