/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor Khawaja <noor.khawaja@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-15 18:04:52 UTC+05:00

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace TurboLabz.Chess
{
    public class SkinContainer : ScriptableObject 
    {
        public const string SPRITE_BACKGROUND = "Background";
        public const string SPRITE_CAPTURED_PREFIX = "c";

        public string skinName = "unassigned";
        public List<Sprite> sprites = new List<Sprite>();
        public Color32 tint;

        public static SkinContainer LoadSkin(string key)
        {
            return Resources.Load(ASSET_PATH + key) as SkinContainer;
        }

        public Sprite GetSprite(string key)
        {
            key += "_" + skinName;

            foreach (Sprite sprite in sprites)
            {
                if (sprite.name == key)
                {
                    return sprite;
                }
            }

            return null;
        }

        #region EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";
        const string SKINS_PATH = "Assets/Game/Images/Skins/";

        [MenuItem("Assets/Create/Turbolabz/Chess Skin")]
        public static void CreateAsset() 
        {
            ScriptableObject.CreateInstance<SkinContainer>().Build();
        }

        public void Build()
        {
            // Select source skin folder
            string sourceSkinPath = EditorUtility.OpenFolderPanel("Select Skin Folder", SKINS_PATH, "");

            if (sourceSkinPath == "")
            {
                Debug.Log("Skin object populate operation cancelled.");
                return;
            }

            string[] files = Directory.GetFiles(sourceSkinPath , "*.png");
            skinName = Path.GetFileName(sourceSkinPath);
            string spritePathPrefx = SKINS_PATH + skinName + Path.DirectorySeparatorChar;

            foreach(string filePath in files)
            {
                string spritePath = spritePathPrefx + Path.GetFileName(filePath);
                sprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(spritePath));
            }

            AssetBuilder.Build(this, skinName, ASSET_PATH);
        }
        #endregion
    }
}