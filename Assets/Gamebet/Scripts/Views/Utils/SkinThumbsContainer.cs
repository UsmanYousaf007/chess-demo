
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TurboLabz.Gamebet;
using TurboLabz.Common;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class SkinThumb
    {
        public Sprite thumbnail;
        public Sprite preview;
    }


    // Class name to match the script file name
    public class SkinThumbsContainer : ScriptableObject 
    {
        private const string assetPath = AssetPaths.SHOP_ASSETS_RESOURCES_PATH;
        private const string assetName = AssetPaths.SKIN_ASSETS_FILE_NAME;
        private const string sourceRootPath = AssetPaths.SKIN_THUMBS_SOURCE_PATH;
        private const string targetRootPath = AssetPaths.SHOP_ASSETS_TARGET_PATH;

        public List<SkinThumb> thumbs;
        private IDictionary<string, SkinThumb> dictionary;

        private void InitializeDictionary()
        {
            dictionary = new Dictionary<string, SkinThumb>();

            for (int i = 0; i < thumbs.Count; i++)
            {
                dictionary.Add(thumbs[i].thumbnail.name, thumbs[i]);
            } 
        }

        public SkinThumb GetThumb(string key)
        {
            //LogUtil.Log("The key is: " + key, "red");
            if (!dictionary.ContainsKey(key + "Thumb"))
            {
                TurboLabz.Common.LogUtil.Log("Container doesn't contain a thumb for key: " + key , "red");
                return null;
            }
            string test = key + "Thumb";
            return dictionary[test];
        }

        static public SkinThumbsContainer Load()
        {
            var container = Resources.Load(assetPath + assetName) as SkinThumbsContainer;
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
            string[] files = Directory.GetFiles(sourceRootPath, "Skin*Thumb.png", SearchOption.TopDirectoryOnly);
            thumbs = new List<SkinThumb>();

            int i = 0;
            foreach (string file in files)
            {
                int index = file.IndexOf("Thumb.png");
                string previewFileName = file.Substring(0,index);
                previewFileName = previewFileName + "Preview.png";
                LogUtil.Log("The index: " + index + " previewFileName: " + previewFileName + " filePath: " + file, "red");

                var item = new SkinThumb();
                item.thumbnail = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;
                item.preview = AssetDatabase.LoadAssetAtPath(previewFileName, typeof(Sprite)) as Sprite;
                thumbs.Add(item);
                i++;
            }  

            InitializeDictionary();
        }

        [MenuItem("Assets/Create/Turbolabz/Skin Thumbs")]
        private static void SOCreateAsset()
        {
            const string targetAssetFilename = targetRootPath + assetName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<SkinThumbsContainer>();
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







