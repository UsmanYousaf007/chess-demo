using System.Collections.Generic;
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
        readonly bool useLatestVersion = false;
        readonly Models.PackageManifest manifest;
        readonly List<Models.Dependency> dependencies = new List<Models.Dependency>();
        List<Models.PackageManifest> packages;
        
        public PackageResolveCommand( Models.PackageManifest manifest, bool useLatest = false )
        {
            this.manifest = manifest;
            useLatestVersion = useLatest;
        }
        
        public override void Execute()
        {
            packages = Core.Packages.Data;
            
            var packageCanBeResolved = packages.Exists( package => package.name == manifest.name );
            
            foreach ( var package in packages )
            {
                if ( !package.IsInstalled )
                {
                    continue;
                }

                foreach ( var excludedPackage in package.huf.exclude )
                {
                    if ( excludedPackage != manifest.name )
                    {
                        continue;
                    }

                    Complete( false,
                        $"Unable to install {manifest.name} because it's conflicts with {package.name}." );
                    return;
                }
            }
            
            if ( !packageCanBeResolved || !ResolveDependencies( manifest ) )
            {
                Complete( false, $"Unable to resolve dependency {manifest.name}." );
                return;
            }

            if ( useLatestVersion )
            {
                dependencies.Add( new Models.Dependency( $"{manifest.name}@{manifest.huf.config.latestVersion}" ) );
            }
            else
            {
                dependencies.Add( new Models.Dependency( $"{manifest.name}@{manifest.version}" ) );
            }

            Complete( true, Utils.Common.FromArrayToJson( dependencies ) );
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            if ( !result )
            {
                Debug.LogError( serializedData );
            }
            
            base.Complete( result, serializedData );
        }

        bool ResolveDependencies( Models.PackageManifest owner )
        {
            foreach ( var entry in owner.huf.dependencies )
            {
                var dependency = new Models.Dependency( entry );
                
                // Infinite loop protection.
                if ( dependency.name == owner.name )
                {
                    Utils.Common.Log( "Recursive dependency found. Skipping..." );
                    continue;
                }

                if ( !dependency.name.Contains( ".huuuge." ) )
                {
                    Utils.Common.Log( $"External dependency {dependency} found." );
                    dependencies.Add( dependency );
                    continue;
                }
                
                // Dependency must exist in package list.
                var package = packages.Find( ( p ) => p.name == dependency.name );
                if ( package == null )
                {
                    Debug.LogError( $"Unable to find dependency: {dependency}." );
                    return false;
                }
                
                // Install only if dependency is in higher version.
                if ( package.IsInstalled && Utils.VersionComparer.Compare( package.version, ">=", dependency.version, true ) )
                {
                    Utils.Common.Log( $"Dependency {dependency} is installed. Skipping..." );
                    continue;
                }
                
                if ( package.IsUpdate )
                {
                    if ( Utils.VersionComparer.Compare( package.huf.config.latestVersion, "<", dependency.version, true ) )
                    {
                        Debug.LogError( $"Missing requirements for dependency: {dependency}." );
                        return false;
                    }
                }
                else if ( Utils.VersionComparer.Compare( package.version, "<", dependency.version, true ) )
                {
                    Debug.LogError( $"Missing requirements for dependency: {dependency}." );
                    return false;
                }
                
                if ( !ResolveDependencies( package ) )
                {
                    Debug.LogError( $"Unable to find dependency: {dependency}" );
                    return false;
                }

                // Add dependency only if not exist in current list.
                if ( !dependencies.Exists( ( d ) => d.name == dependency.name ) )
                {
                    dependencies.Add( dependency );
                    Utils.Common.Log( $"New dependency found {dependency}." );
                }
            }
            
            return true;
        }
    }
}
