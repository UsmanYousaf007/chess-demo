#if UNITY_IOS
using System.Collections.Generic;
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;

namespace HUF.AnalyticsAppsFlyer.Implementation.Editor.iOS
{
    [UsedImplicitly]
    public class iOSProjectAppsflyerFrameworkManager : iOSProjectBaseFrameworkManager
    {
        public override int callbackOrder => 0;
        protected override IEnumerable<string> FrameworksToAdd { get; } = new[]
        {
            "AdSupport.framework",
            "iAd.framework"
        };
    }
}
#endif