#if UNITY_IOS
using System;
using System.Collections;
using System.Linq;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.NativeFunctions;
using Unity.Notifications.iOS;
using UnityEngine;

namespace HUF.NotificationsUnity.Runtime.Implementation.iOS
{
    public class UnityiOSNotificationsService : ILocalNotificationsService
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(UnityiOSNotificationsService) );

        public event Action<ConsentStatus> OnAskForPermissionComplete;

        public string ScheduleNotification( NotificationData notificationData )
        {
            if ( !notificationData.IsNotificationValid() )
                return NotificationData.INVALID_NOTIFICATION_ID;

            var notification = notificationData.CreateIOsNotification();
            iOSNotificationCenter.ScheduleNotification( notification );
            return notification.Identifier;
        }

        public void ClearScheduledNotification( string notificationId )
        {
            if ( iOSNotificationCenter.GetDeliveredNotifications().Any( x => x.Identifier == notificationId ) )
                iOSNotificationCenter.RemoveDeliveredNotification( notificationId );
            else
                iOSNotificationCenter.RemoveScheduledNotification( notificationId );
        }

        public void ClearAllNotifications()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }

        public string GetLastIntentData()
        {
            return iOSNotificationCenter.GetLastRespondedNotification()?.Data;
        }

        public ConsentStatus GetConsentStatus()
        {
            var settings = iOSNotificationCenter.GetNotificationSettings();
            var consentStatus = ConvertConsentStatus( settings.AuthorizationStatus );
            return consentStatus;
        }

        public void Dispose()
        {
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }

        ConsentStatus ConvertConsentStatus( AuthorizationStatus status )
        {
            switch ( status )
            {
                case AuthorizationStatus.Authorized:
                    return ConsentStatus.Granted;
                case AuthorizationStatus.Denied:
                    return ConsentStatus.Denied;
                default:
                    return ConsentStatus.Undefined;
            }
        }

        public void AskForPermission( bool registerForRemoteNotifications )
        {
            CoroutineManager.StartCoroutine(
                AskForPermissionEnumerator( registerForRemoteNotifications ) );
        }

        void HandleApplicationFocusChange( bool hasFocus )
        {
            if ( !hasFocus )
                return;

            PauseManager.Instance.OnApplicationFocusChange -= HandleApplicationFocusChange;

            //if consent is changed to Granted in runtime ConsentStatus won't change without calling AuthorizationRequest
            CoroutineManager.StartCoroutine(
                AskForPermissionEnumerator( true, true ) );
        }

        IEnumerator AskForPermissionEnumerator( bool registerForRemoteNotifications,
            bool forceAuthorizationRequest = false )
        {
            if ( GetConsentStatus() == ConsentStatus.Denied && !forceAuthorizationRequest )
            {
                PauseManager.Instance.OnApplicationFocusChange += HandleApplicationFocusChange;
                HNativeFunctions.OpenAppSettings();
                yield break;
            }

            using ( var req = new AuthorizationRequest( AuthorizationOption.Alert | AuthorizationOption.Badge,
                registerForRemoteNotifications ) )
            {
                while ( !req.IsFinished )
                {
                    yield return null;
                }

                string res = "\n RequestAuthorization: \n";
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                HLog.Log( logPrefix, res );

                if ( req.Granted )
                {
                    OnAskForPermissionComplete.Dispatch( ConsentStatus.Granted );
                }
                else
                {
                    OnAskForPermissionComplete.Dispatch( ConsentStatus.Denied );
                }
            }
        }
    }
}
#endif