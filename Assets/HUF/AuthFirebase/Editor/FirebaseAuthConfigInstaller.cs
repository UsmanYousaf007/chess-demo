using HUF.AuthFirebase.Runtime.Config;
using HUF.Utils.Editor.Configs;

namespace HUF.AuthFirebase.Editor
{
    public class FirebaseAuthConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<FirebaseAuthConfig>(
                "Firebase Authentication",
                $"{nameof(FirebaseAuthConfigInstaller)}.cs",
                imported);
        }
    }
}