using HUF.Utils.Editor.Configs;
using HUFEXT.GenericGDPR.Runtime.API;

namespace HUFEXT.GenericGDPR.Editor
{
    public class GDPRConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<GDPRConfig>(
                "Generic GDPR",
                "GDPRConfigInstaller.cs",
                imported );
        }
    }
}