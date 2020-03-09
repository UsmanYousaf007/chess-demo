using System.IO;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PolicyWindow : EditorWindow
    {
        // Strings
        private const string windowTitle = "Huuuge Unity Framework - Accept license and import our package manager";
        private const string licenseLabel = "Please read and accept our license:";
        private const string checkboxLabel = " I accept the terms and conditions";
        private const string buttonLabel = "Proceed";
        private const string validateLabel = "Validating...";
        private const string errorDialogTitle = "Error...";
        private const string errorDialogDescription = "You must accept our license and enter valid credentials.";
        private const string errorDialogButton = "OK";
        private string license;
        
        private Vector2 scroll;
        private bool licenseAccepted;

        private string developerId = string.Empty;
        private string accessKey = string.Empty;
        private bool waitForValidation = false;
        
        public static void Init()
        {
            var window = CreateInstance( typeof( PolicyWindow ) ) as PolicyWindow;
            if ( window != null )
            {
                window.titleContent = new GUIContent( windowTitle );
                window.minSize = new Vector2( 500f, 480f );
                window.ShowUtility();
            }
        }
        
        private void LoadLicenseText( bool force = false )
        {
            if ( string.IsNullOrEmpty( license ) || force )
            {
                license = File.ReadAllText( Registry.Resources.LICENSE_TEXT );
            }
        }
        
        private void OnEnable()
        {
            LoadLicenseText( true );
        }

        private void OnFocus()
        {
            LoadLicenseText();
        }

        void OnGUI()
        {
            HCommonGUI.BannerWithLogo( position.width );
            GUILayout.BeginArea(new Rect(0, 80, position.width, position.height - 80));
            {
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
            GUILayout.EndArea();
        }
        
        private void DrawLicense()
        {
            if ( string.IsNullOrEmpty( license ) )
            {
                licenseAccepted = true;
                GUILayout.FlexibleSpace();
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField( licenseLabel, EditorStyles.boldLabel );
            HCommonGUI.HorizontalSeparator();

            var rect = EditorGUILayout.BeginVertical();
            {
                EditorGUI.DrawRect( rect, new Color( 0f, 0f, 0f, 0.3f ) );
                scroll = EditorGUILayout.BeginScrollView( scroll, GUILayout.Height( position.height - 300f ) );
                {
                    GUILayout.Label( license, new GUIStyle( EditorStyles.label )
                    {
                        wordWrap = true, 
                        fontSize = 12
                    } );
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
            HCommonGUI.HorizontalSeparator();
        }

        private void DrawCredentials()
        {
            EditorGUI.BeginDisabledGroup( waitForValidation );
            {
                GUILayout.BeginVertical( EditorStyles.helpBox, GUILayout.Width( 320f ) );
                {
                    EditorGUILayout.PrefixLabel( "Developer ID" );
                    developerId = GUILayout.TextField( developerId );

                    EditorGUILayout.PrefixLabel( "Access Token" );
                    accessKey = GUILayout.PasswordField( accessKey, '*' );

                    HCommonGUI.HorizontalCentered( () =>
                    {
                        if ( !string.IsNullOrEmpty( license ) )
                        {
                            licenseAccepted = GUILayout.Toggle( licenseAccepted, checkboxLabel );
                        }
                    } );

                    HCommonGUI.HorizontalCentered( () =>
                    {
                        var label = waitForValidation ? validateLabel : buttonLabel;
                        if ( GUILayout.Button( label, GUILayout.Width( 150f ), GUILayout.Height( 30f ) ) )
                        {
                            OnProceedRequest();
                        }
                    } );

                    EditorGUILayout.Space();
                }
                GUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();
        }
        
        private void OnProceedRequest()
        {
            waitForValidation = true;

            var auth = Token.Create( developerId, accessKey, licenseAccepted );
            auth.Validate( ( success ) =>
            {
                if ( !success )
                {
                    ShowErrorDialog();
                    auth.Invalidate();
                    return;
                }
                
                Close();
                AssetDatabase.Refresh();
                EditorApplication.ExecuteMenuItem( Registry.MenuItems.PACKAGE_MANAGER );
            });
        }

        void ShowErrorDialog()
        {
            waitForValidation = false;
            EditorUtility.DisplayDialog( errorDialogTitle, errorDialogDescription, errorDialogButton );
        }
    }
}
