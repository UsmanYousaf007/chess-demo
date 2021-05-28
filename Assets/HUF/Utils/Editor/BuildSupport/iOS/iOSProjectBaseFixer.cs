#if UNITY_IOS
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    public abstract class iOSProjectBaseFixer : IPostprocessBuildWithReport
    {
        protected string projectPath;
        PBXProject project;

        public abstract int callbackOrder { get; }

        public virtual void OnPostprocessBuild(BuildReport report)
        {
            projectPath = PBXProject.GetPBXProjectPath( report.summary.outputPath );
            project = new PBXProject();
            project.ReadFromString( File.ReadAllText( projectPath ) );

            if ( Process( project ) )
                SaveProject();
        }

        protected abstract bool Process( PBXProject project );

        void SaveProject()
        {
            File.WriteAllText( projectPath, project.WriteToString() );
        }
    }
}
#endif
