#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    public abstract class iOSPlistBaseFixer : IPostprocessBuildWithReport
    {
        const string PLIST_FILE = "/Info.plist";
        
        string plistPath;
        PlistDocument plistDocument;
        
        public abstract int callbackOrder { get; }

        public virtual void OnPostprocessBuild(BuildReport report)
        {
            plistPath = report.summary.outputPath + PLIST_FILE;
            plistDocument = new PlistDocument();
            plistDocument.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict rootDict = plistDocument.root;

            /*if (Process(rootDict, plistPath))
                SavePlist();*/
            Process( rootDict, plistPath );
            SavePlist();
        }

        protected abstract bool Process(PlistElementDict rootDict, string projectPath);

        void SavePlist()
        {
            File.WriteAllText(plistPath, plistDocument.WriteToString());
        }
    }
}
#endif
