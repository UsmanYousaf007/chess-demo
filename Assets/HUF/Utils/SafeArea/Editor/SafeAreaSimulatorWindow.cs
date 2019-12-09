using UnityEngine;
using UnityEditor;

namespace HUF.Utils.SafeArea.Editor
{
    public class SafeAreaSimulatorWindow : EditorWindow
    {
        static SafeAreaSimulatorHelper simulatorMargins;

        [MenuItem("HUF/Windows/Safe Area Simulator")]
        static void OpenSimulatorWindow()
        {
            if (EditorPrefs.HasKey(SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY))
            {
                simulatorMargins = JsonUtility.FromJson<SafeAreaSimulatorHelper>(
                    EditorPrefs.GetString(SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY));
            }

            GetWindow(
                typeof(SafeAreaSimulatorWindow),
                false,
                "Safe area simulator");
        }

        void OnGUI()
        {
            simulatorMargins.top = EditorGUILayout.IntField(
                "topMargin",
                simulatorMargins.top);

            simulatorMargins.bottom = EditorGUILayout.IntField(
                "bottomMargin",
                simulatorMargins.bottom);

            simulatorMargins.left = EditorGUILayout.IntField(
                "leftMargin",
                simulatorMargins.left);

            simulatorMargins.right = EditorGUILayout.IntField(
                "rightMargin",
                simulatorMargins.right);

            if (GUILayout.Button("Reset"))
            {
                simulatorMargins.top = 0;
                simulatorMargins.bottom = 0;
                simulatorMargins.left = 0;
                simulatorMargins.right = 0;
                Save();
            }

            if (GUILayout.Button("Set Test Portrait"))
            {
                simulatorMargins.top = 64;
                simulatorMargins.bottom = 32;
                simulatorMargins.left = 0;
                simulatorMargins.right = 0;
                Save();
            }

            if (GUILayout.Button("Set Test Landscape"))
            {
                simulatorMargins.top = 0;
                simulatorMargins.bottom = 32;
                simulatorMargins.left = 64;
                simulatorMargins.right = 64;
                Save();
            }

            if (GUILayout.Button("Save Custom"))
                Save();
        }

        static void Save()
        {
            EditorPrefs.SetString(
                SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY,
                JsonUtility.ToJson(simulatorMargins));
        }
    }
}