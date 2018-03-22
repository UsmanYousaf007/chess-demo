
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class LootThumb
    {
        public Sprite thumbnail;
    }

    // Class name to match the script file name
    public class LootThumbsContainer : ScriptableObject 
    {
        private const string assetPath = AssetPaths.SHOP_ASSETS_RESOURCES_PATH;
        private const string assetName = AssetPaths.LOOT_BOXES_ASSETS_FILE_NAME;
        private const string sourceRootPath = AssetPaths.LOOT_BOXES_THUMBS_SOURCE_PATH;
        private const string targetRootPath = AssetPaths.SHOP_ASSETS_TARGET_PATH;

        public List<LootThumb> thumbs;
        private IDictionary<string, LootThumb> dictionary;

        private void InitializeDictionary()
        {
            dictionary = new Dictionary<string, LootThumb>();

            for (int i = 0; i < thumbs.Count; i++)
            {
                dictionary.Add(thumbs[i].thumbnail.name, thumbs[i]);
            } 
        }

        public LootThumb GetThumb(string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                TurboLabz.Common.LogUtil.Log("Container doesn't contain a thumb for key: " + key , "red");
                return null;
            }

            return dictionary[key];
        }

        static public LootThumbsContainer Load()
        {
            var container = Resources.Load(assetPath + assetName) as LootThumbsContainer;
            if (container == null)
            {
                TurboLabz.Common.LogUtil.Log("Failed to load asset: " + assetPath + assetName);
                return null;
            }

            container.InitializeDictionary();

            return container;      
        }

        #if UNITY_EDITOR
        private void SOInitialize()
        {
            string[] files = Directory.GetFiles(sourceRootPath, "*.png", SearchOption.TopDirectoryOnly);
            thumbs = new List<LootThumb>();

            int i = 0;
            foreach (string file in files)
            {
                var item = new LootThumb();
                item.thumbnail = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;
                thumbs.Add(item);
                i++;
            }  

            InitializeDictionary();
        }

        [MenuItem("Assets/Create/Turbolabz/Loot Thumbs")]
        private static void SOCreateAsset()
        {
            const string targetAssetFilename = targetRootPath + assetName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<LootThumbsContainer>();
            asset.SOInitialize();

            AssetDatabase.CreateAsset(asset, targetAssetFilename);

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        #endif
    }
}








