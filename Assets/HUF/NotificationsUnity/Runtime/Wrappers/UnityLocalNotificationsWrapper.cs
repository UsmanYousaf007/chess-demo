using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;

namespace HUF.NotificationsUnity.Runtime.Wrappers
{
    public class UnityLocalNotificationsWrapper : ILocalNotificationsService
    {
        readonly ILocalNotificationsService service;

        public UnityLocalNotificationsWrapper()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            service = new Implementation.Android.UnityAndroidNotificationsService();
#elif UNITY_IOS
            service = new Implementation.iOS.UnityiOSNotificationsService();
#else
            service = new Implementation.Dummy.DummyNotificationsService();
#endif
        }

        public void Dispose()
        {
            service.Dispose();
        }

        public string ScheduleNotification( NotificationData notificationData )
        {
            return service.ScheduleNotification( notificationData );
        }

        public void ClearScheduledNotification( string notificationId )
        {
            service.ClearScheduledNotification( notificationId );
        }

        public void ClearAllNotifications()
        {
            service.ClearAllNotifications();
        }

        public string GetLastIntentData()
        {
            return service.GetLastIntentData();
        }

        public ConsentStatus GetConsentStatus()
        {
            return service.GetConsentStatus();
        }
    }
}