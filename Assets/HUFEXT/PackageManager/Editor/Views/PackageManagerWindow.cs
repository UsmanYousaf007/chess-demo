using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HUFEXT.PackageManager.Editor.API.Controllers;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.Implementation.Local.Services;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using HUFEXT.PackageManager.Editor.Utils;
using HUFEXT.PackageManager.Editor.Utils.Helpers;
using UnityEditor;
using UnityEngine;
using Cache = HUFEXT.PackageManager.Editor.Utils.Cache;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageManagerWindow : EditorWindow
    {
        [SerializeField] public PackageManagerState state;
        [SerializeField] public PackageController controller;
        
        private ToolbarView toolbarView;
        private PackageListView packageListView;
        private PackageView packageView;
        private static int nextUpdateDelayInSeconds = 4 * 60 * 60;
            
        public static bool IsOpened = false;
        public string SearchText { get; set; }
        
        [MenuItem( Registry.MenuItems.PACKAGE_MANAGER )]
        static void Init()
        {
            var token = Token.LoadExistingToken();
            if ( token != null && token.IsValidated )
            {
                PlayerPrefs.DeleteKey( Registry.Keys.PACKAGE_MANAGER_FORCE_CLOSE );
                
                var window = GetWindow<PackageManagerWindow>( false );
                if ( window != null )
                {
                    window.titleContent = new GUIContent()
                    {
                        text = " HUF Package Manager",
                        image = HCommonGUI.Icons.WindowIcon
                    };
                    window.minSize = new Vector2( 800f, 250f );
                    window.Show();
                }
            }
            else
            {
                //EditorApplication.ExecuteMenuItem( Registry.MenuItems.POLICY );
                PolicyWindow.Init();
            }
        }
        
        public static void SetDirtyFlag()
        {
            PlayerPrefs.SetInt( Registry.Keys.PACKAGE_MANAGER_DIRTY_FLAG, 1 );
        }

        public static void SetForceCloseFlag()
        {
            PlayerPrefs.SetInt( Registry.Keys.PACKAGE_MANAGER_FORCE_CLOSE, 1 );
        }
        
        private void OnEnable()
        {
            if ( state == null )
            {
                state = new PackageManagerState();
            }

            if( string.IsNullOrEmpty( state.developerId ) )
            {
                state.Load();
            }

            if( controller == null )
            {
                controller = PackageController.CreateOrLoadFromCache();
                controller.FetchLocalPackages();
                controller.FetchRemotePackages();
            }

            InitializeViews();
            
            controller.OnPackageListChanged += OnPackagesFetchHandler;

            IsOpened = true;
            
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            controller.OnPackageListChanged -= OnPackagesFetchHandler;
            state.Save();
            IsOpened = false;
        }
        
        public void SelectPackage( PackageManifest manifest )
        {
            state.selectedPackage = manifest;
            packageView?.FetchOtherVersions();
        }

        void OnPackagesFetchHandler( PackageController.UpdateType updateType )
        {
            state.lastFetchDate = DateTime.Now.ToString( CultureInfo.CurrentCulture );
            state.nextFetchTimestamp = GetTimestamp( nextUpdateDelayInSeconds );
            
            if ( state.selectedPackage != null )
            {
                var package = controller.Packages.Find( ( x ) => x.name == state.selectedPackage.name );
                SelectPackage( package );
            }
            
            Repaint();
        }

        void InitializeViews()
        {
            if ( toolbarView == null )
            {
                toolbarView = new ToolbarView();
                toolbarView.Initialize( this );
                toolbarView.OnRefreshRequest += OnRefresh;
                //toolbarView.OnUpdatePackagesRequest += PackageUpdateWindow.Init;
                toolbarView.OnGenerateBuildReportRequest += OnGenerateBuildReportHandler;
                toolbarView.OnContactWithSupportRequest += OnHelpRequestHandler;
                toolbarView.OnInvalidateTokenRequest += OnInvalidateTokenHandler;
            }

            if ( packageListView == null )
            {
                packageListView = new PackageListView();
                packageListView.Initialize( this );
            }

            if ( packageView == null )
            {
                packageView = new PackageView();
                packageView.Initialize( this );
                packageView.OnInstallPackageRequest += controller.InstallPackageRequest;
                packageView.OnRemovePackageRequest += ( name ) =>
                {
                    controller?.RemovePackageRequest( name );
                    state.selectedPackage = null;
                };
            }
        }

        void OnGUI()
        {
            toolbarView?.Repaint();
            using( new GUILayout.HorizontalScope() )
            {
                packageListView?.Repaint();
                packageView?.Repaint();
            }
        }

        void OnEditorUpdate()
        {
            if( PlayerPrefs.HasKey( Registry.Keys.PACKAGE_MANAGER_DIRTY_FLAG ) )
            {
                controller?.Refresh();
                PlayerPrefs.DeleteKey( Registry.Keys.PACKAGE_MANAGER_DIRTY_FLAG );
            }

            if ( PlayerPrefs.HasKey( Registry.Keys.PACKAGE_MANAGER_FORCE_CLOSE ) )
            {
                PlayerPrefs.DeleteKey( Registry.Keys.PACKAGE_MANAGER_FORCE_CLOSE );
                OnInvalidateTokenHandler();
            }

            if ( state != null )
            {
                if( state.selectedPackage == null && controller?.Packages.Count > 0 )
                {
                    SelectPackage( controller.Packages[0] );
                }

                if ( GetTimestamp() >= state.nextFetchTimestamp )
                {
                    controller?.Refresh();
                    state.nextFetchTimestamp = GetTimestamp( nextUpdateDelayInSeconds );
                }
            }
        }

        #region ToolbarViewBindings

        private void OnRefresh()
        {
            state.lastFetchDate = "Updating...";
            controller.Refresh( PackageController.RefreshType.Clear | PackageController.RefreshType.FetchAll );
        }

        void OnGenerateBuildReportHandler( bool fullLog )
        {
            if ( fullLog )
            {
                BuildReportPreprocess.GenerateBuildInfo( ( report ) =>
                {
                    var serializedReport = BuildReportPreprocess.SerializeReport( report );
                    Debug.Log( "Build report: \n" + serializedReport );
                    GUIUtility.systemCopyBuffer = serializedReport;
                    Debug.Log( "Build report copied to clipboard." );
                } );
            }
            else
            {
                List<string> packageLog = new List<string>();
                new LocalPackagesService().RequestPackagesList( string.Empty, ( localPackages ) =>
                {
                    var builder = new StringBuilder();
                    foreach ( var package in localPackages )
                    {
                        builder.Append( $"{package.displayName} {package.version}\n" );
                    }

                    var log = builder.ToString();
                    Debug.Log( $"HUF Packages installed in {PlayerSettings.applicationIdentifier}: \n" + log );
                    GUIUtility.systemCopyBuffer = log;
                    Debug.Log( "Build report copied to clipboard." );
                } );
            }
        }

        void OnHelpRequestHandler()
        {
            Application.OpenURL( Registry.Urls.CONTACT_SUPPORT_URL + state.developerId );
        }

        void OnInvalidateTokenHandler()
        {
            state.Invalidate();
            Cache.RemoveFromCache( Registry.Keys.PACKAGES_CACHE_LIST_KEY );
            controller?.Refresh( PackageController.RefreshType.Clear );
            Close();
        }

        private int GetTimestamp( int delay = 0 )
        {
            return ( int ) ( DateTime.UtcNow
                                     .AddSeconds( delay )
                                     .Subtract( new DateTime( 1970, 1, 1 ) ) )
                                     .TotalSeconds;
        }
        #endregion
    }
}
