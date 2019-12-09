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

        static bool IsCorrectPlatform(BuildReport report)
        {
            return report.summary.platform == BuildTarget.iOS;
        }

        public virtual void OnPostprocessBuild(BuildReport report)
        {
            if (!IsCorrectPlatform(report)) 
                return;

            projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
            project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            var targetName = PBXProject.GetUnityTargetName();
            var targetGuid = project.TargetGuidByName(targetName);

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