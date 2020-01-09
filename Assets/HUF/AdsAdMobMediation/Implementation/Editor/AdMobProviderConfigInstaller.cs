using HUF.Utils.Configs.API.Editor;

namespace HUF.AdsAdMobMediation.Implementation.Editor
{
    public class AdMobProviderConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<AdMobProviderConfig>(
                "AdMob Mediation", 
                "AdMobProviderConfigInstaller.cs", 
                imported);
        }
    }
}