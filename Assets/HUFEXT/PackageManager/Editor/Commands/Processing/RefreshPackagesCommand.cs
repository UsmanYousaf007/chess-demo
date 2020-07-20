using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HUFEXT.PackageManager.Editor.Commands.Data;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class RefreshPackagesCommand : Core.Command.Base
    {
        public bool downloadLatest = true;
        
        public override void Execute()
        {
            Core.Packages.UpdateInProgress = true;

            Core.Command.Execute( new GetLocalPackagesCommand() );

            if ( !downloadLatest )
            {
                MergePackages( Core.Packages.Local, Core.Packages.Remote );
                return;
            }
            
            Core.Command.Execute( new Data.GetUnityPackagesCommand
            {
                OnComplete = ( r, s ) =>
                {
                    Core.Command.Execute( new Data.GetRemotePackagesCommand()
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
            });
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            Core.Packages.UpdateInProgress = false;
            AssetDatabase.Refresh();
            base.Complete( result, serializedData );
        }

        void MergePackages( List<Models.PackageManifest> local, List<Models.PackageManifest> remote )
        {
            Core.Packages.Clear();
            
            var packages = new List<Models.PackageManifest>();
            packages.AddRange( local );

            DetectVendorPackages();
            
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
                packages[index].huf.config = remotePackage.huf.config;
                packages[index].huf.channel = remotePackage.huf.channel;
                packages[index].huf.scope = remotePackage.huf.scope;

                var nameIsEmpty = packages[index].name == string.Empty;

                if ( packages[index].huf.status == Models.PackageStatus.Git )
                {
                    if ( nameIsEmpty )
                    {
                        packages[index].name = remotePackage.name;
                    }

                    switch ( Utils.VersionComparer.Compare( remotePackage.version, packages[index].version, true ) )
                    {
                        case 0: packages[index].huf.status = Models.PackageStatus.GitUpdate; break;
                        case 1: packages[index].huf.status = Models.PackageStatus.GitError; break;
                    }
                    
                    continue;
                }
                
                if ( nameIsEmpty )
                {
                    packages[index] = remotePackage;
                    packages[index].huf.status = Models.PackageStatus.Migration;
                    packages[index].version = "0.0.0-unknown";
                }
                else
                {
                    var compare = Utils.VersionComparer.Compare( remotePackage.version, packages[index].version, true );

                    switch ( compare )
                    {
                        case 0:
                        {
                            var compareTag =
                                Utils.VersionComparer.Compare( remotePackage.version, packages[index].version );

                            if ( compareTag == 1 )
                            {
                                packages[index].huf.status = Models.PackageStatus.UpdateAvailable;
                            }
                            
                            break;
                        }

                        case 1:
                        {
                            var compareMin = Utils.VersionComparer.Compare( packages[index].version,
                                remotePackage.huf.config.minimumVersion,
                                true );

                            packages[index].huf.status = ( compareMin == -1 )
                                ? Models.PackageStatus.ForceUpdate
                                : Models.PackageStatus.UpdateAvailable;
                            
                            break;
                        }
                    }
                }

                if ( packages[index].version.Contains( "develop" ) )
                {
                    packages[index].huf.status = Models.PackageStatus.Development;
                }
            }

            SetMissingPackageNames();

            packages.AddRange( Core.Packages.Unity );
            
            Core.Packages.Data = packages.OrderBy( package => package.name ).ToList();
            Complete( true );
            
            void DetectVendorPackages()
            {
                for ( var i = 0; i < packages.Count; ++i )
                {
                    if ( !packages[i].name.Contains( ".huuuge." ) )
                    {
                        packages[i].huf.rollout = Models.Rollout.NOT_HUF_LABEL;
                    }
                }
            }

            void SetMissingPackageNames()
            {
                for ( var i = 0; i < packages.Count; ++i ) 
                {
                    if ( string.IsNullOrEmpty( packages[i].name ) )
                    {
                        packages[i].name = "com.unknown." + packages[i].displayName.ToLower();
                    }
                } 
            }
        }
    }
}
