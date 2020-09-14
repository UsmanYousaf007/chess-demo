#if UNITY_ANDROID
using System;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using Unity.Notifications.Android;
using UnityEngine;

namespace HUF.NotificationsUnity.Runtime.Implementation.Android
{
    public class UnityAndroidNotificationsService : ILocalNotificationsService
    {
        const string DEFAULT_NOTIFICATION_CHANNEL = "default_notif_channel_id";
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(UnityAndroidNotificationsService) );

        public UnityAndroidNotificationsService()
        {
            PrepareNotificationsChannel();
        }

        void PrepareNotificationsChannel()
        {
            var channelName = UnityNotificationsConfig.DEFAULT_CHANNEL_NAME;
            var channelDesc = UnityNotificationsConfig.DEFAULT_CHANNEL_DESC;
            var channelImportance = Importance.Default;
            var config = HConfigs.GetConfig<UnityNotificationsConfig>();
            if ( config == null )
            {
                HLog.LogWarning(logPrefix, "Missing UnityNotificationsConfig. The notifications channel will be created with default channel name and description.");
            }
            else
            {
                channelDesc = config.ChannelDescription;
                channelName = config.ChannelName;
                channelImportance = config.ChannelImportance;
            }

            CreateNotificationsChannel(DEFAULT_NOTIFICATION_CHANNEL, channelName, channelDesc, channelImportance);
        }

        void CreateNotificationsChannel( string id, string name, string description, Importance importance )
        {
            try
            {
                var notificationChannel = new AndroidNotificationChannel()
                {
                    Id = id,
                    Name = name,
                    Importance = importance,
                    Description = description
                };
                AndroidNotificationCenter.RegisterNotificationChannel( notificationChannel );
            }
            catch (Exception ex)
            {
                HLog.LogError( logPrefix, $"Exception caught during init Unity Notifications: {ex.Message}" );
            }
        }

        public string ScheduleNotification( NotificationData notificationData )
        {
            if ( !notificationData.IsNotificationValid() )
                return NotificationData.INVALID_NOTIFICATION_ID;
            var notification = notificationData.CreateAndroidNotification();
            return AndroidNotificationCenter.SendNotification( notification, DEFAULT_NOTIFICATION_CHANNEL ).ToString();
        }

        public void ClearAllNotifications()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public string GetLastIntentData()
        {
            return AndroidNotificationCenter.GetLastNotificationIntent()?.Notification.IntentData;
        }

        public ConsentStatus GetConsentStatus()
        {
            HLog.LogWarning( logPrefix, "Not supported on Android platform!" );
            return ConsentStatus.Undefined;
        }

        public void ClearScheduledNotification( string notificationId )
        {
            AndroidNotificationCenter.CancelNotification( int.Parse( notificationId ) );
        }

        public void Dispose()
        {
            AndroidNotificationCenter.DeleteNotificationChannel( DEFAULT_NOTIFICATION_CHANNEL );
        }
    }
}
#endif