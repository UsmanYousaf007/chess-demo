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
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using UnityEngine.U2D;
using UnityEditor.U2D;
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
                    return sprite;
                }
            }

            Debug.Log("Sprite not found: " + name);

            return null;
        }

#if UNITY_EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";
        const string SKINS_PATH = "Assets/Game/Images/Skins/";
        const string ATLAS_PATH = "Assets/Game/Images/Atlases/";

        const string ATLAS_PATH_BUNDLE = "Bundles/Skins/Atlases/";
        const string ASSET_PATH_BUNDLE = "Bundles/Skins/Resouorces/";

        [MenuItem("Assets/Create/Turbolabz/Chess Skin")]
        public static void CreateAsset()
        {
            // Select source skin folder
            string sourceSkinPath = EditorUtility.OpenFolderPanel("Select Skin Folder", SKINS_PATH, "");

            ScriptableObject.CreateInstance<SkinContainer>().Build(sourceSkinPath);
            MoveBundleFiles();
        }

        [MenuItem("Assets/Create/Turbolabz/Chess Skins All")]
        public static void CreateAllAssets()
        {
            string[] folders = Directory.GetDirectories(SKINS_PATH);
            foreach (string folder in folders)
            {
                ScriptableObject.CreateInstance<SkinContainer>().Build(folder);
            }
            MoveBundleFiles();
        }

        public void Build(string sourceSkinPath)
        {
            if (sourceSkinPath == "")
            {
                Debug.Log("Skin object populate operation cancelled.");
                return;
            }

            string[] files = Directory.GetFiles(sourceSkinPath, "*.png");
            skinName = Path.GetFileName(sourceSkinPath);
            string spritePathPrefx = SKINS_PATH + skinName + Path.DirectorySeparatorChar;

            foreach (string filePath in files)
            {
                string spritePath = spritePathPrefx + Path.GetFileName(filePath);
                sprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(spritePath));
            }

            // Save the scriptable object to disk
            AssetBuilder.Build(this, skinName, ASSET_PATH);

            // Save the sprite atlas to disk
            SpriteAtlas spriteAtlas = new SpriteAtlas();
            //spriteAtlas.SetPackingSettings(
            //    new SpriteAtlasPackingSettings
            //    {
            //        enableRotation = false,
            //        enableTightPacking = true,
            //        padding = 4
            //    });

            SpriteAtlasExtensions.Add(spriteAtlas, sprites.ToArray());
            AssetDatabase.CreateAsset(spriteAtlas, ATLAS_PATH + skinName + ".spriteatlas");
            AssetDatabase.SaveAssets();
        }

        private static void MoveBundleFiles()
        {
            List<string> skinBundles = new List<string>{"SkinAmazon","SkinCrayon","SkinDeepSea","SkinDracula","SkinMoonlight",
                                                "SkinMarble","SkinHighrise","SkinIndiana","SkinLuxury"};

            List<string> atlasFiles = Directory.GetFiles(ATLAS_PATH, "*.spriteatlas").ToList();
            MoveFiles(skinBundles, atlasFiles, ATLAS_PATH, ATLAS_PATH_BUNDLE);

            List<string> assetFiles = Directory.GetFiles(ASSET_PATH, "*.asset").ToList();
            MoveFiles(skinBundles, assetFiles, ASSET_PATH, ASSET_PATH_BUNDLE);
        }

        private static void MoveFiles(List<string> skinBundles, List<string> files, string source, string destination)
        {
            CreateDirectory(destination);
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (skinBundles.Contains(fileName))
                {
                    System.IO.File.Move(file, System.IO.Path.Combine(destination, file.Substring(source.Length)));
                }
            }
        }

        private static void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }
#endif
    }
}