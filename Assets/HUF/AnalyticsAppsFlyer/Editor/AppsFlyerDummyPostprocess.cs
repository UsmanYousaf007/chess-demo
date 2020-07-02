using System.Collections.Generic;
using HUF.Utils.Editor.BuildSupport;
using HUF.Utils.Runtime.Logging;
using UnityEditor;

public class AppsFlyerDummyPostprocess : BaseDummyPreprocessBuild
{
#if HUF_ANALYTICS_APPSFLYER_DUMMY
    public override bool Enabled => true;
#else
    public override bool Enabled => false;
#endif
    public override IEnumerable<string> DirectoriesToHide => new[]
    {
        "HUF/AnalyticsAppsFlyer/Plugins",
        "HUF/AnalyticsAppsFlyer/Runtime/Implementation"
    };
    public override HLogPrefix LogPrefix => new HLogPrefix(nameof(AppsFlyerDummyPostprocess));

    [MenuItem( "HUF/Dummy/" + nameof(AppsFlyerDummyPostprocess) + "/Force Hide" )]
    static void ForceHideFolders()
    {
        var dummy = new AppsFlyerDummyPostprocess();
        dummy.HideFolders( true );
    }

    [MenuItem( "HUF/Dummy/" + nameof(AppsFlyerDummyPostprocess) + "/Force Show" )]
    static void ForceShow()
    {
        var dummy = new AppsFlyerDummyPostprocess();
        dummy.HideFolders( false );
    }
}
