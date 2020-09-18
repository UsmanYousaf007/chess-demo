using HUF.Utils.Runtime.Configs.API;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Configs
{
    [CustomEditor( typeof(AbstractConfig), true )]
    public class AbstractConfigEditor : UnityEditor.Editor
    {
        bool showConfigEditor = true;
        static GUIContent headerContent = null;
        static GUIContent copyConfigContent = null;
        static GUIContent applyConfigContent = null;

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

        static GUIContent CopyConfigContent
        {
            get
            {
                if ( copyConfigContent == null )
                {
                    copyConfigContent = new GUIContent()
                    {
                        text = " Copy Config",
                        image = EditorGUIUtility.IconContent( "d_TreeEditor.Duplicate" ).image,
                        tooltip = "Copy config values to clipboard."
                    };
                }

                return copyConfigContent;
            }
        }

        static GUIContent ApplyConfigContent
        {
            get
            {
                if ( applyConfigContent == null )
                {
                    applyConfigContent = new GUIContent()
                    {
                        text = " Apply Config",
                        image = EditorGUIUtility.IconContent( "d_editicon.sml" ).image,
                        tooltip = "Replace config values with values from clipboard."
                    };
                }

                return applyConfigContent;
            }
        }

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
                {
                    var configData =
                        AbstractConfig.RemoveObjectReferences( JsonUtility.ToJson( serializedObject.targetObject ) );
                    GUIUtility.systemCopyBuffer = configData;
                    Debug.Log( "Config copied to clipboard: " + configData );
                }

                if ( GUILayout.Button( ApplyConfigContent, GUILayout.Height( 22f ) ) &&
                     EditorUtility.DisplayDialog( "Are you sure?",
                         "Are you sure you want to replace current config?",
                         "Yes",
                         "No" ) )
                {
                    EditorUtility.SetDirty( target );
                    var config = (AbstractConfig)target;
                    config.ApplyJson( EditorGUIUtility.systemCopyBuffer );
                    AssetDatabase.Refresh();
                }
            }
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