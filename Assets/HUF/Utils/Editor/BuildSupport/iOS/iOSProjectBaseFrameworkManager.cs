using System.Collections.Generic;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    public abstract class iOSProjectBaseFrameworkManager : iOSProjectBaseFixer
    {
        protected abstract IEnumerable<string> FrameworksToAdd { get; }

        protected override bool Process(PBXProject project, string targetGuid, string projectPath)
        {
            foreach (var framework in FrameworksToAdd)
            {
                TryAddFramework(project, targetGuid, framework);
            }

            return true;
        }

        static void TryAddFramework(PBXProject project, string targetGuid, string framework)
        {
            if(!project.ContainsFramework(targetGuid, framework))
                project.AddFrameworkToProject(targetGuid, framework, false);
        }
    }
}