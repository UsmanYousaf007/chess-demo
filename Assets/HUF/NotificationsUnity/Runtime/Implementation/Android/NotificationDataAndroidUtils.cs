#if UNITY_ANDROID || UNITY_EDITOR
using HUF.Notifications.Runtime.Data.Structs;
using Unity.Notifications.Android;

namespace HUF.NotificationsUnity.Runtime.Implementation.Android
{
    public static class NotificationDataAndroidUtils
    {
        public static AndroidNotification CreateAndroidNotification(this NotificationData notificationData)
        {
            return new AndroidNotification
            {
                Title = notificationData.title, 
                Text = notificationData.text, 
                Group = notificationData.categoryIdentifier,
                SmallIcon = notificationData.smallIcon,
                LargeIcon = notificationData.largeIcon, 
                FireTime = notificationData.FireTime,
                IntentData = notificationData.intentData ?? string.Empty
            };
        }
    }
}
#endif