using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views.Items
{
    public class UnityPackageListItem : ListItem
    {
        static GUIStyle style = new GUIStyle( EditorStyles.label )
        {
            fontSize = 11,
            fontStyle = FontStyle.Normal,
            richText = true
        };

        UnityEditor.PackageManager.PackageInfo package;

        public UnityPackageListItem( PackageManagerWindow window, UnityEditor.PackageManager.PackageInfo package ) :
            base( window )
        {
            this.package = package;
        }

        public override void DrawContent( Rect rect )
        {
            if ( package == null )
            {
                GUILayout.Label( "<color=red>Null package reference.</color>", style );
                return;
            }
        }
    }
}