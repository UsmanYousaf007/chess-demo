using HUF.AuthSIWA.Runtime.Config;
using HUF.Utils.Configs.API.Editor;

namespace HUF.AuthSIWA.Editor
{
    public class SIWAAuthConfigInstaller : BaseConfigInstaller
    {
        const string SIWA_AUTH = "SIWA Auth";

        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<AuthSIWAConfig>(
                SIWA_AUTH,
                $"{nameof(SIWAAuthConfigInstaller)}.cs",
                imported);
        }
    }
}