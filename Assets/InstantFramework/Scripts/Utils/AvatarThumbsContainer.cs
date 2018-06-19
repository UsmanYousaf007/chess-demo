
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TurboLabz.TLUtils;
using TurboLabz.Chess;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.InstantGame
{
    // Class name to match the script file name
    [System.Serializable]
    public class AvatarThumbsContainer : ScriptableObject 
    {
        const string ASSET_NAME = "AvatarThumbs";
        public List<Sprite> sprites = new List<Sprite>();

        public static SkinThumbsContainer Load()
        {
            return Resources.Load(ASSET_NAME) as SkinThumbsContainer;
        }

        public Sprite GetSprite(string key)
        {
            key = key + "Thumb";

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
        const string INPUT_PATH = "Assets/Game/Images/AvatarThumbs/";

        [MenuItem("Assets/Create/Turbolabz/Chess Avatar Thumbs")]
        public static void CreateAsset() 
        {
            ScriptableObject.CreateInstance<SkinThumbsContainer>().Build();
        }

        public void Build()
        {
            string[] files = Directory.GetFiles(INPUT_PATH , "*.png");

            foreach(string filePath in files)
            {
                Sprite sprite = new Sprite();
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
                sprites.Add(sprite);
            }

            AssetBuilder.Build(this, ASSET_NAME, ASSET_PATH);
        }
        #endif
    }
}







