using HUF.Utils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(WarningAttribute))]
    public class WarningDrawer : PropertyDrawer
    {
        const float MINIMUM_HEIGHT = 30;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float exactHeight = EditorStyles.helpBox.CalcHeight( new GUIContent( property.stringValue ),
                EditorGUIUtility.currentViewWidth );
            return Mathf.Max( MINIMUM_HEIGHT, exactHeight );
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.HelpBox(position, property.stringValue, MessageType.Warning);
        }
    }
}
