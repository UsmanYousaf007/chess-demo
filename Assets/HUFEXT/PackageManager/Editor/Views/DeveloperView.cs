using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#if HPM_DEV_MODE
namespace HUFEXT.PackageManager.Editor.Views
{
    [Serializable]
    public class DeveloperView : PackageManagerView
    {
        const string PREFS_KEY = "HUF.PackageManager.DevelopmentMode";
        
        [SerializeField] string localEnv;
        [SerializeField] bool buildMode;
        
        public DeveloperView( PackageManagerWindow parent ) : base( parent )
        {
            Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT, out localEnv );
        }

        protected override void OnGUI()
        {
            var noMargins = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            using ( var v = new EditorGUILayout.VerticalScope( noMargins, GUILayout.Height( 30f ) ) )
            {
                EditorGUILayout.Space();
                DrawDeveloperOptions( v.rect );
                EditorGUILayout.Space();
            }
            Utils.HGUI.HorizontalSeparator();
        }

        public override void RefreshView( ViewEvent ev )
        {
            Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT, out localEnv );
            if ( !Directory.Exists( localEnv ) )
            {
                localEnv = string.Empty;
            }
        }

        void DrawDeveloperOptions( Rect rect )
        {
            using ( new EditorGUILayout.HorizontalScope( ) ) 
            {
                EditorGUI.DrawRect( rect, new Color( 0f, 0f, 0f, 0.4f ) );
                GUILayout.Label( "Developer Mode" );
                
                if ( GUILayout.Button( "Disable", EditorStyles.miniButton ) )
                {
                    window.Enqueue( ViewEvent.DisableDeveloperMode );
                }
                
                GUILayout.Label( "| Channel:" );
                GUI.SetNextControlName( "HUF_ChannelToggle" );
                
                EditorGUI.BeginChangeCheck();
                window.state.channel = ( Models.PackageChannel ) EditorGUILayout.EnumPopup( window.state.channel,
                                                                                     GUILayout.Width( 100f ) );
                if ( EditorGUI.EndChangeCheck() )
                {
                    window.Enqueue( ViewEvent.ChangePackagesChannel );
                }
                
                if ( window.state.channel == Models.PackageChannel.Development )
                {
                    GUILayout.Label( "| Local Development Registry:" );
                    var buttonText = string.IsNullOrEmpty( localEnv ) ? "Not specified " : localEnv;

                    if ( buttonText.Length > 60 )
                    {
                        var start = buttonText.Length - 57;
                        buttonText = "..." + buttonText.Substring( start, buttonText.Length - start );
                    }
                    
                    if ( GUILayout.Button( buttonText, EditorStyles.miniPullDown ) )
                    {
                        window.Enqueue( ViewEvent.ChangeDevelopmentEnvPath, localEnv );
                    }
                }

                GUILayout.FlexibleSpace();

                if ( GUI.GetNameOfFocusedControl() == "HUF_ChannelToggle" )
                {
                    GUI.FocusControl( null );
                }
            }
        }
    }
}
#else
namespace HUFEXT.PackageManager.Editor.Views
{
    [Serializable]
    public class DeveloperView : PackageManagerView 
    {
        public DeveloperView( PackageManagerWindow parent ) : base( parent )
        {
        }
    }
}
#endif
