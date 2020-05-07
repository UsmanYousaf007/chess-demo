using HUF.Utils.Editor.Configs;
using HUF.Utils.Runtime.AndroidManifest;
using HUF.Utils.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.AndroidManifest
{
    [CustomEditor(typeof(AndroidManifestKeysConfig), true)]
    public class AndroidManifestConfigCustomInspector : AbstractConfigEditor
    {

        protected override void OnConfigGUI()
        {
            base.OnConfigGUI();
            ShowHelpBox();
            TryToShowReplaceButton();
        }

        void TryToShowReplaceButton()
        {
            var config = ( AndroidManifestKeysConfig ) target;
            if ( config.AndroidManifestTemplatePath.IsNullOrEmpty() || config.AutoUpdateAndroidManifest )
            {
                return;
            }

            var content = new GUIContent()
            {
                text = " Replace values in Android Manifest",
                image = EditorGUIUtility.IconContent( "d_Refresh" ).image
            };
            
            if ( GUILayout.Button( content, GUILayout.Height( 20f ) ) )
            {
                AndroidManifestKeyReplacer.CreateFinalManifest(config, config.AndroidManifestTemplatePath, config.AndroidManifestTemplatePath);
            }
        }

        void ShowHelpBox()
        {
            var config = ( AndroidManifestKeysConfig ) target;
            var message = config.AutoUpdateAndroidManifest
                ? "Android manifest will be updated with config values during build process"
                : "You can enable auto-update to fill android manifest with current config values during build process";

            EditorGUILayout.HelpBox( message, MessageType.Info );
        }
    }
}