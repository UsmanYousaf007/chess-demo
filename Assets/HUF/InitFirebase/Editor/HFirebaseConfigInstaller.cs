using HUF.InitFirebase.Runtime.Config;
using HUF.Utils.Editor.Configs;

namespace HUF.InitFirebase.Editor
{
    public class HFirebaseConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<HFirebaseConfig>(
                "Firebase initialization",
                nameof(HFirebaseConfigInstaller)+".cs",
                imported );
        }
    }
}