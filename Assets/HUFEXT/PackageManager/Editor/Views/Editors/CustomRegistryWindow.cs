using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    [CustomEditor( typeof(CustomRegistryWindow), true )]
    public class ScopedRegistryDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var list = serializedObject.FindProperty( "registries" );
            EditorGUI.BeginChangeCheck();

            if ( list != null )
            {
                EditorGUILayout.PropertyField( list, new GUIContent( "Registries" ), true );
            }

            if ( EditorGUI.EndChangeCheck() )
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public class CustomRegistryWindow : EditorWindow
    {
        [SerializeField] List<Models.ScopedRegistryWrapper> registries = new List<Models.ScopedRegistryWrapper>();

        Vector2 scrollPosition;
        UnityEditor.Editor editor;

        public static void Init()
        {
            var window = CreateInstance( typeof(CustomRegistryWindow) ) as CustomRegistryWindow;

            if ( window != null )
            {
                window.titleContent = new GUIContent( Models.Keys.Views.CustomRegistryEditor.TITLE );
                window.minSize = new Vector2( 430f, 400f );
                window.ShowUtility();
            }
        }

        void OnGUI()
        {
            Utils.HGUI.BannerWithLogo( position.width );

            using ( new GUILayout.AreaScope( new Rect( 0, 80, position.width, position.height - 80f ) ) )
            {
                using ( var v = new GUILayout.ScrollViewScope( scrollPosition, EditorStyles.inspectorDefaultMargins ) )
                {
                    scrollPosition = v.scrollPosition;

                    if ( editor == null )
                    {
                        editor = UnityEditor.Editor.CreateEditor( this );
                    }

                    editor?.OnInspectorGUI();
                }

                if ( GUILayout.Button( "Add Registries" ) )
                {
                    Core.Command.Execute( new Commands.Base.AddScopedRegistriesCommand
                    {
                        registries = registries
                    } );
                    Close();
                }
            }
        }
    }
}