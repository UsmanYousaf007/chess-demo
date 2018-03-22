/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-16 16:32:41 UTC+05:00
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class AvatarBorderThumb
    {
        public Sprite thumbnail;
    }

    // Class name to match the script file name
    public class AvatarBorderThumbsContainer : ScriptableObject 
    {
        static public AvatarBorderThumbsContainer container;

        private const string assetPath = AssetPaths.SHOP_ASSETS_RESOURCES_PATH;
        private const string assetName = AssetPaths.AVATARS_BORDER_ASSETS_FILE_NAME;
        private const string sourceRootPath = AssetPaths.AVATARS_BORDER_THUMBS_SOURCE_PATH;
        private const string targetRootPath = AssetPaths.SHOP_ASSETS_TARGET_PATH;

        public List<AvatarBorderThumb> thumbs;
        private IDictionary<string, AvatarBorderThumb> dictionary;

        private void InitializeDictionary()
        {
            dictionary = new Dictionary<string, AvatarBorderThumb>();

            for (int i = 0; i < thumbs.Count; i++)
            {
                dictionary.Add(thumbs[i].thumbnail.name, thumbs[i]);
            } 
        }

        public AvatarBorderThumb GetThumb(string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                TurboLabz.Common.LogUtil.Log("Container doesn't contain a thumb for key: " + key , "red");
                return null;
            }

            return dictionary[key];
        }

        static public AvatarBorderThumbsContainer Load()
        {
            container = Resources.Load(assetPath + assetName) as AvatarBorderThumbsContainer;
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
            string[] files = Directory.GetFiles(sourceRootPath, "AvatarBorder*.png", SearchOption.TopDirectoryOnly);
            thumbs = new List<AvatarBorderThumb>();

            int i = 0;
            foreach (string file in files)
            {
                var item = new AvatarBorderThumb();
                item.thumbnail = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;
                thumbs.Add(item);
                i++;
            }  

            InitializeDictionary();
        }

        [MenuItem("Assets/Create/Turbolabz/Avatar Border Thumbs")]
        private static void SOCreateAsset()
        {
            const string targetAssetFilename = targetRootPath + assetName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<AvatarBorderThumbsContainer>();
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
