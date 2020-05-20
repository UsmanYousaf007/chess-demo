using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    public class DownloadPackagesListCommand : Core.Command.Base
    {
        readonly List<Models.PackageManifest> packages = new List<Models.PackageManifest>();

        public override void Execute()
        {
            Core.Command.Execute( new AuthorizeTokenCommand
            {
                OnComplete = ( result, serializedData ) =>
                {
                    if ( !result )
                    {
                        Models.Token.Invalidate();
                        Complete( false );
                    }
                    
                    SendRequestForCurrentChannel();
                }
            });
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            if ( result )
            {
                Core.Packages.Remote = packages;
            }
            else
            {
                Core.Packages.ClearRemoteData();
            }
            
            base.Complete( result, serializedData );
        }

        void SendRequestForCurrentChannel()
        {
            Utils.Common.Log( $"Downloading packages from {Core.Packages.Channel.ToString()} channel." );
            
            switch ( Core.Packages.Channel )
            {
                case Models.PackageChannel.Stable: FetchRemotePackages( Models.Keys.Routing.STABLE_CHANNEL ); 
                    break;
                case Models.PackageChannel.Preview: FetchRemotePackages( Models.Keys.Routing.PREVIEW_CHANNEL ); 
                    break;
                case Models.PackageChannel.Experimental: FetchRemotePackages( Models.Keys.Routing.EXPERIMENTAL_CHANNEL );
                    break;
                case Models.PackageChannel.Development: FetchDevelopmentPackages(); break;
                default: Complete( false, "Unsupported channel." );
                    return;
            }
        }
        
        void FetchRemotePackages( string channelName )
        {
            var route = Core.Request.CreateRoute( Models.Keys.Routing.API.LATEST_PACKAGES )
                            .Set( Models.Keys.Routing.Tag.CHANNEL, channelName )
                            .Value;

            var request = new Core.Request( route, response =>
            {
                if ( response.status == Core.RequestStatus.Failure )
                {
                    Complete( false, $"Unable to fetch remote packages from {channelName}. Response is failure." );
                    return;
                }

                var manifests = Utils.Common.FromJsonToArray<Models.PackageManifest>( response.text );
                foreach ( var manifest in manifests )
                {
                    if ( packages.Exists( package => package.name == manifest.name ) )
                    {
                        continue;
                    }
                    
                    manifest.ParseVersion();
                    manifest.huf.status = Models.PackageStatus.NotInstalled;
                    manifest.huf.isLocal = false;
                    packages.Add( manifest );
                }

                if ( channelName == Models.Keys.Routing.STABLE_CHANNEL )
                {
                    FetchRemotePackages( Models.Keys.Routing.PREVIEW_CHANNEL );
                    return;
                }
                
                Complete( true );
            } );
            
            request.Send();
        }

        void FetchDevelopmentPackages()
        {
            Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT, out string devEnvPath );
            if ( string.IsNullOrEmpty( devEnvPath ) )
            {
                Complete( false, "Development environment path is empty." );
                return;
            }

            if ( !Directory.Exists( devEnvPath ) )
            {
                Core.Registry.Remove( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT );
                Complete( false, "Incorrect development environment path." );
                return;
            }

            var directories = Directory.GetFiles( devEnvPath,
                Models.Keys.Filesystem.MANIFEST_EXTENSION,
                SearchOption.AllDirectories );
            
            foreach ( var path in directories )
            {
                var manifest = Models.PackageManifest.ParseManifest( path );
                manifest.huf.status = Models.PackageStatus.NotInstalled;
                manifest.huf.isLocal = false;

                var configPath = path.Replace( Models.Keys.Filesystem.MANIFEST_EXTENSION,
                    Models.Keys.Filesystem.CONFIG_EXTENSION );
                
                if ( File.Exists( configPath ) )
                {
                    var config = new Models.PackageConfig();
                    EditorJsonUtility.FromJsonOverwrite( File.ReadAllText( configPath ), config );
                    manifest.huf.config = config;
                    manifest.huf.config.latestVersion = manifest.version;
                }
                
                packages.Add( manifest );
            }
            
            Complete( true );
        }
    }
}
