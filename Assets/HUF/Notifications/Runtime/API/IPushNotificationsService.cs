using System;
using UnityEngine.Events;
using Firebase.Messaging;

namespace HUF.Notifications.Runtime.API
{
    public interface IPushNotificationsService : IDisposable
    {
        void InitializeNotifications();
        bool IsInitialized { get; }
        event UnityAction<string> OnNotificationReceived;
        string CachedToken { get; }
        FirebaseMessage CachedMessage { get; }
    }
}