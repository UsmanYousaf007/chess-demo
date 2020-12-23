using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views.Editors
{
    public class PackageEditorWindow : EditorWindow
    {
        [SerializeField] Models.PackageManifest manifest;

        Vector2 scrollPosition;
        UnityEditor.Editor editor;
        PackageManagerWindow Parent { set; get; }

        public static void Init( PackageManagerWindow parent )
        {
            var window = CreateInstance( typeof(PackageEditorWindow) ) as PackageEditorWindow;

            if ( window != null )
            {
                window.Parent = parent;
                window.titleContent = new GUIContent( Models.Keys.Views.VersionEditor.TITLE );
                window.minSize = new Vector2( 430f, 400f );
                window.InitManifest( parent.state.selectedPackage );
                window.ShowUtility();
            }
        }

        void InitManifest( Models.PackageManifest manifest )
        {
            this.manifest = manifest;
        }

        void OnGUI()
        {
            if ( Parent == null )
            {
                Utils.Common.LogError( "PackageManagerWindow is not specified." );
                Close();
            }

            Utils.HGUI.BannerWithLogo( position.width );

            using ( new GUILayout.AreaScope( new Rect( 0, 80, position.width, position.height - 80f ) ) )
            {
                using ( var v = new GUILayout.ScrollViewScope( scrollPosition ) )
                {
                    scrollPosition = v.scrollPosition;

                    if ( editor == null )
                    {
                        editor = UnityEditor.Editor.CreateEditor( this );
                    }

                    editor?.OnInspectorGUI();
                }

                Utils.HGUI.HorizontalSeparator();
                EditorGUILayout.Space();

                using ( new GUILayout.HorizontalScope() )
                {
                    if ( GUILayout.Button( "Apply manifest changes" ) )
                    {
                        Models.PackageManifest.SaveManifest( $"{manifest.huf.path}/package.json", manifest );
                        Models.PackageManifest.SaveConfig( $"{manifest.huf.path}/config.json", manifest.huf.config );
                    }

                    if ( GUILayout.Button( "Build" ) )
                    {
                        manifest.huf.rollout = "Development";
                        manifest.huf.prerelease = "develop";
                        manifest.version = $"{manifest.huf.version}-{manifest.huf.prerelease}";

                        Parent.RegisterEvent( new Models.PackageManagerViewEvent
                        {
                            owner = Models.PackageManagerViewType.Unknown,
                            eventType = Models.EventType.BuildSelectedPackage
                        } );
                    }
                }

                EditorGUILayout.Space();
            }
        }

        [CustomEditor( typeof(PackageEditorWindow), true )]
        public class PackageEditorDrawer : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var manifest = serializedObject.FindProperty( nameof(PackageEditorWindow.manifest) );
                EditorGUI.BeginChangeCheck();

                if ( manifest != null )
                {
                    EditorGUILayout.PropertyField( manifest, new GUIContent( "Edit manifest" ), true );
                }

                if ( EditorGUI.EndChangeCheck() )
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}