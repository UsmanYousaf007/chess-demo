using System;
using Firebase.Messaging;
using UnityEngine.Events;

namespace HUF.Notifications.Runtime.API
{
    public interface IPushNotificationsService : IDisposable
    {
        void InitializeNotifications();
        bool IsInitialized { get; }
        event UnityAction<string> OnNotificationReceived;
        string CachedToken { get;  }
        FirebaseMessage CachedMessage { get; }
    }
}