using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public abstract class ListItem
    {
        static GUIStyle NO_MARGINS = new GUIStyle {margin = new RectOffset( 0, 0, 0, 0 )};

        public ListItem( PackageManagerWindow window )
        {
            Window = window;
        }

        public virtual PackageListView.ItemType Type { get; }
        public PackageManagerWindow Window { private set; get; }

        public void Draw()
        {
            using ( var v = new EditorGUILayout.VerticalScope( NO_MARGINS, Models.Keys.GUI.mediumButtonHeight ) )
            {
                DrawContent( v.rect );
            }

            HGUI.HorizontalSeparator();
        }

        public virtual void DrawContent( Rect rect ) { }
    }
}