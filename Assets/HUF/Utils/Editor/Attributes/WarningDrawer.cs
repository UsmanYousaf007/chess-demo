using HUF.Utils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(WarningAttribute))]
    public class WarningDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is WarningAttribute warningAttribute && warningAttribute.size > 0)
            {
                return warningAttribute.size;
            }
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.HelpBox(position, property.stringValue, MessageType.Warning);
        }
    }
}