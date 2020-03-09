using System.Collections.Generic;
using System.IO;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.API.Views;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Data;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using PackageConfig = HUFEXT.PackageManager.Editor.API.Data.PackageConfig;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageView : IPackageManagerView
    {
        const float BUTTON_WIDTH = 120.0f;
        readonly float BUTTON_HEIGHT = EditorGUIUtility.singleLineHeight;

        private PackageManagerWindow window;
        private Vector2 scrollPosition;

        public PackageManagerWindow Window => window;
        public event UnityAction<PackageManifest> OnInstallPackageRequest;
        public event UnityAction<string> OnRemovePackageRequest;

        private static List<string> previewVersions;
        private static List<string> stableVersions;

        public void Initialize( PackageManagerWindow parent )
        {
            window = parent;
        }

        public void FetchOtherVersions()
        {
            previewVersions = null;
            stableVersions = null;

            if(Window.state.selectedPackage == null)
            {
                return;
            }

            Window.controller.RequestPackageVersions( Window.state.selectedPackage, ( v ) => previewVersions = v, RoutingScheme.Channel.Preview );
            Window.controller.RequestPackageVersions( Window.state.selectedPackage, ( v ) => stableVersions = v, RoutingScheme.Channel.Stable );
        }

        public void Repaint()
        {
            if ( Window.state.selectedPackage == null )
            {
                return;
            }

            var package = Window.state.selectedPackage;

            using ( new EditorGUI.DisabledScope( package.huf.status == PackageStatus.Embedded ) )
            {
                using ( new GUILayout.VerticalScope() )
                {
                    EditorGUILayout.Space();
                    DrawHeader();
                    EditorGUILayout.Space();
                    HCommonGUI.HorizontalSeparator();

                    using ( new GUILayout.HorizontalScope() )
                    {
                        EditorGUILayout.Space();
                        DrawBasicInfo( package );
                        EditorGUILayout.Space();
                        HCommonGUI.VerticalSeparator();
                        EditorGUILayout.Space();
                        DrawDetailedInfo();
                        EditorGUILayout.Space();
                    }
                }
            }
        }
        
        void DrawHeader()
        {
            using ( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.Space();
                var package = Window.state.selectedPackage;
                GUILayout.Label( $"{package.displayName} {package.huf.version}", new GUIStyle( EditorStyles.label )
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 18
                } );

                GUILayout.FlexibleSpace();
                DrawPackageActionButtons();
                EditorGUILayout.Space();
            }
        }
        
        void DrawPackageActionButtons()
        {
            var package = Window.state.selectedPackage;

            switch ( package.huf.status )
            {
                case PackageStatus.NotInstalled:
                case PackageStatus.Migration:
                {
                    HCommonGUI.Button( "Install",
                                       () => { OnInstallPackageRequest?.Invoke( package ); },
                                       BUTTON_WIDTH,
                                       BUTTON_HEIGHT );
                    break;
                }

                case PackageStatus.Installed:
                {
                    if( package.huf.config.packageCanBeRemoved )
                    {
                        HCommonGUI.Button( "Remove",
                                           () => { OnRemovePackageRequest?.Invoke( package.huf.path ); },
                                           BUTTON_WIDTH,
                                           BUTTON_HEIGHT );
                    }
                    break;
                }
                
                case PackageStatus.ForceUpdate:
                case PackageStatus.UpdateAvailable:
                {
                    HCommonGUI.Button( $"Update to {package.huf.config.latestVersion}",
                                       () => { OnInstallPackageRequest?.Invoke( Window.state.selectedPackage ); },
                                       BUTTON_WIDTH,
                                       BUTTON_HEIGHT );

                    if ( package.huf.config.packageCanBeRemoved )
                    {
                        HCommonGUI.Button( "Remove",
                                           () => { OnRemovePackageRequest?.Invoke( package.huf.path ); },
                                           BUTTON_WIDTH,
                                           BUTTON_HEIGHT );
                    }
                    break;
                }

                case PackageStatus.Conflict:
                {
                    if ( package.huf.config.packageCanBeRemoved )
                    {
                        HCommonGUI.Button( "Remove",
                                           () => { OnRemovePackageRequest?.Invoke( package.huf.path ); },
                                           BUTTON_WIDTH,
                                           BUTTON_HEIGHT );
                    }
                    break;
                }
            }
        }

        void DrawBasicInfo( PackageManifest package )
        {
            var style = new GUIStyle( EditorStyles.label );
            using ( new GUILayout.VerticalScope( GUILayout.MinWidth( 200f ) ) )
            {
                EditorGUILayout.Space();
                GUILayout.Label( "Package", EditorStyles.boldLabel );
                GUILayout.Label( package.name, style );
                EditorGUILayout.Space();
                GUILayout.Label( "Author", EditorStyles.boldLabel );
                GUILayout.Label( package.author.name, style );
                EditorGUILayout.Space();
                DrawMetadataInfo( package );
                GUILayout.FlexibleSpace();
                //DrawVersionsDropdown();
                EditorGUILayout.Space();
            }
        }

        void DrawMetadataInfo( PackageManifest package )
        {            
            GUILayout.Label( "Metadata", EditorStyles.boldLabel );
            GUILayout.Label( "Version: " + package.version );
            
            if ( !string.IsNullOrEmpty( package.huf.commit) )
            {
                GUILayout.Label( $"Build: {package.huf.commit}-b{package.huf.build}");
                GUILayout.Label( $"Build Time: {package.huf.date}" );
            }
            
            if ( package.huf.isLocal )
            {
                GUILayout.Label( "This package is local." );
            }
            else if( !string.IsNullOrEmpty( package.huf.config.latestVersion ) )
            {
                GUILayout.Label( "Latest version: " + package.huf.config.latestVersion );
            }
            
            if ( !string.IsNullOrEmpty( package.huf.path ) )
            {
                GUILayout.Label( "Path: " + package.huf.path );
            }
        }
        
        void DrawDetailedInfo()
        {
            if (Window.state.selectedPackage == null)
            {
                return;
            }

            using ( new GUILayout.VerticalScope() )
            {
                var package = Window.state.selectedPackage;
                
                EditorGUILayout.Space();
                using ( new GUILayout.HorizontalScope() )
                {
                    GUILayout.Label( "Description", EditorStyles.boldLabel );
                    GUILayout.FlexibleSpace();
                    
                    using ( new EditorGUI.DisabledScope( true ) )
                    {
                        if ( GUILayout.Button( "Documentation", GUILayout.Width( 120f ) ) )
                        {
                            // Placeholder until we implement proper solution.
                        }
                    }
                }

                EditorGUILayout.Space();
                using( var scope = new GUILayout.ScrollViewScope( scrollPosition ) )
                {
                    scrollPosition = scope.scrollPosition;
                    
                    if ( !string.IsNullOrEmpty( package.huf.message ) )
                    {
                        GUILayout.Label( "Read before installing this package", EditorStyles.boldLabel );
                        GUILayout.Label( package.huf.message, new GUIStyle( EditorStyles.label )
                        {
                            normal = { textColor = Color.red },
                            fontStyle = FontStyle.Bold,
                            wordWrap = true,
                            fontSize = 12
                        } );
                        EditorGUILayout.Space();
                    }
                    
                    GUILayout.Label( Window.state.selectedPackage.description, new GUIStyle( EditorStyles.label )
                    {
                        fontSize = 12, 
                        wordWrap = true
                    } );

                    var changelogPath = package.huf.path + "/CHANGELOG.md";
                    if ( File.Exists( changelogPath ) )
                    {
                        EditorGUILayout.Space();
                        GUILayout.Label( "Changelog", EditorStyles.boldLabel );
                        using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
                        {
                            var changelog = File.ReadAllText( changelogPath );
                            GUILayout.Label( changelog, new GUIStyle( EditorStyles.label )
                            {
                                wordWrap = true
                            } );
                        }
                    }
                }
                EditorGUILayout.Space();
            }
        }

        void DrawVersionsDropdown()
        {
            using (new EditorGUI.DisabledScope( previewVersions == null && stableVersions == null ) )
            {
                if (GUILayout.Button( "Other version", EditorStyles.popup ))
                {
                    var menu = new GenericMenu();
                    AddVersions( ref menu, ref previewVersions, "Preview/" );
                    AddVersions( ref menu, ref stableVersions, "Stable/" );
                    if (menu.GetItemCount() == 0)
                    {
                        menu.AddDisabledItem( new GUIContent( "Unable to find other versions of this package." ) );
                    }
                    menu.ShowAsContext();
                }
            }
        }

        void AddVersions(ref GenericMenu menu, ref List<string> versions, string channel )
        {
            if ( versions != null )
            {
                foreach (var version in previewVersions)
                {
                    if ( Window.state.selectedPackage.IsEqual( version ) )
                    {
                        menu.AddDisabledItem( new GUIContent( channel + version ), true );
                    }
                    else
                    {
                        menu.AddItem( new GUIContent( channel + version ), false, ChangeSelectedPackage, version );
                    }
                }
            }
        }

        void ChangeSelectedPackage( object version )
        {
            Window.controller.RequestPackageManifest( Window.state.selectedPackage.huf.config, ( manifest ) =>
            {
                Window.SelectPackage( manifest );
            },
            version as string);
        }
    }
}
