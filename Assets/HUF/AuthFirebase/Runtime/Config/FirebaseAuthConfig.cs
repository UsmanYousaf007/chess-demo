using HUF.AuthFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.AuthFirebase.Runtime.Config
{
    [CreateAssetMenu( fileName = nameof(FirebaseAuthConfig), menuName = "HUF/Auth/Firebase Config" )]
    public class FirebaseAuthConfig : FeatureConfigBase
    {
        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Auth. - Firebase", HAuthFirebase.Init );
        }
    }
}