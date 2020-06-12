using System.Collections.Generic;
using System.IO;
using HUF.Utils.Runtime;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor
{
    public static class AssetsUtils
    {
        public const string ASSETS_ROOT = "Assets";
        public const string RESOURCES_FOLDER = "Resources";
        
        public static List<T> GetAtPath<T> (string path) where T : Object
        {
            var assetsList = new List<T>();
            string fullPath = PathUtils.GetFullPath(path);
            var fileEntries = Directory.GetFiles(fullPath);
            
            foreach(string fileName in fileEntries)
            {
                string localPath = PathUtils.GetLocalPath(fileName);
                var asset = AssetDatabase.LoadAssetAtPath<T>(localPath);

                if(asset != null)
                    assetsList.Add(asset);
            }
            return assetsList;
        }

        public static List<T> GetByFilter<T>(string filter) where T : Object
        {
            var configs = new List<T>();
            var configGUIDs = AssetDatabase.FindAssets(filter);
            foreach (var guid in configGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<T>(path);
                if (config != null)
                    configs.Add(config);
            }

            return configs;
        }
        
        public static void CreateFolderStructure(string path)
        {
            var currentPath = ASSETS_ROOT;
            var folders = path.Split('/');
            foreach (var folder in folders)
            {
                var newPath = $"{currentPath}/{folder}";
                if (!AssetDatabase.IsValidFolder(newPath))
                    AssetDatabase.CreateFolder(currentPath, folder);

                currentPath = newPath;
            }
        }
    }
}