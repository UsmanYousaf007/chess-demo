
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class CoinPackThumb
    {
        public Sprite thumbnail;
    }

    // Class name to match the script file name
    public class CoinPackThumbsContainer : ScriptableObject 
    {
        private const string assetPath = AssetPaths.SHOP_ASSETS_RESOURCES_PATH;
        private const string assetName = AssetPaths.COIN_PACK_ASSETS_FILE_NAME;
        private const string sourceRootPath = AssetPaths.COIN_PACK_THUMBS_SOURCE_PATH;
        private const string targetRootPath = AssetPaths.SHOP_ASSETS_TARGET_PATH;

        public List<CoinPackThumb> thumbs;
        private IDictionary<string, CoinPackThumb> dictionary;

        private void InitializeDictionary()
        {
            dictionary = new Dictionary<string, CoinPackThumb>();

            for (int i = 0; i < thumbs.Count; i++)
            {
                dictionary.Add(thumbs[i].thumbnail.name, thumbs[i]);
            } 
        }

        public CoinPackThumb GetThumb(string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                TurboLabz.Common.LogUtil.Log("Container doesn't contain a thumb for key: " + key , "red");
                return dictionary["CoinPackBucks"];
            }

            return dictionary[key];
        }

        static public CoinPackThumbsContainer Load()
        {
            var container = Resources.Load(assetPath + assetName) as CoinPackThumbsContainer;
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
            thumbs = new List<CoinPackThumb>();

            int i = 0;
            foreach (string file in files)
            {
                var item = new CoinPackThumb();
                item.thumbnail = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;
                thumbs.Add(item);
                i++;
            }  

            InitializeDictionary();
        }

        [MenuItem("Assets/Create/Turbolabz/CoinPack Thumbs")]
        private static void SOCreateAsset()
        {
            const string targetAssetFilename = targetRootPath + assetName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<CoinPackThumbsContainer>();
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







