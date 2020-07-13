using HUF.AuthSIWA.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.AuthSIWA.Runtime.Config
{
    [CreateAssetMenu( fileName = nameof(AuthSIWAConfig), menuName = "HUF/Auth/" + nameof(AuthSIWAConfig) )]
    public class AuthSIWAConfig : FeatureConfigBase
    {
        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Auth. - SIWA", HAuthSIWA.Init );
        }
    }
}