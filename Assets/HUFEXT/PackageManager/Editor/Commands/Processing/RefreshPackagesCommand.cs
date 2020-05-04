using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class RefreshPackagesCommand : Core.Command.Base
    {
        public bool downloadLatest = true;
        
        public override void Execute()
        {
            Core.Packages.UpdateInProgress = true;

            Core.Command.Execute( new FetchLocalPackagesCommand() );

            if ( !downloadLatest )
            {
                MergePackages( Core.Packages.Local, Core.Packages.Remote );
                return;
            }
            
            Core.Command.Execute( new Connection.DownloadPackagesListCommand
            {
                OnComplete = ( result, serializedData ) =>
                {
                    Core.Registry.Save( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY,
                                   DateTime.Now.ToString( CultureInfo.InvariantCulture ) );

                    var next = Utils.Common.GetTimestamp( Models.Keys.AUTO_FETCH_DELAY );
                    Core.Registry.Save( Models.Keys.PACKAGE_MANAGER_NEXT_AUTO_FETCH, next );
                    
                    MergePackages( Core.Packages.Local, Core.Packages.Remote );
                }
            });
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            Core.Packages.UpdateInProgress = false;
            Views.PackageManagerWindow.RefreshViews();
            base.Complete( result, serializedData );
        }

        void MergePackages( List<Models.PackageManifest> local, List<Models.PackageManifest> remote )
        {
            Core.Packages.Clear();
            
            var packages = new List<Models.PackageManifest>();
            packages.AddRange( local );
            
            foreach ( var remotePackage in remote )
            {
                var index = packages.FindIndex( package => package.name == remotePackage.name || 
                                                           package.huf.path == remotePackage.huf.path );
                
                if ( index == -1 )
                {
                    packages.Add( remotePackage );
                    continue;
                }

                packages[index].huf.isLocal = false;
                packages[index].huf.config = remotePackage.huf.config; // Apply latest remote config.
                
                // Not sure about that. Should be verified.
                packages[index].huf.channel = remotePackage.huf.channel;
                packages[index].huf.scope = remotePackage.huf.scope;

                if ( packages[index].name == "" )
                {
                    packages[index] = remotePackage;
                    packages[index].huf.status = Models.PackageStatus.Migration;
                    packages[index].version = "0.0.0-unknown";
                }
                else if ( Utils.VersionComparer.Compare( remotePackage.version, ">", packages[index].version ) )
                {
                    packages[index].huf.status = Models.PackageStatus.UpdateAvailable;

                    // Current version is lower than minimal package version from current config.
                    if ( Utils.VersionComparer.Compare( packages[index].version, "<",
                                                        remotePackage.huf.config.minimumVersion ) )
                    {
                        packages[index].huf.status = Models.PackageStatus.ForceUpdate;
                    }
                }

                if ( packages[index].version.Contains( "develop" ) )
                {
                    packages[index].huf.status = Models.PackageStatus.Development;
                }
            }

            for ( int i = 0; i < packages.Count; ++i ) 
            {
                if ( string.IsNullOrEmpty( packages[i].name ) )
                {
                    packages[i].name = "com.unknown." + packages[i].displayName.ToLower();
                }
            }
            
            Core.Packages.Data = packages.OrderBy( package => package.name ).ToList();
            Complete( true );
        }
    }
}
