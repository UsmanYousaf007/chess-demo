using HUF.Notifications.Runtime.API;
using HUF.NotificationsUnity.Runtime.Implementation;
using HUF.NotificationsUnity.Runtime.Wrappers;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.NotificationsUnity.Runtime.API
{
    public static class HNotificationsUnity
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HNotificationsUnity) );

        /// <summary>
        /// Returns whether Unity Local Notifications is initialized.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized { private set; get; }

        /// <summary>
        /// Initializes Unity Local Notifications.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if ( IsInitialized )
                return;

            var notifications = new UnityLocalNotificationsWrapper();
            HNotifications.Local.RegisterService( notifications );
            IsInitialized = true;
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( HConfigs.HasConfig<UnityNotificationsConfig>() &&
                 HConfigs.GetConfig<UnityNotificationsConfig>().AutoInit )
                Init();
        }
    }
}