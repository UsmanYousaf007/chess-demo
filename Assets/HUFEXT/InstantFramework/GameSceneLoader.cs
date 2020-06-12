using HUF.Utils.Runtime.Configs.API;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "HUFEXT/Instant Framework/Game Scene Loader")]
public class GameSceneLoader : FeatureConfigBase
{
    void LoadScene()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
    }

    public override void RegisterManualInitializers()
    {
        AddManualInitializer("Load Game Scene", LoadScene);
    }
}
