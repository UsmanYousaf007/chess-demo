using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageVersionEditor : EditorWindow
    {
        PackageManagerWindow Parent { set; get; }
        
        public static void Init( PackageManagerWindow parent )
        {
            var window = CreateInstance( typeof( PackageVersionEditor ) ) as PackageVersionEditor;
            if ( window != null )
            {
                window.Parent = parent;
                window.titleContent = new GUIContent( Models.Keys.Views.VersionEditor.TITLE );
                window.minSize      = new Vector2( 430f, 400f );
                window.ShowUtility();
            }
        }

        private void OnGUI()
        {
            if ( Parent == null )
            {
                Debug.LogError( "PackageManagerWindow is not specified." );
                Close();
            }
            
            Utils.HGUI.BannerWithLogo( position.width );
            using ( new GUILayout.AreaScope( new Rect( 0, 80, position.width, position.height - 80f ) ) )
            {
                using ( new GUILayout.HorizontalScope( EditorStyles.inspectorDefaultMargins ) )
                {
                    EditorGUILayout.LabelField( "Package: " + Parent.state.selectedPackage.name );
                }
            }
        }
    }
}
