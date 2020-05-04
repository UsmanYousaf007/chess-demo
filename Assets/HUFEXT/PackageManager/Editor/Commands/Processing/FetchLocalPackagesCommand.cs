using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class FetchLocalPackagesCommand : Core.Command.Base
    {
        readonly List<Models.PackageManifest> packages = new List<Models.PackageManifest>();
        
        public override void Execute()
        {
            var manifests = Directory.GetFiles( Models.Keys.Filesystem.ASSETS_DIR,
                Models.Keys.Filesystem.MANIFEST_EXTENSION,
                SearchOption.AllDirectories );
            
            // Search for new format packages...
            foreach ( var manifest in manifests )
            {
                ParseManifest( manifest );
            }
            
            // Search for old format packages...
            var directories = Directory.GetDirectories( Models.Keys.Filesystem.ASSETS_DIR, "HUF*" );
            foreach ( var scope in directories )
            {
                var localPackages = Directory.GetDirectories( scope );
                foreach ( var package in localPackages )
                {
                    ParseOldPackages( package );
                }
            }

            Complete( true );
        }
        
        protected override void Complete( bool result, string serializedData = "" )
        {
            if ( result )
            {
                Core.Packages.Local = packages;
            }
            else
            {
                Core.Packages.ClearLocalData();
            }
            
            base.Complete( result, serializedData );
        }
        
        void ParseManifest( string path )
        {
            try
            {
                // Get local path to package. It will be compared with exportPath from remote package configs.
                var parsedPath = FixPath( path );
                parsedPath = parsedPath.Substring( 0, parsedPath.LastIndexOf( '/' ) );
                
                var manifest = Models.PackageManifest.ParseManifest( path );
                manifest.huf.status = Models.PackageStatus.Installed;
                manifest.huf.isLocal = true;
                manifest.huf.path = parsedPath;
                packages.Add( manifest );
            }
            catch ( Exception )
            {
                Utils.Common.Log( $"Skipping manifest at path: {path}" );
                Complete( false );
            }
        }
        
        void ParseOldPackages( string path )
        {
            Models.PackageManifest manifest = null;

            var parsedPath = FixPath( path );
            if ( packages.Exists( ( m ) => m.huf.path == parsedPath ) )
            {
                return;
            }
            
            var version = Directory.GetFiles( path, "version.txt", SearchOption.AllDirectories );
            if ( version.Length > 0 )
            {
                var name = new DirectoryInfo( path ).Name;
                manifest = Models.PackageManifest.ParseManifest( version[0] );
                manifest.displayName = name;
                manifest.huf.status = Models.PackageStatus.Installed;
                var include = Directory.GetFiles( path, "includes.txt", SearchOption.AllDirectories );
                if ( include.Length > 0 )
                {
                    var tmp = Models.PackageManifest.ParseManifest( include[0] );
                    manifest.description = tmp.description;
                }
            }
            else
            {
                var defaultName = new DirectoryInfo( path ).Name;

                if ( defaultName == ".cache" )
                {
                    return;
                }

                manifest = new Models.PackageManifest
                {
                    displayName = defaultName,
                    huf = new Models.PackageManifest.Metadata() { status = Models.PackageStatus.Embedded }
                };

                manifest.ParseVersion();
            }

            manifest.huf.path = FixPath( path );
            packages.Add( manifest );
        }

        string FixPath( string path )
        {
            return path.Replace( "\\", "/" );
        }
    }
}
