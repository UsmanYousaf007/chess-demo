using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HUFEXT.PackageManager.Editor.Commands.Base;
using HUFEXT.PackageManager.Editor.Commands.Data;
using HUFEXT.PackageManager.Editor.Models;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageView : PackageManagerView
    {
        const float BUTTON_WIDTH = 120.0f;
        readonly float BUTTON_HEIGHT = EditorGUIUtility.singleLineHeight;

        readonly GUIStyle importantStyle = new GUIStyle()
        {
            normal = { textColor = Color.red },
            margin = new RectOffset( 5, 5, 0, 2 ),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            fontSize = 12
        };

        Vector2 leftSideScroll;
        Vector2 rightSideScroll;

        public PackageView( PackageManagerWindow parent ) : base( parent )
        {
            window = parent;
        }

        public override Models.PackageManagerViewType Type => Models.PackageManagerViewType.PackageView;

        public override void OnEventCompleted( Models.PackageManagerViewEvent ev )
        {
            if ( ev.eventType == Models.EventType.RefreshPackages && window.state.selectedPackage != null )
            {
                RegisterEvent( new Models.PackageManagerViewEvent
                {
                    owner = Type,
                    eventType = Models.EventType.SelectPackage,
                    data = window.state.selectedPackage.name
                } );
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

            if ( window.state.originalSelectedPackage == null )
                window.state.originalSelectedPackage = window.state.selectedPackage;

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

        void DrawHeader( Models.PackageManifest package )
        {
            using ( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.Space();

                GUILayout.Label( $"{package.displayName} {package.huf.version}",
                    new GUIStyle( EditorStyles.label )
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
                case Models.PackageStatus.Git:
                case Models.PackageStatus.GitUpdate:
                case Models.PackageStatus.GitError:
                    TryDrawExternalDependenciesButtons();
                    break;
                case Models.PackageStatus.NotInstalled:
                case Models.PackageStatus.ForceUpdate:
                case Models.PackageStatus.UpdateAvailable:
                case Models.PackageStatus.Migration:
                {
                    var update = package.huf.status != Models.PackageStatus.NotInstalled;
                    var label = update ? $"Update to {package.huf.config.latestVersion}" : "Install";

                    if ( GUILayout.Button( label, GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                    {
                        RegisterEvent( new Models.PackageManagerViewEvent
                        {
                            owner = Type,
                            eventType = Models.EventType.InstallPackage,
                            data = update ? package.LatestPackageVersion().ToString() : package.ToString()
                        } );
                    }

                    if ( update )
                    {
                        using ( new EditorGUI.DisabledScope( !package.huf.config.packageCanBeRemoved ) )
                        {
                            if ( GUILayout.Button( "Remove",
                                GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                            {
                                RegisterEvent( new Models.PackageManagerViewEvent
                                {
                                    owner = Type,
                                    eventType = Models.EventType.RemovePackage,
                                    data = package.ToString()
                                } );
                            }
                        }
                    }

                    break;
                }
                case Models.PackageStatus.Installed:
                case Models.PackageStatus.Embedded:
                case Models.PackageStatus.Conflict:
                {
                    TryDrawExternalDependenciesButtons();

                    if ( GUILayout.Button( "Remove", GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                    {
                        RegisterEvent( new Models.PackageManagerViewEvent
                        {
                            owner = Type,
                            eventType = Models.EventType.RemovePackage,
                            data = package.ToString()
                        } );
                    }

                    break;
                }
            }

            void InstallUnityDependencies( List<PackageManifest> packages )
            {
                var dependencies = new List<Dependency>();
                var hadUnityDependency = false;

                Core.Command.Execute( new GetUnityPackagesCommand
                {
                    OnComplete = ( unityPackagesResult, unityPackageSerializedData ) =>
                    {
                        var unityPackages = Core.Packages.Unity;

                        foreach ( var pack in packages )
                        {
                            foreach ( var dependency in pack.huf.dependencies
                                .Select( p => new Models.Dependency( p ) ).Where( d => !d.IsHufPackage ) )
                            {
                                hadUnityDependency = true;

                                var existingDependency =
                                    unityPackages.FirstOrDefault( d => d.name == dependency.name );

                                if ( existingDependency == null )
                                {
                                    dependencies.Add( dependency );
                                }
                                else if ( dependency.IsVersionHigherTo( existingDependency ) )
                                {
                                    dependencies.Remove( existingDependency.ToDependency() );
                                    dependencies.Add( dependency );
                                }
                            }
                        }

                        if ( !hadUnityDependency )
                            Utils.Common.LogAlways(
                                $"No Unity dependencies are needed for chosen package{( packages.Count == 1 ? string.Empty : "s" )}." );
                        else if ( dependencies.Count == 0 )
                            Utils.Common.LogAlways( "All needed Unity dependencies are installed." );
                        else
                        {
                            var data = Utils.Common.FromListToJson( dependencies );
                            Utils.Common.LogAlways( $"Installing Unity dependencies: {data}" );

                            Core.Command.Enqueue( new Commands.Processing.PackageLockCommand()
                            {
                                data = data,
                                lastResult = true
                            } );
                            Core.Command.Enqueue( new Commands.Processing.ProcessPackageLockCommand() );
                        }
                    }
                } );
            }

            void TryDrawExternalDependenciesButtons()
            {
                if ( !package.HasExternalDependencies )
                {
                    return;
                }

                if ( GUILayout.Button( "Install Unity dependencies", GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                {
                    InstallUnityDependencies( new List<PackageManifest>() { package } );
                }

                if ( GUILayout.Button( "Install all Unity dependencies", GUILayout.MinWidth( BUTTON_WIDTH ) ) )
                {
                    InstallUnityDependencies( Core.Packages.Local );
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

                    if ( !string.IsNullOrEmpty( package.unity ) )
                    {
                        GUILayout.Label( "Supported Unity versions", EditorStyles.boldLabel );

                        GUILayout.Label( ProcessSupportedUnityVersions( package.unity ),
                            package.SupportsCurrentUnityVersion ? style : importantStyle );
                        EditorGUILayout.Space();
                    }

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

            string ProcessSupportedUnityVersions( string supportedUnityVersions )
            {
                string[] versionRanges = supportedUnityVersions.Replace( " ", "" ).Split( ',' );
                StringBuilder stringBuilder = new StringBuilder();

                foreach ( var versionRange in versionRanges )
                {
                    if ( stringBuilder.Length > 0 )
                        stringBuilder.Append( ", " );
                    stringBuilder.Append( versionRange.Contains( "-" ) ? versionRange : $"{versionRange} and up" );
                }

                return stringBuilder.ToString();
            }
        }

        void DrawMetadataInfo( Models.PackageManifest package )
        {
            GUILayout.Label( "Metadata", EditorStyles.boldLabel );
            GUILayout.Label( $"Version: {package.version}" );

            if ( !string.IsNullOrEmpty( package.huf.commit ) )
            {
                GUILayout.Label( $"Build: {package.huf.commit}-b{package.huf.build}" );
                GUILayout.Label( $"Build Time: {package.huf.date}" );
            }

            if ( !string.IsNullOrEmpty( package.huf.rollout ) )
            {
                GUILayout.Label( $"Rollout: {package.huf.rollout}" );
            }

            if ( package.huf.isLocal )
            {
                GUILayout.Label( "This package is local." );
            }

            if ( !string.IsNullOrEmpty( package.huf.config.latestVersion ) )
            {
                GUILayout.Label( $"Latest version: {package.huf.config.latestVersion}" );
            }

            if ( !string.IsNullOrEmpty( package.huf.config.minimumVersion ) )
            {
                GUILayout.Label( $"Minimum version: {package.huf.config.minimumVersion}" );
            }

            if ( !string.IsNullOrEmpty( package.huf.path ) )
            {
                GUILayout.Label( $"Path: {package.huf.path}" );
            }
        }

        void DrawDependenciesInfo( Models.PackageManifest package )
        {
            if ( package == null )
            {
                return;
            }

            if ( package.huf.dependencies.Count > 0 )
                DrawDependenciesInfo( "Dependencies", package.huf.dependencies );

            if ( package.huf.optionalDependencies.Count > 0 )
            {
                if ( package.huf.dependencies.Count > 0 )
                    EditorGUILayout.Space();
                DrawDependenciesInfo( "Optional dependencies", package.huf.optionalDependencies );
            }
        }

        void DrawDependenciesInfo( string title, List<string> dependencies )
        {
            GUILayout.Label( title, EditorStyles.boldLabel );

            foreach ( var dependency in dependencies )
            {
                GUILayout.Label( $"• {dependency}",
                    new GUIStyle( EditorStyles.label )
                    {
                        fontStyle = FontStyle.Normal,
                        fontSize = 10
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
                GUILayout.Label( $"• {dependency}",
                    new GUIStyle( EditorStyles.label )
                    {
                        fontStyle = FontStyle.Normal,
                        fontSize = 10
                    } );
            }
        }

        void DrawDetailedInfo( Models.PackageManifest package )
        {
            using ( new GUILayout.VerticalScope() )
            {
                using ( var scope = new GUILayout.ScrollViewScope( rightSideScroll ) )
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
                        GUILayout.Label( package.huf.message, importantStyle );
                        EditorGUILayout.Space();
                    }

                    GUILayout.Label( package.description,
                        new GUIStyle( EditorStyles.label )
                        {
                            fontSize = 12,
                            wordWrap = true
                        } );
                    
                    DrawDetailsData( package );

                    if ( package.TryGetChangelog( out string changelog ) )
                    {
                        EditorGUILayout.Space();
                        GUILayout.Label( "Changelog", EditorStyles.boldLabel );
                        EditorGUILayout.Space();

                        using ( new GUILayout.VerticalScope() )
                        {
                            GUILayout.Label( changelog,
                                new GUIStyle( EditorStyles.label )
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
                EditorGUILayout.Space();
                using ( new GUILayout.VerticalScope( GUILayout.Height( 200f ) ) )
                {
                    for ( int i = 0; i < package.huf.details.Count; ++i )
                    {
                        GUILayout.Label( package.huf.details[i],
                            new GUIStyle( EditorStyles.label )
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
                if ( package.IsInstalled || package.IsRepository )
                {
                    if ( GUILayout.Button( new GUIContent( "Documentation" ), EditorStyles.miniButton ) )
                    {
                        try
                        {
                            var directoryInfo = Directory.Exists( $"{package.huf.path}/Documentation~" )
                                ? new DirectoryInfo( $"{package.huf.path}/Documentation~" )
                                : new DirectoryInfo( $"{package.huf.path}/Documentation" );

                            if ( directoryInfo.Exists )
                            {
                                var files = directoryInfo.GetFiles( "*.md", SearchOption.AllDirectories );

                                if ( files.Length == 0 )
                                {
                                    UnableToFindDocumentationDialog();
                                }
                                else
                                {
                                    EditorUtility.RevealInFinder( files[0].FullName );
                                }
                            }
                            else
                            {
                                UnableToFindDocumentationDialog();
                            }
                        }
                        catch ( Exception )
                        {
                            UnableToFindDocumentationDialog();
                        }
                    }
                }

                var originalSelectedPackage = window.state.originalSelectedPackage;

                //using window.state.originalSelectedPackage as DownloadPackageManifestCommand() does not get versions
                using ( new EditorGUI.DisabledScope(
                    originalSelectedPackage.huf.config.stableVersions.Count == 0 &&
                    originalSelectedPackage.huf.config.previewVersions.Count == 0 &&
                    originalSelectedPackage.huf.config.developmentVersions.Count == 0    &&
                    originalSelectedPackage.huf.config.experimentalVersions.Count == 0 ) )
                {
                    if ( GUILayout.Button( "Other versions", EditorStyles.popup ) )
                    {
                        var menu = new GenericMenu();

                        DisplayVersionsMenu( ref menu,
                            originalSelectedPackage.huf.config.stableVersions,
                            Models.Keys.Routing.STABLE_CHANNEL,
                            package );

                        DisplayVersionsMenu( ref menu,
                            originalSelectedPackage.huf.config.previewVersions,
                            Models.Keys.Routing.PREVIEW_CHANNEL,
                            package );
                        
                        DisplayVersionsMenu( ref menu,
                            originalSelectedPackage.huf.config.developmentVersions,
                            Models.Keys.Routing.DEVELOPMENT_CHANNEL,
                            package );

                        DisplayVersionsMenu( ref menu,
                            originalSelectedPackage.huf.config.experimentalVersions,
                            Models.Keys.Routing.EXPERIMENTAL_CHANNEL,
                            package );
                        menu.ShowAsContext();
                    }
                }
            }
        }

        void UnableToFindDocumentationDialog()
        {
            EditorUtility.DisplayDialog( "Documentation",
                "Documentation is not found for this package.",
                "Ok" );
        }

        void DisplayVersionsMenu( ref GenericMenu menu,
            List<Models.Version> versions,
            string channel,
            Models.PackageManifest package )
        {
            versions.Sort( ( v1, v2 ) => Utils.VersionComparer.Compare( v2.version, v1.version ) );
            var versionSuffix = channel == Keys.Routing.STABLE_CHANNEL ? "" : $"-{channel}";

            foreach ( var version in versions )
            {
                var versionWithSuffix = $"{version.version}{versionSuffix}";

                if ( Utils.VersionComparer.Compare( package.version, "=", versionWithSuffix ) )
                {
                    menu.AddDisabledItem( new GUIContent( $"{channel}/{version.version}" ),
                        IsPackageCurrentlyInstalled( versionWithSuffix ) );
                }
                else
                {
                    menu.AddItem( new GUIContent( $"{channel}/{version.version}" ),
                        IsPackageCurrentlyInstalled( versionWithSuffix ),
                        () =>
                        {
                            if ( window.state.originalSelectedPackage.version == versionWithSuffix )
                            {
                                window.state.selectedPackage = window.state.originalSelectedPackage;
                                window.state.selectedPackage.CheckIfCurrentUnitySupportsThisPackage();
                                return;
                            }

                            string scope = string.IsNullOrEmpty( version.scope ) ? package.huf.scope : version.scope;
                            DownloadPackageManifestAndChangelog( version.version, package.name, scope );
                        } );
                }

                void DownloadPackageManifestAndChangelog( string packageVersion,
                    string packageName,
                    string packageScope )
                {
                    Core.Command.Execute( new Commands.Connection.DownloadPackageManifestCommand()
                    {
                        version = packageVersion,
                        channel = channel,
                        packageName = packageName,
                        scope = packageScope,
                        OnComplete = ( success, serializedData ) =>
                        {
                            if ( !success )
                            {
                                return;
                            }

                            package = Models.PackageManifest.ParseManifest( serializedData, true );
                            DownloadPackageChangelog( packageVersion, packageName, packageScope );
                        }
                    } );
                }

                void DownloadPackageChangelog( string packageVersion, string packageName, string packageScope )
                {
                    Core.Command.Execute( new Commands.Connection.DownloadPackageChangelogCommand()
                    {
                        version = packageVersion,
                        channel = channel,
                        packageName = packageName,
                        scope = packageScope,
                        OnComplete = ( success2, serializedData2 ) =>
                        {
                            package.RemoteChangelog = serializedData2;
                            window.state.selectedPackage = package;
                            window.state.selectedPackage.CheckIfCurrentUnitySupportsThisPackage();
                        }
                    } );
                }
            }
        }

        bool IsPackageCurrentlyInstalled( string version )
        {
            if ( window.state.originalSelectedPackage.IsInstalled )
                return Utils.VersionComparer.Compare( window.state.originalSelectedPackage.version,
                    "=",
                    version );

            return false;
        }
    }
}