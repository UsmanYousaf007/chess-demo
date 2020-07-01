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
        public override Models.PackageManagerViewType Type => Models.PackageManagerViewType.DeveloperView;
        
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
        
        void DrawDeveloperOptions( Rect rect )
        {
            using ( new EditorGUILayout.HorizontalScope( ) ) 
            {
                EditorGUI.DrawRect( rect, new Color( 0f, 0f, 0f, 0.4f ) );
                GUILayout.Label( "Developer Mode" );
                
                if ( GUILayout.Button( "Disable", EditorStyles.miniButton ) )
                {
                    RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner     = Type,
                        eventType = Models.EventType.DisableDeveloperMode
                    } );
                }

                if ( window != null && window.state.selectedPackage != null )
                {
                    GUILayout.Label( "|" );

                    using ( new EditorGUI.DisabledScope( window.state.selectedPackage == null ) )
                    {
                        if ( GUILayout.Button( "Package Editor", EditorStyles.miniButton ) )
                        {
                            Editors.PackageEditorWindow.Init( window );
                        }
                    }
                }

                GUILayout.Label( "| Channel:" );
                GUI.SetNextControlName( "HUF_ChannelToggle" );
                
                EditorGUI.BeginChangeCheck();
                window.state.channel = ( Models.PackageChannel ) EditorGUILayout.EnumPopup( window.state.channel,
                                                                                     GUILayout.Width( 100f ) );
                if ( EditorGUI.EndChangeCheck() )
                {
                    RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner     = Type,
                        eventType = Models.EventType.ChangePackagesChannel
                    } );
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
                        //PackageManagerWindow.Enqueue( ViewEvent.ChangeDevelopmentEnvPath, localEnv );
                        RegisterEvent( new Models.PackageManagerViewEvent
                        {
                            owner     = Type,
                            eventType = Models.EventType.ChangeDevelopmentEnvPath,
                            data      = localEnv
                        } );
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
        public override Models.PackageManagerViewType Type => Models.PackageManagerViewType.DeveloperView;
        
        public DeveloperView( PackageManagerWindow parent ) : base( parent )
        {
        }
    }
}
#endif
