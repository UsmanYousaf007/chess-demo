using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public class ConditionalDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldDraw(property))
                EditorGUI.PropertyField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldDraw(property))
                return base.GetPropertyHeight(property, label);

            return 0;
        }

        bool ShouldDraw(SerializedProperty property)
        {
            return attribute is ConditionalAttribute conditionalAttribute && 
                   property.serializedObject.FindProperty(conditionalAttribute.conditionProperty).boolValue;
        }
    }
}