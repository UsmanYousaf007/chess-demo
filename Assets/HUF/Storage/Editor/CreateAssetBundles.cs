using System.IO;
using System.Text;
using UnityEditor;

namespace HUF.Storage.Editor
{
    public static class CreateAssetBundles
    {
        [MenuItem("Assets/AssetBundle/Build For Android")]
        static void BuildAllAssetBundlesAndroid()
        {
            BuildAllAssetBundles(BuildTarget.Android, "android");
        }

        static void BuildAllAssetBundles(BuildTarget target, string folder = null)
        {
            var assetBundleDirectory = "Assets/AssetBundles";
            if (folder != null)
                assetBundleDirectory = new StringBuilder(assetBundleDirectory).Append($"/{folder}").ToString();
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, target);
        }

        [MenuItem("Assets/AssetBundle/Build For iOS")]
        static void BuildAllAssetBundlesIOS()
        {
            BuildAllAssetBundles(BuildTarget.iOS, "iOS");
        }
    }
}