#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Crosstales.OnlineCheck.EditorTask
{
    /// <summary>Automatically adds the necessary TrueRandom-prefabs to the current scene.</summary>
    [InitializeOnLoad]
    public class AutoInitialize
    {

        #region Variables

        private static Scene currentScene;

        #endregion


        #region Constructor

        static AutoInitialize()
        {
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += hierarchyWindowChanged;
#else
            EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;
#endif
        }

        #endregion


        #region Private static methods

        private static void hierarchyWindowChanged()
        {
            if (currentScene != EditorSceneManager.GetActiveScene())
            {
                if (EditorUtil.EditorConfig.PREFAB_AUTOLOAD)
                {
                    if (!EditorUtil.EditorHelper.isOnlineCheckInScene)
                        EditorUtil.EditorHelper.InstantiatePrefab(Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME);
                }

                currentScene = EditorSceneManager.GetActiveScene();
            }
        }

        #endregion
    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)