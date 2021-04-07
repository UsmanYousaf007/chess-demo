#if HUFEXT_PACKAGE_MANAGER
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public class FirebaseVersionConsolidator : IPreprocessBuildWithReport
{
    static readonly string PACKAGE_PREFIX = "com.google.firebase";
    static readonly string CORE_PACKAGE = "com.google.firebase.app";

    public int callbackOrder => int.MinValue;

    public virtual void OnPreprocessBuild(BuildReport report)
    {
        Execute(true);
    }

    [MenuItem("HUF/Consolidate Firebase SDKs versions")]
    static void MenuItem()
    {
        Execute(false);
    }

    static void Execute(bool check)
    {
#if UNITY_2019_1_OR_NEWER
        var listRequest = Client.List(false, true);
#else
        var listRequest = Client.List(false);
#endif
        EditorApplication.update += ListResponse;

        void ListResponse()
        {
            if (!listRequest.IsCompleted || listRequest.Status != StatusCode.Success)
                return;

            EditorApplication.update -= ListResponse;
            var corePackage = listRequest.Result.ToDictionary(info => info.name)[CORE_PACKAGE];
            if (corePackage == null)
                return;

            var packagesToUpdateText = string.Empty;
            var packagesToUpdate = new Queue<string>();
            foreach (var package in listRequest.Result)
                if (package.name.Contains(PACKAGE_PREFIX) && package.version != corePackage.version)
                {
                    packagesToUpdate.Enqueue($"{package.name}@{corePackage.version}");
                    packagesToUpdateText += $"{package.packageId}\n";
                }

            if (packagesToUpdate.Count == 0)
                return;

            if (check)
            {
                EditorUtility.DisplayDialog("IMPORTANT",
                    $"Firebase SDK packages not matching version {corePackage.version} were found:\n\n{packagesToUpdateText}\n" +
                    "Please use \"HUF\" -> \"Consolidate Firebase SDKs versions\" menu in order to unify their versions.",
                    "OK");
                return;
            }

            AddRequest addRequest = Client.Add(packagesToUpdate.Dequeue());
            EditorApplication.update += AddResponse;

            void AddResponse()
            {
                if (!addRequest.IsCompleted || addRequest.Status != StatusCode.Success)
                    return;

                if (packagesToUpdate.Count > 0)
                    addRequest = Client.Add(packagesToUpdate.Dequeue());
                else
                    EditorApplication.update -= AddResponse;
            }
        }
    }
}
#endif