using System;
using HUFEXT.PackageManager.Editor.API.Views;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static HUFEXT.PackageManager.Editor.API.Data.PackageManagerState;
using Cache = HUFEXT.PackageManager.Editor.Utils.Cache;


namespace HUFEXT.PackageManager.Editor.Views
{
    public class ToolbarView : IPackageManagerView
    {
        private PackageManagerWindow window;

        public PackageManagerWindow Window => window;

        public event UnityAction OnRefreshRequest;
        //public event UnityAction OnUpdatePackagesRequest;
        public event UnityAction<bool> OnGenerateBuildReportRequest;
        public event UnityAction OnContactWithSupportRequest;
        public event UnityAction OnInvalidateTokenRequest;

        #region Content
        private GUIContent copyButtonContent;
        private GUIContent refreshButtonContent;
        private GUIContent settingsButtonContent;
        private const string SEARCH_FIELD_NAME = "HUF_PM_Toolbar_Search";
        #endregion
        
        public void Initialize( PackageManagerWindow parent )
        {
            window = parent;
            
            copyButtonContent = new GUIContent()
            {
                image = EditorGUIUtility.IconContent( "d_TreeEditor.Duplicate" ).image,
                tooltip = "Copy your developer ID to clipboard."
            };
            
            refreshButtonContent = new GUIContent()
            {
                image = EditorGUIUtility.IconContent("d_Refresh" ).image,
                tooltip = "Fetch packages info from remote server."
            };

            settingsButtonContent = new GUIContent()
            {
                image = EditorGUIUtility.IconContent( "_Popup" ).image,
                tooltip = "Settings for package manager."
            };
        }
        
        public void Repaint()
        {
            using( new GUILayout.HorizontalScope( EditorStyles.toolbar ) )
            {
                DrawDeveloperInfo();
                DrawSortingTypeDropdown();
                DrawCategoryTypeDropdown();
                DrawRefreshLabel();
                GUILayout.FlexibleSpace();
                DrawSearchBar();
                DrawSettingsDropdown();
            }
        }
        
        void DrawDeveloperInfo()
        {
            GUILayout.Label( $"Developer ID: {Window.state.developerId}", EditorStyles.toolbarButton );
            
            if ( GUILayout.Button( copyButtonContent, EditorStyles.toolbarButton ) )
            {
                GUIUtility.systemCopyBuffer = Window.state.developerId;
                Debug.Log( $"Your developer ID was copied to clipboard: {GUIUtility.systemCopyBuffer}" );
            }
        }

        void DrawSortingTypeDropdown()
        {
            Window.state.sortingType = (SortingType) EditorGUILayout.EnumPopup( Window.state.sortingType,
                                                                     EditorStyles.toolbarDropDown,
                                                                     GUILayout.Width( 110f ) );
        }

        void DrawCategoryTypeDropdown()
        {
            Window.state.categoryType = ( CategoryType ) EditorGUILayout.EnumPopup( Window.state.categoryType,
                                                                                    EditorStyles.toolbarDropDown,
                                                                                    GUILayout.Width( 60f ) );
        }
        
        void DrawRefreshLabel()
        {
            GUILayout.Label( $"Last update: {Window.state.lastFetchDate}", EditorStyles.toolbarButton );
            if ( GUILayout.Button( refreshButtonContent, EditorStyles.toolbarButton ) )
            {
                OnRefreshRequest?.Invoke();
            }
        }
        
        void DrawSettingsDropdown()
        {
            if ( GUILayout.Button( settingsButtonContent, EditorStyles.toolbarDropDown ) )
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem( new GUIContent("Show preview packages"), Window.state.showPreviewPackages, () =>
                {
                    Window.state.showPreviewPackages = !Window.state.showPreviewPackages;
                } );

                /*menu.AddItem( new GUIContent( "Update packages..." ), false, () =>
                {
                    OnUpdatePackagesRequest?.Invoke();
                } );*/

                menu.AddSeparator( "" );
                
                menu.AddItem( new GUIContent( "Generate HUF report/Full Log" ), false, () =>
                {
                    OnGenerateBuildReportRequest?.Invoke( true );
                } );
                
                menu.AddItem( new GUIContent( "Generate HUF report/Only HUF Packages" ), false, () =>
                {
                    OnGenerateBuildReportRequest?.Invoke( false );
                } );
                
                menu.AddItem( new GUIContent( "Help" ), false, () =>
                {
                    OnContactWithSupportRequest?.Invoke();
                } );
                
                menu.AddSeparator( "" );
                
                menu.AddItem( new GUIContent("Clear packages cache"), false, () =>
                {
                    Cache.RemoveFromCache( Registry.Keys.PACKAGES_CACHE_LIST_KEY );
                    PackageManagerWindow.SetDirtyFlag();
                });
                
                menu.AddSeparator( "" );
                
                menu.AddItem( new GUIContent( "Invalidate Token" ), false, () =>
                {
                    OnInvalidateTokenRequest?.Invoke();
                } );
                
                menu.ShowAsContext();
            }
        }
        
        void DrawSearchBar()
        {
            if ( Window == null )
            {
                return;
            }

            using ( new GUILayout.HorizontalScope( EditorStyles.toolbar, GUILayout.Width( 300f ) ) )
            {
                GUI.SetNextControlName( SEARCH_FIELD_NAME );
                
                var style = GUI.skin.FindStyle( "ToolbarSeachTextField" ) ?? new GUIStyle( "toolbarTextField" );
                Window.SearchText = GUILayout.TextField( Window.SearchText, style,GUILayout.Width( 300f ) );
                
                var focused = String.Compare( GUI.GetNameOfFocusedControl(),
                                              SEARCH_FIELD_NAME,
                                              StringComparison.Ordinal ) == 0;
                
                if ( focused && Event.current.type == EventType.MouseDown )
                {
                    GUI.FocusControl( null );
                    Window.Repaint();
                }
            }
        }
    }
}
