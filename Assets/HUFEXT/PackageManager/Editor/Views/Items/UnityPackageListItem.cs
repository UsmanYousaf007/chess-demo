using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views.Items
{
    public class UnityPackageListItem : ListItem
    {
        private UnityEditor.PackageManager.PackageInfo package;

        private static GUIStyle style = new GUIStyle( EditorStyles.label )
        {
            fontSize  = 11,
            fontStyle = FontStyle.Normal,
            richText  = true
        };
        
        public UnityPackageListItem( PackageManagerWindow window, UnityEditor.PackageManager.PackageInfo package ) : base( window )
        {
            this.package = package;
        }

        public override void DrawContent( Rect rect )
        {
            if ( package == null )
            {
                GUILayout.Label( "<color=red>Null manifest reference.</color>", style );
                return;
            }
        }
    }
}
