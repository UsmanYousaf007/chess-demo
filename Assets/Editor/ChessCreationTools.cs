using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class ChessTools
{
    const string ASSET_PATH = "Assets/AssetBundles/BundleCreationAssets/";
    const string SKINS_PATH = "Assets/Game/Images/Skins/";
    const string ATLAS_PATH = "Assets/Game/Images/Atlases/";

    [MenuItem("TLTools/Create/Skin Bundles", false, 13)]

    public static void CreateSkinBundles()
    {
        BuildTarget target = BuildTarget.iOS;
        AssetBundleBuild[] bundleMap = CreateBundleMap(target);
        BuildAllAssetBundles(target, target.ToString(), bundleMap);

        target = BuildTarget.Android;
        bundleMap = CreateBundleMap(target);
        BuildAllAssetBundles(target, target.ToString(), bundleMap);
    }


    private static AssetBundleBuild[] CreateBundleMap(BuildTarget target)
    {
        string[] folders = Directory.GetDirectories(SKINS_PATH);
        AssetBundleBuild[] bundleMaps = new AssetBundleBuild[folders.Length];
        for(int i=0;i<folders.Length;i++)
        {
            string skinName = new DirectoryInfo(folders[i]).Name;
            string scriptableObj = Directory.GetFiles(ASSET_PATH, skinName + ".*").FirstOrDefault();

            bundleMaps[i].assetBundleName = skinName + "_" + target.ToString();
            bundleMaps[i].assetNames = new string[] {
                scriptableObj
            };
        }

        return bundleMaps;
    }

    private static void BuildAllAssetBundles(BuildTarget target, string folder = null, AssetBundleBuild[] buildMap = null)
    {
        var assetBundleDirectory = "Assets/AssetBundles";
        if (folder != null)
            assetBundleDirectory = new StringBuilder(assetBundleDirectory).Append($"/{folder}").ToString();
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, target);
    }
}
