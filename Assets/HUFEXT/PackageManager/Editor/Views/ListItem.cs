using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public abstract class ListItem
    {
        const  float    ITEM_HEIGHT = 30f;
        static GUIStyle NO_MARGINS  = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            
        public virtual PackageListView.ItemType Type   { get; }
        public PackageManagerWindow Window { private set; get; }

        public ListItem( PackageManagerWindow window )
        {
            Window = window;
        }
        
        public virtual void DrawContent( Rect rect ) { }

        public void Draw()
        {
            using ( var v = new EditorGUILayout.VerticalScope( NO_MARGINS, GUILayout.Height( ITEM_HEIGHT ) ) )
            {
                DrawContent( v.rect );
            }
            HGUI.HorizontalSeparator();
        }
    }
}
