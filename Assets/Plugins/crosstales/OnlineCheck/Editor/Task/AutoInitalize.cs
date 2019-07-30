#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Crosstales.OnlineCheck.EditorUtil;

namespace Crosstales.OnlineCheck.EditorTask
{
    /// <summary>Automatically adds the neccessary TrueRandom-prefabs to the current scene.</summary>
    [InitializeOnLoad]
    public class AutoInitalize
    {

        #region Variables

        private static Scene currentScene;

        #endregion


        #region Constructor

        static AutoInitalize()
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
                if (EditorConfig.PREFAB_AUTOLOAD)
                {
                    EditorHelper.InstantiatePrefab(Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME);
                }

                currentScene = EditorSceneManager.GetActiveScene();
            }
        }

        #endregion
    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)