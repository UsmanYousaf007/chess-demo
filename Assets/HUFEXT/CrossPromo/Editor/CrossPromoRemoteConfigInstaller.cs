using HUF.Utils.Editor.Configs;
using HUFEXT.CrossPromo.Runtime.Implementation;

namespace HUFEXT.CrossPromo.Editor
{
    public class CrossPromoRemoteConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<CrossPromoRemoteConfig>(
                "Cross Promo Remote",
                "CrossPromoRemoteConfigInstaller.cs",
                imported );
        }
    }
}