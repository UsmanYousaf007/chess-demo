#if !HUF_ANALYTICS_APPSFLYER_DUMMY
using HUF.AnalyticsAppsFlyer.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.AnalyticsAppsFlyer.Editor
{
    public class AppsFlyerAnalyticsConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<AppsFlyerAnalyticsConfig>(
                "AppsFlyer Analytics",
                "AppsFlyerAnalyticsConfigInstaller.cs",
                imported);
        }
    }
}
#endif