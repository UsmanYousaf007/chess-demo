using HUF.Utils.Configs.API.Editor;

namespace HUF.AnalyticsHBI.Implementation.Editor
{
    public class HBIAnalyticsConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<HBIAnalyticsConfig>("HBIAnalytics", "HBIAnalyticsConfigInstaller.cs", imported);
        }
    }
}