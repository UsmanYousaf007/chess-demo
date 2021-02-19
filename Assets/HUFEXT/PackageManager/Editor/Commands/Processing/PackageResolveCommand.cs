using System;
using System.Collections.Generic;
using System.Linq;
using HUFEXT.PackageManager.Editor.Commands.Connection;
using HUFEXT.PackageManager.Editor.Models;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    // 1. Only available packages can be resolved (exist in packages list).
    // 2. Dependency can be added only if:
    //     -> is not installed
    //     -> is installed but version is smaller than requested
    // 3. Dependency can be skipped only if is installed and version is equal or higher than requested.
    // 4. Resolver will fail if package or dependency can't be found.
    public class PackageResolveCommand : Core.Command.Base
    {
        public const string HPM_PACKAGES_TO_INSTALL = "HPM_PackagesToInstall";
        public const string COM_HUUUGE_HUFEXT_PACKAGE_MANAGER = "com.huuuge.hufext.packagemanager";
        readonly bool useLatestVersion = false;
        readonly List<Models.Dependency> dependencies = new List<Models.Dependency>();
        readonly List<Models.PackageManifest> packagesToInstall = new List<PackageManifest>();

        List<Models.PackageManifest> packages;

        public PackageResolveCommand( Models.PackageManifest packageToInstall, bool useLatest = false ) : this(
            new List<PackageManifest> {packageToInstall},
            useLatest ) { }

        public PackageResolveCommand( List<Models.PackageManifest> packagesToInstall, bool useLatest = false )
        {
            Core.Registry.ClearCache();
            useLatestVersion = useLatest;

            foreach ( var packageToInstall in packagesToInstall )
            {
                this.packagesToInstall.Add( useLatest ? packageToInstall.LatestPackageVersion() : packageToInstall );
            }
        }

        public override void Execute()
        {
            packages = Core.Packages.Data;

            foreach ( var packageToInstall in packagesToInstall )
            {
                var packageCanBeResolved = packages.Exists( package => package.name == packageToInstall.name );

                foreach ( var package in packages )
                {
                    if ( !package.IsInstalled )
                    {
                        continue;
                    }

                    foreach ( var excludedPackage in package.huf.exclude )
                    {
                        if ( excludedPackage != packageToInstall.name )
                        {
                            continue;
                        }

                        Complete( false,
                            $"Unable to install {packageToInstall.name} because it's conflicts with {package.name}." );
                        return;
                    }
                }

                if ( !packageCanBeResolved )
                {
                    Complete( false, $"Unable to resolve package {packageToInstall.name}." );
                    return;
                }

                ResolveDependencies( packageToInstall,
                    didResolveSucceeded =>
                    {
                        if ( !didResolveSucceeded )
                        {
                            Complete( false, $"Unable to resolve package {packageToInstall.name}." );
                            return;
                        }

                        Dependency dependency =
                            new Models.Dependency( $"{packageToInstall.name}@{packageToInstall.version}" );

                        FindDependencyVersionChannelAndScope( dependency,
                            () =>
                            {
                                dependencies.Add( dependency );

                                // Ask if the repositories should be replaced.
                                for ( int i = dependencies.Count - 1; i >= 0; i-- )
                                {
                                    dependency = dependencies[i];
                                    var package = packages.Find( ( p ) => p.name == dependency.name );

                                    if ( package != null && package.IsRepository )
                                    {
                                        bool shouldReplace = EditorUtility.DisplayDialog(
                                            "Replacing repository with a newer package",
                                            $"Should \"{package.displayName}\" repository be deleted and replaced with a newer package[{package.version} -> {dependency.version}]?",
                                            "Replace",
                                            "Skip" );

                                        if ( !shouldReplace )
                                            dependencies.RemoveAt( i );
                                    }
                                }

                                if ( packagesToInstall.Last() == packageToInstall )
                                {
                                    PauseInstallationIfPackageManagerNeedsToBeUpdated();
                                    Complete( true, Utils.Common.FromListToJson( dependencies ) );
                                }
                            } );
                    } );
            }
        }

        void PauseInstallationIfPackageManagerNeedsToBeUpdated()
        {
            if ( !packagesToInstall.Exists( dep => dep.name == COM_HUUUGE_HUFEXT_PACKAGE_MANAGER ) &&
                 dependencies.Exists( dep => dep.name == COM_HUUUGE_HUFEXT_PACKAGE_MANAGER ) )
            {
                Dependency packageManagerDependency =
                    dependencies.First( dep => dep.name == COM_HUUUGE_HUFEXT_PACKAGE_MANAGER );

                PlayerPrefs.SetString( HPM_PACKAGES_TO_INSTALL,
                    Utils.Common.FromListToJson( packagesToInstall ) );
                dependencies.Clear();
                dependencies.Add( packageManagerDependency );
            }
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            Utils.Common.Log( $"Complete {result} {serializedData}" );

            if ( !result )
            {
                Utils.Common.LogError( serializedData );
            }

            base.Complete( result, serializedData );
        }

        void ResolveDependencies( Models.PackageManifest owner, Action<bool> didResolveSucceededCallback )
        {
            int dependenciesLeftCount = owner.huf.dependencies.Count;

            if ( dependenciesLeftCount == 0 )
                didResolveSucceededCallback?.Invoke( true );

            foreach ( var entry in owner.huf.dependencies )
            {
                var dependency = new Models.Dependency( entry );

                // Infinite loop protection.
                if ( dependency.name == owner.name && owner.IsVersionHigherOrEqualTo( dependency ) )
                {
                    Utils.Common.Log( "Recursive dependency found. Skipping..." );
                    DecreaseDependenciesLeft();
                    continue;
                }

                if ( !dependency.IsHufPackage )
                {
                    Utils.Common.Log( $"External dependency {dependency} found." );
                    var existingDependency = dependencies.FirstOrDefault( d => d.name == dependency.name );

                    if ( existingDependency == null )
                    {
                        dependencies.Add( dependency );
                    }
                    else if ( dependency.IsVersionHigherTo( existingDependency ) )
                    {
                        dependencies.Remove( existingDependency );
                        dependencies.Add( dependency );
                    }

                    DecreaseDependenciesLeft();
                    continue;
                }

                // Dependency must exist in package list.
                var package = packages.Find( ( p ) => p.name == dependency.name );

                if ( package == null )
                {
                    Utils.Common.LogError( $"Unable to find dependency: {dependency}." );
                    didResolveSucceededCallback?.Invoke( false );
                    return;
                }

                // Install if the dependency is a higher version than installed.
                if ( ( ( package.IsRepository || package.IsInstalled ) &&
                       package.IsVersionHigherOrEqualTo( dependency ) ) )
                {
                    Utils.Common.Log( $"Dependency {dependency} is installed. Skipping..." );
                    DecreaseDependenciesLeft();
                    continue;
                }

                //Check what version is available
                FindDependencyVersionChannelAndScope( dependency,
                    () =>
                    {
                        Core.Command.Execute( new Commands.Connection.DownloadPackageManifestCommand()
                        {
                            version = dependency.version,
                            channel = dependency.channel.ToString().ToLower(),
                            packageName = dependency.name,
                            scope = dependency.scope,
                            OnComplete = ( success, serializedData ) =>
                            {
                                if ( !success )
                                {
                                    didResolveSucceededCallback?.Invoke( false );
                                    return;
                                }

                                package = Models.PackageManifest.ParseManifest( serializedData, true );

                                ResolveDependencies( package,
                                    didResolveSucceeded =>
                                    {
                                        if ( !didResolveSucceeded )
                                        {
                                            Utils.Common.LogError( $"Unable to find dependency: {dependency}" );
                                            didResolveSucceededCallback?.Invoke( false );
                                            return;
                                        }

                                        var dependencyWithTheSameName =
                                            dependencies.Find( ( d ) => d.name == dependency.name );

                                        // Add dependency if it doesn't exist in the current list or if it has a higher version than one in the list
                                        if ( dependencyWithTheSameName == null )
                                        {
                                            dependencies.Add( dependency );
                                            Utils.Common.Log( $"New dependency found {dependency}." );
                                        }
                                        else if ( dependency.IsVersionHigherTo( dependencyWithTheSameName ) )
                                        {
                                            dependencies.Remove( dependencyWithTheSameName );
                                            dependencies.Add( dependency );
                                        }

                                        DecreaseDependenciesLeft();
                                    } );
                            }
                        } );
                    }
                );
            }

            void DecreaseDependenciesLeft()
            {
                dependenciesLeftCount--;
                Utils.Common.Log( $"dependenciesLeftCount: {dependenciesLeftCount} owner:{owner.name}" );

                if ( dependenciesLeftCount <= 0 )
                    didResolveSucceededCallback?.Invoke( true );
            }
        }

        void FindDependencyVersionChannelAndScope( Dependency dependency, Action dependencyFoundCallback )
        {
            var package = packages.Find( ( p ) => p.name == dependency.name );

            Core.Command.Execute( new Commands.Connection.GetPackageVersionsCommand()
            {
                package = package,
                OnComplete = ( success, serializedData ) =>
                {
                    if ( !success )
                    {
                        return;
                    }

                    var manifest = Models.PackageManifest.ParseManifest( serializedData, true );
                    int channelCount = Enum.GetNames( typeof(Models.PackageChannel) ).Length;
                    var channel = Core.Packages.Channel;
                    var versionParts = dependency.version.Split( '-' );
                    var channelString = "";

                    if ( dependency.version.Contains( "-" ) )
                    {
                        channelString = versionParts[1];
                        dependency.version = versionParts[0];
                    }
                    else
                        channelString = package.huf.channel;

                    if ( channelString.Length > 0 )
                        Enum.TryParse( channelString.Substring( 0, 1 ).ToUpper() + channelString.Substring( 1 ),
                            out channel );
                    var startingChannel = channel;
                    Utils.Common.Log( $"startingChannel {startingChannel}" );

                    for ( int i = 0; i < channelCount + 1; i++ )
                    {
                        if ( i > 0 )
                            channel = (Models.PackageChannel)( i - 1 );
                        var versions = manifest.huf.config.GetVersionsForChannel( channel );
                        var version = versions.FirstOrDefault( v => v.version == dependency.version );

                        if ( ContinueIfVersionIsNotNull( version ) )
                            return;
                    }

                    Utils.Common.Log(
                        $"{package.name} {dependency.version} version not found in any channel. Trying to find newer version..." );
                    channel = startingChannel;

                    for ( int i = 0; i < channelCount + 1; i++ )
                    {
                        if ( i > 0 )
                            channel = (Models.PackageChannel)( i - 1 );
                        var versions = manifest.huf.config.GetVersionsForChannel( channel );

                        var version = versions.FirstOrDefault( v => dependency.HaveSameMajorVersion( v.version ) &&
                                                                    !dependency.IsVersionHigherTo( v.version ) );

                        if ( ContinueIfVersionIsNotNull( version ) )
                            return;
                    }

                    Complete( false,
                        $"{package.name} {dependency.version} version or newer not found in any channel!" );
                    return;

                    bool ContinueIfVersionIsNotNull( Models.Version version )
                    {
                        if ( version == null )
                            return false;

                        dependency.channel = channel;
                        dependency.version = version.version;
                        dependency.scope = version.scope;
                        dependencyFoundCallback?.Invoke();
                        return true;
                    }
                }
            } );
        }
    }
}