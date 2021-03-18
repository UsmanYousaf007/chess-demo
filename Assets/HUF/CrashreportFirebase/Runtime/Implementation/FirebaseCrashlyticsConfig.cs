using HUF.CrashreportFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.CrashreportFirebase.Runtime.Implementation
{
    [CreateAssetMenu( menuName = "HUF/Crashreport/FirebaseCrashlyticsConfig",
        fileName = "FirebaseCrashlyticsConfig" )]
    public class FirebaseCrashlyticsConfig : FeatureConfigBase
    {
        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Crashreport - Crashlytics", HCrashreportFirebase.Init );
        }
    }
}