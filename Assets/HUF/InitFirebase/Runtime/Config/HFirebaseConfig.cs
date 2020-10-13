using HUF.InitFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.InitFirebase.Runtime.Config
{
    [CreateAssetMenu(menuName = "HUF/Firebase/Firebase Wrapper Config", fileName = "FirebaseWrapperConfig")]
    public class HFirebaseConfig : FeatureConfigBase
    {
        public override void RegisterManualInitializers()
        {
            AddManualSynchronousInitializer( "Firebase", HInitFirebase.Init );
        }
    }
}