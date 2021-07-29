#if UNITY_IOS

using System.Collections.Generic;
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Editor.BuildSupport;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;

namespace HUF.InitFirebase.Editor
{
    [UsedImplicitly]
#if UNITY_2018
    public class iOSFirebaseFrameworkManager : iOSProjectBaseFrameworkManager
#elif UNITY_2019_3_OR_NEWER
    public class iOSFirebaseFrameworkManager : iOSProjectBaseFixer
#endif
    {
        public override int callbackOrder => PostProcessBuildNumbers.PODFILE_GENERATION - 1;

#if UNITY_2018
        protected override IEnumerable<string> FrameworksToAdd { get; } = new[]
        {
            "GameKit.framework"
        };
#elif UNITY_2019_3_OR_NEWER
        protected override bool Process( PBXProject project )
        {
            project.AddFrameworkToProject( project.GetUnityFrameworkTargetGuid(), "GameKit.framework", false );
            return true;
        }
#endif
    }
}
#endif
