using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    const string PREFAB_PATH = "Assets/InstantFramework/Prefabs/";

    [MenuItem("TLTools/Create/Skin Bundles", false, 13)]

    public static void CreateSkinBundles()
    {
        
        BuildTarget target = BuildTarget.Android;
        AssetBundleBuild[] testbundlemap = CreateTestBundle();
        BuildTestAssetBundle(target, target.ToString(), testbundlemap);

        //BuildTarget target = BuildTarget.iOS;
        //AssetBundleBuild[] bundleMap = CreateBundleMap(target);
        //BuildAllAssetBundles(target, target.ToString(), bundleMap);

        //target = BuildTarget.Android;
        //bundleMap = CreateBundleMap(target);
        //BuildAllAssetBundles(target, target.ToString(), bundleMap);
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

    private static AssetBundleBuild[] CreateTestBundle()
    {
        const string DLL_PATH = "Assets/";
        string[] folders = Directory.GetDirectories(SKINS_PATH);
        AssetBundleBuild[] bundleMap = new AssetBundleBuild[1];

        var prefab = Directory.GetFiles(PREFAB_PATH, "TestExternalGameObj.prefab").FirstOrDefault();
        var scriptdll = Directory.GetFiles(DLL_PATH, "ClassLibrary.bytes").FirstOrDefault();
        bundleMap[0].assetBundleName = "DllBundle";
            bundleMap[0].assetNames = new string[] {
                scriptdll,
                prefab
            };

        return bundleMap;
    }

    private static void BuildTestAssetBundle(BuildTarget target, string folder = null, AssetBundleBuild[] buildMap = null)
    {
        var assetBundleDirectory = "Assets/AssetBundles";
        if (folder != null)
            assetBundleDirectory = new StringBuilder(assetBundleDirectory).Append($"/{folder}").ToString();
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, target);
    }
}
