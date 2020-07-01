using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageListView : PackageManagerView
    {
        public override Models.PackageManagerViewType Type => Models.PackageManagerViewType.PackageListView;
        
        const float ITEM_HEIGHT = 30f;
        const float VIEW_WIDTH = 310f;
        const string SEARCH_FIELD_NAME = "HUF_PM_Toolbar_Search";
        
        private static readonly Color labelColor = new Color( 0.27f, 0.27f, 0.27f );

        private bool isInitialized = false;
        private Vector2 scrollPosition;

        private readonly List<ListItem> baseItems = new List<ListItem>();
        private readonly List<ListItem> currentItems = new List<ListItem>();
        private readonly List<ListItem> unityItems = new List<ListItem>();
        
        bool refreshItemsList = false;
        bool regenerateItemsList = false;
        GUIStyle searchBarStyle;
        GUIStyle searchBarButtonStyle;
        
        UnityEditor.PackageManager.Requests.ListRequest unityListRequest;
        
        public enum ItemType
        {
            RolloutTag,
            PackageHUF,
            PackageUnity
        }
        
        public PackageListView( PackageManagerWindow parent ) : base( parent )
        {
            regenerateItemsList = true;
        }

        public override void OnEventCompleted( Models.PackageManagerViewEvent ev )
        {
            switch ( ev.eventType )
            {
                case Models.EventType.RefreshListView:
                case Models.EventType.ShowPreviewPackages:
                case Models.EventType.ShowUnityPackages:
                {
                    refreshItemsList = true;
                    break;
                }
                
                case Models.EventType.RefreshPackages:
                {
                    regenerateItemsList = true;
                    break;
                }
            }
        }

        protected override void OnGUI()
        {
            if ( !isInitialized || regenerateItemsList )
            {
                RegenerateItems();
                regenerateItemsList = false;
                isInitialized = true;
            }

            FetchUnityPackages();
            
            if ( refreshItemsList )
            {
                RefreshItems();
                refreshItemsList = false;
            }

            using ( new EditorGUILayout.VerticalScope() )
            {
                DrawSearchBar();

                using ( var scope =
                    new GUILayout.ScrollViewScope( scrollPosition, false, true, GUILayout.MinWidth( VIEW_WIDTH ) ) )
                {
                    scrollPosition = scope.scrollPosition;

                    if ( currentItems.Count == 0 )
                    {
                        using ( new EditorGUILayout.VerticalScope() )
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label( "No packages available.", EditorStyles.centeredGreyMiniLabel );
                            GUILayout.FlexibleSpace();
                        }
                    }
                    else
                    {
                        DrawPackages();
                    }
                }
            }

            Utils.HGUI.VerticalSeparator();
        }

        void RegenerateItems()
        {
            var packages = Core.Packages.Data;
            var tags = GetSortedTags( ref packages );
            
            baseItems.Clear();
            AddItemsByTag( Models.Rollout.VCS_LABEL, true, true, false );
            AddItemsByTag( Models.Rollout.EXPERIMENTAL_LABEL, true, true, false);
            foreach ( var tag in tags )
            {
                AddItemsByTag( $"Rollout {tag}", true, true, false);
            }
            AddItemsByTag( Models.Rollout.UNDEFINED_LABEL, true, true, false );
            AddItemsByTag( Models.Rollout.NOT_HUF_LABEL, true, true, false );
            AddItemsByTag( Models.Rollout.DEVELOPMENT_LABEL, true, true, false );
            AddItemsByTag( Models.Rollout.NOT_INSTALLED_LABEL, false, false, true );
            AddItemsByTag( Models.Rollout.UNITY_LABEL, true, true, false );
            
            unityListRequest = UnityEditor.PackageManager.Client.List( true );
            
            RefreshItems();
            
            void AddItemsByTag( string tag, bool compareTag, bool ignoreNotInstalled, bool ignoreInstalled )
            {
                var anyItemAdded = false;
                var rolloutTag = new Items.RolloutLabel( tag, labelColor );
                baseItems.Add( rolloutTag );
                
                foreach ( var package in packages )
                {
                    if ( tag.Contains( package.huf.rollout ) || !compareTag )
                    {
                        if ( ( ignoreNotInstalled && package.huf.status == Models.PackageStatus.NotInstalled ) || 
                             ( ignoreInstalled && package.huf.status != Models.PackageStatus.NotInstalled ) )
                        {
                            continue;
                        }

                        if ( string.IsNullOrEmpty( package.huf.rollout ) && tag != Models.Rollout.UNDEFINED_LABEL )
                        {
                            continue;
                        }
                        
                        baseItems.Add( new Items.HPackageListItem( window, package ) );
                        anyItemAdded = true;
                    }
                }

                if ( !anyItemAdded )
                {
                    baseItems.Remove( rolloutTag );
                }
            }

            refreshItemsList = true;
        }

        void FetchUnityPackages()
        {
            if ( unityListRequest != null && unityListRequest.IsCompleted )
            {
                if ( unityListRequest.Status == StatusCode.Success )
                {
                    baseItems.Add( new Items.RolloutLabel( "Unity Packages", labelColor ) );
                    foreach ( var package in unityListRequest.Result )
                    {
                        baseItems.Add( new Items.UnityPackageListItem( window, package ) );
                    }

                    refreshItemsList = true;
                }

                unityListRequest = null;
            }
        }
        
        List<string> GetSortedTags( ref List<Models.PackageManifest> packages )
        {
            var tags = packages
                       .Where( package => package.huf.rollout.Contains( "." ) ||
                                          package.huf.rollout.All( char.IsDigit ) &&
                                          !string.IsNullOrEmpty( package.huf.rollout ) )
                       .Select( p => p.huf.rollout )
                       .Distinct()
                       .ToList();
            
            tags.Sort( ( s, s2 ) =>
            {
                float.TryParse( s.Contains( '.' ) ? s.Split( '.' )[1] : s, out var first );
                float.TryParse( s2.Contains( '.' ) ? s2.Split( '.' )[1] : s2, out var second );
                return first > second ? -1 : 1;
            } );

            return tags;
        }
        
        void RefreshItems()
        {
            currentItems.Clear();
            for( int i = 0; i < baseItems.Count; ++i )
            {
                switch ( baseItems[i].Type )
                {
                    case ItemType.RolloutTag:
                    {
                        // Replace last rollout tag if there are no packages in.
                        if ( i != baseItems.Count - 1 )
                        {
                            var lastIndex = currentItems.Count - 1;
                            if ( currentItems.Count == 0 || currentItems[lastIndex].Type != ItemType.RolloutTag )
                            {
                                currentItems.Add( baseItems[i] );
                            }
                            else
                            {
                                currentItems[lastIndex] = baseItems[i];
                            }
                        }
                        
                        break;
                    }

                    case ItemType.PackageHUF:
                    {
                        var package = ( Items.HPackageListItem ) baseItems[i];
                        if ( ShouldDrawPackageItem( package.manifest ) )
                        {
                            currentItems.Add( baseItems[i] );
                        }
                        break;
                    }

                    case ItemType.PackageUnity:
                    {
                        currentItems.Add( baseItems[i] );
                        break;
                    }
                }
            }

            // Don't render empty rollout labels.
            if ( currentItems.Count == 1 )
            {
                currentItems.Clear();
            }
            else if( currentItems.Count > 0 && currentItems[currentItems.Count - 1].Type == ItemType.RolloutTag )
            {
                currentItems.RemoveAt( currentItems.Count - 1 );
            }
        }

        void DrawPackages()
        {
            var index = ( int ) ( scrollPosition.y / ITEM_HEIGHT );
            var visibleCount = ( int ) window.position.height / 30 + 1;
                
            index = Mathf.Clamp( index, 0, Mathf.Max( 0, currentItems.Count - visibleCount ) );
            GUILayout.Space( index * ITEM_HEIGHT );
            for ( var i = index; i < Mathf.Min( currentItems.Count, index + visibleCount ); i++ )
            {
                currentItems[i].Draw();
            }
            GUILayout.Space( Mathf.Max( 0, ( currentItems.Count - index - visibleCount ) * ITEM_HEIGHT ) );
        }
        
        void DrawSearchBar()
        {
            using ( new GUILayout.HorizontalScope( EditorStyles.toolbar,
                GUILayout.MinWidth( VIEW_WIDTH ) ) )
            {
                if ( searchBarStyle == null )
                {
                    searchBarStyle = GUI.skin.FindStyle( "ToolbarSeachTextField" ) ??
                                     new GUIStyle( "toolbarTextField" );
                }

                if ( searchBarButtonStyle == null )
                {
                    searchBarButtonStyle = GUI.skin.FindStyle( "ToolbarSeachCancelButton" ) ?? 
                                           EditorStyles.toolbarButton;
                }

                GUI.SetNextControlName( SEARCH_FIELD_NAME );
                
                EditorGUI.BeginChangeCheck();
                
                window.state.searchText = GUILayout.TextField( window.state.searchText, searchBarStyle );
                
                var isSearchBarFocused = string.Compare( GUI.GetNameOfFocusedControl(),
                    SEARCH_FIELD_NAME,
                    StringComparison.Ordinal ) == 0;

                if ( isSearchBarFocused && Event.current != null && Event.current.type == EventType.MouseDown  )
                {
                    GUI.FocusControl( null );
                }

                if ( GUILayout.Button(string.Empty, searchBarButtonStyle ) )
                {
                    window.state.searchText = string.Empty;
                    GUI.FocusControl( null );
                }
                
                refreshItemsList = EditorGUI.EndChangeCheck();
            }
        }
        
        private bool ShouldDrawPackageItem( Models.PackageManifest manifest )
        {
            // If user search for package we should always return it even if it's disabled by other sorting option.
            if ( !string.IsNullOrEmpty( window.state.searchText ) )
            {
                return manifest.name.IndexOf( window.state.searchText, StringComparison.OrdinalIgnoreCase ) > -1;
            }

            if ( manifest.huf.status == Models.PackageStatus.NotInstalled &&
                 manifest.huf.isPreview &&
                 !window.state.showPreviewPackages )
            {
                return false;
            }

            if ( manifest.huf.isUnity && !window.state.showUnityPackages )
            {
                return false;
            }
            
            switch ( window.state.categoryType )
            {
                case Models.PackageCategoryType.HUF:
                    if ( !manifest.name.Contains( "huf." ) ) { return false; } break;
                case Models.PackageCategoryType.HUFEXT:
                    if ( !manifest.name.Contains( "hufext." ) ) { return false; } break;
                case Models.PackageCategoryType.SDK:
                    if ( !manifest.name.Contains( ".plugins." ) ) { return false; } break;
                case Models.PackageCategoryType.UNITY:
                    if ( !manifest.name.Contains( ".unity." ) ) { return false; } break;
            }
            
            switch ( window.state.sortingType )
            {
                case Models.PackageSortingType.AllPackages: return true;
                case Models.PackageSortingType.InProject:
                    return manifest.huf.status == Models.PackageStatus.Installed ||
                           manifest.huf.status == Models.PackageStatus.UpdateAvailable ||
                           manifest.huf.status == Models.PackageStatus.ForceUpdate ||
                           manifest.huf.status == Models.PackageStatus.Migration;
                case Models.PackageSortingType.UpdateAvailable:
                    return manifest.huf.status == Models.PackageStatus.UpdateAvailable ||
                           manifest.huf.status == Models.PackageStatus.ForceUpdate ||
                           manifest.huf.status == Models.PackageStatus.Migration;
                case Models.PackageSortingType.PreviewPackages: return manifest.huf.isPreview;
                default: return false;
            }
        }
    }
}
