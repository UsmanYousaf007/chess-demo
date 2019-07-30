﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.OnlineCheck.EditorUtil
{
    /// <summary>Editor helper class.</summary>
    public abstract class EditorHelper : Common.EditorUtil.BaseEditorHelper
    {
        #region Static variables

        /// <summary>Start index inside the "GameObject"-menu.</summary>
        //public const int GO_ID = 23;
        public const int GO_ID = 20;

        /// <summary>Start index inside the "Tools"-menu.</summary>
        public const int MENU_ID = 11415; // 1, O = 14, N = 15

        private static Texture2D logo_asset;
        private static Texture2D logo_asset_small;

        #endregion


        #region Static properties

        public static Texture2D Logo_Asset
        {
            get
            {
                return loadImage(ref logo_asset, "logo_asset_pro.png");
            }
        }

        public static Texture2D Logo_Asset_Small
        {
            get
            {
                return loadImage(ref logo_asset_small, "logo_asset_small_pro.png");
            }
        }

        #endregion


        #region Static methods

        /// <summary>Shows an "Online Check unavailable"-UI.</summary>
        public static void OCUnavailable()
        {
            EditorGUILayout.HelpBox("Online Check not available!", MessageType.Warning);

            EditorGUILayout.HelpBox("Did you add the '" + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME + "'-prefab to the scene?", MessageType.Info);

            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent("Add " + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME, Icon_Plus, "Add the '" + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME + "'-prefab to the current scene.")))
            {
                InstantiatePrefab(Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME);
            }
        }

        /// <summary>Instantiates a prefab.</summary>
        /// <param name="prefabName">Name of the prefab.</param>
        public static void InstantiatePrefab(string prefabName)
        {
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets" + EditorConfig.PREFAB_PATH + prefabName + ".prefab", typeof(GameObject)));
        }

        /// <summary>Checks if the 'OnlineCheck'-prefab is in the scene.</summary>
        /// <returns>True if the 'OnlineCheck'-prefab is in the scene.</returns>
        public static bool isOnlineCheckInScene
        {
            get
            {
                return GameObject.Find(Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME) != null;
            }
        }

        /// <summary>Checks if the 'Proxy'-prefab is in the scene.</summary>
        /// <returns>True if the 'Proxy'-prefab is in the scene.</returns>
        public static bool isProxyInScene
        {
            get
            {
                return GameObject.Find(Util.Constants.PROXY_SCENE_OBJECT_NAME) != null;
            }
        }

        /// <summary>Loads an image as Texture2D from 'Editor Default Resources'.</summary>
        /// <param name="logo">Logo to load.</param>
        /// <param name="fileName">Name of the image.</param>
        /// <returns>Image as Texture2D from 'Editor Default Resources'.</returns>
        private static Texture2D loadImage(ref Texture2D logo, string fileName)
        {
            if (logo == null)
            {
#if CT_DEVELOP
                logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets" + EditorConfig.ASSET_PATH + "Icons/" + fileName, typeof(Texture2D));
#else
                logo = (Texture2D)EditorGUIUtility.Load("crosstales/OnlineCheck/" + fileName);
#endif

                if (logo == null)
                {
                    Debug.LogWarning("Image not found: " + fileName);
                }
            }

            return logo;
        }

        #endregion
    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)