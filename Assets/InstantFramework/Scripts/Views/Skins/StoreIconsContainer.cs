
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.InstantFramework
{
    // Class name to match the script file name
    [System.Serializable]
    public class StoreIconsContainer : ScriptableObject
    {
        const string ASSET_NAME = "StoreIcons";
        public List<Sprite> sprites = new List<Sprite>();

        public static StoreIconsContainer Load()
        {
            return Resources.Load(ASSET_NAME) as StoreIconsContainer;
        }

        public Sprite GetSprite(string key)
        {
            key = key + "Icon";

            foreach (Sprite sprite in sprites)
            {
                if (sprite.name == key)
                {
                    return sprite;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";
        const string INPUT_PATH = "Assets/Game/Images/StoreIcons/";

        [MenuItem("Assets/Create/Turbolabz/Chess Store Icons")]
        public static void CreateAsset()
        {
            ScriptableObject.CreateInstance<StoreIconsContainer>().Build();
        }

        public void Build()
        {
            string[] files = Directory.GetFiles(INPUT_PATH, "*.png");

            foreach (string filePath in files)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
                sprites.Add(sprite);
            }

            AssetBuilder.Build(this, ASSET_NAME, ASSET_PATH);
        }
#endif
    }
}







