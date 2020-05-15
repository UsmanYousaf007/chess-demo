using HUF.Notifications.Runtime.API;
using HUF.NotificationsFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.NotificationsFirebase.Runtime.API
{
    public static class HNotificationsFirebase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if (ShouldAutoInit())
            {
                Init();
            }
        }

        static bool ShouldAutoInit()
        {
            return HConfigs.HasConfig<FirebaseNotificationsConfig>() &&
                   HConfigs.GetConfig<FirebaseNotificationsConfig>().AutoInit;
        }

        /// <summary>
        /// Use this method to initialize Firebase Cloud Messaging (Firebase Notifications).
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            var firebaseNotifications = new FirebasePushNotificationsService();
            HNotifications.Push.RegisterService(firebaseNotifications);
            firebaseNotifications.InitializeNotifications();
        }
    }
}