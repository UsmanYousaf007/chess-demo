#if UNITY_IOS
using System.Collections.Generic;
using AppleAuth.Editor;
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Runtime.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace HUF.AuthSIWA.Editor.iOS
{
    public static class iOSProjectSIWAFrameworkManager
    {
        const string ENTITLEMENT_FILE = "Unity-iPhone/ios.entitlements";
        const string TARGET_NAME = "Unity-iPhone";
        const string CONFIG_NAME = "Release";
        const string ENTITLEMENTS_PROPERTY = "CODE_SIGN_ENTITLEMENTS";

        [PostProcessBuild( 1 )]
        public static void OnPostProcessBuild( BuildTarget target, string path )
        {
            var projectPath = PBXProject.GetPBXProjectPath( path );
#if UNITY_2019_3_OR_NEWER
            var project = new PBXProject();
            project.ReadFromString( System.IO.File.ReadAllText( projectPath ) );
            
            var targetGuid = project.GetUnityMainTargetGuid();
            var configGuid = project.BuildConfigByName( targetGuid, CONFIG_NAME );
            
            var relativeEntitlementPath = project.GetBuildPropertyForConfig( configGuid, ENTITLEMENTS_PROPERTY );
            var entitlementFile = relativeEntitlementPath.IsNullOrEmpty() ? ENTITLEMENT_FILE : relativeEntitlementPath;
            
            var manager = new ProjectCapabilityManager( projectPath, entitlementFile, TARGET_NAME );
            manager.AddSignInWithAppleWithCompatibility( project.GetUnityFrameworkTargetGuid() );
            manager.WriteToFile();
            project.SetBuildProperty( targetGuid, ENTITLEMENTS_PROPERTY, entitlementFile );
#elif UNITY_2019_2_OR_NEWER || UNITY_2018_4
            var manager = new ProjectCapabilityManager(projectPath, ENTITLEMENT_FILE, TARGET_NAME);
            manager.AddSignInWithAppleWithCompatibility();
            manager.WriteToFile();
#endif
        }
    }
}
#endif