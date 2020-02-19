using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor
{
    public static class HEditorGUI
    {
        private const string WINDOW_ICON_PATH = "Assets/HUF/Utils/Editor/Resources/Icons/huf_icon.png";
        private const string BANNER_LOGO_PATH = "Assets/HUF/Utils/Editor/Resources/Common/huf_logo.png";
        
        private static Texture2D cachedBanner = null;
        
        public static Color BrandColor => new Color( 238f / 255f, 48f / 255f, 56f / 255f );

        public static class Res
        {
            public static Texture2D WindowIcon => EditorGUIUtility.Load( WINDOW_ICON_PATH ) as Texture2D;
            public static Texture2D BannerLogo => EditorGUIUtility.Load( BANNER_LOGO_PATH ) as Texture2D;
        }
        
        public static void VerticalSeparator()
        {
            Separator( new GUIStyle
            {
                normal = { background = EditorGUIUtility.whiteTexture },
                margin = new RectOffset( 0, 0, 0, 0 ),
                fixedWidth = 1,
                stretchHeight = true
            } );
        }

        public static void HorizontalSeparator()
        {
            Separator( new GUIStyle
            {
                normal = { background = EditorGUIUtility.whiteTexture },
                margin = new RectOffset( 0, 0, 0, 0 ),
                fixedHeight = 1,
                stretchWidth = true
            } );
        }
        
        static void Separator( GUIStyle style )
        {
            var c = GUI.color;
            GUI.color = new Color ( 0f, 0f, 0f, 0.3f );
            GUILayout.Box( GUIContent.none, style );
            GUI.color = c;
        }
        
        public static void BannerWithLogo( float width, float height = 80f )
        {
            if ( cachedBanner == null )
            {
                cachedBanner = Res.BannerLogo;
            }
            
            EditorGUI.DrawRect( new Rect( 0, 0, width, height ), BrandColor );
            EditorGUI.DrawPreviewTexture( new Rect( ( height - 80 ) / 2, 0, 144, 80 ), cachedBanner );
        }
    }
}
