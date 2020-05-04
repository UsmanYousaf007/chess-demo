using System;
using HUFEXT.PackageManager.Editor.Models;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class ToolbarView : PackageManagerView
    {
        const string SEARCH_FIELD_NAME = "HUF_PM_Toolbar_Search";

        readonly GUIContent copyButton;
        readonly GUIContent refreshButton;
        readonly GUIContent settingsButton;

        public ToolbarView( PackageManagerWindow window ) : base(window)
        {
            copyButton = EditorGUIUtility.IconContent( "d_TreeEditor.Duplicate", "Copy your developer ID to clipboard." );
            refreshButton = EditorGUIUtility.IconContent( "d_Refresh", "Fetch packages info from remote server." );
            settingsButton = EditorGUIUtility.IconContent( "_Popup", "Settings for package manager." );
        }

        protected override void OnGUI()
        {
            using( new GUILayout.HorizontalScope( EditorStyles.toolbar ) )
            {
                DrawCommonControls();
                GUILayout.FlexibleSpace();
                DrawSearchBar();
                DrawSettingsDropdown();
            }
        }
        
        void DrawCommonControls()
        {
            GUILayout.Label( $"Developer ID: {window.state.developerId}" );
                
            if ( GUILayout.Button( copyButton, EditorStyles.toolbarButton ) )
            {
                window.Enqueue( ViewEvent.CopyDeveloperID );
            }

            EditorGUI.BeginChangeCheck();
            
            window.state.sortingType = ( Models.PackageSortingType ) EditorGUILayout.EnumPopup( window.state.sortingType,
                                                                                         EditorStyles.toolbarDropDown,
                                                                                         GUILayout.Width( 110f ) );
                
            window.state.categoryType = ( Models.PackageCategoryType ) EditorGUILayout.EnumPopup( window.state.categoryType,
                                                                                                  EditorStyles.toolbarDropDown,
                                                                                                  GUILayout.Width( 110f ) );

            if ( EditorGUI.EndChangeCheck() )
            {
                window.Enqueue( ViewEvent.RefreshListView );
            }

            if ( Core.Packages.UpdateInProgress )
            {
                GUILayout.Label( "Update in progress..." );
            }
            else
            {
                GUILayout.Label( $"Last update: {window.state.lastFetchDate}" );
            }
            
            if ( GUILayout.Button( refreshButton, EditorStyles.toolbarButton ) )
            {
                window.Enqueue( ViewEvent.RefreshPackages );
            }
        }
        
        void DrawSettingsDropdown()
        {
            if ( GUILayout.Button( settingsButton, EditorStyles.toolbarDropDown ) )
            {
                var menu = new GenericMenu();

                RegisterMenuItem( ref menu, "Show preview packages", ViewEvent.ShowPreviewPackages, window.state.showPreviewPackages );
                
                menu.AddSeparator( "" );
                
                RegisterMenuItem( ref menu, "Update packages...", ViewEvent.ShowUpdateWindow );

                menu.AddSeparator( "" );
                
                RegisterMenuItem( ref menu, "Generate report/Only HUF", ViewEvent.GenerateReportHUF );
                RegisterMenuItem( ref menu, "Generate report/Full", ViewEvent.GenerateReportFull );
                RegisterMenuItem( ref menu, "Help", ViewEvent.ContactSupport );

                menu.AddSeparator( "" );
                
                //RegisterMenuItem( ref menu, "Force resolve", ViewEvent.ForceResolvePackages );
                RegisterMenuItem( ref menu, "Clear cache", ViewEvent.ClearCache );
                
                menu.AddSeparator( "" );
                
                menu.AddDisabledItem( new GUIContent( $"Current channel: {window.state.channel.ToString()}" ) );
                RegisterMenuItem( ref menu,
                                  window.state.channel == PackageChannel.Stable
                                      ? "Switch to Preview"
                                      : "Switch to Stable", ViewEvent.TogglePreviewOrStableChannel );

                menu.AddSeparator( "" );

                RegisterMenuItem( ref menu, "Revoke HUF license", ViewEvent.RevokeLicense );
                
#if HPM_DEV_MODE
                AddDebugOptions( menu );
#endif
                
                menu.ShowAsContext();
            }
        }

        void RegisterMenuItem( ref GenericMenu menu, string label, ViewEvent ev, bool enabled = false )
        {
            menu.AddItem( new GUIContent( label ), enabled, () => window.Enqueue( ev ) );
        }

        void DrawSearchBar()
        {
            using ( new GUILayout.HorizontalScope( EditorStyles.toolbar ) )
            {
                GUI.SetNextControlName( SEARCH_FIELD_NAME );
                
                window.state.searchText = GUILayout.TextField( window.state.searchText,
                                                         GUI.skin.FindStyle( "ToolbarSeachTextField" ) ??
                                                         new GUIStyle( "toolbarTextField" ),
                                                         GUILayout.Width( 200f ) );
                
                if ( string.Compare( GUI.GetNameOfFocusedControl(), SEARCH_FIELD_NAME, StringComparison.Ordinal ) == 0 && 
                     Event.current.type == EventType.MouseDown )
                {
                    GUI.FocusControl( null );
                    window.Repaint();
                }
            }
        }

        void AddDebugOptions( GenericMenu menu )
        {
            menu.AddSeparator( "" );
            menu.AddDisabledItem( new GUIContent( "Development Options" ) );
            menu.AddItem( new GUIContent("Ignore version tags"), window.state.ignoreVersionTags, () =>
            {
                window.state.ignoreVersionTags = !window.state.ignoreVersionTags;
                window.state.Save();
                Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand { downloadLatest = false } );
            } );
            menu.AddItem( new GUIContent("Enable debug logs"), window.state.enableDebugLogs, () =>
            {
                window.state.enableDebugLogs = !window.state.enableDebugLogs;
                window.state.Save();
                if ( window.state.enableDebugLogs )
                {
                    Core.Registry.Push( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS );
                }
                else
                {
                    Core.Registry.Pop( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS );
                }
            } );
            menu.AddItem( new GUIContent("Show policy window"), window.state.ignoreVersionTags, PolicyWindow.Init );
        }
    }
}
