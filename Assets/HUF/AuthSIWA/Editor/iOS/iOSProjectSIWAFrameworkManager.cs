#if UNITY_IOS
using System.Collections.Generic;
using AppleAuth.Editor;
using HUF.Utils.BuildSupport.Editor.iOS;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace HUF.AuthSIWA.Editor.iOS
{
    public static class iOSProjectSIWAFrameworkManager
    {
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            var projectPath = PBXProject.GetPBXProjectPath(path);
#if UNITY_2019_3_OR_NEWER
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
            manager.WriteToFile();
#elif UNITY_2019_2_OR_NEWER || UNITY_2018_4
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
            manager.AddSignInWithAppleWithCompatibility();
            manager.WriteToFile();
#endif
        }
    }
}
#endif
