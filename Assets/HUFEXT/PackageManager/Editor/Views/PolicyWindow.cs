using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PolicyWindow : EditorWindow
    {
        const float MINIMUM_WINDOW_WIDTH = 500f;
        const float MINIMUM_WINDOW_HEIGHT = 480f;
        const float BANNER_LOGO_HEIGHT = 80f;
        const float BOTTOM_SECTION_HEIGHT = 300f;
        const float CREDENTIALS_PANEL_WIDTH = 350f;
        const float PROCEED_BUTTON_WIDTH = 150f;
        const float PROCEED_BUTTON_HEIGHT = 30f;
        const int LICENSE_FONT_SIZE = 12;

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

            var window = CreateInstance( typeof(PolicyWindow) ) as PolicyWindow;

            if ( window == null )
            {
                return;
            }

            window.titleContent = new GUIContent( Models.Keys.Views.Policy.TITLE );
            window.minSize = new Vector2( MINIMUM_WINDOW_WIDTH, MINIMUM_WINDOW_HEIGHT );
            window.ShowUtility();
        }

        void OnGUI()
        {
            if ( !LoadLicense() )
            {
                Close();
                return;
            }

            Utils.HGUI.BannerWithLogo( position.width, BANNER_LOGO_HEIGHT );

            using ( new GUILayout.AreaScope( new Rect( 0,
                BANNER_LOGO_HEIGHT,
                position.width,
                position.height - BANNER_LOGO_HEIGHT ) ) )
            {
                EditorGUILayout.Space();
                DrawLicense();
                GUILayout.FlexibleSpace();

                using ( new GUILayout.HorizontalScope() )
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

            using ( var v = new EditorGUILayout.VerticalScope() )
            {
                EditorGUI.DrawRect( v.rect, new Color( 0f, 0f, 0f, 0.3f ) );
                var height = GUILayout.Height( position.height - BOTTOM_SECTION_HEIGHT );

                using ( var s = new EditorGUILayout.ScrollViewScope( scroll, height ) )
                {
                    scroll = s.scrollPosition;

                    GUILayout.Label( license,
                        new GUIStyle( EditorStyles.label )
                        {
                            wordWrap = true,
                            fontSize = LICENSE_FONT_SIZE
                        } );
                }
            }

            Utils.HGUI.HorizontalSeparator();
        }

        void DrawCredentials()
        {
            using ( new EditorGUI.DisabledGroupScope( waitForValidation ) )
            {
                using ( new EditorGUILayout.VerticalScope( EditorStyles.helpBox,
                    GUILayout.Width( CREDENTIALS_PANEL_WIDTH ) ) )
                {
                    EditorGUILayout.PrefixLabel( "Developer ID" );
                    developerId = GUILayout.TextField( developerId );
                    EditorGUILayout.PrefixLabel( "Access Token" );
                    accessKey = GUILayout.PasswordField( accessKey, '*' );
                    EditorGUILayout.Space();

                    using ( new GUILayout.HorizontalScope() )
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

                            var label = waitForValidation
                                ? Models.Keys.Views.Policy.VALIDATE
                                : Models.Keys.Views.Policy.BUTTON;

                            if ( GUILayout.Button( label, GUILayout.Width( PROCEED_BUTTON_WIDTH ), GUILayout.Height( PROCEED_BUTTON_HEIGHT ) ) )
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
            if ( !licenseAccepted )
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
                OnComplete = ( success, serializedData ) =>
                {
                    if ( !success )
                    {
                        OnAuthorizationFailed( serializedData );
                        return;
                    }

                    Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand() );
                    EditorApplication.ExecuteMenuItem( Models.Keys.MENU_ITEM_OPEN_PACKAGE_MANAGER );
                    Close();
                }
            } );
        }

        void OnAuthorizationFailed( string serializedData = "" )
        {
            waitForValidation = false;

            EditorUtility.DisplayDialog( Models.Keys.Views.Policy.ERROR_TITLE,
                serializedData == Core.RequestStatus.Unauthorized.ToString()
                    ? Models.Keys.Views.Policy.VALIDATION_ERROR_DESC
                    : Models.Keys.Views.Policy.CONNECTION_ERROR_DESC,
                Models.Keys.Views.Policy.ERROR_BUTTON );
        }
    }
}