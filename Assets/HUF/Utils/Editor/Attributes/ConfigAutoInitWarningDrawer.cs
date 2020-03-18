using HUF.Utils.Assets.Editor;
using HUF.Utils.Configs.API;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ConfigAutoInitWarningAttribute))]
    public class ConfigAutoInitWarningDrawer : PropertyDrawer
    {
        const float ERROR_HEIGHT = 45.0f;
        static string ConfigsPath => $"/{AssetsUtils.RESOURCES_FOLDER}/{HConfigs.CONFIGS_FOLDER}/";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyHeight = base.GetPropertyHeight(property, label);
            
            if (ShouldShowConfigAutoInitializationError(property))
                propertyHeight += ERROR_HEIGHT;

            if (ShouldShowResourcesError(property))
                propertyHeight += ERROR_HEIGHT;

            return propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShowConfigAutoInitializationError(property))
            {
                ShowErrorBox(position, "Auto initialization works only if HConfigs is auto-initialized too.");
                position.y += ERROR_HEIGHT;
            }

            if (ShouldShowResourcesError(property))
            {
                ShowErrorBox(position, $"Auto initialization works only if current config is under {ConfigsPath} path");
                position.y += ERROR_HEIGHT;
            }

            EditorGUI.PropertyField(position, property, label);
        }

        static bool ShouldShowConfigAutoInitializationError(SerializedProperty property)
        {
            return !HConfigs.IsAutoInitEnabled && property.boolValue;
        }

        static bool ShouldShowResourcesError(SerializedProperty property)
        {
            var configObject = property.serializedObject.targetObject;
            var configPath = AssetDatabase.GetAssetPath(configObject);
            return !configPath.Contains(ConfigsPath) && property.boolValue;
        }

        static void ShowErrorBox(Rect position, string message)
        {
            var fieldPosition = position;
            fieldPosition.height = ERROR_HEIGHT;
            EditorGUI.HelpBox(fieldPosition, message, MessageType.Error);
        }
    }
}