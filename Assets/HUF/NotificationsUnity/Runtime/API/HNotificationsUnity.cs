using HUF.Notifications.Runtime.API;
using HUF.NotificationsUnity.Runtime.Implementation;
using HUF.NotificationsUnity.Runtime.Wrappers;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.NotificationsUnity.Runtime.API
{
    public static class HNotificationsUnity
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if (ShouldAutoInit())
                Init();
        }

        /// <summary>
        /// Initializes Unity Local Notifications service.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            var notifications = new UnityLocalNotificationsWrapper();
            HNotifications.Local.RegisterService(notifications);
        }

        static bool ShouldAutoInit()
        {
            return HConfigs.HasConfig<UnityNotificationsConfig>() &&
                   HConfigs.GetConfig<UnityNotificationsConfig>().AutoInit;
        }
    }
}