using HUF.Utils.Runtime.PlayerPrefs;
using UnityEditor;

namespace HUF.Utils.Editor
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