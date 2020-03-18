using System.Collections.Generic;
using System.IO;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.API.Services;
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.Implementation.Local.Services
{
    public class LocalPackagesService : IPackageService
    {
        public void RequestPackagesList( string channel, UnityAction<List<PackageManifest>> onComplete )
        {
            var packages = new List<PackageManifest>();
            var directories = Directory.GetDirectories( "Assets", "HUF*" );
            foreach ( var scope in directories )
            {
                var localPackages = Directory.GetDirectories( scope );
                foreach ( var package in localPackages )
                {
                    ParsePackageManifest( ref packages, package );
                }
            }
            onComplete?.Invoke( packages );
        }

        private void ParsePackageManifest( ref List<PackageManifest> packages, string path )
        {
            PackageManifest manifest = null;

            var files = Directory.GetFiles( path, "package.json", SearchOption.AllDirectories );
            if ( files.Length > 0 )
            {
                manifest = PackageManifest.ParseManifest( files[0] );
                manifest.huf.status = PackageStatus.Installed;
                manifest.huf.isLocal = true;
            }
            else
            {
                var version = Directory.GetFiles( path, "version.txt", SearchOption.AllDirectories );
                if ( version.Length > 0 )
                {
                    var name = new DirectoryInfo( path ).Name;
                    manifest = PackageManifest.ParseManifest( version[0] );
                    manifest.name = name;
                    manifest.displayName = name;
                    manifest.huf.status = PackageStatus.Installed;
                    var include = Directory.GetFiles( path, "includes.txt", SearchOption.AllDirectories );
                    if ( include.Length > 0 )
                    {
                        var tmp = PackageManifest.ParseManifest( include[0] );
                        manifest.description = tmp.description;
                    }
                }
                else
                {
                    var defaultName = new DirectoryInfo( path ).Name;

                    if (defaultName == ".cache")
                    {
                        return;
                    }

                    manifest = new PackageManifest
                    {
                        name = defaultName,
                        displayName = defaultName,
                        huf = new PackageManifest.Metadata() { status = PackageStatus.Embedded }
                    };

                    manifest.ParseVersion();
                }
            }

            manifest.huf.path = path.Replace( "\\", "/" ); ;
            packages.Add( manifest );
        }
    }
}
