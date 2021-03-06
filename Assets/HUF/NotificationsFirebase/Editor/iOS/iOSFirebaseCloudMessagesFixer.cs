#if UNITY_IOS
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;

namespace HUF.NotificationsFirebase.Editor.iOS
{
    [UsedImplicitly]
    public class iOSFirebaseCloudMessagesFixer : iOSProjectBaseFixer
    {
        const string ENTITLEMENT_FILE = "Unity-IPhone/ios.entitlements";
        public override int callbackOrder => 0;

        protected override bool Process( PBXProject project )
        {
#if UNITY_2019_3_OR_NEWER
            string targetGuid = project.GetUnityMainTargetGuid();
#else
            string targetGuid = project.TargetGuidByName( PBXProject.GetUnityTargetName() );
#endif
            project.AddCapability( targetGuid, PBXCapabilityType.PushNotifications, ENTITLEMENT_FILE );

            var pbxCapabilityManager =
                new ProjectCapabilityManager( projectPath, ENTITLEMENT_FILE, "Unity-iPhone" );
            
            pbxCapabilityManager.AddPushNotifications( true );
            pbxCapabilityManager.AddBackgroundModes( BackgroundModesOptions.RemoteNotifications );
            pbxCapabilityManager.WriteToFile();
            return true;
        }
    }
}
#endif
