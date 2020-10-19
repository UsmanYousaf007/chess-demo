using HUF.AdsIronSourceMediation.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.AdsIronSourceMediation.Editor
{
    public class IronSourceConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(
            string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromAssetPaths )
        {
            PostprocessImport<IronSourceAdsProviderConfig>(
                "IronSource Ads",
                "IronSourceConfigInstaller.cs",
                imported );
        }
    }
}