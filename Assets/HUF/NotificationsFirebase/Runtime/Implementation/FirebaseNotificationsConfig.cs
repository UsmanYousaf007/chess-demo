using HUF.NotificationsFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.NotificationsFirebase.Runtime.Implementation
{
    [CreateAssetMenu( menuName = "HUF/Notifications/FirebaseNotificationsConfig",
        fileName = "FirebaseNotificationsConfig" )]
    public class FirebaseNotificationsConfig : FeatureConfigBase
    {
        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Notifications - Firebase", HNotificationsFirebase.Init );
        }
    }
}