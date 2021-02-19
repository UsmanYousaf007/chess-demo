using System.IO;
using System.Linq;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class ProcessPackageLockCommand : Core.Command.Base
    {
        static bool downloadingPackage = false;

        public override void Execute()
        {
            if ( !Core.Packages.Locked )
            {
                Core.Packages.Installing = false;
                Complete( false );
                return;
            }

            var locked = Core.Registry.Get<Models.Dependencies>( Models.Keys.FILE_PACKAGE_LOCK, Core.CachePolicy.File );
            var packages = Core.Packages.Data;

            if ( downloadingPackage )
            {
                Complete( false );
                return;
            }

            if ( locked.Items.Count == 0 )
            {
                Core.Packages.RemoveLock();
                Core.Packages.Installing = false;
                Complete( true );
                AssetDatabase.Refresh();
                Views.PackageManagerWindow.RefreshPackages();
                return;
            }

            Core.Packages.Installing = true;
            var dependency = locked.Items[0];

            if ( !dependency.IsHufPackage )
            {
                Common.Log( $"UPM dependency: {dependency.name}@{dependency.version}" );
                var request = UnityEditor.PackageManager.Client.List();
                while ( !request.IsCompleted ) { }
                var unityPackages = request.Result.ToList();
                var unityPackage = unityPackages.FirstOrDefault( p => p.name == dependency.name );

                if ( string.IsNullOrEmpty( dependency.version ) )
                {
                    if ( unityPackage == null )
                    {
                        Common.Log( "Adding UPM dependency" );
                        UnityEditor.PackageManager.Client.Add( dependency.name );
                    }
                }
                else
                {
                    if ( unityPackage == null ||
                         VersionComparer.Compare( dependency.version, unityPackage.version, true ) > 0 )
                    {
                        Common.Log( "Adding UPM dependency" );
                        UnityEditor.PackageManager.Client.Add( $"{dependency.name}@{dependency.version}" );
                    }
                }

                locked.Items.RemoveAt( 0 );
                Core.Registry.Save( Models.Keys.FILE_PACKAGE_LOCK, locked, Core.CachePolicy.File );
                Complete( true );
                return;
            }

            var package = packages.Find( p => p.name == dependency.name );
            locked.Items.RemoveAt( 0 );
            Core.Registry.Save( Models.Keys.FILE_PACKAGE_LOCK, locked, Core.CachePolicy.File );

            if ( package == null )
            {
                Core.Packages.RemoveLock();
                Complete( false, $"Unable to process lock file. {dependency.name} not found." );
                return;
            }

            // If package exist in cache directory, can be installed immediately.
            if ( InstallPackageIfExistInCache( package ) )
            {
                Complete( true );
                return;
            }

            downloadingPackage = true;
            var dependencyChannel = dependency.channel.ToString().ToLower();

            if ( dependency.version.Contains( "-" ) )
            {
                dependencyChannel = dependency.version.Split( '-' )[1];
            }

            Utils.Common.Log(
                $"Downloading package {package.name} from {dependencyChannel} channel" );

            Core.Command.Execute( new Connection.DownloadPackageCommand()
            {
                scope = dependency.scope,
                channel = dependencyChannel,
                packageName = dependency.name,
                version = dependency.version.Split( '-' )[0],
                OnComplete = ( success, serializedData ) =>
                {
                    if ( !success )
                    {
                        SendRequestForLatestPackage( package );
                        return;
                    }

                    InstallPackageIfExistInCache( package );
                }
            } );
            Complete( true );
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            Utils.Common.Log( serializedData );
            base.Complete( result, serializedData );
        }

        bool InstallPackageIfExistInCache( Models.PackageManifest package )
        {
            if ( File.Exists( Utils.Common.GetPackagePath( package.name ) ) )
            {
                downloadingPackage = false;

                Core.Command.Execute( new RemovePackageCommand
                {
                    path = package.huf.path,
                    OnComplete = ( r, d ) => Core.Command.Execute( new InstallPackageCommand
                    {
                        packageName = package.name
                    } )
                } );
                return true;
            }

            return false;
        }

        void SendRequestForLatestPackage( Models.PackageManifest package )
        {
            Core.Command.Execute( new Connection.DownloadPackageCommand()
            {
                scope = package.huf.scope,
                channel = package.huf.channel,
                packageName = package.name,
                version = package.huf.config.latestVersion,
                OnComplete = ( success, serializedData ) =>
                {
                    if ( !success )
                    {
                        Core.Registry.Remove( Models.Keys.FILE_PACKAGE_LOCK );
                        return;
                    }

                    InstallPackageIfExistInCache( package );
                }
            } );
        }
    }
}