
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class ChatpackThumb
    {
        public Sprite thumbnail;
    }

    // Class name to match the script file name
    public class ChatpackThumbsContainer : ScriptableObject 
    {
        private const string assetPath = "Store/";
        private const string assetName = "ChatpackThumbs";
        private const string sourceRootPath = "Assets/Game/Store/Chatpack/";
        private const string targetRootPath = "Assets/Resources/Store/";

        public List<ChatpackThumb> thumbs;
        private IDictionary<string, ChatpackThumb> dictionary;

        private void InitializeDictionary()
        {
            dictionary = new Dictionary<string, ChatpackThumb>();

            for (int i = 0; i < thumbs.Count; i++)
            {
                dictionary.Add(thumbs[i].thumbnail.name, thumbs[i]);
            } 
        }

        public ChatpackThumb GetThumb(string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                TurboLabz.Common.LogUtil.Log("Container doesn't contain a thumb for key: " + key , "red");
                return null;
            }

            return dictionary[key];
        }

        static public ChatpackThumbsContainer Load()
        {
            var container = Resources.Load(assetPath + assetName) as ChatpackThumbsContainer;
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
            thumbs = new List<ChatpackThumb>();

            int i = 0;
            foreach (string file in files)
            {
                var item = new ChatpackThumb();
                item.thumbnail = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;
                thumbs.Add(item);
                i++;
            }  

            InitializeDictionary();
        }

        [MenuItem("Assets/Create/Turbolabz/Chatpack Thumbs")]
        private static void SOCreateAsset()
        {
            const string targetAssetFilename = targetRootPath + assetName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<ChatpackThumbsContainer>();
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







