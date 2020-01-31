using HUF.Utils.Configs.API.Editor;
using HUF.Utils.Extensions;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.AndroidManifest.Editor
{
    [CustomEditor(typeof(AndroidManifestKeysConfig), true)]
    public class AndroidManifestConfigCustomInspector : AbstractConfigEditor
    {
        readonly AndroidManifestKeyReplacer androidManifestKeyReplacer;
        
        public AndroidManifestConfigCustomInspector()
        {
            androidManifestKeyReplacer = new AndroidManifestKeyReplacer();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TryToShowReplaceButton();
            ShowHelpBox();
        }

        void TryToShowReplaceButton()
        {
            var config = (AndroidManifestKeysConfig) target;
            if (config.AndroidManifestTemplatePath.IsNullOrEmpty() || config.AutoUpdateAndroidManifest)
            {
                return;
            }
            
            if (GUILayout.Button("Replace values in Android Manifest"))
            {
                androidManifestKeyReplacer.CreateFinalManifest(target);
            }
        }

        void ShowHelpBox()
        {
            var config = (AndroidManifestKeysConfig) target;
            var message = config.AutoUpdateAndroidManifest
                ? "Android manifest will be updated with config values during build process"
                : "You can enable auto-update to fill android manifest with current config values during build process";
             
            EditorGUILayout.HelpBox(message, MessageType.Info);
        }
    }
}