using UnityEditor;
using UnityEngine;
using HUF.Utils.Editor;

namespace HUF.Utils.Configs.API.Editor
{
    [CustomEditor( typeof( AbstractConfig ), true )]
    public class AbstractConfigEditor : UnityEditor.Editor
    {
        private bool m_showConfigEditor = true;
        private static GUIContent m_headerContent = null;
        private static GUIContent m_copyConfigContent = null;
        private static GUIContent m_applyConfigContent = null;
        
        private static GUIContent HeaderContent
        {
            get
            {
                if ( m_headerContent == null )
                {
                    m_headerContent = new GUIContent()
                    {
                        text = " HUF Config",
                        image = HEditorGUI.Res.WindowIcon
                    };
                }
                return m_headerContent;
            }
        }
        
        private static GUIContent CopyConfigContent
        {
            get
            {
                if ( m_copyConfigContent == null )
                {
                    m_copyConfigContent = new GUIContent()
                    {
                        text = " Copy Config",
                        image = EditorGUIUtility.IconContent( "d_TreeEditor.Duplicate" ).image,
                        tooltip = "Copy config values to clipboard."
                    };
                }
                return m_copyConfigContent;
            }
        }

        private static GUIContent ApplyConfigContent
        {
            get
            {
                if ( m_applyConfigContent == null )
                {
                    m_applyConfigContent = new GUIContent()
                    {
                        text = " Apply Config",
                        image = EditorGUIUtility.IconContent( "d_editicon.sml" ).image,
                        tooltip = "Replace config values with values from clipboard."
                    };
                }
                return m_applyConfigContent;
            }
        }

        public override void OnInspectorGUI()
        {
            using( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
            {
                DrawHeaderPanel();
                if ( m_showConfigEditor )
                {
                    HEditorGUI.HorizontalSeparator();
                    OnConfigGUI();
                }
            }
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
        
        private void DrawHeaderPanel()
        {
            using( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.LabelField( HeaderContent, EditorStyles.boldLabel );
                GUILayout.FlexibleSpace();
                if ( GUILayout.Button( m_showConfigEditor ? "Hide" : "Show", EditorStyles.miniButton ) )
                {
                    m_showConfigEditor = !m_showConfigEditor;
                }
            }
        }

        protected virtual void OnConfigGUI()
        {
            using( new GUILayout.HorizontalScope() )
            {
                if ( GUILayout.Button( CopyConfigContent, GUILayout.Height( 22f ) ) )
                {
                    var configData = JsonUtility.ToJson( serializedObject.targetObject );
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
                    var config = ( AbstractConfig ) target;
                    config.ApplyJson( EditorGUIUtility.systemCopyBuffer );
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}