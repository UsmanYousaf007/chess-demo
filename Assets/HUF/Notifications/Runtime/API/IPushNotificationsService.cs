using System;
using Firebase.Messaging;

namespace HUF.Notifications.Runtime.API
{
    public interface IPushNotificationsService : IDisposable
    {
        void InitializeNotifications();
        bool IsInitialized { get; }
        string CachedToken { get; }
        FirebaseMessage CachedMessage { get; }
        event Action<byte[]> OnNotificationReceived;
    }
}
