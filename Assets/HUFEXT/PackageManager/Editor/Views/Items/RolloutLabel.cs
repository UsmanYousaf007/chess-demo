using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views.Items
{
    public class RolloutLabel : ListItem
    {
        public readonly Color color = Color.clear;
        public readonly string label = "Unknown";
        public readonly string tooltip = string.Empty;

        public RolloutLabel( string label, Color color ) : base( null )
        {
            this.label = label;
            this.color = color;
        }

        public override PackageListView.ItemType Type => PackageListView.ItemType.RolloutTag;

        public override void DrawContent( Rect rect )
        {
            GUILayout.FlexibleSpace();
            EditorGUI.DrawRect( rect, color );

            GUILayout.Label( new GUIContent( label, tooltip ),
                new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                {
                    normal = {textColor = Color.white},
                    fontStyle = FontStyle.Bold
                } );
            GUILayout.FlexibleSpace();
        }
    }
}