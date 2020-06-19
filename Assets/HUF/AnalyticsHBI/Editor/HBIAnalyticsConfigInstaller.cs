using HUF.AnalyticsHBI.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.AnalyticsHBI.Editor
{
    public class HBIAnalyticsConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<HBIAnalyticsConfig>("HBIAnalytics", "HBIAnalyticsConfigInstaller.cs", imported);
        }
    }
}