#if UNITY_IOS
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    [UsedImplicitly]
    public class DisablingBitcodeiOSFixer : iOSProjectBaseFixer
    {
        public override int callbackOrder => 10;

        protected override bool Process(PBXProject project, string targetGuid, string projectPath)
        {
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            return true;
        }
    }
}
#endif