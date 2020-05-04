using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.Utils
{
    internal static class HGUI
    {
        private static Texture2D cachedHUFLogoBanner = null;
        
        public static Color MainColor => new Color( 238f / 255f, 48f / 255f, 56f / 255f );

        public static Texture2D GetIcon( string path )
        {
            return EditorGUIUtility.Load( path ) as Texture2D;
        }

        public static class Icons
        {
            public static Texture2D WindowIcon => GetIcon( Models.Keys.Resources.Icons.WINDOW );
            public static Texture2D PackageInstalledIcon => GetIcon( Models.Keys.Resources.Icons.INSTALLED );
            public static Texture2D PackageNotInstalledIcon => GetIcon( Models.Keys.Resources.Icons.NOT_INSTALLED );
            public static Texture2D PackageUpdateIcon => GetIcon( Models.Keys.Resources.Icons.UPDATE );
            public static Texture2D PackageErrorIcon => GetIcon( Models.Keys.Resources.Icons.ERROR );
            public static Texture2D PackageForceUpdateIcon => GetIcon( Models.Keys.Resources.Icons.FORCE_UPDATE );
            public static Texture2D PackageMigrationIcon => GetIcon( Models.Keys.Resources.Icons.MIGRATION );
            public static Texture2D PackageConflictIcon => GetIcon( Models.Keys.Resources.Icons.CONFLICT );
        }

        public static class Banners
        {
            public static Texture2D HUFLogoBanner => GetIcon( Models.Keys.Resources.HUF_LOGO );
        }
        
        static void Separator( GUIStyle style )
        {
            var c = GUI.color;
            GUI.color = new Color ( 0f, 0f, 0f, 0.3f );
            GUILayout.Box( GUIContent.none, style );
            GUI.color = c;
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
        
        public static void Button( string name, UnityAction action, float width = 0.0f, float height = 0.0f )
        {
            var guiOptions = new List<GUILayoutOption>
            {
                GUILayout.ExpandWidth(false)
            };

            if (!Mathf.Approximately( width, 0.0f ))
            {
                guiOptions.Add( GUILayout.Width( width ) );
            }

            if (!Mathf.Approximately( height, 0.0f ))
            {
                guiOptions.Add( GUILayout.Height( height ) );
            }

            if (GUILayout.Button( name, guiOptions.ToArray() ))
            {
                action?.Invoke();
            }
        }
        
        public static void BannerWithLogo( float width, float height = 80f )
        {
            if ( cachedHUFLogoBanner == null )
            {
                cachedHUFLogoBanner = Banners.HUFLogoBanner;
            }
            
            EditorGUI.DrawRect( new Rect( 0, 0, width, height ), MainColor );
            EditorGUI.DrawPreviewTexture( new Rect( ( height - 80 ) / 2, 0, 144, 80 ), 
                                          cachedHUFLogoBanner );
        }
    }
}
