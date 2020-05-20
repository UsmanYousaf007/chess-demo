#if UNITY_IOS
using System.Collections.Generic;
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;

namespace HUF.AnalyticsHBI.Implementation.Editor.iOS
{
    [UsedImplicitly]
    public class iOSProjectHBIFrameworkManager : iOSProjectBaseFrameworkManager
    {
        public override int callbackOrder => 0;
        protected override IEnumerable<string> FrameworksToAdd { get; } = new[]
        {
            "libsqlite3.0.dylib"
        };
    }
}
#endif