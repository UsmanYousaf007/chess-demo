using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

public class PostProcessHyprMX : MonoBehaviour {
    const string hyprMXPodsDirectory = "Pods/HyprMX/";
    const string coreHyprMXFrameworkName = "HyprMX.xcframework";
    const string hyprPermissionsFrameworkName = "HYPRPermissions.xcframework";
    [PostProcessBuild(101)]
    private static void PostProcessBuild_HyprMX(BuildTarget target, string buildPath)
    {
#if UNITY_EDITOR_OSX
        if (target == BuildTarget.iOS) {
            
            string projPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
               
#if UNITY_2019_3_OR_NEWER
            // HyprMX is installed/linked by CocoaPods to the UnityFramework.framework, but the dynamic lib still
            // needs to be embedded into the application target.
            Debug.Log ("Embedding HyprMX frameworks into Unity-iPhone target...");
            string targetGuid = proj.GetUnityMainTargetGuid();
            EmbedHyprMXFramework(proj, projPath, targetGuid, coreHyprMXFrameworkName);
            EmbedHyprMXFramework(proj, projPath, targetGuid, hyprPermissionsFrameworkName);
#else
            // Unity projects before 2019.3 do not contain runtime search paths needed for dynamic libraries.
            Debug.Log ("Adding the runtime search path to Unity-iPhone target...");
            string targetGuid = proj.TargetGuidByName("Unity-iPhone");
            proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
#endif
            
            proj.WriteToFile (projPath);
        }
#endif
    }
    
    private static void EmbedHyprMXFramework(PBXProject proj, string projPath, string targetGuid, string framework)
    {
#if UNITY_EDITOR_OSX
        string hyprMXframeworkPath = hyprMXPodsDirectory + framework;
        string fileGuid = proj.AddFile(hyprMXframeworkPath, hyprMXframeworkPath, PBXSourceTree.Source);
        PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
#endif
    }
}