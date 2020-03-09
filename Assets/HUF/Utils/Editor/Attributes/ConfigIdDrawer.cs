using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ConfigIdAttribute))]
    public class ConfigIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var configId = property.stringValue;
            bool isValid = IsConfigIdValid(configId);

            var color = UnityEngine.GUI.color;
            if (!isValid)
                UnityEngine.GUI.color = Color.red;

            EditorGUI.PropertyField(position, property, label);

            UnityEngine.GUI.color = color;
        }

        static bool IsConfigIdValid(string configId)
        {
            foreach (var c in configId)
            {
                if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || char.IsDigit(c) || c == '_'))
                    return false;
            }

            return true;
        }
    }
}