using HUF.RemoteConfigsFirebase.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.RemoteConfigsFirebase.Editor
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