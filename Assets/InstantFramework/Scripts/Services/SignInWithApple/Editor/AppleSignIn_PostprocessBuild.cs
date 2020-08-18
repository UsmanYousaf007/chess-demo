#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;

public class AppleSignIn_PostprocessBuild
	{
		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
		{
			if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)
			{
				string projPath = PBXProject.GetPBXProjectPath(path);

				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));

#if UNITY_2019_3_OR_NEWER
				string target = proj.GetUnityMainTargetGuid();
#else
				string targetName = PBXProject.GetUnityTargetName();
				string target = proj.TargetGuidByName(targetName);
#endif

			if (buildTarget != BuildTarget.iOS)
				return;

			ProjectCapabilityManager projCapability = new ProjectCapabilityManager(projPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
			ProjectCapabilityManagerExtension.AddSignInWithAppleWithCompatibility(projCapability, target);
			projCapability.WriteToFile();
		}
		}
}
#endif