using System;
using System.Linq;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.API.Views;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;
using SortingType = HUFEXT.PackageManager.Editor.API.Data.PackageManagerState.SortingType;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageListView : IPackageManagerView
    {
        private PackageManagerWindow window;
        private Vector2 scrollPosition;
        private IOrderedEnumerable<PackageManifest> packages;
        
        public PackageManagerWindow Window => window;

        public void Initialize( PackageManagerWindow parent )
        {
            window = parent;
        }
        
        public void Repaint()
        {
            using( new GUILayout.HorizontalScope() )
            {
                using ( var scope = new GUILayout.ScrollViewScope( scrollPosition,
                                                                  false,
                                                                  true,
                                                                  GUILayout.MinWidth( 300f ) ) )
                {
                    scrollPosition = scope.scrollPosition;
                    using( new GUILayout.VerticalScope() )
                    {
                        if( packages == null )
                        {
                            packages = Window.controller.Packages.OrderBy( p => p.displayName );
                        }

                        bool anyPackageWasPainted = false;
                        foreach (var package in packages)
                        {
                            if( ShouldDrawPackageItem( package ) )
                            {
                                DrawPackageItem( package );
                                anyPackageWasPainted = true;
                            }
                        }

                        if ( !anyPackageWasPainted )
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField( new GUIContent( "No packages found." ),
                                                        EditorStyles.centeredGreyMiniLabel );
                        }
                        
                        GUILayout.FlexibleSpace();
                    }
                }
                HCommonGUI.VerticalSeparator();
            }
        }

        void DrawPackageItem( PackageManifest manifest )
        {
            var defaultColor = GUI.contentColor;
            var isActive = Window.state.selectedPackage != null && Window.state.selectedPackage.name == manifest.name;
            var myStyle = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            var rect = EditorGUILayout.BeginVertical( myStyle, GUILayout.Height( 30f ) );
            {
                GUILayout.FlexibleSpace();
                using( new GUILayout.HorizontalScope() )
                {
                    if ( isActive )
                    {
                        EditorGUI.DrawRect( rect, new Color( 0f, 0f, 0f, 0.3f ) );
                        GUI.contentColor = Color.white;
                    }

                    if ( GUI.Button( rect, GUIContent.none, EditorStyles.label ) && !isActive )
                    {
                        Window.SelectPackage( manifest );
                    }

                    GUILayout.Label( manifest.displayName, new GUIStyle( EditorStyles.label )
                    {
                        fontSize = 12,
                        fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal
                    } );
                    
                    if ( manifest.huf.isLocal )
                    {
                        var style = new GUIStyle( EditorStyles.centeredGreyMiniLabel ); ;
                        style.normal.textColor = isActive ? Color.white : Color.grey;
                        GUILayout.Label( "local",  style );
                    }

                    if ( manifest.huf.isPreview )
                    {
                        var style = new GUIStyle( EditorStyles.centeredGreyMiniLabel ); ;
                        style.normal.textColor = isActive ? Color.white : Color.gray;
                        GUILayout.Label( "preview", style );
                    }

                    GUILayout.FlexibleSpace();
                    DrawPackageVersionAndStatus( manifest, isActive );
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();
            HCommonGUI.HorizontalSeparator();
            GUI.contentColor = defaultColor;
        }
        
        private void DrawPackageVersionAndStatus( PackageManifest manifest, bool isActive = false )
        {
            var width = GUILayout.Width( 16f );
            
            switch ( manifest.huf.status )
            {
                case PackageStatus.NotInstalled:
                {
                    GUILayout.Label( manifest.huf.version );
                    GUILayout.Label( new GUIContent()
                                     {
                                         image = HCommonGUI.Icons.PackageNotInstalledIcon,
                                         tooltip = "This package is not installed."
                                     }, 
                                     width );
                    break;
                }

                case PackageStatus.Installed:
                {
                    GUILayout.Label( manifest.huf.version );
                    GUILayout.Label( new GUIContent()
                                     {
                                         image = HCommonGUI.Icons.PackageInstalledIcon,
                                         tooltip = "This package is installed."
                                     }, 
                                     width );
                    break;
                }

                case PackageStatus.Migration:
                {
                    if ( manifest.huf.version != "0.0.0" )
                    {
                        GUILayout.Label( manifest.huf.version );
                    }
                    else
                    {
                        GUILayout.Label( "Migrate", EditorStyles.centeredGreyMiniLabel );
                    }
                    
                    GUILayout.Label( new GUIContent()
                                    {
                                        image = HCommonGUI.Icons.PackageMigrationIcon,
                                        tooltip = "This package is in old format. Package migration is recommended."
                                    },
                                     width );
                    break;
                }

                case PackageStatus.UpdateAvailable:
                {
                    GUILayout.Label( manifest.huf.version );
                    GUILayout.Label( new GUIContent()
                                     {
                                         image = HCommonGUI.Icons.PackageUpdateIcon,
                                         tooltip = "There is new version of this package available."
                                     }, 
                                     width );
                    break;
                }

                case PackageStatus.ForceUpdate:
                {
                    GUILayout.Label( manifest.huf.version );
                    GUILayout.Label( new GUIContent()
                                     {
                                         image = HCommonGUI.Icons.PackageForceUpdateIcon,
                                         tooltip = "This package must be updated immediately."
                                     }, 
                                     width );
                    break;
                }
                
                case PackageStatus.Unknown:
                case PackageStatus.Unavailable:
                {
                    GUILayout.Label( new GUIContent()
                                     {
                                         image = HCommonGUI.Icons.PackageErrorIcon,
                                         tooltip = "There is an error with this package. Try reimport package."
                                     }, 
                                     width );
                    break;
                }

                case PackageStatus.Conflict:
                {
                    GUILayout.Label( new GUIContent()
                                     {
                                         image = HCommonGUI.Icons.PackageConflictIcon,
                                         tooltip = "This package has conflict with other package. Please, contact with HUF support."
                                     },
                                     width );
                    break;
                }
                
                case PackageStatus.Embedded:
                {
                    var style = new GUIStyle( EditorStyles.centeredGreyMiniLabel ); ;
                    style.normal.textColor = isActive ? Color.white : Color.grey;
                    GUILayout.Label( new GUIContent()
                                     {
                                         text = "embedded",
                                         tooltip = "Package manifest not found or package is part of other package."
                                     },
                                     style );
                    break;
                }
            }
        }

        private bool ShouldDrawPackageItem( PackageManifest manifest )
        {
            if ( Window.state.sortingType != SortingType.PreviewPackages &&
                 !Window.state.showPreviewPackages &&
                 manifest.huf.isPreview )
            {
                return false;
            }

            if ( !string.IsNullOrEmpty( Window.SearchText ) &&
                 manifest.name.IndexOf( Window.SearchText, StringComparison.OrdinalIgnoreCase ) < 0 )
            {
                return false;
            }

            bool shouldDrawItem = false;
            
            switch ( Window.state.sortingType )
            {
                case SortingType.AllPackages:
                {
                    shouldDrawItem = true;
                    break;
                }
                case SortingType.InProject:
                {
                    shouldDrawItem = manifest.huf.status == PackageStatus.Installed; 
                    break;
                }
                case SortingType.UpdateAvailable:
                {
                    shouldDrawItem = manifest.huf.status == PackageStatus.UpdateAvailable ||
                                     manifest.huf.status == PackageStatus.ForceUpdate ||
                                     manifest.huf.status == PackageStatus.Migration;
                    break;
                }
                case SortingType.PreviewPackages:
                {
                    shouldDrawItem = manifest.huf.isPreview;
                    break;
                }
                default: return false;
            }

            if ( !shouldDrawItem )
            {
                return false;
            }
            
            switch ( Window.state.categoryType )
            {
                case PackageManagerState.CategoryType.All: return true;
                case PackageManagerState.CategoryType.HUF: return !manifest.name.Contains( "hufext" );
                case PackageManagerState.CategoryType.HUFEXT: return manifest.name.Contains( "hufext" );
                default: return false;
            }
        }
    }
}
