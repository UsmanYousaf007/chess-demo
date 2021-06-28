using HUF.Auth.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.CrossPromo.Runtime.API;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "HUFEXT/Instant Framework/Game Scene Loader")]
public class GameSceneLoader : FeatureConfigBase
{
    void LoadScene()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

        HAuth.OnSignInResult += (serviceName, result) =>
        {
            HLog.Log($"HAuth.OnSignInResult: serviceName: {serviceName} result: {result}");
            if (serviceName == AuthServiceName.FIREBASE && result == AuthSignInResult.Success)
            {
                HCrossPromo.Init();
                HCrossPromo.Fetch();
            }
        };

        HAuth.SignIn(AuthServiceName.FIREBASE);
    }

    public override void RegisterManualInitializers()
    {
        AddManualInitializer("Load Game Scene", LoadScene);
    }
}
