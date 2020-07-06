#if UNITY_IOS
using System.Collections.Generic;
using HUF.Utils.BuildSupport.Editor.iOS;
using UnityEditor.iOS.Xcode;

namespace HUF.AuthSIWA.Editor.iOS
{
    public class iOSProjectSIWAFrameworkManager : iOSProjectBaseFrameworkManager
    {
        const string ENTITLEMENT_FILE = "Unity-IPhone/ios_sign_in.entitlements";
        public override int callbackOrder => 100;
        protected override IEnumerable<string> FrameworksToAdd { get; } = new[]
        {
            "AuthenticationServices.framework"
        };
        protected override bool Process(PBXProject project, string targetGuid, string projectPath)
        {
#if UNITY_2019_2_OR_NEWER || UNITY_2018_4
            project.AddCapability(targetGuid, PBXCapabilityType.SignInWithApple, ENTITLEMENT_FILE);
            var pbxCapabilityManager = new ProjectCapabilityManager(projectPath, ENTITLEMENT_FILE, "Unity-iPhone");
            pbxCapabilityManager.AddSignInWithApple();
            pbxCapabilityManager.WriteToFile();
            return true;
#endif
            return false;
        }
    }
}
#endif
