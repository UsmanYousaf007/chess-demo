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
            // Clear all notifications from device
        }

        public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            var notification = e.Message.Notification;
            if (notification != null)
            {
                NotificationVO notificationVO;
                notificationVO.title = notification.Title;
                notificationVO.body = notification.Body;
                notificationVO.senderPlayerId = e.Message.Data["senderPlayerId"];
                notificationVO.matchGroup = e.Message.Data["matchGroup"];

                notificationRecievedSignal.Dispatch(notificationVO);
            }

        }

    }
}