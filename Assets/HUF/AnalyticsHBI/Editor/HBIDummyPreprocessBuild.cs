using System.Collections.Generic;
using HUF.Utils.Editor.BuildSupport;
using HUF.Utils.Runtime.Logging;

namespace HUF.AnalyticsHBI.Editor
{
    public class HBIDummyPreprocessBuild : BaseDummyPreprocessBuild
    {
#if HUF_ANALYTICS_FIREBASE_DUMMY
        public override bool Enabled => true;
#else
        public override bool Enabled => false;
#endif
        public override IEnumerable<string> DirectoriesToHide { get; } = new[]
        {
            "HUF/AnalyticsHBI/Plugins",
            "HUF/AnalyticsHBI/Runtime/Implementation"
        };
        public override HLogPrefix LogPrefix { get; } = new HLogPrefix(nameof(HBIDummyPreprocessBuild));
    }
}