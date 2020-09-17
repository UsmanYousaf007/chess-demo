#if UNITY_ANDROID
using AndroidUnityNotificationsImportance = Unity.Notifications.Android.Importance;
#endif

namespace HUF.NotificationsUnity.Runtime.Implementation
{
        public enum NotificationImportance
        {
            None = 0,
            Low = 2,
            Default = 3,
            High = 4,
        }
#if UNITY_ANDROID
        public static class Extensions
        {
            public static AndroidUnityNotificationsImportance Convert(this NotificationImportance importance)
            {
                return (AndroidUnityNotificationsImportance)(int)importance;
            }
        }
#endif
}