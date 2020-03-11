using System.Collections.Generic;
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;

namespace HUF.InitFirebase.Implementation.Editor
{
    [UsedImplicitly]
    public class iOSFirebaseFrameworkManager : iOSProjectBaseFrameworkManager
    {
        public override int callbackOrder => 0;
        protected override IEnumerable<string> FrameworksToAdd { get; } = new[]
        {
            "GameKit.framework"
        };
    }
}