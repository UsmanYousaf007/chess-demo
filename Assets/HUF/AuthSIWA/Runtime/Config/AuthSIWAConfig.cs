using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.AuthSIWA.Runtime.Config
{
    [CreateAssetMenu(fileName = nameof(AuthSIWAConfig)+".asset", menuName = "HUF/Auth/"+nameof(AuthSIWAConfig))]
    public class AuthSIWAConfig : FeatureConfigBase {}
}