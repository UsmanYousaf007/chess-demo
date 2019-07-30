using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crosstales.OnlineCheck.Demo
{
    /// <summary>Main GUI scene manager for all demo scenes.</summary>
	[HelpURL("https://crosstales.com/media/data/assets/OnlineCheck/api/class_crosstales_1_1_online_check_1_1_demo_1_1_g_u_i_scenes.html")]
    public class GUIScenes : MonoBehaviour
    {

        #region Variables

        [Tooltip("Name of the previous scene.")]
        /// <summary>Name of the previous scene.</summary>
        public string PreviousScene;

        [Tooltip("Name of the next scene.")]
        /// <summary>Name of the next scene.</summary>
        public string NextScene;


        #endregion


        #region Public methods

        /// <summary>Load previous scene.</summary>
        public void LoadPreviousScene()
        {
            SceneManager.LoadScene(PreviousScene);
        }

        /// <summary>Load next scene.</summary>
        public void LoadNextScene()
        {
            SceneManager.LoadScene(NextScene);
        }

        public void OpenAssetURL()
        {
            Application.OpenURL(Common.Util.BaseConstants.ASSET_CT_URL);
        }

        public void OpenCTURL()
        {
            Application.OpenURL(Common.Util.BaseConstants.ASSET_AUTHOR_URL);
        }

        /// <summary>Quit the application.</summary>
        public void Quit()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        #endregion
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)