using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class ChatEmote
    {
        public Sprite sprite;
    }

    // Class name to match the script file name
    public class ChatEmotesContainer : ScriptableObject 
    {
        private const string assetPath = "Store/";
        private const string assetName = "ChatEmotes";

        #if UNITY_EDITOR
        private const string sourceRootPath = "Assets/Game/Store/ChatEmote/";
        private const string targetRootPath = "Assets/Resources/Store/";
        #endif

        public List<ChatEmote> list;
        private IDictionary<string, ChatEmote> dictionary;

        private void InitializeDictionary()
        {
            dictionary = new Dictionary<string, ChatEmote>();

            for (int i = 0; i < list.Count; i++)
            {
                dictionary.Add(list[i].sprite.name, list[i]);
            } 
        }

        public ChatEmote GetItem(string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                TurboLabz.Common.LogUtil.Log("Container doesn't contain a thumb for key: " + key , "red");
                return null;
            }

            return dictionary[key];
        }

        static public ChatEmotesContainer Load()
        {
            var container = Resources.Load(assetPath + assetName) as ChatEmotesContainer;
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
            list = new List<ChatEmote>();

            int i = 0;
            foreach (string file in files)
            {
                var item = new ChatEmote();
                item.sprite = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;
                list.Add(item);
                i++;
            }  

            InitializeDictionary();
        }

        [MenuItem("Assets/Create/Turbolabz/Chat Emotes")]
        private static void SOCreateAsset()
        {
            const string targetAssetFilename = targetRootPath + assetName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<ChatEmotesContainer>();
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
