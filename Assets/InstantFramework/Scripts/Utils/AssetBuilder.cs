#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TurboLabz.Chess
{
    public static class AssetBuilder
    {
        public static void Build(ScriptableObject obj, 
                                string fileName,
                                string filePath)
        {
            string assetPath = filePath + fileName + ".asset"; 
            AssetDatabase.CreateAsset(obj, assetPath);
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
        }
    }
}
#endif

