using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Configs.Implementation;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Configs
{
    [CustomEditor( typeof(AbstractConfig), true )]
    public class AbstractConfigEditor : UnityEditor.Editor
    {

        protected static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AbstractConfigEditor) );

        static GUIContent headerContent = null;
        static GUIContent copyConfigContent = null;
        static GUIContent applyConfigContent = null;
        static GUIContent applyPresetContent = null;

        bool showConfigEditor = true;

        public static GUIContent HeaderContent
        {
            get
            {
                if ( headerContent == null )
                {
                    headerContent = new GUIContent()
                    {
                        text = " HUF Config",
                        image = HEditorGUI.Res.WindowIcon
                    };
                }

                return headerContent;
            }
        }

        static GUIContent CopyConfigContent =>
            copyConfigContent ?? ( copyConfigContent = new GUIContent()
            {
                text = " Copy Config",
                image = EditorGUIUtility.IconContent( "d_TreeEditor.Duplicate" ).image,
                tooltip = "Copies config values to the clipboard."
            } );

        static GUIContent ApplyConfigContent =>
            applyConfigContent ?? ( applyConfigContent = new GUIContent()
            {
                text = " Apply Config",
                image = EditorGUIUtility.IconContent( "d_editicon.sml" ).image,
                tooltip = "Replaces config values with values from the clipboard."
            } );

        static GUIContent ApplyPresetContent =>
            applyPresetContent ?? ( applyPresetContent = new GUIContent()
            {
                text = " Overlay Default Preset",
                image = EditorGUIUtility.IconContent( "d_Preset.Context" ).image,
                tooltip = "Applies default preset properties, without clearing fields that were not defined in the preset."
            } );



        public override void OnInspectorGUI()
        {
            using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
            {
                DrawHeaderPanel();

                if ( showConfigEditor )
                {
                    HEditorGUI.HorizontalSeparator();
                    OnConfigGUI();
                }
            }

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        protected virtual void OnConfigGUI()
        {
            using ( new GUILayout.HorizontalScope() )
            {
                if ( GUILayout.Button( CopyConfigContent, GUILayout.Height( 22f ) ) )
                    CopyConfig();

                if ( GUILayout.Button( ApplyConfigContent, GUILayout.Height( 22f ) ) &&
                     EditorUtility.DisplayDialog( "Are you sure?",
                         "Are you sure you want to replace current config?",
                         "Yes",
                         "No" ) )
                {
                    ApplyConfig();
                }
            }

            using ( new GUILayout.HorizontalScope() )
            {
                if ( GUILayout.Button( ApplyPresetContent, GUILayout.Height( 22f ) ) )
                    ConfigUtils.OverlayPreset( target );
            }
        }

        void ApplyConfig()
        {
            EditorUtility.SetDirty( target );
            var config = (AbstractConfig)target;
            config.ApplyJson( EditorGUIUtility.systemCopyBuffer );
            AssetDatabase.Refresh();
        }

        void CopyConfig()
        {
            var configData =
                AbstractConfig.RemoveObjectReferences( JsonUtility.ToJson( serializedObject.targetObject ) );
            GUIUtility.systemCopyBuffer = configData;
            HLog.Log( logPrefix, $"Config copied to clipboard: {configData}" );
        }

        void DrawHeaderPanel()
        {
            using ( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.LabelField( HeaderContent, EditorStyles.boldLabel );
                GUILayout.FlexibleSpace();

                if ( GUILayout.Button( showConfigEditor ? "Hide" : "Show", EditorStyles.miniButton ) )
                {
                    showConfigEditor = !showConfigEditor;
                }
            }
        }
    }
}
