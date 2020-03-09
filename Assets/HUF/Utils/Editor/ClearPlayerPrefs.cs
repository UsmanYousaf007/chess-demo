using UnityEditor;

namespace HUF.Utils.PlayerPrefs.Editor
{
    public class ClearPlayerPrefs : EditorWindow
    {
        [MenuItem("HUF/Clear Player Prefs")]
        static void Init()
        {
            HPlayerPrefs.DeleteAll();
        }
    }
}