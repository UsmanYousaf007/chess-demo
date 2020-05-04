using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageView : PackageManagerView
    {
        const float BUTTON_WIDTH = 120.0f;
        readonly float BUTTON_HEIGHT = EditorGUIUtility.singleLineHeight;
        
        Vector2 leftSideScroll;
        Vector2 rightSideScroll;

        public PackageView( PackageManagerWindow parent ) : base( parent )
        {
            window = parent;
        }

        public override void RefreshView( ViewEvent ev )
        {
            if ( window.state.selectedPackage != null )
            {
                Core.Command.Execute( new Commands.Processing.SelectPackageCommand( window,
                                                                                    window.state.selectedPackage.name ) );
            }
        }

        protected override void OnGUI()
        {
            if ( window.state.selectedPackage == null )
            {
                using ( new GUILayout.VerticalScope() )
                {
                    GUILayout.FlexibleSpace();
                    using ( new GUILayout.HorizontalScope() )
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label( "No package selected.", EditorStyles.centeredGreyMiniLabel );
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.FlexibleSpace();
                }

                return;
            }

            //using ( new EditorGUI.DisabledScope( window.state.selectedPackage.huf.status == Models.PackageStatus.Embedded ) )
            {
                using ( new GUILayout.VerticalScope() )
                {
                    EditorGUILayout.Space();
                    DrawHeader( window.state.selectedPackage );
                    EditorGUILayout.Space();
                    Utils.HGUI.HorizontalSeparator();

                    using ( new GUILayout.HorizontalScope() )
                    {
                        DrawBasicInfo( window.state.selectedPackage );
                        Utils.HGUI.VerticalSeparator();
                        DrawDetailedInfo( window.state.selectedPackage );
                    }
                }
            }
        }

        void DrawHeader( Models.PackageManifest package )
        {
            using ( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.Space();
                GUILayout.Label( $"{package.displayName} {package.huf.version}", new GUIStyle( EditorStyles.label )
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 18
                } );
                GUILayout.FlexibleSpace();
                DrawPackageActionButtons( package );
                EditorGUILayout.Space();
            }
        }
        
        void DrawPackageActionButtons( Models.PackageManifest package )
        {
            switch ( package.huf.status )
            {
                case Models.PackageStatus.NotInstalled:
                case Models.PackageStatus.ForceUpdate:
                case Models.PackageStatus.UpdateAvailable:
                case Models.PackageStatus.Migration:
                case Models.PackageStatus.Development:
                {
                    var update = package.huf.status != Models.PackageStatus.NotInstalled ||
                                 package.huf.status == Models.PackageStatus.Development;
                    
                    var label = !update ? "Install" : $"Update to {package.huf.config.latestVersion}";
                    
                    if ( GUILayout.Button( label,GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                    {
                        window.Enqueue( ViewEvent.InstallPackage, package );
                    }

                    if ( update )
                    {
                        using ( new EditorGUI.DisabledScope( !package.huf.config.packageCanBeRemoved ) )
                        {
                            if ( GUILayout.Button( "Remove", 
                                                   GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                            {
                                window.Enqueue( ViewEvent.RemovePackage, package );
                            }
                        }
                    }
                    break;
                }

                case Models.PackageStatus.Installed:
                case Models.PackageStatus.Embedded:
                case Models.PackageStatus.Conflict:
                {
                    if ( GUILayout.Button( "Remove", GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                    {
                        window.Enqueue( ViewEvent.RemovePackage, package );
                    }
                    break;
                }
            }
        }
        
        void DrawBasicInfo( Models.PackageManifest package )
        {
            var style = new GUIStyle( EditorStyles.label );
            using ( new GUILayout.VerticalScope() )
            {
                using ( var scope = new GUILayout.ScrollViewScope( leftSideScroll,
                                                                   GUILayout.MinWidth( 330f ), 
                                                                   GUILayout.ExpandWidth( true ) ) )
                {
                    leftSideScroll = scope.scrollPosition;
                    EditorGUILayout.Space();
                    GUILayout.Label( "Package", EditorStyles.boldLabel );
                    GUILayout.Label( package.name, style );
                    EditorGUILayout.Space();
                    DrawAdditionalButtons( package );
                    EditorGUILayout.Space();
                    GUILayout.Label( "Author", EditorStyles.boldLabel );
                    GUILayout.Label( package.author.name, style );
                    EditorGUILayout.Space();
                    DrawMetadataInfo( package );
                    EditorGUILayout.Space();
                    DrawDependenciesInfo( package );
                    EditorGUILayout.Space();
                    DrawExcludedInfo( package );
                    GUILayout.FlexibleSpace();
                }
            }
        }

        void DrawMetadataInfo( Models.PackageManifest package )
        {            
            GUILayout.Label( "Metadata", EditorStyles.boldLabel );
            GUILayout.Label( "Version: " + package.version );

            if ( !string.IsNullOrEmpty( package.huf.commit) )
            {
                GUILayout.Label( $"Build: {package.huf.commit}-b{package.huf.build}");
                GUILayout.Label( $"Build Time: {package.huf.date}" );
            }
            
            if ( !string.IsNullOrEmpty( package.huf.rollout ) )
            {
                GUILayout.Label( "Rollout: " + package.huf.rollout );
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

        void DrawDependenciesInfo( Models.PackageManifest package )
        {
            if ( package == null || package.huf.dependencies.Count == 0 )
            {
                return;
            }

            GUILayout.Label( "Dependencies", EditorStyles.boldLabel );
            foreach ( var dependency in package.huf.dependencies )
            {
                GUILayout.Label( "• " + dependency, new GUIStyle( EditorStyles.label )
                {
                    fontStyle = FontStyle.Normal,
                    fontSize  = 10
                } );
            }
        }
        
        void DrawExcludedInfo( Models.PackageManifest package )
        {
            if ( package == null || package.huf.exclude.Count == 0 )
            {
                return;
            }

            GUILayout.Label( new GUIContent( "Conflicts",
                    "This package cannot be installed with following packages" ),
                EditorStyles.boldLabel );
            
            foreach ( var dependency in package.huf.exclude )
            {
                GUILayout.Label( "• " + dependency, new GUIStyle( EditorStyles.label )
                {
                    fontStyle = FontStyle.Normal,
                    fontSize  = 10
                } );
            }
        }
        
        void DrawDetailedInfo( Models.PackageManifest package )
        {
            using ( new GUILayout.VerticalScope() )
            {
                using( var scope = new GUILayout.ScrollViewScope( rightSideScroll ) )
                {
                    rightSideScroll = scope.scrollPosition;
                    
                    using ( new GUILayout.HorizontalScope() )
                    {
                        GUILayout.Label( "Description", EditorStyles.boldLabel );
                        GUILayout.FlexibleSpace();
                    }
                    
                    EditorGUILayout.Space();
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
                    
                    GUILayout.Label( package.description, new GUIStyle( EditorStyles.label )
                    {
                        fontSize = 12, 
                        wordWrap = true
                    } );

                    if ( package.huf.details.Count > 0 )
                    {
                        EditorGUILayout.Space();
                        DrawDetailsData( package );
                    }

                    var changelogPath = package.huf.path + "/CHANGELOG.md";
                    if ( File.Exists( changelogPath ) )
                    {
                        EditorGUILayout.Space();
                        using ( new GUILayout.VerticalScope(  ) )
                        {
                            var changelog = File.ReadAllText( changelogPath );
                            GUILayout.Label( changelog, new GUIStyle( EditorStyles.label )
                            {
                                wordWrap = true
                            } );
                        }
                    }
                }
            }
        }

        void DrawDetailsData( Models.PackageManifest package )
        {
            if ( package.huf.details.Count > 0 )
            {
                using ( new GUILayout.VerticalScope( GUILayout.Height( 200f ) ) )
                {
                    for ( int i = 0; i < package.huf.details.Count; ++i )
                    {
                        GUILayout.Label( package.huf.details[i], new GUIStyle( EditorStyles.label )
                        {
                            fontSize = 10
                        } );
                    }
                }
            }
        }
        
        void DrawAdditionalButtons( Models.PackageManifest package )
        {
            using ( new GUILayout.HorizontalScope() )
            {
                using ( new EditorGUI.DisabledScope( true ) )
                {
                    if ( GUILayout.Button( new GUIContent("Documentation", "Coming soon"), EditorStyles.miniButton ) )
                    {
                        // Placeholder until we implement proper solution.
                    }
                }

                using ( new EditorGUI.DisabledScope( package.huf.config.previewVersions.Count == 0 &&
                                                     package.huf.config.stableVersions.Count == 0 && 
                                                     package.huf.config.experimentalVersions.Count == 0 ) )
                {
                    if ( GUILayout.Button( "Other versions", EditorStyles.popup ) )
                    {
                        var menu = new GenericMenu();
                        DisplayVersionsMenu( ref menu,
                            ref package.huf.config.stableVersions,
                            Models.Keys.Routing.STABLE_CHANNEL,
                            package );
                        
                        DisplayVersionsMenu( ref menu,
                                             ref package.huf.config.previewVersions,
                                             Models.Keys.Routing.PREVIEW_CHANNEL,
                                             package );

                        DisplayVersionsMenu( ref menu,
                            ref package.huf.config.experimentalVersions,
                            Models.Keys.Routing.EXPERIMENTAL_CHANNEL,
                            package );
                        menu.ShowAsContext();
                    }
                }
            }
        }

        void DisplayVersionsMenu( ref GenericMenu menu, ref List<string> versions, string channel, Models.PackageManifest package )
        {
            foreach ( var version in versions )
            {
                if ( Utils.VersionComparer.Compare( package.version, "=", version ) )
                {
                    menu.AddDisabledItem( new GUIContent( $"{channel}/{version}" ),
                                          package.huf.status == Models.PackageStatus.Installed );
                }
                else
                {
                    menu.AddItem( new GUIContent( $"{channel}/{version}" ), false, () =>
                    {
                        Core.Command.Execute( new Commands.Connection.DownloadPackageManifestCommand
                        {
                            version = version,
                            channel = channel,
                            packageName = package.name,
                            scope = package.huf.scope,
                            OnComplete = ( success, serializedData ) =>
                            {
                                if ( !success )
                                {
                                    return;
                                }
                                
                                package = Models.PackageManifest.ParseManifest( serializedData, true );
                                window.state.selectedPackage = package;
                            }
                        });
                    } );
                }
            }
        }
    }
}
