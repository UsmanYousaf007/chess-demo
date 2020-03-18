using HUF.Utils.Configs.API.Editor;

namespace HUF.RemoteConfigsFirebase.Implementation.Editor
{
    public class FirebaseRemoteConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            PostprocessImport<FirebaseRemoteConfigsConfig>("FirebaseRemoteConfig", "FirebaseRemoteConfigInstaller",
                importedAssets);
        }
    }
}