using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HUF.InitFirebase.Editor
{
    public class FirebaseXMLFixer : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; } = 1;

        public void OnPreprocessBuild( BuildReport report )
        {
            AssetDatabase.ImportAsset( "Assets/google-services.json" );
        }
    }
}
