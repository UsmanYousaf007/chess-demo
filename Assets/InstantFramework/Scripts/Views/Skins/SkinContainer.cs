/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor Khawaja <noor.khawaja@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-15 18:04:52 UTC+05:00

using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.InstantFramework
{
    public class SkinContainer : ScriptableObject 
    {
        public string skinName = "unassigned";
        public List<Sprite> sprites = new List<Sprite>();

        public static SkinContainer LoadSkin(string key)
        {
            return Resources.Load(key) as SkinContainer;
        }

        public Sprite GetSprite(string name)
        {
            foreach (Sprite sprite in sprites)
            {
                string spriteName = sprite.name.Split(',')[0];

                if (spriteName == skinName + "_" + name)
                {
                    TurboLabz.TLUtils.LogUtil.Log("SPRITE BORDER IS: " + sprite.border);
                    return sprite;
                }
            }

            Debug.Log("Sprite not found: " + name);

            return null;
        }

        #if UNITY_EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";
        const string SKINS_PATH = "Assets/Game/Images/Skins/";

        [MenuItem("Assets/Create/Turbolabz/Chess Skin")]
        public static void CreateAsset() 
        {
            // Select source skin folder
            string sourceSkinPath = EditorUtility.OpenFolderPanel("Select Skin Folder", SKINS_PATH, "");

            ScriptableObject.CreateInstance<SkinContainer>().Build(sourceSkinPath);
        }

        [MenuItem("Assets/Create/Turbolabz/Chess Skins All")]
        public static void CreateAllAssets()
        {
            string[] folders = Directory.GetDirectories(SKINS_PATH);
            foreach (string folder in folders)
            {
                ScriptableObject.CreateInstance<SkinContainer>().Build(folder);
            }
        }

        public void Build(string sourceSkinPath)
        {
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

        #endif
    }
}