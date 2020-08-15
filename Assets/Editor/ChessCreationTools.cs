using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class ChessTools
{
    const string ASSET_PATH = "Assets/Game/Images/Resources/";
    const string SKINS_PATH = "Assets/Game/Images/Skins/";
    const string ATLAS_PATH = "Assets/Game/Images/Atlases/";

    [MenuItem("TLTools/Create/Skin Bundles", false, 13)]

    public static void CreateSkinBundles()
    {
        AssetBundleBuild[] bundleMap = CreateBundleMap();
        BuildAllAssetBundles(BuildTarget.iOS, "iOS", bundleMap);
        BuildAllAssetBundles(BuildTarget.iOS, "Android", bundleMap);
    }


    private static AssetBundleBuild[] CreateBundleMap()
    {
        string[] folders = Directory.GetDirectories(SKINS_PATH);
        AssetBundleBuild[] bundleMape = new AssetBundleBuild[folders.Length];
        for(int i=0;i<folders.Length;i++)
        {
            string skinName = new DirectoryInfo(folders[i]).Name;
            //string spriteAtlas = Directory.GetFiles(ATLAS_PATH, skinName + ".*").FirstOrDefault();
            string scriptableObj = Directory.GetFiles(ASSET_PATH, skinName + ".*").FirstOrDefault();

            bundleMape[i].assetBundleName = skinName;
            bundleMape[i].assetNames = new string[] {
              //  spriteAtlas,
                scriptableObj
            };
        }

        return bundleMape;
    }

    private static void BuildAllAssetBundles(BuildTarget target, string folder = null, AssetBundleBuild[] buildMap = null)
    {
        var assetBundleDirectory = "Assets/AssetBundles";
        if (folder != null)
            assetBundleDirectory = new StringBuilder(assetBundleDirectory).Append($"/{folder}").ToString();
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, target);
        //BuildPipeline.BuildAssetBundles(
        //    )
        
    }
}
