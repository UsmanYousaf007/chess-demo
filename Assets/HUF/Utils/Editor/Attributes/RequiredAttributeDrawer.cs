using HUF.Utils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Attributes {
    [CustomPropertyDrawer( typeof(RequiredAttribute) )]
    public class RequiredAttributeDrawer : PropertyDrawer {
        const float WARNING_HEIGHT = 17;

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
            float height = EditorGUI.GetPropertyHeight( property );

            bool modified = !CheckProperty( property, out bool hasContent );
            return modified? GetModifiedHeight(hasContent) + height : height;
        }

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
            if( !CheckProperty( property, out bool hasContent ) ) {

                float heightAddition = GetModifiedHeight( hasContent );

                string message = string.Empty;

                if( hasContent )
                    message = $"Field \"{label.text}\" should be of type {(attribute as RequiredAttribute ).ShortName}";
                else
                    message = $"Field \"{label.text}\" is required!";

                EditorGUI.HelpBox(
                                  new Rect( position.x, position.y, position.width, heightAddition ),
                                  message,
                                  hasContent ? MessageType.Warning : MessageType.Error );
                Color color = GUI.backgroundColor;
                GUI.backgroundColor =  hasContent? Color.yellow : Color.red;
                position.y          += heightAddition;
                EditorGUI.PropertyField( position, property, label, true );
                GUI.backgroundColor = color;
            } else {
                EditorGUI.PropertyField( position, property, label, true );
            }
        }

        static float GetModifiedHeight( bool hasContent ) {
            return hasContent ? WARNING_HEIGHT * 2 : WARNING_HEIGHT;
        }

        bool CheckProperty( SerializedProperty property, out bool hasContent ) {
            RequiredAttribute requirement = attribute as RequiredAttribute;

            bool isReference = property.propertyType == SerializedPropertyType.ExposedReference
                            || property.propertyType == SerializedPropertyType.ObjectReference;

            hasContent = true;

            if( !isReference )
                return true;

            hasContent = property.objectReferenceValue != null;

            if( !hasContent )
                return false;

            if( requirement.TargetType == null )
                return true;

            return requirement.TargetType.IsInstanceOfType( property.objectReferenceValue );
        }
    }
}