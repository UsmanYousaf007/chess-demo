


using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace TurboLabz.Gamebet
{
    [System.Serializable]
    public class AvatarBorder
    {
        public Sprite sprite;
    }

    // Class name to match the script file name
    public class AvatarBorderContainer : ScriptableObject 
    {
        private const string assetPath = "Store/";

        #if UNITY_EDITOR
        private const string sourceRootPath = "Assets/Game/Store/Avatar/";
        private const string targetRootPath = "Assets/Resources/Store/";
        #endif

        public AvatarBorder item;

        public AvatarBorder GetItem()
        {
            return item;
        }

        static public AvatarBorderContainer Load(string key)
        {
            string assetName = key + "Border";

            var container = Resources.Load(assetPath + assetName) as AvatarBorderContainer;
            if (container == null)
            {
                TurboLabz.Common.LogUtil.Log("Failed to load asset: " + assetPath + assetName);
                return null;
            }

            return container;      
        }

        public void Unload()
        {
            Resources.UnloadAsset(item.sprite);
            Resources.UnloadAsset(this);
        }

        #if UNITY_EDITOR
        static private void SOSaveAsset(string file)
        {
            string fileName = file.Substring(file.LastIndexOf("/"));
            fileName = fileName.Remove(fileName.LastIndexOf("."));
            string targetAssetFilename = targetRootPath + fileName + ".asset"; 

            var asset = ScriptableObject.CreateInstance<AvatarBorderContainer>();

            asset.item = new AvatarBorder();
            asset.item.sprite = AssetDatabase.LoadAssetAtPath(file, typeof(Sprite)) as Sprite;

            AssetDatabase.CreateAsset(asset, targetAssetFilename);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/Turbolabz/Avatar Borders")]
        private static void SOCreateAsset()
        {
            string[] files = Directory.GetFiles(sourceRootPath, "*Border.png", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                SOSaveAsset(file);
            }  
        }
        #endif
    }
}















