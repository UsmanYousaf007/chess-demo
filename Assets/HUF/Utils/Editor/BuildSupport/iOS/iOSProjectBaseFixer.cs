#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    public abstract class iOSProjectBaseFixer : IPostprocessBuildWithReport
    {
        string projectPath;
        PBXProject project;
        
        public abstract int callbackOrder { get; }

        public virtual void OnPostprocessBuild(BuildReport report)
        {
            projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
#if UNITY_2019_3_OR_NEWER
            var targetGuid = project.GetUnityMainTargetGuid();
#else
            string targetName = PBXProject.GetUnityTargetName();
            var targetGuid = project.TargetGuidByName(targetName);
#endif
            if (Process(project, targetGuid, projectPath))
                SaveProject();
        }

        protected abstract bool Process(PBXProject project, string targetGuid, string projectPath);

        void SaveProject()
        {
            File.WriteAllText(projectPath, project.WriteToString());
        }
    }
}
#endif