using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace HUF.Storage.Editor
{
    public static class CreateAssetBundles
    {

        static void BuildAllAssetBundles(BuildTarget target, string folder = null, AssetBundleBuild[] buildMap = null)
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

        [MenuItem("Assets/AssetBundle/Build For Android")]
        static void BuildAllAssetBundlesAndroid()
        {
            BuildAllAssetBundles(BuildTarget.Android, "android");
        }

        [MenuItem("Assets/AssetBundle/Build For iOS")]
        static void BuildAllAssetBundlesIOS()
        {
            BuildAllAssetBundles(BuildTarget.iOS, "iOS");
        }

        static void CreateBundleBuildMap()
        {
            AssetBundleBuild[] buildMap = LoadBundleFromXML();
            BuildAllAssetBundles(BuildTarget.iOS, "iOS", buildMap);
            BuildAllAssetBundles(BuildTarget.Android, "Android", buildMap);
        }

        private static AssetBundleBuild[] LoadBundleFromXML()
        {
            AssetBundle b;
            
            AssetBundleBuild[] buildMap = null;
            int bundleIndex = 0;
            int assetIndex = 0;
            XDocument xdocument = XDocument.Load("Assets/Resources/Bundles.xml");
            IEnumerable<XElement> bundles = xdocument.Root.Elements();

            if (bundles != null && bundles.Count() > 0)
            {
                buildMap = new AssetBundleBuild[bundles.Count()];
                foreach (var bundle in bundles)
                {
                    buildMap[bundleIndex].assetBundleName = bundle.Attribute("Name").Value;
                    
                    var xmlAssets = bundle.Descendants("Asset");
                    if (xmlAssets != null && xmlAssets.Count() > 0)
                    {
                        assetIndex = 0;
                        string[] assets = new string[xmlAssets.Count()];
                        
                        foreach (var xmlAsset in xmlAssets)
                        {
                            assets[assetIndex] = xmlAsset.Value;
                            assetIndex++;
                        }
                        buildMap[bundleIndex].assetNames = assets;
                    }
                    bundleIndex++;
                }
            }

            return buildMap;
        }
    }
}