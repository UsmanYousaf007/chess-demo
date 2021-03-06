
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.InstantFramework
{
    // Class name to match the script file name
    [System.CLSCompliantAttribute(false)]
    [System.Serializable]
    public class NotificationAssetsContainer : ScriptableObject
    {
        const string ASSET_NAME = "NotificationAssets";
        public List<Sprite> sprites = new List<Sprite>();

        public static NotificationAssetsContainer Load()
        {
            return Resources.Load(ASSET_NAME) as NotificationAssetsContainer;
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
        const string INPUT_PATH = "Assets/Game/Images/NotificationIcons/";

        [MenuItem("Assets/Create/Turbolabz/Notification Assets")]
        public static void CreateAsset()
        {
            ScriptableObject.CreateInstance<NotificationAssetsContainer>().Build();
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