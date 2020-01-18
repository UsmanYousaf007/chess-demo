using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Configs.API.Editor
{
    [CustomEditor(typeof(AbstractConfig), true)]
    public class AbstractConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawAbstractConfigControls();
        }

        protected virtual void DrawAbstractConfigControls()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Json tools");
            DrawToJsonButton();
            DrawFromJsonButton();
        }

        protected void DrawToJsonButton()
        {
            if (GUILayout.Button("To JSON (print in console and copy to clipboard)"))
            {
                var json = JsonUtility.ToJson(serializedObject.targetObject);
                Debug.Log(json);
                GUIUtility.systemCopyBuffer = json;
            }
        }

        protected void DrawFromJsonButton()
        {
            if (GUILayout.Button("From JSON"))
            {
                var json = EditorGUIUtility.systemCopyBuffer;
                EditorUtility.SetDirty(target);
                
                var config = (AbstractConfig) target;
                config.ApplyJson(json);
                AssetDatabase.Refresh();
            }
        }
    }
}