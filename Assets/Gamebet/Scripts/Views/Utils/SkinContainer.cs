/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-15 18:04:52 UTC+05:00

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using TurboLabz.Gamebet;

namespace TurboLabz.MPChess
{
    public static class SkinSpriteKey
    {
        public const string BACKGROUND = "background";
        public const string r = "r";
        public const string n = "n";
        public const string b = "b";
        public const string q = "q";
        public const string k = "k";
        public const string p = "p";
        public const string R = "R";
        public const string N = "N";
        public const string B = "B";
        public const string Q = "Q";
        public const string K = "K";
        public const string P = "P";
        public const string cr = "cr";
        public const string cn = "cn";
        public const string cb = "cb";
        public const string cq = "cq";
        public const string cp = "cp";
        public const string cR = "cR";
        public const string cN = "cN";
        public const string cB = "cB";
        public const string cQ = "cQ";
        public const string cP = "cP";
    }

    static class SkinSpriteFilenameKey
    {
        public const string BACKGROUND = "background";
        public const string r = "br";
        public const string n = "bn";
        public const string b = "bb";
        public const string q = "bq";
        public const string k = "bk";
        public const string p = "bp";
        public const string R = "wR";
        public const string N = "wN";
        public const string B = "wB";
        public const string Q = "wQ";
        public const string K = "wK";
        public const string P = "wP";
        public const string cr = "cbr";
        public const string cn = "cbn";
        public const string cb = "cbb";
        public const string cq = "cbq";
        public const string cp = "cbp";
        public const string cR = "cwR";
        public const string cN = "cwN";
        public const string cB = "cwB";
        public const string cQ = "cwQ";
        public const string cP = "cwP";
    }

    public class SkinContainer : ScriptableObject 
    {
        static public SkinContainer container; 
        public string objectName = "unassigned";

        public Sprite background;
        public Sprite br, bn, bb, bq, bk, bp;
        public Sprite wR, wN, wB, wQ, wK, wP;
        public Sprite cbr, cbn, cbb, cbq, cbp;
        public Sprite cwR, cwN, cwB, cwQ, cwP;

        IDictionary<string, Sprite> sprites;

        public Sprite GetSprite(string key)
        {
            if (!sprites.ContainsKey(key))
            {
                TurboLabz.Common.LogUtil.Log("Skin container doesn't contain a sprite for key: " + key);
                return null;
            }

            return sprites[key];
        }

        static public SkinContainer LoadSkin(string key)
        {
            container = Resources.Load("Store/" + key) as SkinContainer;
            if (container == null)
            {
                TurboLabz.Common.LogUtil.Log("Failed to load skin: " + "Store/" + key);
                return null;
            }

            TurboLabz.Common.LogUtil.Log("Loaded skin: " + "Store/" + key);

            return container;      
        }

        public void Unload()
        {
            foreach (KeyValuePair<string, Sprite> item in sprites)
            {
                Resources.UnloadAsset(item.Value);
            }

            Resources.UnloadAsset(this);
        }

        // Called when skin is loaded.
        void OnEnable()
        {
            InitializeDictionary();
        }

        void InitializeDictionary()
        {
            sprites = new Dictionary<string, Sprite> {
                { SkinSpriteKey.BACKGROUND, background },
                { SkinSpriteKey.r, br },
                { SkinSpriteKey.n, bn }, 
                { SkinSpriteKey.b, bb }, 
                { SkinSpriteKey.q, bq }, 
                { SkinSpriteKey.k, bk }, 
                { SkinSpriteKey.p, bp }, 
                { SkinSpriteKey.R, wR }, 
                { SkinSpriteKey.N, wN }, 
                { SkinSpriteKey.B, wB }, 
                { SkinSpriteKey.Q, wQ }, 
                { SkinSpriteKey.K, wK }, 
                { SkinSpriteKey.P, wP }, 
                { SkinSpriteKey.cr, cbr },
                { SkinSpriteKey.cn, cbn },
                { SkinSpriteKey.cb, cbb },
                { SkinSpriteKey.cq, cbq },
                { SkinSpriteKey.cp, cbp },
                { SkinSpriteKey.cR, cwR },
                { SkinSpriteKey.cN, cwN },
                { SkinSpriteKey.cB, cwB },
                { SkinSpriteKey.cQ, cwQ },
                { SkinSpriteKey.cP, cwP }
            };
        }
        #if UNITY_EDITOR
        public void Initialize()
        {
            // Input skin sprite path and name fomrat: 
            // Assets/Images/Skins/<SkinName>/<SkinName>_<SpriteName>.png

            const string sourceRootPath = AssetPaths.SKIN_ITEMS_SOURCE_PATH;

            // Select source skin folder
            string sourceSkinPath = EditorUtility.OpenFolderPanel("Select Skin Folder", sourceRootPath, "");

            if (sourceSkinPath == "")
            {
                Debug.Log("Skin object populate operation cancelled.");
                return;
            }

            // Skin folder name is skin name
            string skinName = sourceSkinPath.Substring(sourceSkinPath.LastIndexOf("/") + 1);
            objectName = skinName;

            background = LoadSprite(SkinSpriteFilenameKey.BACKGROUND, sourceRootPath);

            br = LoadSprite(SkinSpriteFilenameKey.r, sourceRootPath);
            bn = LoadSprite(SkinSpriteFilenameKey.n, sourceRootPath);
            bb = LoadSprite(SkinSpriteFilenameKey.b, sourceRootPath);
            bq = LoadSprite(SkinSpriteFilenameKey.q, sourceRootPath);
            bk = LoadSprite(SkinSpriteFilenameKey.k, sourceRootPath);
            bp = LoadSprite(SkinSpriteFilenameKey.p, sourceRootPath);
            wR = LoadSprite(SkinSpriteFilenameKey.R, sourceRootPath);
            wN = LoadSprite(SkinSpriteFilenameKey.N, sourceRootPath);
            wB = LoadSprite(SkinSpriteFilenameKey.B, sourceRootPath);
            wQ = LoadSprite(SkinSpriteFilenameKey.Q, sourceRootPath);
            wK = LoadSprite(SkinSpriteFilenameKey.K, sourceRootPath);
            wP = LoadSprite(SkinSpriteFilenameKey.P, sourceRootPath);
            cbr = LoadSprite(SkinSpriteFilenameKey.cr, sourceRootPath);
            cbn = LoadSprite(SkinSpriteFilenameKey.cn, sourceRootPath);
            cbb = LoadSprite(SkinSpriteFilenameKey.cb, sourceRootPath);
            cbq = LoadSprite(SkinSpriteFilenameKey.cq, sourceRootPath);
            cbp = LoadSprite(SkinSpriteFilenameKey.cp, sourceRootPath);
            cwR = LoadSprite(SkinSpriteFilenameKey.cR, sourceRootPath);
            cwN = LoadSprite(SkinSpriteFilenameKey.cN, sourceRootPath);
            cwB = LoadSprite(SkinSpriteFilenameKey.cB, sourceRootPath);
            cwQ = LoadSprite(SkinSpriteFilenameKey.cQ, sourceRootPath);
            cwP = LoadSprite(SkinSpriteFilenameKey.cP, sourceRootPath);
        }

        Sprite LoadSprite(string spriteName, string sourceRootPath)
        {
            string spritePath = sourceRootPath + "/" + objectName + "/" + objectName + "_" + spriteName + ".png";

            var sprite = AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite)) as Sprite;
            if (sprite == null)
            {
                Debug.Log("Failed to load sprite: " + spritePath);
                return null;
            }

            return sprite;
        }
           
        [MenuItem("Assets/Create/Turbolabz/Chess Skin")]
        public static void CreateAsset()
        {
            const string targetRootPath = AssetPaths.SHOP_SKIN_ITEMS_ASSETS_TARGET_PATH;

            SkinContainer asset = ScriptableObject.CreateInstance<SkinContainer>();
            asset.Initialize();

            string assetPath = targetRootPath + "/" + asset.objectName + ".asset"; 

            AssetDatabase.CreateAsset(asset, assetPath);

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        #endif
    }
}
