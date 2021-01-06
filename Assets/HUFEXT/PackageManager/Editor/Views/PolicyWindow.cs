using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PolicyWindow : EditorWindow
    {
        Vector2 scroll;
        string license;
        string developerId = string.Empty;
        string accessKey = string.Empty;
        bool licenseAccepted;
        bool waitForValidation = false;
        
        public static void Init()
        {
            if ( !File.Exists( Models.Keys.Views.Policy.LICENSE_PATH ) )
            {
                Utils.Common.LogError( "Unable to find LICENSE file. Please re-install package." );
                return;
            }
            
            var window = CreateInstance( typeof( PolicyWindow ) ) as PolicyWindow;
            if ( window == null )
            {
                return;
            }
            
            window.titleContent = new GUIContent( Models.Keys.Views.Policy.TITLE );
            window.minSize = new Vector2( 500f, 480f );
            window.ShowUtility();
        }
        
        void OnGUI()
        {
            if ( !LoadLicense() )
            {
                Close();
                return;
            }
            
            Utils.HGUI.BannerWithLogo( position.width );
            using ( new GUILayout.AreaScope( new Rect( 0, 80, position.width, position.height - 80 ) ) )
            {
                EditorGUILayout.Space();
                DrawLicense();
                GUILayout.FlexibleSpace();
                using( new GUILayout.HorizontalScope() )
                {
                    GUILayout.FlexibleSpace();
                    DrawCredentials();
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }
        }

        bool LoadLicense()
        {
            if ( !string.IsNullOrEmpty( license ) )
            {
                return true;
            }
            
            if ( File.Exists( Models.Keys.Views.Policy.LICENSE_PATH ) )
            {
                license = File.ReadAllText( Models.Keys.Views.Policy.LICENSE_PATH );
                return true;
            }
                
            Utils.Common.LogError( "Unable to find LICENSE file. Please re-install package." );
            return false;
        }
        
        void DrawLicense()
        {
            EditorGUILayout.LabelField( Models.Keys.Views.Policy.LICENSE, EditorStyles.boldLabel );
            Utils.HGUI.HorizontalSeparator();
            using( var v = new EditorGUILayout.VerticalScope() )
            {
                EditorGUI.DrawRect( v.rect, new Color( 0f, 0f, 0f, 0.3f ) );
                var height = GUILayout.Height( position.height - 300f );
                using ( var s = new EditorGUILayout.ScrollViewScope( scroll, height ) )
                {
                    scroll = s.scrollPosition;
                    GUILayout.Label( license, new GUIStyle( EditorStyles.label )
                    {
                        wordWrap = true, 
                        fontSize = 12
                    } );
                }
            }
            Utils.HGUI.HorizontalSeparator();
        }

        void DrawCredentials()
        {
            using( new EditorGUI.DisabledGroupScope( waitForValidation ) )
            {
                using ( new EditorGUILayout.VerticalScope(  EditorStyles.helpBox, GUILayout.Width( 350f ) ) )
                {
                    EditorGUILayout.PrefixLabel( "Developer ID" );
                    developerId = GUILayout.TextField( developerId );

                    EditorGUILayout.PrefixLabel( "Access Token" );
                    accessKey = GUILayout.PasswordField( accessKey, '*' );

                    EditorGUILayout.Space();
                    using( new GUILayout.HorizontalScope() )
                    {
                        GUILayout.FlexibleSpace();
                        licenseAccepted = GUILayout.Toggle( licenseAccepted, Models.Keys.Views.Policy.CHECKBOX );
                        GUILayout.FlexibleSpace();
                    }

                    EditorGUILayout.Space();
                    using ( new EditorGUI.DisabledGroupScope( !licenseAccepted ) )
                    {
                        using ( new GUILayout.HorizontalScope() )
                        {
                            GUILayout.FlexibleSpace();
                            var label = waitForValidation ? Models.Keys.Views.Policy.VALIDATE : Models.Keys.Views.Policy.BUTTON;
                            if ( GUILayout.Button( label, GUILayout.Width( 150f ), GUILayout.Height( 30f ) ) )
                            {
                                OnProceedRequest();
                            }

                            GUILayout.FlexibleSpace();
                        }
                    }

                    EditorGUILayout.Space();
                }
            }
        }
        
        void OnProceedRequest()
        {
            if( !licenseAccepted )
            {
                return;
            }

            waitForValidation = true;
            if ( !Models.Token.CreateUnsignedToken( developerId, accessKey ) )
            {
                OnAuthorizationFailed();
                return;
            }
            
            Core.Command.Execute( new Commands.Connection.AuthorizeTokenCommand()
            {
                OnComplete = ( success, msg ) =>
                {
                    if ( !success )
                    {
                        OnAuthorizationFailed();
                        return;
                    }

                    Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand() );
                    EditorApplication.ExecuteMenuItem( Models.Keys.MENU_ITEM_OPEN_PACKAGE_MANAGER );
                    Close();
                }
            });
        }

        void OnAuthorizationFailed()
        {
            waitForValidation = false;
            EditorUtility.DisplayDialog( Models.Keys.Views.Policy.ERROR_TITLE, 
                                         Models.Keys.Views.Policy.ERROR_DESC,
                                         Models.Keys.Views.Policy.ERROR_BUTTON );
        }
    }
}
