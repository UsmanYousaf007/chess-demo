#if UNITY_IOS
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;

namespace HUF.AdsAdMobMediation.Editor
{
    [UsedImplicitly]
    public class iOSAdMobProjectFixer : iOSProjectBaseFixer
    {
        public override int callbackOrder => 100;

        protected override bool Process(PBXProject project, string targetGuid, string projectPath)
        {
            project.AddBuildProperty(targetGuid, "CLANG_ENABLE_MODULES", "YES");
            return true;
        }
    }
}
#endif