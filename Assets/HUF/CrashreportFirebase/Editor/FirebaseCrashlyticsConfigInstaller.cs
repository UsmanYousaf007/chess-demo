using HUF.CrashreportFirebase.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.CrashreportFirebase.Editor
{
    public class FirebaseCrashlyticsConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<FirebaseCrashlyticsConfig>(
                "Firebase Crashreport", 
                "FirebaseCrashlyticsConfigInstaller.cs", 
                imported);
        }
    }
}