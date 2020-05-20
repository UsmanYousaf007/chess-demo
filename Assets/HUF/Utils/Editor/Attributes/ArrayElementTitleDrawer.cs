using HUF.Utils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
    public class ArrayElementTitleDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        ArrayElementTitleAttribute Atribute
        {
            get { return (ArrayElementTitleAttribute) attribute; }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string fullPathName = $"{property.propertyPath}.{Atribute.varname}";
            var titleNameProp = property.serializedObject.FindProperty(fullPathName);
            string newLabel = GetTitle(titleNameProp);
            if (string.IsNullOrEmpty(newLabel))
                newLabel = label.text;
            EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
        }

        static string GetTitle(SerializedProperty titleNameProp)
        {
            switch (titleNameProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return titleNameProp.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return titleNameProp.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return titleNameProp.floatValue.ToString();
                case SerializedPropertyType.String:
                    return titleNameProp.stringValue;
                case SerializedPropertyType.Color:
                    return titleNameProp.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return titleNameProp.objectReferenceValue.ToString();
                case SerializedPropertyType.Enum:
                    return titleNameProp.enumNames[titleNameProp.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return titleNameProp.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return titleNameProp.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return titleNameProp.vector4Value.ToString();
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                    break;
            }

            return "";
        }
    }
}