using HUF.Utils.Editor.Configs;
using HUFEXT.AdsManager.Runtime.Config;

namespace HUFEXT.AdsManager.Editor
{
    public class AdsManagerConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<AdsManagerConfig>(
                "Ads Manager",
                "AdsManagerConfigInstaller.cs",
                imported );
        }
    }
}