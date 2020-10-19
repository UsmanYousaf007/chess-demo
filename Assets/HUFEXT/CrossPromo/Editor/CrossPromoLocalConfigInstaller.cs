using HUF.Utils.Editor.Configs;
using HUFEXT.CrossPromo.Runtime.Implementation;

namespace HUFEXT.CrossPromo.Editor
{
    public class CrossPromoLocalConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<CrossPromoLocalConfig>(
                "Cross Promo Local",
                "CrossPromoLocalConfigInstaller.cs",
                imported );
        }
    }
}