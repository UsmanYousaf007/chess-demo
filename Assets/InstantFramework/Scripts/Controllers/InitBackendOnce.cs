/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor + Faraz <noor@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-07 17:57:19 UTC+05:00
/// 
/// @description
/// This is the entry point to the game.

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using Facebook.Unity;
using GameSparks.Core;
using GameSparks.Api.Messages;

namespace TurboLabz.InstantFramework
{
    public class InitBackendOnce : Command
    {
        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IPushNotificationService pushNotificationService { get; set; }
        [Inject] public IAutoSubscriptionDailogueService autoSubscriptionDailogueService { get; set; }

        //Signals
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

        public override void Execute()
        {
            backendService.MonitorConnectivity(true);
            backendService.AddMessageListeners();
            backendService.AddChallengeListeners();
            backendService.StartPinger();
            backendService.OnlineCheckerStart();

            if (autoSubscriptionDailogueService.CanShow())
            {
                subscriptionDlgClosedSignal.AddOnce(RegisterNotification);
            }
            else
            {
                RegisterNotification();
            }
        }

        private void RegisterNotification()
        {
            pushNotificationService.Init();
#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.None);
#endif
            pushNotificationService.ClearNotifications();
        }
    }
}