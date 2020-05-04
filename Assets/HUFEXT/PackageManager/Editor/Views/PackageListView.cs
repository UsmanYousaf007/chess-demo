using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageListView : PackageManagerView
    {
        class PackageItemView
        {
            public readonly Color color = Color.clear;
            public readonly Models.PackageManifest manifest = null;
            public readonly string rollout = string.Empty;
            public readonly string tooltip = string.Empty;
            public readonly bool isTag = false;

            public PackageItemView( Models.PackageManifest manifest )
            {
                this.manifest = manifest;
                isTag = false;
            }

            public PackageItemView( string rollout, string tooltip, Color color )
            {
                this.rollout = rollout;
                this.color = color;
                isTag = true;
            }
        }
        
        const float ITEM_HEIGHT = 30f;
        const string EXPERIMENTAL_LABEL = "Experimental";
        const string DEVELOPMENT_LABEL = "Development";
        const string NOT_INSTALLED_LABEL = "Not Installed";
        const string UNDEFINED_LABEL = "Undefined";
        const float ICON_WIDTH = 16f;

        private static readonly Color labelColor = new Color( 0.27f, 0.27f, 0.27f ); //new Color( 0.25f, 0.25f, 0.25f );
        
        private Vector2 scrollPosition;
        private List<Models.PackageManifest> packages;
        private readonly List<PackageItemView> items = new List<PackageItemView>();
        private string selectedPackageName = string.Empty;

        static readonly Dictionary<string, PackageItemView> rolloutLabels = new Dictionary<string, PackageItemView>
        {
            {
                EXPERIMENTAL_LABEL, 
                new PackageItemView( EXPERIMENTAL_LABEL, "", labelColor)
            },
            {
                DEVELOPMENT_LABEL, 
                new PackageItemView( DEVELOPMENT_LABEL, "", labelColor)
            },
            {
                UNDEFINED_LABEL, 
                new PackageItemView( UNDEFINED_LABEL, "", labelColor)
            },
            {
                NOT_INSTALLED_LABEL, 
                new PackageItemView( NOT_INSTALLED_LABEL, "", labelColor)
            }
        };
        
        static readonly Dictionary<Models.PackageStatus, GUIContent> statusContent = new Dictionary<Models.PackageStatus, GUIContent>
        {
            {
                Models.PackageStatus.NotInstalled, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageNotInstalledIcon,
                    tooltip = "This package is not installed."
                }
            },
            {
                Models.PackageStatus.Installed, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageInstalledIcon,
                    tooltip = "This package is installed."
                }
            },
            {
                Models.PackageStatus.Migration, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageMigrationIcon,
                    tooltip = "This package has old format. Migration will update package to latest version."
                }
            },
            {
                Models.PackageStatus.UpdateAvailable, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageUpdateIcon,
                    tooltip = "There is new version of this package available."
                }
            },
            {
                Models.PackageStatus.ForceUpdate, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageForceUpdateIcon,
                    tooltip = "There is new mandatory version of this package available."
                }
            },
            {
                Models.PackageStatus.Unknown, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageErrorIcon,
                    tooltip = "There is an error with this package. Try reimport package."
                }
            },
            {
                Models.PackageStatus.Unavailable, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageErrorIcon,
                    tooltip = "This package is unavailable. Please, contact HUF support."
                }
            },
            {
                Models.PackageStatus.Conflict, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageConflictIcon,
                    tooltip = "There are different packages with same path. Please, contact HUF support."
                }
            },
            {
                Models.PackageStatus.Development, new GUIContent()
                {
                    image   = Utils.HGUI.Icons.PackageConflictIcon,
                    tooltip = "This package is still in development."
                }
            },
            {
                Models.PackageStatus.Embedded, new GUIContent()
                {
                    text = "embedded",
                    tooltip = "Package manifest not found or package is part of other package."
                }
            }
        };
        
        public PackageListView( PackageManagerWindow parent ) : base( parent )
        {
            GenerateItemsList();
        }
        
        public override void RefreshView( ViewEvent ev ) 
        {
            GenerateItemsList();
        }

        protected override void OnGUI()
        {
            using ( var scope =
                new GUILayout.ScrollViewScope( scrollPosition, false, true, GUILayout.MinWidth( 310f ) ) )
            {
                scrollPosition = scope.scrollPosition;

                if ( items.Count == 0 )
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

            Utils.HGUI.VerticalSeparator();
        }

        void DrawPackages()
        {
            var index = ( int ) ( scrollPosition.y / ITEM_HEIGHT );
            var visibleCount = ( int ) window.position.height / 30 + 1;
                
            index = Mathf.Clamp( index, 0, Mathf.Max( 0, items.Count - visibleCount ) );
            GUILayout.Space( index * ITEM_HEIGHT );
            for ( var i = index; i < Mathf.Min( items.Count, index + visibleCount ); i++ )
            {
                if ( items[i].isTag )
                {
                    DrawPackageLabel( items[i] );
                }
                else
                {
                    DrawPackageItem( items[i] );
                }
            }
            GUILayout.Space( Mathf.Max( 0, ( items.Count - index - visibleCount ) * ITEM_HEIGHT ) );
        }
        
        void DrawPackageItem( PackageItemView item )
        {
            var isActive = false;
            if ( window.state.selectedPackage != null && item.manifest != null )
            {
                isActive = window.state.selectedPackage.name == item.manifest.name;
            }
            
            var myStyle = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            
            using ( var v = new EditorGUILayout.VerticalScope( myStyle, GUILayout.Height( 30f ) ) )
            {
                GUILayout.FlexibleSpace();
                using ( new GUILayout.HorizontalScope() )
                {
                    if ( isActive )
                    {
                        EditorGUI.DrawRect( v.rect, new Color( 0f, 0f, 0f, 0.2f ) );
                        GUI.contentColor = Color.white;
                    }

                    if ( GUI.Button( v.rect, GUIContent.none, EditorStyles.label ) && !isActive )
                    {
                        selectedPackageName = item.manifest.name;
                        window.Enqueue( ViewEvent.SelectPackage, selectedPackageName );
                    }

                    var displayName = item.manifest.displayName.Length > 25
                                          ? item.manifest.displayName.Substring( 0, 22 ) + "..."
                                          : item.manifest.displayName;

                    if ( item.manifest.huf.status == Models.PackageStatus.UpdateAvailable )
                    {
                        displayName = $"<color=orange>{displayName}</color>";
                    }

                    if ( item.manifest.huf.status == Models.PackageStatus.ForceUpdate )
                    {
                        displayName = $"<color=red>{displayName}</color>";
                    }

                    GUILayout.Label( displayName, new GUIStyle( EditorStyles.label )
                    {
                        fontSize = 11,
                        fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal,
                        richText = true
                    } );
                    
                    if ( item.manifest.huf.isLocal )
                    {
                        var style = new GUIStyle( EditorStyles.centeredGreyMiniLabel ); ;
                        style.normal.textColor = isActive ? Color.white : Color.grey;
                        GUILayout.Label( "local",  style );
                    }

                    if ( item.manifest.huf.isPreview )
                    {
                        var style = new GUIStyle( EditorStyles.centeredGreyMiniLabel ); ;
                        style.normal.textColor = isActive ? Color.white : Color.gray;
                        GUILayout.Label( "preview", style );
                    }

                    GUILayout.FlexibleSpace();
                    DrawPackageVersionAndStatus( item.manifest, isActive );
                }
                GUILayout.FlexibleSpace();
            }
            Utils.HGUI.HorizontalSeparator();
        }

        void DrawPackageLabel( PackageItemView view )
        {
            var noMargins = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            using ( var v = new EditorGUILayout.VerticalScope( noMargins, GUILayout.Height( 30f ) ) )
            {
                GUILayout.FlexibleSpace();
                EditorGUI.DrawRect( v.rect, view.color );
                GUILayout.Label( new GUIContent( view.rollout, view.tooltip ),
                                 new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                                 {
                                     normal = { textColor = Color.white },
                                     fontStyle = FontStyle.Bold
                                 } );
                GUILayout.FlexibleSpace();
            }

            Utils.HGUI.HorizontalSeparator();
        }
        
        private void DrawPackageVersionAndStatus( Models.PackageManifest manifest, bool isActive = false )
        {
            if ( manifest == null )
            {
                return;
            }
            
            var width = GUILayout.Width( 16f );
            
            switch ( manifest.huf.status )
            {
                case Models.PackageStatus.NotInstalled:
                case Models.PackageStatus.Installed:
                case Models.PackageStatus.UpdateAvailable:
                case Models.PackageStatus.ForceUpdate:
                case Models.PackageStatus.Migration:
                case Models.PackageStatus.Development:
                {
                    GUILayout.Label( manifest.huf.version );
                    GUILayout.Label( statusContent[manifest.huf.status], width );
                    break;
                }

                case Models.PackageStatus.Unknown:
                case Models.PackageStatus.Unavailable:
                case Models.PackageStatus.Conflict:
                {
                    GUILayout.Label( statusContent[manifest.huf.status], width );
                    break;
                }

                case Models.PackageStatus.Embedded:
                {
                    GUILayout.Label( statusContent[Models.PackageStatus.Embedded], new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                    {
                        normal = { textColor = isActive ? Color.white : Color.grey }
                    } );
                    break;
                }
            }
        }
        
        void GenerateItemsList()
        {
            Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand { downloadLatest = false } );
            
            packages = Core.Packages.Data;

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

            items.Clear();
            AddItemsByTag( rolloutLabels[EXPERIMENTAL_LABEL], EXPERIMENTAL_LABEL, true, true, false);
            foreach ( var tag in tags )
            {
               AddItemsByTag( new PackageItemView( $"Rollout {tag}", "Test", labelColor ),
                              tag, true, true, false);
            }
            AddItemsByTag( rolloutLabels[UNDEFINED_LABEL], "", true, true, false);
            AddItemsByTag( rolloutLabels[DEVELOPMENT_LABEL], DEVELOPMENT_LABEL, true, true, false);
            AddItemsByTag( rolloutLabels[NOT_INSTALLED_LABEL], "", false, false, true);
        }

        void AddItemsByTag( PackageItemView view, string tag, bool compareTag, bool ignoreNotInstalled, bool ignoreInstalled )
        {
            var anyItemAdded = false;
            items.Add( view );
            foreach ( var package in packages )
            {
                if ( package.huf.rollout == tag || !compareTag )
                {
                    if ( ( ignoreNotInstalled && package.huf.status == Models.PackageStatus.NotInstalled ) || 
                         ( ignoreInstalled && package.huf.status != Models.PackageStatus.NotInstalled ) ||
                         !ShouldDrawPackageItem( package ) )
                    {
                        continue;
                    }

                    items.Add( new PackageItemView( package ) );
                    anyItemAdded = true;
                }
            }

            if ( !anyItemAdded )
            {
                items.Remove( view );
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
            
            switch ( window.state.categoryType )
            {
                case Models.PackageCategoryType.HUF:
                    if ( !manifest.name.Contains( "huf." ) ) { return false; } break;
                case Models.PackageCategoryType.HUFEXT:
                    if ( !manifest.name.Contains( "hufext." ) ) { return false; } break;
                case Models.PackageCategoryType.SDK:
                    if ( !manifest.name.Contains( ".plugins." ) ) { return false; } break;
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
