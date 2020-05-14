using HUF.AdsAdMobMediation.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.AdsAdMobMediation.Editor
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